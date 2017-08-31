using Fity.Data;
using Fity.Models;
using Fity.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls.Maps;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Fity.Utils
{
    public class MapDataManager
    {
        private ActivityManager activityManager;
        private double minLatitude;
        private double minLongitude;
        private double maxLatitude;
        private double maxLongitude;

        const double mapMargin = .85;

        public GpsDataManager DataManager { get; private set; }
        public IReadOnlyList<IStorageItem> Files { get; }
        public List<string> FileNames { get; }
        public List<MapElement> MapElements { get; private set; }
        public List<MapElement> MergedMapElements { get; private set; }
        public Session MergedActivity { get; private set; }
        public Windows.Devices.Geolocation.Geopoint Center { get; private set; }

        private bool loadComplete;

        public double GetZoomLevel(double actualWidth, double actualHeight)
        {
            if (!loadComplete)
            {
                throw new NotSupportedException("Load not complete. Please await LoadCompleteAsync() before calling this method");
            }

            double latitudeDiffInMeters = (this.maxLatitude - this.minLatitude) * 111000;
            double longitudeDiffInMeters = (this.maxLongitude - this.minLongitude) * 111000;
            double desiredZoomLevel =
                Math.Log(
                    Math.Abs(156543.04 * Math.Cos(this.Center.Position.Latitude) /
                        Math.Max(
                        latitudeDiffInMeters / actualHeight,
                        longitudeDiffInMeters / actualWidth) * mapMargin),
                    newBase: 2);

            return this.BoundZoomLevel(desiredZoomLevel);
        }

        public MapDataManager(IReadOnlyList<IStorageItem> files)
        {
            this.DataManager = new GpsDataManager();
            this.Files = files;

            this.FileNames = new List<string>();
            this.MapElements = new List<MapElement>();
            this.MergedMapElements = new List<MapElement>();

            foreach (StorageFile file in this.Files)
            {
                this.FileNames.Add(file.Name);
                var gpsFileInfo = file.ToGpsFileInfo();
                var loader = this.DataManager.AddToSession(gpsFileInfo);
            }
        }

        public async Task LoadCompleteAsync()
        {
            this.activityManager = new ActivityManager(this.DataManager);
            double minLatitude, minLongitude;
            minLatitude = minLongitude = long.MaxValue;
            double maxLatitude, maxLongitude;
            maxLatitude = maxLongitude = long.MinValue;

            foreach (var activityExtended in await this.activityManager.GetSessions())
            {
                foreach (var mapEle in activityExtended.GetMapElements())
                {
                    this.MapElements.Add(mapEle);
                }

                if (activityExtended.HasGps)
                {
                    foreach (var tpPos in activityExtended.TrackpointsWithPosition.Select(tp => tp.Position))
                    {
                        minLatitude = Math.Min(minLatitude, tpPos.LatitudeDegrees);
                        minLongitude = Math.Min(minLongitude, tpPos.LongitudeDegrees);
                        maxLatitude = Math.Max(maxLatitude, tpPos.LatitudeDegrees);
                        maxLongitude = Math.Max(maxLongitude, tpPos.LongitudeDegrees);
                    }
                }
            }

            this.minLatitude = minLatitude;
            this.minLongitude = minLongitude;
            this.maxLatitude = maxLatitude;
            this.maxLongitude = maxLongitude;

            this.MergedActivity = await this.activityManager.GetMerged();
            foreach (var mapEle in this.MergedActivity.GetMapElements())
            {
                this.MergedMapElements.Add(mapEle);
            }

            // Set default map location based on input data
            var latitude = (maxLatitude + minLatitude) / 2;
            var longitude = (maxLongitude + minLongitude) / 2;
            this.Center = new Windows.Devices.Geolocation.Geopoint(new Windows.Devices.Geolocation.BasicGeoposition
            {
                Latitude = latitude,
                Longitude = longitude
            });

            loadComplete = true;
        }

        private double BoundZoomLevel(double zoomLevel) => Math.Max(1, Math.Min(19, zoomLevel));

    }
}
