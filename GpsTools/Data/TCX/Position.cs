using System.Xml.Serialization;

namespace GpsTools.Data.TCX
{
    public class Position
    {
        [XmlElement("LatitudeDegrees")]
        public double LatitudeDegrees { get; set; }

        [XmlElement("LongitudeDegrees")]
        public double LongitudeDegrees { get; set; }
    }
}