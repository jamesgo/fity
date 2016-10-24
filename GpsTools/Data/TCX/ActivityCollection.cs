using System.Xml.Serialization;

namespace GpsTools.Data.TCX
{
    [XmlRoot("Activities")]
    public class ActivityCollection
    {
        [XmlElement("Activity")]
        public Activity[] Activities { get; set; }
    }
}