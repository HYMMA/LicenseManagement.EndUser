using Hymma.Lm.EndUser.Receipt.Handlers;
using Hymma.Lm.EndUser.Time;
using Hymma.Lm.EndUser.Time.EndPoint;
using System;
using System.CodeDom;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Hymma.Lm.EndUser.License.Handlers
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
                case LicenseStatusTitles.InValidTrial:
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
            //var _apiClient = new DateTimeApiEndPoint(context.PublisherPreferences.ApiKey);
            //DateTime now;
            //try
            //{
            //    //try to get the date time from our servers
            //    now = await _apiClient.GetCurrentUtcTimeAsync();
            //}
            //catch (Exception)
            //{
            //    //fall back to user's computer if something went wrong
            //    now = DateTime.UtcNow;
            //}
            SetNextHandler(context, GetCurrentTime(context));
            await nextHandler.HandleContextAsync(context);
        }

        public DateTime GetCurrentTime(LicHandlingContext context)
        {
            var diagnosis = new TimeSyncDiagnostic();
            bool isRunning = diagnosis.IsTimeServiceRunning();
            string syncType = diagnosis.GetTimeSyncType();

            if (isRunning && syncType == "NTP")
            {
                double drift = diagnosis.TryGetTimeDriftHours();
                if (Math.Abs(drift) < 1)
                {
                    return DateTime.UtcNow;
                }
            }

            //user has modifed their computer time by more than 10 hours
            try
            {
                //gets ntp from pool.ntp.org    
                return NtpConnection.Utc();
            }
            catch (Exception)
            {
                try
                {
                    var _apiClient = new DateTimeApiEndPoint(context.PublisherPreferences.ApiKey);
                    var now = _apiClient.GetCurrentUtcTime();
                    return now;
                }
                catch (Exception)
                {
                    //fall back to user's computer if something went wrong
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
