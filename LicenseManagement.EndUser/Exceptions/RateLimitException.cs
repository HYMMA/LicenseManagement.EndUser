using System;

namespace LicenseManagement.EndUser.Exceptions
{
    /// <summary>
    /// Thrown when the license server returned HTTP 429 and the library exhausted its retry budget.
    /// </summary>
    [Serializable]
    public class RateLimitException : Exception
    {
        /// <summary>
        /// The Retry-After value sent by the server, if any.
        /// </summary>
        public TimeSpan? RetryAfter { get; }

        public RateLimitException() : base("License server rate-limited the request.") { }

        public RateLimitException(string message, TimeSpan? retryAfter = null) : base(message)
        {
            RetryAfter = retryAfter;
        }

        public RateLimitException(string message, Exception inner) : base(message, inner) { }

        protected RateLimitException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
