using System.IO;
using System.Threading.Tasks;

namespace Fity.Data
{
    public interface IGpsFileInfo
    {
        string FileName { get; }
        string FilePath { get; }

        Task<Stream> GetStream();

        int GetHashCode();
        string ToString();
    }
}