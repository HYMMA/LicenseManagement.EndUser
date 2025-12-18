using System;
using System.Runtime.Serialization;

namespace LicenseManagement.EndUser.Exceptions
{
    internal class NotUnregisteredException : Exception
    {
        public NotUnregisteredException()
        {
        }

        public NotUnregisteredException(string message) : base(message)
        {
        }

        public NotUnregisteredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotUnregisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
