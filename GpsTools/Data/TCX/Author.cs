using System.Xml.Serialization;

namespace GpsTools.Data.TCX
{
    [XmlType("Application_t")]
    public class Author
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("LangID")]
        public string LangID { get; set; }
    }
}