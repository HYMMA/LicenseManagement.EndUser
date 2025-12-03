using System;
using System.Net;

namespace Hymma.Lm.EndUser.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException() { }
        public ApiException(string message, HttpStatusCode statusCode) : base(message+ " Status Code = " + statusCode) { }
        public ApiException(string message, Exception inner) : base(message, inner) { }
        protected ApiException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
