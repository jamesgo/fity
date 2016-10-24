using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fity.Data.TCX
{
    [XmlInclude(typeof(HeartRateInBeatsPerMinute))]
    public class Trackpoint
    {
        [XmlElement("Time")]
        public string Time { get; set; }

        [XmlIgnore]
        public DateTime? TimeUtc => this.Time != null ? (DateTime?)DateTime.Parse(this.Time) : null;

        [XmlElement("DistanceMeters")]
        public double DistanceMeters { get; set; }

        [XmlElement("Position")]
        public Position Position { get; set; }

        [XmlElement("AltitudeMeters")]
        public float AltitudeMeters { get; set; }

        [XmlElement("HeartRateBpm")]
        public HeartRateInBeatsPerMinute HeartRateBpm { get; set; }

        [XmlElement("Extensions")]
        public TrackpointExtensions Extensions { get; set; }
    }
}
