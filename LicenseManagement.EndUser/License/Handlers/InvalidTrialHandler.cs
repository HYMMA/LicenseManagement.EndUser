using LicenseManagement.EndUser.Exceptions;
using System;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser.License.Handlers
{
    internal class InvalidTrialHandler : LicenseValidationHandler
    {
        //makes sure we don't go through a loop next time we are here. sets the context.IsLicenseFreshOurOfServer to true
        private void GetLicenseFileFromServer() =>
            SetNext(new ApiGetLicenseHandler());

        // Check if the server has extended the trial by comparing TrialEndDate before and after refetch
        private bool TrialExtended(LicHandlingContext context)
        {
            var trialEndDateBefore = context.LicenseModel.TrialEndDate;

            // Notify the publisher that trial has ended - they may trigger a server-side extension
            context.RaiseOnTrialEnded();

            // If we have fresh data from server, check if TrialEndDate was extended
            if (context.IsLicenseFreshOutOfServer)
            {
                return context.LicenseModel.TrialEndDate > trialEndDateBefore;
            }

            return false;
        }

        void SetNextHandler(LicHandlingContext context)
        {
            if (TrialExtended(context))
                SetNext(new LicenseStatusHandler());
            else
            {
                if (context.IsLicenseFreshOutOfServer)
                    PatchLicenseWithReceipt(context);

                else
                    GetLicenseFileFromServer();
            }
        }

        private void PatchLicenseWithReceipt(LicHandlingContext context)
        {
            context.RaiseOnCustomerMustEnterProductKey();
            if (context.LicenseModel.Receipt != null && !string.IsNullOrEmpty(context.LicenseModel.Receipt.Code))
                SetNext(new PatchLicenseWithReceiptHandler());
            else
                SetNext(new LastLicenseHandler());
            //ThrowTrialEndedException(context);
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
