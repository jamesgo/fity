using Fity.Data.TCX;
using System;

namespace Fity.Utils
{
    internal class TrackpointExtended
    {
        private Trackpoint tp;

        public TrackpointExtended(Trackpoint tp)
        {
            this.tp = tp;
        }

        public Position Position { get { return this.tp.Position; } }

        public bool HasPosition { get { return this.tp.Position?.LatitudeDegrees != null && this.tp.Position?.LongitudeDegrees != null; } }

        public HeartRateInBeatsPerMinute HeartRateBpm { get { return this.tp.HeartRateBpm; } }

        private DateTime? timeUtc;

        public DateTime? TimeUtc
        {
            get
            {
                if (this.timeUtc == null)
                {
                    DateTime result;
                    if (DateTime.TryParse(this.tp.Time, out result))
                    {
                        this.timeUtc = result;
                    }
                }
                return this.timeUtc;
            }
        }

        public bool IsValid { get { return this.TimeUtc != null; } }
    }
}