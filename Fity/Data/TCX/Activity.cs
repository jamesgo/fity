using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fity.Data.TCX
{
    [XmlRootAttribute("Activity")]
    public class Activity
    {
        [XmlAttribute("Sport")]
        public string Sport { get; set; }

        [XmlElement("Id")]
        public string Id { get; set; }

        [XmlElement("Notes")]
        public string Notes { get; set; }

        [XmlElement("Lap")]
        public Lap Lap { get; set; }
    }
}
