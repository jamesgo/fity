using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fity.Models
{
    public class FileLaunchParameters
    {
        public IReadOnlyList<IStorageItem> Files { get; internal set; }
    }
}
