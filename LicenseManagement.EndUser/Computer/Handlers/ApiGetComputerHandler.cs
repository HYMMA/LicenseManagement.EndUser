using Hymma.Lm.EndUser.Product.Handlers;
using System;
using System.Threading.Tasks;

namespace Hymma.Lm.EndUser
{
    internal class ApiGetComputerHandler : LicenseValidationHandler
    {
        /// <summary>
        /// gets a <see cref="Models.ComputerModel"/> by it's name and sets next handler to <see cref="ApiGetProductHandler"/>
        /// </summary>
        public ApiGetComputerHandler()
        {

        }

        public override void HandleContext(LicHandlingContext context)
        {
            try
            {
                //update context computer model from db
                var _apiClient = new ComputerApiEndPoint(context.PublisherPreferences.ApiKey.ToString());
                context.LicenseModel.Computer = _apiClient
                    .GetComputer();

                //next: get update the product model from db
                SetNext(new ApiGetProductHandler());
            }
            catch (Exception e)
            {
                context.Exception = e;
                SetNext(new ErrorHandler());
            }
            nextHandler.HandleContext(context);
        }

        public override async Task HandleContextAsync(LicHandlingContext context)
        {
            try
            {
                //update context computer model from db
                var _apiClient = new ComputerApiEndPoint(context.PublisherPreferences.ApiKey.ToString());
                context.LicenseModel.Computer = await _apiClient
                    .GetComputerAsync()
                    .ConfigureAwait(false);

                //next: get update the product model from db
                SetNext(new ApiGetProductHandler());
            }
            catch (Exception e)
            {
                context.Exception = e;
                SetNext(new ErrorHandler());
            }
            await nextHandler.HandleContextAsync(context).ConfigureAwait(false);
        }
    }
}
