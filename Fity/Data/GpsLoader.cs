using System;
using System.Threading.Tasks;

namespace Fity.Data
{
    internal class GpsLoader
    {
        internal static GprxLoader LoadGprx(IGpsFileInfo filePath)
        {
            return new GprxLoader(filePath);
        }
    }
}