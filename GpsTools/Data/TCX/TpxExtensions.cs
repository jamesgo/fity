using System.Xml.Serialization;

namespace GpsTools.Data.TCX
{
    public class TpxExtensions
    {
        [XmlElement("Speed")]
        public double Speed { get; set; }
    }
}