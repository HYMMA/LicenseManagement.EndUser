using LicenseManagement.EndUser.Exceptions;
using LicenseManagement.EndUser.Models;
using LicenseManagement.EndUser.Registrars;
using LicenseManagement.EndUser.Signature.Handlers;
using System;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser
{
    public class LicenseHandlingLaunch : LicenseHandlingStrategy
    {

        /// <summary>
        /// will be called during app launch we cannot rely on Internet connection, and everything should be read from computer
        /// </summary>
        ///<param name="context">the context of license to handle. Simply create an object using the default constructor</param>
        ///<param name="OnComputerUnregistered">happens when the computer is unregistered from the license management server. this will free one more seat for the purchaser to use on another computer. during launch if the receipt is expired (is not paid or suspended or subscription has ended) the handler will try to remove the computer from that receipt list of approved computers.</param>
        ///<param name="OnCustomerMustEnterProductKey">During launch if the receipt is expired (is not paid or suspended or subscription has ended) the handler raises an event so end user inserts a new product key (code for a new receipt) before trying to remove the computer from that receipt list of approved computers. </param>
        ///<param name="OnLicenseHandledSuccessfully">When the cycle of license checking is complete and no error was encountered.</param>
        ///<param name="OnLicFileNotFound">When the license file was not found. This allows the consumer of the library to call <see cref="LicenseHandlingInstall.HandleLicense"/> to get a new license from servers. the handler will automatically save the new license in the correct location (cannot be changed)</param>
        ///<param name="OnTrialEnded">Happens when the trial set in the <see cref="LicHandlingContext.PublisherPreferences"/> is over, this allows the publisher to give their end user extra trial days as a marketing strategy</param>
        public LicenseHandlingLaunch(LicHandlingContext context,
                                     Func<string> OnCustomerMustEnterProductKey = null,
                                     Action<LicHandlingContext> OnLicFileNotFound = null,
                                     Action<PublisherPreferences> OnTrialEnded = null,
                                     Action<ComputerModel> OnComputerUnregistered = null,
                                     Action<LicenseModel> OnLicenseHandledSuccessfully = null,
                                     Func<string> OnTrialValidated=null
                                     ) : base(context, OnLicenseHandledSuccessfully)
        {

            HandlingContext.ContextEnvironment = HandlerStrategy.Launch;

            if (OnCustomerMustEnterProductKey != null)
                HandlingContext.OnCustomerMustEnterProductKey += (l) =>
                {
                    //if raised when receipt is unregistered the receipt would be null
                    if (l.Receipt == null)
                    {
                        l.Receipt = new ReceiptModel();
                    }
                    l.Receipt.Code = OnCustomerMustEnterProductKey.Invoke();
                };

            if (OnLicFileNotFound != null)
                HandlingContext.OnLicenseFileNotFound += (c) =>
                OnLicFileNotFound.Invoke(new LicHandlingContext(c.PublisherPreferences)); // if we pass the same context the environment (strategy) could change by the subscriber

            if (OnTrialEnded != null)
                HandlingContext.OnTrialEnded += (p) =>
                OnTrialEnded.Invoke(p);

            if (OnTrialValidated != null)
                HandlingContext.OnTrialValidated += (l) =>
                {
                    if (l.Receipt == null)
                    {
                        l.Receipt = new ReceiptModel();
                    }
                    l.Receipt.Code = OnTrialValidated.Invoke();
                };
        }

        void SetNextHandler()
        {
            //if mac-address was explicitly provided 
            //or alternatively could be read form registry
            //in other words, if we somehow managed to get hold of MacAddress
            if (!string.IsNullOrEmpty(ComputerId.Instance.MachineId))
            {
                if (LicenseRegister.TryRead(out string signedLic))
                {
                    HandlingContext.SetLicenseData(signedLic, false);
                    NextHandler = (new LicenseSignatureValidationHandler());
                }
                else
                {
                    HandlingContext.Exception = new CouldNotReadLicenseFromDiskException();
                    NextHandler = (new ErrorHandler());
                }
                //SetNextError(new ApiGetComputerHandler());
            }
            else
            {
                HandlingContext.Exception = new CouldNotReadComputerIdException();
                NextHandler = (new ErrorHandler());
            }
        }
        public override void HandleLicense()
        {
            SetNextHandler();
            NextHandler.HandleContext(HandlingContext);
        }

        public override Task HandleLicenseAsync()
        {
            SetNextHandler();
            return NextHandler.HandleContextAsync(HandlingContext);
        }
    }
}
