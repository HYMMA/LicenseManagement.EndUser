using System;

namespace LicenseManagement.EndUser.Exceptions
{

    [Serializable]
    public class ProductNameException : Exception
    {
        public ProductNameException() { }
        public ProductNameException(string message) : base(message) { }
        public ProductNameException(string message, Exception inner) : base(message, inner) { }
        protected ProductNameException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
