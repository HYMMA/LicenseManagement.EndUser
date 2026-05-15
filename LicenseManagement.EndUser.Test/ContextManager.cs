using LicenseManagement.EndUser.Models;
using System.Data;

namespace LicenseManagement.EndUser.Test
{
    internal static class ContextManager
    {
        internal static string ApiKey => "MST_01JAP1JE7FRCJ63FHE5DQJGPY3_xYnvvm6HMOwMWtcGkn3NIrVI34LGTHm1i1tE4qQ7d5Y95iIR3L__vr-m4v6POxnF_JKsM23b-bYpLTM8zLzv3g";

        /// <summary>
        /// The server's current RSA public key for license signature validation.
        /// Set by <see cref="Server.TestServer"/> constructor on each test class fixture creation.
        /// Keeps in sync automatically when TestSetup regenerates keys.
        /// </summary>
        internal static string ServerPublicKey { get; set; } = string.Empty;
        internal static LicHandlingContext FromLic(LicenseModel license, uint trial = 0)
        {
            int validDays = 90;
            if (license.Expires != null && license.Created != null)
            {
                validDays = (license.Expires - license.Created).Value.Days;
            }
            return GetContext(license.Product.Id, license.Product.Vendor.Id, trial, license.Computer.MacAddress, license.Computer.Name, ((uint)validDays));
        }

        internal static LicHandlingContext GetContext(string productId, string publisher, uint trial, string? macAddress = "", string? machineName = "", uint validDays = 90)
        {
            return new LicHandlingContext(new PublisherPreferences(publisher, productId, ApiKey)
            {
                PublicKey = ServerPublicKey,
                ValidDays = validDays
            });
        }
    }
}
