using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fity.Models
{
    public class Lap
    {
        public Lap(DateTime startTime)
        {
            this.StartTime = startTime;
        }

        public DateTime StartTime { get; private set; }

        public float TotalTimeSeconds { get; set; }

        public float DistanceMeters { get; set; }

        public float MaximumSpeed { get; set; }

        public float Calories { get; set; }

        public HeartRateInBeatsPerMinute AverageHeartRateBpm { get; set; }

        public HeartRateInBeatsPerMinute MaximumHeartRateBpm { get; set; }

        public string Intensity { get; set; }

        public string TriggerMethod { get; set; }

        public IEnumerable<Trackpoint> Trackpoints { get; set; }
        public IEnumerable<Trackpoint> TrackpointsWithPosition => this.Trackpoints.Where(tp => tp.HasPosition);
        public IEnumerable<Trackpoint> TrackpointsWithHeartRate => this.Trackpoints.Where(tp => tp.HasHeartRate);
    }
}
