using System;
using System.Runtime.Serialization;

namespace Hymma.Lm.EndUser.Exceptions
{
    [Serializable]
    public class ReceiptCodeException : Exception
    {
        public ReceiptCodeException() { }
        public ReceiptCodeException(string message) : base(message) { }
        public ReceiptCodeException(string message, Exception inner) : base(message, inner) { }
        protected ReceiptCodeException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }
}