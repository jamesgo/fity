using System;
using System.Xml.Serialization;

namespace Fity.Data.TCX
{
    [XmlRoot("Activities")]
    public class ActivityCollection
    {
        [XmlElement("Activity")]
        public Activity[] Activities { get; set; }
    }
}