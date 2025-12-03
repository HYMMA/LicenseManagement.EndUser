using System;
using System.Runtime.Serialization;

namespace Hymma.Lm.EndUser.Exceptions
{
    [Serializable]
    public class ReceiptNotAvailableException : Exception
    {
        public ReceiptNotAvailableException() { }
        public ReceiptNotAvailableException(string message) : base(message) { }
        public ReceiptNotAvailableException(string message, Exception inner) : base(message, inner) { }
        protected ReceiptNotAvailableException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }
}