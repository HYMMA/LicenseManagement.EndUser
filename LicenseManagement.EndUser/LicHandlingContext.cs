using Hymma.Lm.EndUser.Models;
using System;
using System.Threading;

namespace Hymma.Lm.EndUser
{
    /// <summary>
    /// a singleton, thread safe context to hold <see cref="LicenseValidationHandler.HandleContextAsync(LicHandlingContext)"/> in memory
    /// </summary>
    public sealed class LicHandlingContext
    {
        private string _signedLicense = null;
        private bool _isFromServer;
        public LicHandlingContext(PublisherPreferences publisherPreferences)
        {
            PublisherPreferences = publisherPreferences;
        }

        /// <summary>
        /// represents a license, does not hold any value for the signature, use <see cref="SignedLicense"/> for that purpose
        /// </summary>
        public LicenseModel LicenseModel { get;internal set; } = new LicenseModel();

        /// <summary>
        /// Exceptions to handle with <see cref="ErrorHandler"/>
        /// </summary>
        public Exception Exception { get;internal set; }

        /// <summary>
        /// signed license from api
        /// </summary>
        public string SignedLicense => _signedLicense;

        /// <summary>
        /// important data about the software owner and their preferences
        /// </summary>
        public PublisherPreferences PublisherPreferences { get; set; }

        /// <summary>
        /// indicates when this context is being called, during install that we have access to admin and internet or during launch
        /// </summary>
        public HandlerStrategy ContextEnvironment { get; internal set; }

        /// <summary>
        ///if <see cref="SignedLicense"/> data was retrieved within 15 minutes
        /// </summary>
        public bool IsLicenseFreshOutOfServer => _isFromServer;

        /// <summary>
        /// sets the content of <see cref="SignedLicense"/>
        /// </summary>
        /// <param name="signedLic"></param>
        /// <param name="isFromServer">true if the data is coming from server and false if its read from lic file</param>
        ///<remarks>for testing only</remarks>
        public void SetLicenseData(string signedLic, bool isFromServer)
        {
            _signedLicense = signedLic;
            _isFromServer = isFromServer;
        }
        #region Events
        internal void RaiseOnCustomerMustEnterProductKey()
            =>OnCustomerMustEnterProductKey?.Invoke(LicenseModel);
        internal void RaiseOnTrialValidated()
            =>OnTrialValidated?.Invoke(LicenseModel);

        internal void RaiseOnLicenseFileNotFound()
            =>OnLicenseFileNotFound?.Invoke(this);

        internal void RaiseOnTrialEnded()
            =>OnTrialEnded?.Invoke(PublisherPreferences);

        internal void RaiseOnLicenseHandledSuccessfully()
            =>OnLicenseHandledSuccessfully?.Invoke(LicenseModel);


        /// <summary>
        /// raised by <see cref="LicenseValidationHandler"/> once the license is handled successfully 
        /// </summary>
        internal event Action<LicenseModel> OnLicenseHandledSuccessfully;

        /// <summary>
        /// raised when license file is out of it's trial period and customer needs to provide a proof of purchase aka receipt code aka product key
        /// </summary>
        internal event Action<LicenseModel> OnCustomerMustEnterProductKey;


        /// <summary>
        /// raised when license file is in trial period and it is a valid trial license
        /// </summary>
        internal event Action<LicenseModel> OnTrialValidated;

        /// <summary>
        /// raised when license file not found, allows ui pass the value
        /// </summary>
        internal event Action<LicHandlingContext> OnLicenseFileNotFound;

        /// <summary>
        /// triggered when a trial license is detected and is over the trial period in <see cref="PublisherPreferences"/>
        /// </summary>
        internal event Action<PublisherPreferences> OnTrialEnded;
        #endregion
    }
}
