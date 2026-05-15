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
            // Read the existing on-disk license to get its ID for the server-side PATCH.
            // Unlike install/launch, uninstall does not need to re-register the computer.
            var register = new Registrars.LicenseRegister(HandlingContext);
            if (register.TryRead(out string signedLic))
            {
                HandlingContext.SetLicenseData(signedLic, false);
                NextHandler = new Signature.Handlers.LicenseSignatureValidationHandler();
            }
            else
            {
                HandlingContext.Exception = new Exceptions.CouldNotReadLicenseFromDiskException();
                NextHandler = new ErrorHandler();
            }
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
