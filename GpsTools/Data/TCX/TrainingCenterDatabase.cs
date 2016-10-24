using System.Xml.Serialization;

namespace GpsTools.Data.TCX
{
    [XmlRoot("TrainingCenterDatabase", Namespace = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2", IsNullable = false)]
    [XmlInclude(typeof(Author))]
    public class TrainingCenterDatabase
    {
        [XmlElement("Activities")]
        public ActivityCollection Activities { get; set; }

        [XmlElement("Author")]
        public Author Author { get; set; }
    }
}
