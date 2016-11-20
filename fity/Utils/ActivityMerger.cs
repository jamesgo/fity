using Fity.Data.TCX;
using Fity.Utils.Interpolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fity.Utils
{
    public class ActivityMerger
    {
        private readonly IEnumerable<GprxExtended> activities;

        public IEnumerable<TrackpointExtended> TrackpointsWithPosition => this.activities.SelectMany(a => a.TrackpointsWithPosition);

        public IEnumerable<TrackpointExtended> TrackpointsWithHeartRate => this.activities.SelectMany(a => a.TrackpointsWithHeartRate);

        public ActivityMerger(IEnumerable<GprxExtended> activities)
        {
            this.activities = activities;
        }

        public IEnumerable<TrackpointExtended> GetMerged()
        {
            var mergedTrackpoints = this.activities.SelectMany(a => a.TrackpointsWithPosition);

            foreach (var trackpoint in mergedTrackpoints.Where(tp => !tp.HasHeartRate))
            {
                yield return trackpoint.UpdateClone(this.GetInterpolatedHeartRate(trackpoint.TimeUtc));
            }
        }

        public Position GetInterpolatedGps(DateTime time)
        {
            var latitudeInterpolater = new LinearInterpolater(this.TrackpointsWithPosition.Select(tp => new InterpolationDatapoint
            {
                Time = tp.TimeUtc,
                Value = tp.Position.LatitudeDegrees
            }));
            var longitudeInterpolater = new LinearInterpolater(this.TrackpointsWithPosition.Select(tp => new InterpolationDatapoint
            {
                Time = tp.TimeUtc,
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
            var heartrateInterpolator = new LinearInterpolater(this.TrackpointsWithHeartRate.Select(tp => new InterpolationDatapoint
            {
                Time = tp.TimeUtc,
                Value = tp.HeartRateBpm.Value
            }));
            return new HeartRateInBeatsPerMinute
            {
                Value = (float)heartrateInterpolator.GetAtTime(time)
            };
        }
    }
}
