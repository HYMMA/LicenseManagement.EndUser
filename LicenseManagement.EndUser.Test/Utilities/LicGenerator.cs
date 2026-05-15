using LicenseManagement.EndUser.License;
using LicenseManagement.EndUser.Models;
using LicenseManagement.EndUser.Registrars;
using LicenseManagement.EndUser.Test.Server;

namespace LicenseManagement.EndUser.Test.Utilities
{
    public class LicGenerator
    {
        private TestServer server;

        public LicGenerator(TestServer server)
        {
            this.server = server;
        }

        /// <summary>
        /// Gets a signed license from the test server for the given status and writes it to disk.
        /// Uses the seed-data computer/product IDs directly rather than the full install chain,
        /// because the install chain relies on the real machine's DeviceId which is not in the seed DB.
        /// </summary>
        public async Task<LicenseModel> SaveNewLicOnDiskAsync(LicenseStatusTitles title, uint trial = 10)
        {
            var lic = await server.RegisterRandomLicenseAsync(title);
            var context = ContextManager.FromLic(lic, trial);

            // Get signed XML for the seed license and write directly to disk.
            // This avoids the full install chain (ApiPostComputerHandler etc.) that requires
            // the real machine's DeviceId to be registered on the test server.
            var signedXml = await server.GetSignedLicenseXmlAsync(lic);
            context.SetLicenseData(signedXml, false);
            var register = new LicenseRegister(context);
            register.TryWrite();

            return lic;
        }
    }
}
