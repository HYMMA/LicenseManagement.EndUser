using System.Threading.Tasks;

namespace LicenseManagement.EndUser.License.Handlers
{
    internal class ValidTrialHandler : LicenseValidationHandler
    {

        void SetNextHandler(LicHandlingContext context)
        {
            context.RaiseOnTrialValidated();
            if (context.LicenseModel.Receipt != null && !string.IsNullOrEmpty(context.LicenseModel.Receipt.Code))
                SetNext(new PatchLicenseWithReceiptHandler());
            else
                SetNext(new LastLicenseHandler());
        }


        public override void HandleContext(LicHandlingContext context)
        {
            SetNextHandler(context);
            nextHandler.HandleContext(context);
        }

        public override Task HandleContextAsync(LicHandlingContext context)
        {
            SetNextHandler(context);
            return nextHandler.HandleContextAsync(context);
        }
    }
}
