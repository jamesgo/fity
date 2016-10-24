using System.Xml.Serialization;

namespace GpsTools.Data.TCX
{
    public class TrackpointCollection
    {
        [XmlElement("Trackpoint")]
        public Trackpoint[] Trackpoints { get; set; }
    }
}