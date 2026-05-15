using LicenseManagement.EndUser.License.EndPoint;
using LicenseManagement.EndUser.Signature.Handlers;
using System;
using System.Threading.Tasks;
namespace LicenseManagement.EndUser.License.Handlers
{
    internal class ApiGetLicenseHandler : LicenseValidationHandler
    {
        public ApiGetLicenseHandler()
        {

        }

         void UpdateLicenseData(LicHandlingContext context, string response) =>
                context.SetLicenseData(response, true);

        private void SetNextHandler() => SetNext(new LicenseSignatureValidationHandler());

        public override void HandleContext(LicHandlingContext context)
        {
            try
            {
                //get signed license from db
                var apiClient = new LicenseApiEndPoint(context.PublisherPreferences.ApiKey);
                var response = apiClient
                     .GetLicense(context.LicenseModel.Computer.Id,
                                      context.LicenseModel.Product.Id,
                                      context.PublisherPreferences.IncludeFeatures,
                                      context.PublisherPreferences.ValidDays);
                UpdateLicenseData(context, response);
                SetNextHandler();
            }
            catch (System.Exception e)
            {
                SetNextError(context, e);
            }
            nextHandler.HandleContext(context);
        }

        public override async Task HandleContextAsync(LicHandlingContext context)
        {
            try
            {
                //get signed license from db
                var apiClient = new LicenseApiEndPoint(context.PublisherPreferences.ApiKey);
                var response = await apiClient
                     .GetLicenseAsync(context.LicenseModel.Computer.Id,
                                      context.LicenseModel.Product.Id,
                                      context.PublisherPreferences.IncludeFeatures,
                                      context.PublisherPreferences.ValidDays)
                     .ConfigureAwait(false);

                UpdateLicenseData(context, response);
                SetNextHandler();
            }
            catch (System.Exception e)
            {
                SetNextError(context, e);
            }
            await nextHandler.HandleContextAsync(context).ConfigureAwait(false);
        }
    }
}
