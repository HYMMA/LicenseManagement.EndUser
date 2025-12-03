using Hymma.Lm.EndUser.License;
using Hymma.Lm.EndUser.Models;
using Hymma.Lm.EndUser.Test.Server;

namespace Hymma.Lm.EndUser.Test.Utilities
{
    public class LicGenerator
    {
        private TestServer server;

        public LicGenerator(TestServer server)
        {
            this.server = server;
        }
        public async Task<LicenseModel> SaveNewLicOnDiskAsync(LicenseStatusTitles title, uint trial = 10)
        {
            var lic = await server.RegisterRandomLicenseAsync(title);
            //var signedLic = await server.SignLicenseAsync(lic);
            var context = ContextManager.FromLic(lic, trial);
            var installHandler = new LicenseHandlingInstall(context, (l) => { lic = l; });
            installHandler.HandleLicense();


            //context.SetLicenseData(signedLic, false);

            //write on disk
            //var register = new LicenseRegister(context);
            //register.TryWrite();
            return lic;
        }
    }
}
