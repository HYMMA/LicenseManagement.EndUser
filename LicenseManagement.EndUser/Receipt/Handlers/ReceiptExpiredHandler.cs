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
                // The receipt code is no longer carried in the signed license file (it is redacted /
                // removed on the server), so Receipt may be null and there is nothing on-file to
                // compare against. Prompt for a (renewed) code and re-patch only if the user actually
                // enters one that differs from what we had; otherwise keep the current license. Using
                // Receipt?.Code keeps this null-safe whether or not the file still carries a receipt.
                var oldCode = context.LicenseModel.Receipt?.Code;
                context.RaiseOnCustomerMustEnterProductKey();
                var newCode = context.LicenseModel.Receipt?.Code;

                if (string.IsNullOrEmpty(newCode)
                    || string.Equals(oldCode, newCode, System.StringComparison.OrdinalIgnoreCase))
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
            await nextHandler.HandleContextAsync(context).ConfigureAwait(false);
        }
    }
}
