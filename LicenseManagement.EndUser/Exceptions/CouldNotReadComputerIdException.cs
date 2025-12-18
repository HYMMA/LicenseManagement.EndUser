using System;

namespace LicenseManagement.EndUser.Exceptions
{
    public class CouldNotReadComputerIdException : Exception
    {
        public CouldNotReadComputerIdException() { }
        public CouldNotReadComputerIdException(string message) : base(message) { }
        public CouldNotReadComputerIdException(string message, Exception inner) : base(message, inner) { }
        protected CouldNotReadComputerIdException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
