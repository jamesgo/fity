﻿using Fity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fity.Utils
{
    public static class Extensions
    {
        public static IGpsFileInfo ToGpsFileInfo(this StorageFile storageFile)
        {
            return new GpsStorageFileInfo(storageFile);
        }

    }
}
