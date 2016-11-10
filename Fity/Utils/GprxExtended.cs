using Fity.Data;
using Fity.Data.TCX;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls.Maps;
using System;
using Fity.Utils.Interpolation;

namespace Fity.Utils
{
    internal class GprxExtended
    {
        private Gprx gprx;

        private TrackpointExtended[] trackpoints => gprx.TrainingCenterDatabase.Activities.Activities.First().Lap.Trackpoints.Trackpoints.Select(tp => new TrackpointExtended(tp)).ToArray();

        private IEnumerable<TrackpointExtended> trackpointsWithPosition => trackpoints.Where(tp => tp.IsValid && tp.HasPosition).ToList();

        private IEnumerable<TrackpointExtended> trackpointsWithHeartRate => trackpoints.Where(tp => tp.IsValid && tp.HeartRateBpm?.Value != null).ToList();

        public GprxExtended(Gprx gprx)
        {
            this.gprx = gprx;
        }

        public IEnumerable<MapElement> GetMapElements()
        {
            foreach (var trackpoint in trackpoints)
            {
                if (trackpoint.Position != null)
                {
                    var icon = new MapIcon();
                    icon.Location = new Windows.Devices.Geolocation.Geopoint(new Windows.Devices.Geolocation.BasicGeoposition
                    {
                        Latitude = trackpoint.Position.LatitudeDegrees,
                        Longitude = trackpoint.Position.LongitudeDegrees
                    });
                    yield return icon;
                }
            }
        }

        public bool HasGps()
        {
            return this.trackpointsWithPosition.Any();
        }

        public bool HasHeartRate()
        {
            return this.trackpointsWithHeartRate.Any();
        }

        public Tuple<double, double, int> GetDefaultLocationWithWeights()
        {
            return new Tuple<double, double, int>(
                trackpointsWithPosition.Average(tp => tp.Position.LatitudeDegrees),
                trackpointsWithPosition.Average(tp => tp.Position.LongitudeDegrees),
                trackpointsWithPosition.Count());
        }

        public Position GetInterpolatedGps(DateTime time)
        {
            var latitudeInterpolater = new LinearInterpolater(this.trackpointsWithPosition.Select(tp => new InterpolationDatapoint
            {
                Time = tp.TimeUtc.Value,
                Value = tp.Position.LatitudeDegrees
            }));
            var longitudeInterpolater = new LinearInterpolater(this.trackpointsWithPosition.Select(tp => new InterpolationDatapoint
            {
                Time = tp.TimeUtc.Value,
                Value = tp.Position.LongitudeDegrees
            }));
            return new Position
            {
                LatitudeDegrees = latitudeInterpolater.GetAtTime(time),
                LongitudeDegrees = longitudeInterpolater.GetAtTime(time)
            };
        }

        public HeartRateInBeatsPerMinute GetInterpolatedHeartRate(DateTime time)
        {
            var heartrateInterpolator = new LinearInterpolater(this.trackpointsWithHeartRate.Select(tp => new InterpolationDatapoint
            {
                Time = tp.TimeUtc.Value,
                Value = tp.HeartRateBpm.Value
            }));
            return new HeartRateInBeatsPerMinute
            {
                Value = (float)heartrateInterpolator.GetAtTime(time)
            };
        }
    }
}