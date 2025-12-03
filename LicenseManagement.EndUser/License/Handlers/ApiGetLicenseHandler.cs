using Hymma.Lm.EndUser.License.EndPoint;
using Hymma.Lm.EndUser.Signature.Handlers;
using System;
using System.Threading.Tasks;
namespace Hymma.Lm.EndUser.License.Handlers
{
    public class ApiGetLicenseHandler : LicenseValidationHandler
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

                //if no license was found, we have a rare situation where the computer is registered but has no license yet

                /*Since we are calling this only after a post to License endpoint, it is unlikely that a license would not exist at this stage, also calling the post license endpoint from here would cause a loop because that one calls this*/

                //else if(response.StatusCode== HttpStatusCode.NoContent)
                //{
                //    SetNextError(new ApiPostLicenseHandler());
                //}
            }
            catch (System.Exception e)
            {
                SetNextError(context, e);
            }
            await nextHandler.HandleContextAsync(context);
        }
    }
}
