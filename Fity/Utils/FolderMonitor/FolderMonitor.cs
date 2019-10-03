using Fity.Data;
using Fity.Data.TCX;
using Fity.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
        }

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
                this.Folders.Add(await StorageFolder.GetFolderFromPathAsync(path));
            }

            await this.RefreshFilesAsync();
        }

        public async Task AddPath(string path)
        {
            var storageFolder = await StorageFolder.GetFolderFromPathAsync(path);
            this.Folders.Add(storageFolder);
            await this.RefreshFolderAsync(storageFolder);
            await this.BackupLibrary();
        }

        public async Task RemovePath(string path)
        {
            var folder = this.Folders.Single(f => f.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
            foreach (var fileInfo in this.FilesIndex[folder])
            {
                if (this.GpsFiles.TryRemove(fileInfo, out var session))
                {
                    this.SessionFiles.TryRemove(session, out var _);
                }

            }
            await this.BackupLibrary();
        }

        private async Task BackupLibrary()
        {
            var libraryFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("library.json", CreationCollisionOption.ReplaceExisting);

            var serializer = new JsonSerializer();

            using (var streamWriter = new StreamWriter(await libraryFile.OpenStreamForWriteAsync()))
            {
                serializer.Serialize(streamWriter, libraryFile);
            }
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
