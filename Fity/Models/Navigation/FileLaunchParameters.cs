using System.Collections.Generic;
using Windows.Storage;

namespace Fity.Models.Navigation
{
    public class FileLaunchParameters
    {
        public IReadOnlyList<IStorageItem> Files { get; internal set; }
    }
}
