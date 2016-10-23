﻿using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace GpsTools.Data
{
    public class GpsStorageFileInfo : IGpsFileInfo
    {
        private readonly StorageFile storageFile;

        public GpsStorageFileInfo(StorageFile storageFile)
        {
            this.storageFile = storageFile;
        }
        public string FilePath { get { return this.storageFile.Path; } }

        public string FileName {  get { return this.storageFile.Name; } }

        public async Task<Stream> GetStream()
        {
            var stream = await this.storageFile.OpenAsync(FileAccessMode.Read);
            return stream.GetInputStreamAt(0);
        }

        public override int GetHashCode()
        {
            return this.FilePath.GetHashCode();
        }

        public override string ToString()
        {
            return this.FileName.ToString();
        }
    }
}