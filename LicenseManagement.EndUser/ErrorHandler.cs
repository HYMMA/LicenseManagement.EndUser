using LicenseManagement.EndUser.Exceptions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser
{
    internal class ErrorHandler : LicenseValidationHandler
    {
        public override void HandleContext(LicHandlingContext context)
        {
            throw Translate(context.Exception);
        }

        public override Task HandleContextAsync(LicHandlingContext context)
        {
            return Task.FromException(Translate(context.Exception));
        }

        // Both code paths must surface the same typed exception for the same underlying failure.
        // HttpRequestException historically meant "no network"; map it to NetworkUnavailableException
        // so callers can branch on it cleanly instead of pattern-matching on free-form message text.
        private static Exception Translate(Exception source)
        {
            switch (source)
            {
                case null:
                    return new InvalidOperationException("ErrorHandler invoked without an exception in context.");
                case HttpRequestException httpEx:
                    return new NetworkUnavailableException(
                        "Could not connect to the license server. Check your internet connection, DNS, and firewall/proxy settings.",
                        httpEx);
                default:
                    return source;
            }
        }
    }
}
