using Fity.Models;
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
        private readonly IEnumerable<Session> sessions;

        public IEnumerable<Trackpoint> TrackpointsWithPosition => this.sessions.SelectMany(s => s.Activities.SelectMany(a => a.Lap.TrackpointsWithPosition));

        public IEnumerable<Trackpoint> TrackpointsWithHeartRate => this.sessions.SelectMany(s => s.Activities.SelectMany(a => a.Lap.TrackpointsWithHeartRate));

        public bool HasHeartRate => this.TrackpointsWithHeartRate.Count() >= 2;

        public ActivityMerger(IEnumerable<Session> sessions)
        {
            this.sessions = sessions;
        }

        public Session GetMerged()
        {
            var orderedActivities = this.sessions.SelectMany(s => s.Activities).OrderBy(a => a.Lap.StartTime).ToList();

            // TODO: Copy non-null properties from all sessions to fill output session.
            var session = new Session()
            {
                Activities = new List<Activity>
                {
                    new Activity
                    {
                        Lap = new Lap(orderedActivities.First().Lap.StartTime)
                        {

                            Trackpoints = GetMergedTrackpoints()
                        }
                    }
                }
            };

            return session;
        }


        private IEnumerable<Trackpoint> GetMergedTrackpoints()
        {
            var mergedTrackpoints = this.sessions.SelectMany(s => s.Activities.SelectMany(a => a.Lap.TrackpointsWithPosition));

            foreach (var trackpoint in mergedTrackpoints)
            {
                if (this.HasHeartRate && !trackpoint.HasHeartRate)
                {
                    yield return trackpoint.UpdateClone(this.GetInterpolatedHeartRate(trackpoint.TimeUtc));
                }
                else
                {
                    yield return trackpoint;
                }
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
                Value = tp.HeartRate.Value
            }));
            return new HeartRateInBeatsPerMinute
            {
                Value = (float)heartrateInterpolator.GetAtTime(time)
            };
        }
    }
}
