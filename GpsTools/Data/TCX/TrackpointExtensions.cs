using System.Xml.Serialization;

namespace GpsTools.Data.TCX
{
    public class TrackpointExtensions
    {
        [XmlElement("TPX", Namespace = "http://www.garmin.com/xmlschemas/ActivityExtension/v2")]
        public TpxExtensions TPX { get; set; }
    }
}