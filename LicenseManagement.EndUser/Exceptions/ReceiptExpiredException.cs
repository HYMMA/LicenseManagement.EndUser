using System;
using System.Runtime.Serialization;

namespace LicenseManagement.EndUser.Exceptions
{
    [Serializable]
    internal class ReceiptExpiredException : Exception
    {
        public ReceiptExpiredException()
        {
        }

        public ReceiptExpiredException(string message) : base(message)
        {
        }

        public ReceiptExpiredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ReceiptExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}