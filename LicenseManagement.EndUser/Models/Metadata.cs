using System.Xml.Serialization;

namespace LicenseManagement.EndUser.Models
{
    /// <summary>
    /// A serializable key-value pair for XML serialization (Dictionary cannot be serialized by XmlSerializer)
    /// </summary>
    public class MetadataEntry
    {
        [XmlAttribute]
        public string Key { get; set; } = string.Empty;

        [XmlAttribute]
        public string Value { get; set; } = string.Empty;
    }
}
