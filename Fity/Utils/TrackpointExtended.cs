using Fity.Data.TCX;
using System;

namespace Fity.Utils
{
    public class TrackpointExtended
    {
        public TrackpointExtended(Trackpoint tp)
        {
            this.HeartRateBpm = tp.HeartRateBpm;
            this.Position = tp.Position;

            DateTime result;
            if (DateTime.TryParse(tp.Time, out result))
            {
                this.TimeUtc = result;
            }
            else
            {
                this.IsValid = false;
            }
        }

        private TrackpointExtended(TrackpointExtended tp, HeartRateInBeatsPerMinute newHeartRate = null, Position newPosition = null)
        {
            this.HeartRateBpm = newHeartRate ?? tp.HeartRateBpm;
            this.Position = newPosition ?? tp.Position;
            this.TimeUtc = tp.TimeUtc;
            this.IsValid = tp.IsValid;
        }

        public TrackpointExtended UpdateClone(HeartRateInBeatsPerMinute newHeartRate = null, Position newPosition = null)
        {
            if ((newHeartRate == null || newHeartRate == this.HeartRateBpm) && (newPosition == null || newPosition == this.Position))
            {
                return this;
            }

            return new TrackpointExtended(this, newHeartRate, newPosition);
        }

        public readonly Position Position;

        public bool HasPosition => this.Position?.LatitudeDegrees != null && this.Position?.LongitudeDegrees != null;

        public readonly HeartRateInBeatsPerMinute HeartRateBpm;

        public bool HasHeartRate => this.HeartRateBpm?.Value != null;

        public DateTime TimeUtc { get; private set; }

        public bool IsValid { get; private set; } = true;
    }
}