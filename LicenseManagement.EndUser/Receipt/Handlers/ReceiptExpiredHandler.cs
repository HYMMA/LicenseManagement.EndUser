using LicenseManagement.EndUser.Exceptions;
using LicenseManagement.EndUser.License.Handlers;
using System.IO;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser.Receipt.Handlers
{

    /// <summary>
    /// this handler will just sets the next handler and does not change the state of the license 
    /// </summary>
    internal class ReceiptExpiredHandler : LicenseValidationHandler
    {

        //makes sure we don't go through a loop next time we are here. sets the context.IsLicenseFreshOurOfServer to true 
        private void GetLicenseFileFromServer() =>
            SetNext(new ApiGetLicenseHandler());
        private void PatchLicenseWithNewCode() =>
            SetNext(new PatchLicenseWithReceiptHandler());

        void SetNextHandler(LicHandlingContext context)
        {
            //make sure this is the latest state of the license
            if (context.IsLicenseFreshOutOfServer)
            {
                //receipt is not null when it is expired, it is null when it is unregistered
                var oldCode = context.LicenseModel.Receipt.Code;
                context.RaiseOnCustomerMustEnterProductKey();

                if (string.Equals(oldCode, context.LicenseModel.Receipt.Code, System.StringComparison.OrdinalIgnoreCase))
                {
                    SetNext(new LastLicenseHandler());
                }
                else
                {
                    PatchLicenseWithNewCode();
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

        public override async Task HandleContextAsync(LicHandlingContext context)
        {
            SetNextHandler(context);
            await nextHandler.HandleContextAsync(context);
        }
    }
}
