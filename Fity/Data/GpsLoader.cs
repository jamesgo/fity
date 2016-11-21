using System;
using System.Threading.Tasks;

namespace Fity.Data
{
    internal class GpsLoader
    {
        internal static TcxLoader LoadGprx(IGpsFileInfo filePath)
        {
            return new TcxLoader(filePath);
        }
    }
}