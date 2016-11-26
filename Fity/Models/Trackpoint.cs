using Fity.Data.TCX;
using System;

namespace Fity.Models
{
    public class Trackpoint
    {
        private Trackpoint(Trackpoint tp, HeartRateInBeatsPerMinute newHeartRate = null, Position newPosition = null)
        {
            this.HeartRate = newHeartRate ?? tp.HeartRate;
            this.Position = newPosition ?? tp.Position;
            this.TimeUtc = tp.TimeUtc;
            this.IsValid = tp.IsValid;
            this.AltitudeMeters = tp.AltitudeMeters;
            this.DistanceMeters = tp.DistanceMeters;
        }

        public Trackpoint(DateTime? result, Position position = null, HeartRateInBeatsPerMinute heartRate = null)
        {
            if (result.HasValue)
            {
                this.TimeUtc = result.Value;
            }
            else
            {
                IsValid = false;
            }

            this.Position = position;
            this.HeartRate = heartRate;
        }

        public Trackpoint UpdateClone(HeartRateInBeatsPerMinute newHeartRate = null, Position newPosition = null)
        {
            if ((newHeartRate == null || newHeartRate == this.HeartRate) && (newPosition == null || newPosition == this.Position))
            {
                return this;
            }

            return new Trackpoint(this, newHeartRate, newPosition);
        }

        public bool HasPosition => this.Position?.LatitudeDegrees != null && this.Position?.LongitudeDegrees != null;
        public bool HasHeartRate => this.HeartRate?.Value != null;

        public Position Position { get; internal set; }
        
        public HeartRateInBeatsPerMinute HeartRate { get; internal set; }

        public DateTime TimeUtc { get; private set; }

        public bool IsValid { get; private set; } = true;
        public double? Speed { get; internal set; }

        public double? DistanceMeters { get; internal set; }

        public float? AltitudeMeters { get; internal set; }
    }
}