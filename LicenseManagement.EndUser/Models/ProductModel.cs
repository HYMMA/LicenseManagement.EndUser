using System;
using System.Collections.Generic;

namespace Hymma.Lm.EndUser.Models
{

    public class ProductModel : DbModelWithDate, IEquatable<ProductModel>
    {
        string _id;
        string _name;
        /// <summary>
        /// this is actually the Ulid of the product with prefix PRD_
        /// </summary>
        public string Id
        {
            get => _id; set
            {
                if (_id != value)
                {
                    _id = value;
                }
            }
        }
        /// <summary>
        /// the publisher
        /// </summary>
        public VendorModel Vendor { get; set; }

        /// <summary>
        /// this is the user defined name of the product
        /// </summary>
        public string Name
        {
            get => _name; set
            {
                if (_name != value)
                {
                    _name = value;
                }
            }
        }

        //[XmlArrayItem("Feature")]
        public List<string> Features { get; set; }

        public override int GetHashCode()
        {
            return
                (string.IsNullOrEmpty(Id) ? 0 : Id.GetHashCode())
                ^ (string.IsNullOrEmpty(Name) ? 0 : Name.GetHashCode())
                ^ (Vendor == null ? 0 : Vendor.GetHashCode())
                ^ (Created == null ? 0 : Created.GetHashCode())
                ^ (Updated == null ? 0 : Updated.GetHashCode());
        }

        public override bool Equals(object other)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (other is null || GetType() != other.GetType())
                return false;
            return Equals(other as ProductModel);
        }

        public bool Equals(ProductModel other)
        {
            if (ReferenceEquals(this, other))
                return true;

            //if only the other is null
            if (other is null || GetType() != other.GetType())
                return false;

            return other.Name == Name && other.Id == Id && other.Vendor.Equals(Vendor)  && other.Updated == Updated && other.Created == Created && other.Features == Features;
        }

        public static bool operator !=(ProductModel other, ProductModel another)
        {
            return !(other == another);
        }
        public static bool operator ==(ProductModel other, ProductModel another)
        {
            return Equals(other, another);
        }
    }
}
