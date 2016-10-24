using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fity.Data.TCX
{
    public class Lap
    {
        [XmlAttribute("StartTime")]
        public string StartTime { get; set; }

        [XmlIgnore]
        public DateTime? StartTimeUtc => this.StartTime != null ? (DateTime?)DateTime.Parse(this.StartTime) : null;

        [XmlElement("TotalTimeSeconds")]
        public float TotalTimeSeconds { get; set; }

        [XmlElement("DistanceMeters")]
        public float DistanceMeters { get; set; }

        [XmlElement("MaximumSpeed")]
        public float MaximumSpeed { get; set; }

        [XmlElement("Calories")]
        public float Calories { get; set; }

        [XmlElement("AverageHeartRateBpm")]
        public HeartRateInBeatsPerMinute AverageHeartRateBpm { get; set; }

        [XmlElement("MaximumHeartRateBpm")]
        public HeartRateInBeatsPerMinute MaximumHeartRateBpm { get; set; }

        [XmlElement("Intensity")]
        public string Intensity { get; set; }

        [XmlElement("TriggerMethod")]
        public string TriggerMethod { get; set; }

        [XmlElement("Track")]
        public TrackpointCollection Trackpoints { get; set; }
    }
}
