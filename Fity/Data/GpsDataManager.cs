using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fity.Data
{
    public class GpsDataManager
    {
        private IDictionary<IGpsFileInfo, TcxLoader> gpsFiles = new Dictionary<IGpsFileInfo, TcxLoader>();
        public TcxLoader AddToSession(IGpsFileInfo filePath)
        {
            var gprx = GpsLoader.LoadGprx(filePath);
            gpsFiles.Add(filePath, gprx);
            return gprx;
        }

        internal IEnumerable<KeyValuePair<IGpsFileInfo, TcxLoader>> GetAll()
        {
            return gpsFiles;
        }

        internal Task LoadAllAsync()
        {
            return Task.WhenAll(gpsFiles.Values.Select(l => l.Task));
        }

        public Task<Tcx> Get(IGpsFileInfo file)
        {
            TcxLoader fileTask;
            this.gpsFiles.TryGetValue(file, out fileTask);
            return fileTask.Task;
        }

        public bool IsLoadComplete
        {
            get { return this.gpsFiles.Values.All(gp => gp.Task.IsCompleted); }
        }

        internal void Clear()
        {
            foreach (var gpsFile in gpsFiles.Where(gp => !gp.Value.Task.IsCompleted))
            {
                gpsFile.Value.Cancel();
            }

            this.gpsFiles.Clear();
        }
    }
}
