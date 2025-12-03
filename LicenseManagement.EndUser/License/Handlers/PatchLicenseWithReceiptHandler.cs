using Hymma.Lm.EndUser.Exceptions;
using Hymma.Lm.EndUser.License.EndPoint;
using System;
using System.Threading.Tasks;
using Hymma.Lm.EndUser.Models;


namespace Hymma.Lm.EndUser.License.Handlers
{
    internal sealed class PatchLicenseWithReceiptHandler : LicenseValidationHandler
    {
        public PatchLicenseWithReceiptHandler()
        {
        }

        PatchLicenseModel GetModel(LicHandlingContext context) =>
            new PatchLicenseModel()
            {
                License = context.LicenseModel.Id,
                Code = context.LicenseModel.Receipt?.Code
            };

        public override async Task HandleContextAsync(LicHandlingContext context)
        {
            //var beforeEvent = context.LicenseModel.Receipt;
            //context.RaiseOnCustomerMustEnterProductKey();

            //if customer did change the receipt
            //if (context.LicenseModel.Receipt != beforeEvent)
            //{
            var apiClient = new LicenseApiEndPoint(context.PublisherPreferences.ApiKey);
            try
            {
                var status = await apiClient.PatchLicenseAsync(GetModel(context));
                if (status == System.Net.HttpStatusCode.NoContent)
                {
                    SetNext(new ApiGetLicenseHandler());
                }
                else
                {
                    SetNextError(context, new CouldNotPatchLicenseWithReceiptException());
                }
            }
            catch (Exception e)
            {
                SetNextError(context, e);
            }
            //}
            //else
            //{
            //    SetNextError(context, new ReceiptCodeException());
            //}
            await nextHandler.HandleContextAsync(context);
        }

        public override void HandleContext(LicHandlingContext context)
        {
            //var beforeEvent = context.LicenseModel.Receipt;
            //context.RaiseOnCustomerMustEnterProductKey();
            //if (context.LicenseModel.Receipt != beforeEvent)
            //{
            var apiClient = new LicenseApiEndPoint(context.PublisherPreferences.ApiKey);
            try
            {
                var status = apiClient.PatchLicense(GetModel(context));

                if (status == System.Net.HttpStatusCode.NoContent)
                {
                    SetNext(new ApiGetLicenseHandler());
                }
                else
                {
                    SetNextError(context, new CouldNotPatchLicenseWithReceiptException());
                }
            }
            catch (Exception e)
            {
                SetNextError(context, e);
            }
            //}
            /*else
            {
                SetNextError(context, new ReceiptCodeException());
            }*/
            nextHandler.HandleContext(context);
        }
    }
}