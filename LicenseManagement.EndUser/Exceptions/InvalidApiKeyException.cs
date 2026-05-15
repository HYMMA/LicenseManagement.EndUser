using System;

namespace LicenseManagement.EndUser.Exceptions
{
    /// <summary>
    /// Thrown when the license server rejects the API key (HTTP 401 or 403).
    /// </summary>
    [Serializable]
    public class InvalidApiKeyException : Exception
    {
        public InvalidApiKeyException()
            : base("The supplied API key was rejected by the license server. Verify the key in PublisherPreferences.ApiKey.") { }

        public InvalidApiKeyException(string message) : base(message) { }
        public InvalidApiKeyException(string message, Exception inner) : base(message, inner) { }
        protected InvalidApiKeyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
