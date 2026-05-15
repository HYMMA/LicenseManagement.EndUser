using LicenseManagement.EndUser.Diagnostics;
using LicenseManagement.EndUser.Time;
using System;
using System.Management;

namespace LicenseManagement.EndUser.License.Handlers
{
    internal static class TimeSyncConstants
    {
        internal const string SyncTypeNtp = "NTP";
        internal const double AcceptableDriftHours = 1.0;
    }

    public class TimeSyncDiagnostic
    {
        public bool IsTimeServiceRunning()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Service WHERE Name='w32time'"))
            using (var collection = searcher.Get())
            {
                foreach (ManagementObject service in collection)
                {
                    using (service)
                    {
                        return service["State"]?.ToString() == "Running";
                    }
                }
            }
            return false;
        }

        public string GetTimeSyncType()
        {
            using (var reg = new ManagementClass(@"\\.\root\default:StdRegProv"))
            using (ManagementBaseObject inParams = reg.GetMethodParameters("GetStringValue"))
            {
                inParams["hDefKey"] = 0x80000002; // HKEY_LOCAL_MACHINE
                inParams["sSubKeyName"] = @"SYSTEM\CurrentControlSet\Services\W32Time\Parameters";
                inParams["sValueName"] = "Type";

                using (ManagementBaseObject outParams = reg.InvokeMethod("GetStringValue", inParams, null))
                {
                    return outParams["sValue"]?.ToString() ?? "Unknown";
                }
            }
        }

        /// <summary>
        /// Tries to determine the drift (in hours) between the local clock and an NTP server.
        /// Returns true when the lookup succeeded; the out value is the drift.
        /// </summary>
        public bool TryGetTimeDriftHours(out double driftHours)
        {
            try
            {
                var ntp = new NtpConnection("time.windows.com");
                driftHours = (DateTime.UtcNow - ntp.GetUtc()).TotalHours;
                return true;
            }
            catch (Exception ex)
            {
                LicenseHandlingOptions.Logger.Log(LicenseLogLevel.Debug, "NTP drift lookup failed: " + ex.Message);
                driftHours = 0;
                return false;
            }
        }

        public void ReportStatus()
        {
            bool isRunning = IsTimeServiceRunning();
            string syncType = GetTimeSyncType();
            bool gotDrift = TryGetTimeDriftHours(out double drift);

            var logger = LicenseHandlingOptions.Logger;
            logger.Log(LicenseLogLevel.Information, $"Windows Time Service Running: {isRunning}");
            logger.Log(LicenseLogLevel.Information, $"Time Sync Type: {syncType}");
            logger.Log(LicenseLogLevel.Information, gotDrift ? $"Time Drift: {drift:F2} hours" : "Time Drift: unknown (NTP lookup failed)");

            bool healthy = isRunning && syncType == TimeSyncConstants.SyncTypeNtp && gotDrift && Math.Abs(drift) < TimeSyncConstants.AcceptableDriftHours;
            logger.Log(healthy ? LicenseLogLevel.Information : LicenseLogLevel.Warning,
                healthy ? "System time is healthy and synchronized." : "System time may be out of sync or misconfigured.");
        }
    }
}
