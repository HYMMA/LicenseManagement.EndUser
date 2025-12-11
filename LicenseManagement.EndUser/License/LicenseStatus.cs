using Hymma.Lm.EndUser.Models;
using System;

namespace Hymma.Lm.EndUser.License
{
    public enum LicenseStatusTitles
    {
        /// <summary>
        /// when the license data is expired
        /// </summary>
        Expired,

        /// <summary>
        /// this is when you want to allow users to use
        /// </summary>
        Valid,

        /// <summary>
        /// license is in trial mode and is valid
        /// </summary>
        ValidTrial,

        /// <summary>
        /// license is in trial mode but has past the trial time-frame 
        /// </summary>
        InValidTrial,

        /// <summary>
        /// the receipt is present but its time stamp is expired, meaning its out of subscription
        /// </summary>
        ReceiptExpired,

        /// <summary>
        /// when a receipt is present but has no receipt code indicating an un-install
        /// </summary>
        ReceiptUnregistered
    }

    /// <summary>
    /// allows categorizing a <see cref="LicenseModel"/> to  <see cref="LicenseStatusTitles"/>
    /// </summary>
    public class LicenseStatus
    {
        private LicenseModel _license;
        private DateTime _now;

        public LicenseStatus(LicenseModel model, DateTime now)
        {
            _license = model;
            _now = now;
        }

        /// <summary>
        /// assesses a <see cref="LicenseModel"/> against certain sets of criteria
        /// </summary>
        /// <param name="criteria">license key pieces of data will be compared to this set</param>
        /// <returns></returns>
        public LicenseStatusTitles GetLicenseStatus(PublisherPreferences criteria)
        {
            if (IsExpired(_license))
            {
                _license.Status = LicenseStatusTitles.Expired;
            }
            else
            {
                if (_license.Updated == null)
                {
                    _license.Status = TrialStatus();
                }
                else
                {
                    _license.Status = PaidLicenseStatus();
                }
            }
            return _license.Status;
        }

        private LicenseStatusTitles TrialStatus()
        {
            // Check if still within trial period using the server-set TrialEndDate
            if (_now < _license.TrialEndDate)
                return LicenseStatusTitles.ValidTrial;
            else
                return LicenseStatusTitles.InValidTrial;
        }

        private LicenseStatusTitles PaidLicenseStatus()
        {
            if (_license.Receipt is null)
                return LicenseStatusTitles.ReceiptUnregistered;

            if (IsExpired(_license.Receipt))
                return LicenseStatusTitles.ReceiptExpired;
            else
                return LicenseStatusTitles.Valid;
        }

        bool IsExpired(LicenseModel license)
            => IsExpired(license.Expires, _now);

        bool IsExpired(ReceiptModel receipt) =>
            IsExpired(receipt.Expires, _now);

        bool IsExpired(DateTime? expires, DateTime currentTime)
            => expires <= currentTime;
    }
}
