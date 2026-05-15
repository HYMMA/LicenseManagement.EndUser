using LicenseManagement.EndUser.Receipt.Handlers;
using LicenseManagement.EndUser.Time;
using LicenseManagement.EndUser.Time.EndPoint;
using System;
using System.CodeDom;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser.License.Handlers
{
    internal class LicenseStatusHandler : LicenseValidationHandler
    {
        /// <summary>
        /// checks expiry date of license and sets the next handler to <see cref="ExpiredLicenseHandler"/> or <see cref="LastLicenseHandler"/>
        /// </summary>
        public LicenseStatusHandler()
        {

        }
        void SetNextHandler(LicHandlingContext context, DateTime now)
        {
            var licenseStatus = new LicenseStatus(context.LicenseModel, now);
            var status = licenseStatus.GetLicenseStatus(context.PublisherPreferences);
            switch (status)
            {
                case LicenseStatusTitles.Expired:
                    SetNext(new ExpiredLicenseHandler());
                    break;
                case LicenseStatusTitles.Valid:
                    SetNext(new LastLicenseHandler());
                    break;
                case LicenseStatusTitles.ValidTrial:
                    SetNext(new ValidTrialHandler());
                    break;
                case LicenseStatusTitles.InvalidTrial:
                    SetNext(new InvalidTrialHandler());
                    break;
                case LicenseStatusTitles.ReceiptExpired:
                    SetNext(new ReceiptExpiredHandler());
                    break;
                case LicenseStatusTitles.ReceiptUnregistered:
                    SetNext(new ReceiptUnregisteredHandler());
                    break;
                default:
                    break;
            }
        }
        public override async Task HandleContextAsync(LicHandlingContext context)
        {
            SetNextHandler(context, GetCurrentTime(context));
            await nextHandler.HandleContextAsync(context).ConfigureAwait(false);
        }

        public DateTime GetCurrentTime(LicHandlingContext context)
        {
            var diagnosis = new TimeSyncDiagnostic();

            // If the local clock is being kept in sync via NTP and the drift from a public NTP server
            // is small (< AcceptableDriftHours), trust the local clock. This is the cheap, common path.
            if (diagnosis.IsTimeServiceRunning()
                && diagnosis.GetTimeSyncType() == TimeSyncConstants.SyncTypeNtp
                && diagnosis.TryGetTimeDriftHours(out double drift)
                && Math.Abs(drift) < TimeSyncConstants.AcceptableDriftHours)
            {
                return DateTime.UtcNow;
            }

            // The user may have manually shifted their clock. Fall back to a public NTP server,
            // then to our own time endpoint, then finally to the local clock as a last resort.
            try
            {
                return NtpConnection.Utc();
            }
            catch (Exception)
            {
                try
                {
                    var apiClient = new DateTimeApiEndPoint(context.PublisherPreferences.ApiKey);
                    return apiClient.GetCurrentUtcTime();
                }
                catch (Exception)
                {
                    return DateTime.UtcNow;
                }
            }
        }

        public override void HandleContext(LicHandlingContext context)
        {
            var time = GetCurrentTime(context);
            SetNextHandler(context, time);
            nextHandler.HandleContext(context);
        }
    }
}
