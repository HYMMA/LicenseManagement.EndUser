using DeviceId.Encoders;
using Hymma.Lm.EndUser.Exceptions;
using Hymma.Lm.EndUser.Registrars;
using Hymma.Lm.EndUser.Time.EndPoint;
using Hymma.Lm.EndUser.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hymma.Lm.EndUser.License.Handlers
{
    internal class LastLicenseHandler : LicenseValidationHandler
    {

        /// <summary>
        /// the last handler in the chain unless there has been some exceptions
        /// </summary>
        internal LastLicenseHandler()
        {
        }

        bool TryWriteLicenseOnDisk(LicHandlingContext context)
        {
            var licenseFileRegistrar = new LicenseRegister(context);
            var wrote = licenseFileRegistrar.TryWrite();
            return wrote;
        }

        void SetNextHandler(LicHandlingContext context)
        {
            //we only need to write license onto file, if it's new
            //if it's been read from file there is no need for writing it 
            if (context.IsLicenseFreshOutOfServer)
            {
                if (TryWriteLicenseOnDisk(context))
                {

                    //since we don't check for status during install/or uninstall we must update the status here
                    if (context.ContextEnvironment != HandlerStrategy.Launch)
                    {
                        var handler = new LicenseStatusHandler();
                        var st = new LicenseStatus(context.LicenseModel, handler.GetCurrentTime(context));
                        context.LicenseModel.Status = st.GetLicenseStatus(context.PublisherPreferences);
                    }

                    //all good
                    context.RaiseOnLicenseHandledSuccessfully();
                }
                else
                {
                    SetNextError(context, new CouldNotWriteLicenseOnDiskException());
                }
            }
            else
            {
                context.RaiseOnLicenseHandledSuccessfully();
            }
        }

        public override void HandleContext(LicHandlingContext context)
        {
            //we only need to write license onto file, if it's new
            SetNextHandler(context);
            if (nextHandler != null)
                nextHandler?.HandleContext(context);
        }

        public override async Task HandleContextAsync(LicHandlingContext context)
        {
            SetNextHandler(context);
            if (nextHandler != null)
                await nextHandler.HandleContextAsync(context);

        }
    }
}
