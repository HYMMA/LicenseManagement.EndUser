using System;

namespace LicenseManagement.EndUser.Models
{
    public class ComputerModel : DbModelWithDate, IEquatable<ComputerModel>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string MacAddress { get; set; }
        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Equals((ComputerModel)obj);
        }

        public static bool operator !=(ComputerModel other,ComputerModel another)
        {
            return !(other == another); 
        }
        public static bool operator == (ComputerModel other, ComputerModel another)
        {
            return Equals(other, another);
        }
        public bool Equals(ComputerModel other)
        {
            if (ReferenceEquals(this,other))
                return true;
            
            if (other is null)
                return false;
            return Id == other.Id && MacAddress == other.MacAddress; 
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ MacAddress.GetHashCode() ;
        }
    }
}
