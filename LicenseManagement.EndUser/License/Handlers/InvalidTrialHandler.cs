using Hymma.Lm.EndUser.Exceptions;
using System;
using System.Threading.Tasks;

namespace Hymma.Lm.EndUser.License.Handlers
{
    internal class InvalidTrialHandler : LicenseValidationHandler
    {

        //makes sure we don't go through a loop next time we are here. sets the context.IsLicenseFreshOurOfServer to true 
        private void GetLicenseFileFromServer() =>
            SetNext(new ApiGetLicenseHandler());

        //if extra value has been granted to the trial
        //sometimes the publisher might decide to grant extra trial period after the initial value is over
        //this is a marketing strategy like snagit does.
        private bool TrialExtended(LicHandlingContext context)
        {
            var beforeEvent = context.PublisherPreferences.TrialDays;

            //allow the publisher to update the value 
            context.RaiseOnTrialEnded();
            return context.PublisherPreferences.TrialDays > beforeEvent;
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
