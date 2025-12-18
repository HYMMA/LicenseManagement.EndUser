using System.Net.Http;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser
{
    internal class ErrorHandler : LicenseValidationHandler
    {
        public override void HandleContext(LicHandlingContext context)
        {
            if (context.Exception is HttpRequestException e)
            {
                throw new System.Exception("We Could not connect to license-management.com, please check your internet connection. This is ususally caused by a setting in your firewall. If you are on public network, please change your wifi network.");
            }
            else
            {
                throw context.Exception;
            }
        }

        public override Task HandleContextAsync(LicHandlingContext context)
        {
            return Task.FromException(context.Exception);
        }
    }
}
