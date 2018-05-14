using Fity.Data;
using Fity.Data.TCX;
using Fity.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace Fity.Utils.FolderMonitor
{
    public class FolderMonitor
    {
        public FolderMonitor(string[] paths)
        {
            this.Paths = paths;
            this.initializeTask = this.InitializeAsync();
        }

        private Task initializeTask;

        public string[] Paths { get; }

        public List<StorageFolder> Folders { get; private set; }

        private ConcurrentDictionary<StorageFolder, List<IGpsFileInfo>> FilesIndex { get; } 
            = new ConcurrentDictionary<StorageFolder, List<IGpsFileInfo>>();

        public ConcurrentDictionary<IGpsFileInfo, Session> GpsFiles { get; }
            = new ConcurrentDictionary<IGpsFileInfo, Session>();

        public ConcurrentDictionary<Session, IGpsFileInfo> SessionFiles { get; }
            = new ConcurrentDictionary<Session, IGpsFileInfo>();

        public event GpsFilesChangedHandler Files_Changed;

        public async Task InitializeAsync()
        {
            this.Folders = new List<StorageFolder>();
            foreach (var path in this.Paths)
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(path);
                this.Folders.Add(folder);
            }

            await this.RefreshFilesAsync();
        }

        public async Task RefreshFilesAsync()
        {
            var tasks = new List<Task>();
            foreach (var folder in this.Folders)
            {
                tasks.Add(this.RefreshFolderAsync(folder));
            }

            await Task.WhenAll(tasks);

            this.Files_Changed?.Invoke(this);
        }

        private async Task RefreshFolderAsync(StorageFolder folder)
        {
            List<string> fileTypeFilter = new List<string>
            {
                ".tcx"
            };

            var loadedFiles = new List<IGpsFileInfo>();
            var queryOptions = new QueryOptions(CommonFileQuery.OrderBySearchRank, fileTypeFilter);
            var files = await folder.CreateFileQueryWithOptions(queryOptions).GetFilesAsync();
            foreach (var file in files)
            {
                loadedFiles.Add(file.ToGpsFileInfo());
            }
            this.FilesIndex[folder] = loadedFiles;

            await Task.WhenAll(
                loadedFiles.AsParallel().Select(async file =>
                {
                    var loader = GpsLoader.LoadGprx(file);
                    var session = (await loader.Task).ToModel(file.FilePath);
                    this.GpsFiles.TryAdd(file, session);
                    this.SessionFiles.TryAdd(session, file);
                }));
        }

        public delegate void GpsFilesChangedHandler(object sender);
    }
}
