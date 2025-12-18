using System.Threading.Tasks;

namespace LicenseManagement.EndUser.License.Handlers
{
    internal class ReceiptUnregisteredHandler : LicenseValidationHandler
    {
        public ReceiptUnregisteredHandler() { }

        //makes sure we don't go through a loop next time we are here. sets the context.IsLicenseFreshOurOfServer to true 
        private void GetLicenseFileFromServer() =>
            SetNext(new ApiGetLicenseHandler());

        private void PatchLicenseWithNewCode() =>
            SetNext(new PatchLicenseWithReceiptHandler());

        private void SetNextHandler(LicHandlingContext context)
        {
            if (context.IsLicenseFreshOutOfServer)
            {
                //this should be null here
                var beforeEvent = context.LicenseModel.Receipt;
                context.RaiseOnCustomerMustEnterProductKey();
                if (beforeEvent != context.LicenseModel.Receipt)
                {
                    PatchLicenseWithNewCode();
                }
                else
                {
                    SetNext(new LastLicenseHandler());
                }
            }
            else
            {
                GetLicenseFileFromServer();
            }
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
