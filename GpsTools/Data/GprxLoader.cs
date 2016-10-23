using GpsTools.Data.TCX;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GpsTools.Data
{
    public class GprxLoader
    {
        private IGpsFileInfo filePath;

        public GprxLoader(IGpsFileInfo filePath)
        {
            this.filePath = filePath;
            this.CancellationToken = new CancellationTokenSource();

            this.Task = Load();
        }

        private async Task<Gprx> Load()
        {
            using (TextReader reader = new StreamReader(await this.filePath.GetStream()))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TrainingCenterDatabase));
                return new Gprx
                {
                    TrainingCenterDatabase = (TrainingCenterDatabase)serializer.Deserialize(reader)
                };
            }
        }

        private CancellationTokenSource CancellationToken { get; set; }

        public Task<Gprx> Task { get; private set; }

        internal void Cancel()
        {
            this.CancellationToken.Cancel();
        }
    }
}