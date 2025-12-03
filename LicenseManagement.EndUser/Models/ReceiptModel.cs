using System;

namespace Hymma.Lm.EndUser.Models
{
    public class ReceiptModel : DbModelWithDate, IEquatable<ReceiptModel>
    {
        private string _code;
        private DateTime? _expires;
        public string Id { get; set; }
        /// <summary>
        /// this is the hash of the email and transaction id. will be used as identifier for a customer purchase
        /// </summary>
        public string Code
        {
            get => _code;
            set
            {
                if (_code != value)
                {
                    _code = value;
                }
            }
        }

        public int Qty { get; set; }

        public ProductModel Product { get; set; }

        /// <summary>
        /// the email of the customer who purchased the product
        /// </summary>
        public string BuyerEmail { get; set; }

        /// <summary>
        /// indicates when a receipt expires. in a one-year subscription product this will be one year from date of creation
        /// </summary>
        public DateTime? Expires
        {
            get => _expires;
            set
            {
                if (_expires != value)
                {
                    _expires = value;
                }
            }
        }

        // override object.Equals
        public override bool Equals(object other)
        {
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238

            if (other == null || GetType() != other.GetType())
                return false;
            return Equals(other as ReceiptModel);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Qty.GetHashCode() ^ Id.GetHashCode() ^ Updated.GetHashCode() ^ Expires.GetHashCode();
        }

        public bool Equals(ReceiptModel other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other is null)
                return false;
            return other.Qty == Qty 
                && other.Id == Id 
                && other.Code == Code 
                && other.Product == Product 
                && other.Updated == Updated 
                && other.Expires == this.Expires;
        }
        public static bool operator !=(ReceiptModel other, ReceiptModel another)
        {
            return !(other == another);
        }
        public static bool operator ==(ReceiptModel other, ReceiptModel another)
        {
            return Equals(other, another);
        }
    }
}
