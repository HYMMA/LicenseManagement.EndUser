using System;
using System.Runtime.Serialization;

namespace LicenseManagement.EndUser.Exceptions
{
    [Serializable]
    internal class LicenseExpiredException : Exception
    {
        public LicenseExpiredException()
        {
        }

        public LicenseExpiredException(string message) : base(message)
        {
        }

        public LicenseExpiredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LicenseExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}