using LicenseManagement.EndUser.Computer.Handlers;
using LicenseManagement.EndUser.Exceptions;
using LicenseManagement.EndUser.License.EndPoint;
using LicenseManagement.EndUser.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser.License.Handlers
{
    /// <summary>
    /// Severs the link between a license and its receipt. Consequently, other computers can refer to that receipt. Note that each computer has a unique license.
    /// </summary>
    internal class UnregisterReceiptHandler : LicenseValidationHandler
    {
        public override void HandleContext(LicHandlingContext context)
        {
            try
            {
                var client = new LicenseApiEndPoint(context.PublisherPreferences.ApiKey);
                var statusCode = client.PatchLicense(GetModel(context));
                if (statusCode == HttpStatusCode.NoContent)
                {
                    GetNewLicenseFromServer(context);
                }
                else
                {
                    SetNextError(context, new CouldNotUnregisterComputerException());
                }
            }
            catch (Exception e)
            {
                SetNextError(context, e);
            }
            if (nextHandler != null)
            {
                nextHandler?.HandleContext(context);
            }
        }

        private void GetNewLicenseFromServer(LicHandlingContext context)
        {
            //this line will switch the ApiGetLic to StatusHandler witch switches to ReceiptUnregisteredHandler
            context.ContextEnvironment = HandlerStrategy.Launch;
            SetNext(new ApiGetLicenseHandler());
        }

        public override async Task HandleContextAsync(LicHandlingContext context)
        {
            try
            {
                var client = new LicenseApiEndPoint(context.PublisherPreferences.ApiKey);
                var statusCode = await client.PatchLicenseAsync(GetModel(context));
                if (statusCode == HttpStatusCode.NoContent)
                {
                    GetNewLicenseFromServer(context);
                }
                else
                {
                    SetNextError(context, new CouldNotUnregisterComputerException());
                }
            }
            catch (Exception e)
            {
                SetNextError(context, e);
            }
            if (nextHandler != null)
            {
                await nextHandler?.HandleContextAsync(context);
            }
        }

        PatchLicenseModel GetModel(LicHandlingContext context)
            => new PatchLicenseModel()
            {
                License = context.LicenseModel.Id,
                Code = null //this will unregister it
            };

    }
}