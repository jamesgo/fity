using System.Xml.Serialization;

namespace Fity.Data.TCX
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