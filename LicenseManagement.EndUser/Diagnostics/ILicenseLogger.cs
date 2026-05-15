using System;

namespace LicenseManagement.EndUser.Diagnostics
{
    /// <summary>
    /// Severity level for log entries emitted by the library.
    /// </summary>
    public enum LicenseLogLevel
    {
        Debug,
        Information,
        Warning,
        Error
    }

    /// <summary>
    /// Optional consumer-supplied logger. The library calls this for HTTP plumbing diagnostics
    /// (request URL, status code, latency, masked API key, correlation id) and for handler-chain
    /// transitions. Pass an instance via <see cref="LicenseHandlingOptions.Logger"/> to surface
    /// failures from the field.
    /// </summary>
    /// <remarks>
    /// The library never logs API keys, license bodies, or PII. Implementations are responsible
    /// for forwarding to whatever backend the host application uses (Serilog, NLog, ILogger, etc.).
    /// </remarks>
    public interface ILicenseLogger
    {
        void Log(LicenseLogLevel level, string message, Exception exception = null);
    }

    /// <summary>
    /// Default no-op implementation. Used when the consumer doesn't supply a logger.
    /// </summary>
    internal sealed class NullLicenseLogger : ILicenseLogger
    {
        internal static readonly NullLicenseLogger Instance = new NullLicenseLogger();
        public void Log(LicenseLogLevel level, string message, Exception exception = null) { }
    }
}
