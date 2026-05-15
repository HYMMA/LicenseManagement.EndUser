using System;

namespace LicenseManagement.EndUser.Exceptions
{
    /// <summary>
    /// Thrown when the library cannot reach the license server. Distinct from <see cref="ComputerOfflineException"/>
    /// in that this also covers DNS failures, TLS handshake failures, and proxy/firewall blocks.
    /// </summary>
    [Serializable]
    public class NetworkUnavailableException : Exception
    {
        public NetworkUnavailableException()
            : base("Could not connect to the license server. Check internet connection, DNS, and firewall/proxy settings.") { }

        public NetworkUnavailableException(string message, Exception inner) : base(message, inner) { }

        protected NetworkUnavailableException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
