using LicenseManagement.EndUser.Models;
using System;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser
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
            // Always resolve machine identity from hardware (ComputerId.Instance.MachineId),
            // never from the license file. This prevents a shared or deleted .lic file from
            // letting one machine unregister another's seat. ApiGetComputerHandler GETs the
            // server-side Computer record by the live hardware ID, then the chain POSTs the
            // license (409 = already exists → fetch it), unregisters the receipt, and writes
            // the updated license back to disk via LastLicenseHandler.
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
            await NextHandler.HandleContextAsync(HandlingContext).ConfigureAwait(false);
        }
    }
}
