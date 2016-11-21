using Fity.Data.TCX;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fity.Data
{
    public class TcxLoader
    {
        private IGpsFileInfo filePath;

        public TcxLoader(IGpsFileInfo filePath)
        {
            this.filePath = filePath;
            this.CancellationToken = new CancellationTokenSource();

            this.Task = Load();
        }

        private async Task<Tcx> Load()
        {
            using (TextReader reader = new StreamReader(await this.filePath.GetStream()))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TrainingCenterDatabase));
                return new Tcx
                {
                    TrainingCenterDatabase = (TrainingCenterDatabase)serializer.Deserialize(reader)
                };
            }
        }

        private CancellationTokenSource CancellationToken { get; set; }

        public Task<Tcx> Task { get; private set; }

        internal void Cancel()
        {
            this.CancellationToken.Cancel();
        }
    }
}