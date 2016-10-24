using System.Xml.Serialization;

namespace Fity.Data.TCX
{
    public class TrackpointCollection
    {
        [XmlElement("Trackpoint")]
        public Trackpoint[] Trackpoints { get; set; }
    }
}