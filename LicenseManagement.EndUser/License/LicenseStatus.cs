using Hymma.Lm.EndUser.Models;
using System;

namespace Hymma.Lm.EndUser.License
{
    public enum LicenseStatusTitles
    {
        /// <summary>
        /// Status unknown - for backwards compatibility with old license files without Status field
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// when the license file is expired (needs refresh from server)
        /// </summary>
        Expired,

        /// <summary>
        /// license is valid with active subscription
        /// </summary>
        Valid,

        /// <summary>
        /// license is in trial mode and is valid
        /// </summary>
        ValidTrial,

        /// <summary>
        /// license is in trial mode but has past the trial time-frame
        /// </summary>
        InvalidTrial,

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
        /// Gets the license status. Uses server-provided status if available,
        /// otherwise falls back to client-side calculation for backwards compatibility.
        /// </summary>
        /// <param name="criteria">Publisher preferences (unused when server provides status)</param>
        /// <returns>The license status</returns>
        public LicenseStatusTitles GetLicenseStatus(PublisherPreferences criteria)
        {
            // First check if license file is expired (always check this client-side)
            if (IsLicenseFileExpired(_license))
            {
                _license.Status = LicenseStatusTitles.Expired;
                return _license.Status;
            }

            // Use server-provided status if available (new license files)
            if (_license.Status != LicenseStatusTitles.Unknown)
            {
                return _license.Status;
            }

            // Fallback: calculate status client-side for old license files without Status field
            _license.Status = CalculateStatusFallback();
            return _license.Status;
        }

        /// <summary>
        /// Fallback calculation for old license files that don't have server-provided Status
        /// </summary>
        private LicenseStatusTitles CalculateStatusFallback()
        {
            if (_license.Updated == null)
            {
                // Trial license - check TrialEndDate
                if (_now < _license.TrialEndDate)
                    return LicenseStatusTitles.ValidTrial;
                else
                    return LicenseStatusTitles.InvalidTrial;
            }
            else
            {
                // Paid license - check receipt
                if (_license.Receipt is null)
                    return LicenseStatusTitles.ReceiptUnregistered;

                if (IsReceiptExpired(_license.Receipt))
                    return LicenseStatusTitles.ReceiptExpired;
                else
                    return LicenseStatusTitles.Valid;
            }
        }

        bool IsLicenseFileExpired(LicenseModel license)
            => license.Expires <= _now;

        bool IsReceiptExpired(ReceiptModel receipt)
            => receipt.Expires <= _now;
    }
}
