using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fity.Data
{
    public class GpsDataManager
    {
        private IDictionary<IGpsFileInfo, GprxLoader> gpsFiles = new Dictionary<IGpsFileInfo, GprxLoader>();
        public GprxLoader AddToSession(IGpsFileInfo filePath)
        {
            return GpsLoader.LoadGprx(filePath);
        }

        public Task<Gprx> Get(IGpsFileInfo file)
        {
            GprxLoader fileTask;
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
