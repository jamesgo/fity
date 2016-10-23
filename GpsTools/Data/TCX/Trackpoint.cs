using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsTools.Data.TCX
{
    public class Trackpoint
    {
        public DateTime? Time { get; set; }

        public double DistanceMeters { get; set; }

        public double LatitudeDegrees { get; set; }

        public double LongitudeDegrees { get; set; }

        public float AltitudeMeters { get; set; }

        public float HeartRateBpm { get; set; }

        public double Speed { get; set; }
    }
}
