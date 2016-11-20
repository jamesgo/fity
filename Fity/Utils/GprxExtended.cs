using Fity.Data;
using Fity.Data.TCX;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls.Maps;
using System;
using Fity.Utils.Interpolation;

namespace Fity.Utils
{
    public class GprxExtended
    {
        private readonly TrackpointExtended[] trackpoints;

        public IEnumerable<TrackpointExtended> TrackpointsWithPosition => trackpoints.Where(tp => tp.HasPosition).ToList();

        public IEnumerable<TrackpointExtended> TrackpointsWithHeartRate => trackpoints.Where(tp => tp.HasHeartRate).ToList();

        public GprxExtended(Gprx gprx)
        {
            this.trackpoints = gprx.TrainingCenterDatabase.Activities.Activities.First().Lap.Trackpoints.Trackpoints.Select(tp => new TrackpointExtended(tp)).Where(tp => tp.IsValid).ToArray();
        }

        public GprxExtended(IEnumerable<TrackpointExtended> trackpoints)
        {
            this.trackpoints = trackpoints.ToArray();
        }

        public IEnumerable<MapElement> GetMapElements()
        {
            foreach (var trackpoint in trackpoints)
            {
                if (trackpoint.Position != null)
                {
                    var icon = new MapIcon();
                    icon.Title = trackpoint.HasHeartRate ? trackpoint.HeartRateBpm.Value.ToString() : "NoHR";

                    icon.Location = new Windows.Devices.Geolocation.Geopoint(new Windows.Devices.Geolocation.BasicGeoposition
                    {
                        Latitude = trackpoint.Position.LatitudeDegrees,
                        Longitude = trackpoint.Position.LongitudeDegrees
                    });
                    yield return icon;
                }
            }
        }

        public bool HasGps => this.TrackpointsWithPosition.Any();

        public bool HasHeartRate => this.TrackpointsWithHeartRate.Any();

        public Tuple<double, double, int> GetDefaultLocationWithWeights()
        {
            return new Tuple<double, double, int>(
                TrackpointsWithPosition.Average(tp => tp.Position.LatitudeDegrees),
                TrackpointsWithPosition.Average(tp => tp.Position.LongitudeDegrees),
                TrackpointsWithPosition.Count());
        }
    }
}