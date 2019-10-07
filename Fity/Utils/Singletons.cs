using Fity.Utils.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fity.Utils
{
    static class Singletons
    {
        static readonly Lazy<FolderMonitor> folderMonitor = new Lazy<FolderMonitor>(() => new FolderMonitor());

        public static FolderMonitor FolderMonitor => Singletons.folderMonitor.Value;
    }
}
