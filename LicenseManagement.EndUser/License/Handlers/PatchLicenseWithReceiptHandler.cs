using LicenseManagement.EndUser.Exceptions;
using LicenseManagement.EndUser.License.EndPoint;
using System;
using System.Threading.Tasks;
using LicenseManagement.EndUser.Models;


namespace LicenseManagement.EndUser.License.Handlers
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
            var apiClient = new LicenseApiEndPoint(context.PublisherPreferences.ApiKey);
            try
            {
                var status = await apiClient.PatchLicenseAsync(GetModel(context)).ConfigureAwait(false);
                if (status == System.Net.HttpStatusCode.NoContent)
                {
                    SetNext(new ApiGetLicenseHandler());
                }
                else if (status == System.Net.HttpStatusCode.NotFound)
                {
                    // Server has rotated/deleted the license row. Re-POST creates a fresh license
                    // for this computer/product instead of failing the chain.
                    SetNext(new ApiPostLicenseHandler());
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
            await nextHandler.HandleContextAsync(context).ConfigureAwait(false);
        }

        public override void HandleContext(LicHandlingContext context)
        {
            var apiClient = new LicenseApiEndPoint(context.PublisherPreferences.ApiKey);
            try
            {
                var status = apiClient.PatchLicense(GetModel(context));

                if (status == System.Net.HttpStatusCode.NoContent)
                {
                    SetNext(new ApiGetLicenseHandler());
                }
                else if (status == System.Net.HttpStatusCode.NotFound)
                {
                    SetNext(new ApiPostLicenseHandler());
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
            nextHandler.HandleContext(context);
        }
    }
}