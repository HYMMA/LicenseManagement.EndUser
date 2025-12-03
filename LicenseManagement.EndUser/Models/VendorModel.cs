using System;
using System.Xml.Serialization;

namespace Hymma.Lm.EndUser.Models
{
    [XmlType("Vendor")]
    public class VendorModel : DbModelWithDate, IEquatable<VendorModel>
    {
        /// <summary>
        /// this is the ulid of the vendor that starts with VRD_
        /// </summary>
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Equals(VendorModel other)
        {
            if (ReferenceEquals(this, other))
                return true;

            //if only the other is null
            if (other is null || GetType() != other.GetType())
                return false;

            return other.Name == Name 
                && other.Id == Id 
                && other.Updated == Updated 
                && other.Created == Created;
        }
        public override int GetHashCode()
        {
            return
                 (string.IsNullOrEmpty(Id) ? 0 : Id.GetHashCode())
                ^ (string.IsNullOrEmpty(Name) ? 0 : Name.GetHashCode());
        }
    }
}
