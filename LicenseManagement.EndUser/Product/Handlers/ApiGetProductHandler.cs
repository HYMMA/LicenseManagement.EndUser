using LicenseManagement.EndUser.License.Handlers;
using LicenseManagement.EndUser.Product.EndPoint;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser.Product.Handlers
{
    /// <summary>
    /// creates a handler to get <see cref="Models.ProductModel"/>
    /// </summary>
    internal class ApiGetProductHandler : LicenseValidationHandler
    {
        public override void HandleContext(LicHandlingContext context)
        {
            try
            {
                //update product model from db
                var apiClient = new ProductApiEndPoint(context.PublisherPreferences.ApiKey);
                context.LicenseModel.Product = apiClient.GetProduct(context.PublisherPreferences.ProductId);

                if (context.LicenseModel.Product is null)
                {
                    SetNextError(context, new Exception($"no such product {context.PublisherPreferences.ProductId}"));
                }
                else
                {
                    //next get the license
                    SetNext(new ApiPostLicenseHandler());
                }
            }
            catch (HttpRequestException ex)
            {
                SetNextError(context, ex);
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
                var apiClient = new ProductApiEndPoint(context.PublisherPreferences.ApiKey);
                var product = await apiClient
                    .GetProductAsync(context.PublisherPreferences.ProductId)
                    .ConfigureAwait(false);

                if (product is null)
                {
                    SetNextError(context, new Exceptions.ApiException(
                        $"no such product {context.PublisherPreferences.ProductId}",
                        System.Net.HttpStatusCode.NotFound));
                }
                else
                {
                    context.LicenseModel.Product = product;
                    SetNext(new ApiPostLicenseHandler());
                }
            }
            catch (HttpRequestException ex)
            {
                SetNextError(context, ex);
            }
            catch (Exception e)
            {
                SetNextError(context, e);
            }
            await nextHandler.HandleContextAsync(context).ConfigureAwait(false);
        }
    }
}
