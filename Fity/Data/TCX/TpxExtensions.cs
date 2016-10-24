using System.Xml.Serialization;

namespace Fity.Data.TCX
{
    public class TpxExtensions
    {
        [XmlElement("Speed")]
        public double Speed { get; set; }
    }
}