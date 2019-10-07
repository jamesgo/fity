using Fity.Data;
using Fity.Data.TCX;
using Fity.Models;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace Fity.Utils.Library
{
    public class FolderMonitor
    {
        bool isInitialized = false;
        SemaphoreSlim initLock = new SemaphoreSlim(1);
        public FolderMonitor()
        {
        }

        public List<MonitoredFolder> Folders { get; private set; } = new List<MonitoredFolder>();

        public ConcurrentDictionary<IGpsFileInfo, Session> GpsFiles { get; }
            = new ConcurrentDictionary<IGpsFileInfo, Session>();

        public ConcurrentDictionary<Session, IGpsFileInfo> SessionFiles { get; }
            = new ConcurrentDictionary<Session, IGpsFileInfo>();

        public event GpsFilesChangedHandler Files_Changed;

        public async Task InitializeAsync()
        {
            if (!isInitialized)
            {
                await initLock.WaitAsync();
                if (!isInitialized)
                {
                    await this.RestoreLibrary();
                }
            }

            await this.RefreshAsync();
        }

        public async Task AddPath(string path)
        {
            var folder = new MonitoredFolder(path);
            await folder.InitializeAsync();
            this.Folders.Add(folder);

            await this.BackupLibrary();
        }

        public async Task RemovePath(string path)
        {
            var folder = this.Folders.Single(f => f.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
            foreach (var fileInfo in folder.FileInfos)
            {
                if (this.GpsFiles.TryRemove(fileInfo, out var session))
                {
                    this.SessionFiles.TryRemove(session, out var _);
                }

            }
            this.Folders.Remove(folder);
            await this.BackupLibrary();
        }

        public async Task Clear()
        {
            foreach (var folder in this.Folders.ToList())
            {
                foreach (var fileInfo in folder.FileInfos)
                {
                    if (this.GpsFiles.TryRemove(fileInfo, out var session))
                    {
                        this.SessionFiles.TryRemove(session, out var _);
                    }

                }
                this.Folders.Remove(folder);
            }

            await this.BackupLibrary();
        }

        private async Task RestoreLibrary()
        {
            var storageHelper = new LocalObjectStorageHelper();

            if (!await storageHelper.FileExistsAsync("library.json"))
            {
                return;
            }

            var backupFile = await storageHelper.ReadFileAsync<BackupFile>("library.json");
            if (backupFile == null)
            {
                return;
            }

            foreach (var path in backupFile.Paths)
            {
                if (this.Folders.Any(f => f.Path.Equals(path)))
                {
                    continue;
                }

                var monitoredFolder = new MonitoredFolder(path);
                await monitoredFolder.InitializeAsync();
                this.Folders.Add(monitoredFolder);
            }
        }

        private async Task BackupLibrary()
        {
            var storageHelper = new LocalObjectStorageHelper();
            await storageHelper.SaveFileAsync("library.json", new BackupFile { Paths = this.Folders.Select(f => f.Path).ToArray() });
        }

        public async Task RefreshAsync()
        {
            var tasks = new List<Task>();
            foreach (var folder in this.Folders)
            {
                tasks.Add(folder.RefreshAsync());
            }

            await Task.WhenAll(tasks);

            foreach (var folder in this.Folders)
            {
                await Task.WhenAll(
                    folder.FileInfos.AsParallel().Select(async file =>
                    {
                        var loader = GpsLoader.LoadGprx(file);
                        var session = (await loader.Task).ToModel(file.FilePath);
                        this.GpsFiles.TryAdd(file, session);
                        this.SessionFiles.TryAdd(session, file);
                    }));
            }

            this.Files_Changed?.Invoke(this);
        }

        public delegate void GpsFilesChangedHandler(object sender);

        public class BackupFile
        {
            public string[] Paths { get; set; }
        }
    }

    public class MonitoredFolder
    {
        public MonitoredFolder(string path)
        {
            this.Path = path;
        }

        public string Path { get; }

        public StorageFolder Folder { get; private set; }

        public HashSet<IGpsFileInfo> FileInfos { get; private set; }
            = new HashSet<IGpsFileInfo>();

        public async Task InitializeAsync()
        {
            this.Folder = await StorageFolder.GetFolderFromPathAsync(this.Path);

            await this.RefreshAsync();
        }

        public async Task RefreshAsync()
        {
            List<string> fileTypeFilter = new List<string>
            {
                ".tcx"
            };

            var loadedFiles = new HashSet<IGpsFileInfo>();
            var queryOptions = new QueryOptions(CommonFileQuery.OrderBySearchRank, fileTypeFilter);
            var files = await this.Folder.CreateFileQueryWithOptions(queryOptions).GetFilesAsync();
            foreach (var file in files)
            {
                loadedFiles.Add(file.ToGpsFileInfo());
            }
            this.FileInfos.IntersectWith(loadedFiles);
            this.FileInfos.UnionWith(loadedFiles);
        }

    }
}
