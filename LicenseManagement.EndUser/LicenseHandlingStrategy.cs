using System;
using System.Threading.Tasks;
using Hymma.Lm.EndUser.Registrars;
using Hymma.Lm.EndUser.Models;
namespace Hymma.Lm.EndUser
{
    public abstract class LicenseHandlingStrategy
    {
        internal LicenseHandlingStrategy(LicHandlingContext context, Action<LicenseModel> OnLicenseHandledSuccessfully)
        {
            HandlingContext = context;
            LicenseRegister = new LicenseRegister(HandlingContext);

            if (OnLicenseHandledSuccessfully != null)
                HandlingContext.OnLicenseHandledSuccessfully += (l) => OnLicenseHandledSuccessfully.Invoke(l);
        }

        internal LicenseRegister LicenseRegister { get; }
        internal LicenseValidationHandler NextHandler { get; set; }
        public LicHandlingContext HandlingContext { get; }
        public abstract Task HandleLicenseAsync();
        public abstract void HandleLicense();
    }
}