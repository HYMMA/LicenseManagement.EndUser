using Hymma.Lm.EndUser.Exceptions;
using Hymma.Lm.EndUser.License.EndPoint;
using Hymma.Lm.EndUser.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Hymma.Lm.EndUser.License.Handlers
{
    /// <summary>
    /// Creates a license for a computer on DB, if didn't exist already.
    /// </summary>
    ///<remarks>if license existed prior to calling the API end point, will not change it and will call the <see cref="ApiGetLicenseHandler"/> to retrieve it.</remarks>
    internal class ApiPostLicenseHandler : LicenseValidationHandler
    {
        private void SetNextHandler() =>
                    SetNext(new ApiGetLicenseHandler());
        private string GetApiExceptionMsg(LicHandlingContext context) =>
                $"Could not register license for computer {context.LicenseModel.Computer.MacAddress} with Id {context.LicenseModel.Computer.Id} and product {context.LicenseModel.Product.Name} with Id {context.LicenseModel.Product.Id}";
        private PostLicenseModel GetModel(LicHandlingContext context)
        {
            // Fire the BeforeLicensePost event to allow publishers to attach metadata
            var metadata = context.PublisherPreferences.OnBeforeLicensePost(
                context.LicenseModel.Computer.MacAddress,
                context.LicenseModel.Computer.Name,
                context.LicenseModel.Product.Id);

            return new PostLicenseModel
            {
                Computer = context.LicenseModel.Computer.Id,
                Product = context.LicenseModel.Product.Id,
                Metadata = metadata
            };
        }

        public override void HandleContext(LicHandlingContext context)
        {
            try
            {
                //call api end point
                var apiClient = new LicenseApiEndPoint(context.PublisherPreferences.ApiKey);

                var response = apiClient.PostLicense(GetModel(context));

                //if license existed or we created it
                if (response == HttpStatusCode.Created || response == HttpStatusCode.Conflict)
                {
                    SetNextHandler();
                }
                else
                {
                    var msg = GetApiExceptionMsg(context);
                    SetNextError(context, new ApiException(msg,response));
                }
            }
            catch (Exception e)
            {
                SetNextError(context, e);
            }
            nextHandler.HandleContext(context);
        }

        public override async Task HandleContextAsync(LicHandlingContext context)
        {
            try
            {
                //call api end point
                var apiClient = new LicenseApiEndPoint(context.PublisherPreferences.ApiKey);

                var response = await apiClient.PostLicenseAsync(GetModel(context));

                //if license existed or we created it
                if (response == HttpStatusCode.Created || response == HttpStatusCode.Conflict)
                {
                    SetNextHandler();
                }
                else
                {
                    var msg = GetApiExceptionMsg(context);
                    SetNextError(context, new ApiException(msg, response));
                }
            }
            catch (Exception e)
            {
                SetNextError(context, e);
            }
            await nextHandler.HandleContextAsync(context);
        }
    }
}
