using System;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser
{
    internal abstract class LicenseValidationHandler
    {
        protected LicenseValidationHandler nextHandler;
        protected void SetNext(LicenseValidationHandler handler) => nextHandler = handler;
        //public abstract Task HandleAsync();
        public abstract Task HandleContextAsync(LicHandlingContext context);
        public abstract void HandleContext(LicHandlingContext context);

        protected void SetNextError(LicHandlingContext context, Exception e)
        {
            context.Exception = e;
            SetNext(new ErrorHandler());
        }
    }
}
