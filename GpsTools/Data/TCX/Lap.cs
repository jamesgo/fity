using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsTools.Data.TCX
{
    public class Lap
    {
        public DateTime? StartTime { get; set; }

        public float TotalTimeSeconds { get; set; }

        public float DistanceMeters { get; set; }

        public float MaximumSpeed { get; set; }

        public float Calories { get; set; }

        public float AverageHeartRateBpm { get; set; }

        public float MaximumHeartRateBpm { get; set; }

        public string Intensity { get; set; }

        public string TriggerMethod { get; set; }

        public IEnumerable<Trackpoint> Track { get; set; }
    }
}
