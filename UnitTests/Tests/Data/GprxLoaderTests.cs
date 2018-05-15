using Fity.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace UnitTests
{
    [TestClass]
    public class GprxLoaderTests
    {
        [TestMethod]
        public async Task Sanity()
        {
            var fileInfo = new GpsStorageFileInfo(
                await StorageFile.GetFileFromApplicationUriAsync(
                    new Uri(@"ms-appx:///Data/Samples/2016-09-05_Lunch Run_Running.tcx")));

            var gprxLoader = new TcxLoader(fileInfo);
            var tcx = await gprxLoader.Task;

            Assert.IsNotNull(tcx);

            var trainingCenterDatabase = tcx.TrainingCenterDatabase;
            Assert.IsNotNull(trainingCenterDatabase);

            var activities = trainingCenterDatabase?.Activities?.Activities;
            Assert.IsNotNull(activities);

            var author = trainingCenterDatabase.Author;
            Assert.IsNotNull(author?.Name);
        }

        [TestMethod]
        public async Task Sanity2()
        {
            var fileInfo = new GpsStorageFileInfo(
                await StorageFile.GetFileFromApplicationUriAsync(
                    new Uri(@"ms-appx:///Data/Samples/2016-09-05_Lunch Run_Running (1).tcx")));

            var gprxLoader = new TcxLoader(fileInfo);
            var tcx = await gprxLoader.Task;

            Assert.IsNotNull(tcx);

            var trainingCenterDatabase = tcx.TrainingCenterDatabase;
            Assert.IsNotNull(trainingCenterDatabase);

            var activities = trainingCenterDatabase?.Activities?.Activities;
            Assert.IsNotNull(activities);

            var author = trainingCenterDatabase.Author;
            Assert.IsNotNull(author?.Name);

        }

    }
}
