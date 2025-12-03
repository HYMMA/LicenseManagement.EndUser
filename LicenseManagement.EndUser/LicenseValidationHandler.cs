using System;
using System.Threading.Tasks;

namespace Hymma.Lm.EndUser
{
    public abstract class LicenseValidationHandler
    {
        protected LicenseValidationHandler nextHandler;
        protected void SetNext(LicenseValidationHandler hander) => nextHandler = hander;
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
