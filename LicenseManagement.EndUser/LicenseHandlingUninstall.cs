using Hymma.Lm.EndUser.Models;
using System;
using System.Threading.Tasks;

namespace Hymma.Lm.EndUser
{
    /// <summary>
    /// should be called during uninstall to unregister the computer from the license
    /// </summary>
    public class LicenseHandlingUninstall : LicenseHandlingStrategy
    {
        public LicenseHandlingUninstall(LicHandlingContext context,
                                        Action<LicenseModel> OnLicenseHandledSuccessfully = null) : base(context, OnLicenseHandledSuccessfully)
        {
            HandlingContext.ContextEnvironment = HandlerStrategy.UnInstall;
        }

        void SetNextHandler()
        {

            //we must have internet so we can un
            NextHandler = new ApiGetComputerHandler();
        }
        public override void HandleLicense()
        {
            SetNextHandler();
            NextHandler.HandleContext(HandlingContext);
        }

        public override async Task HandleLicenseAsync()
        {
            SetNextHandler();
            await NextHandler.HandleContextAsync(HandlingContext);
        }
    }
}
