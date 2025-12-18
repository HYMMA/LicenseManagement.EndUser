using LicenseManagement.EndUser.Exceptions;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser.License.Handlers
{
    internal class ExpiredLicenseHandler : LicenseValidationHandler
    {
        /// <summary>
        /// sets the next handler to <see cref="ExpiredLicenseHandler"/> or <see cref="ApiGetComputerHandler"/> if online
        /// </summary>
        ///<remarks>expiry date of a license is never more than its receipt</remarks>
        public ExpiredLicenseHandler()
        {

        }
        //makes sure we don't go through a loop next time we are here. sets the context.IsLicenseFreshOurOfServer to true 
        private void GetLicenseFileFromServer() =>
            SetNext(new ApiGetLicenseHandler());
        /*private void ThrowLicenseExpiredException(LicHandlingContext context) =>
                SetNextError(context, new LicenseExpiredException());*/
        private void SetNextHandler(LicHandlingContext context)
        {
            //SOMETIMES A LICENS IS EXPIRED AND YOU NEED TO GET THE NEW CONTENT FORM SERVERS
            // BUT IF YOU GOT THE CONTENT AND IT'S STILL EXPIRED, THEN IT'S BECAUSE THE RECEIPT CODE IS EXPIRED
            if (context.IsLicenseFreshOutOfServer)
            {
                SetNext(new LastLicenseHandler());
                //ThrowLicenseExpiredException(context);
            }
            else
            {
                GetLicenseFileFromServer();
            }
        }

        public override Task HandleContextAsync(LicHandlingContext context)
        {
            SetNextHandler(context);    
            return nextHandler.HandleContextAsync(context);
        }
        
        public override void HandleContext(LicHandlingContext context)
        {
            SetNextHandler(context);    
            nextHandler.HandleContext(context);
        }
    }
}
