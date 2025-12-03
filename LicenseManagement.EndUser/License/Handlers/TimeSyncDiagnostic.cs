using Hymma.Lm.EndUser.Time;
using Hymma.Lm.EndUser.Time.EndPoint;
using System;
using System.Management;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Hymma.Lm.EndUser.License.Handlers
{
    public class TimeSyncDiagnostic
    {
        public bool IsTimeServiceRunning()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Service WHERE Name='w32time'");
            foreach (ManagementObject service in searcher.Get())
            {
                string state = service["State"]?.ToString();
                return state == "Running";
            }
            return false;
        }

        public string GetTimeSyncType()
        {
            var reg = new ManagementClass(@"\\.\root\default:StdRegProv");
            ManagementBaseObject inParams = reg.GetMethodParameters("GetStringValue");
            inParams["hDefKey"] = 0x80000002; // HKEY_LOCAL_MACHINE
            inParams["sSubKeyName"] = @"SYSTEM\CurrentControlSet\Services\W32Time\Parameters";
            inParams["sValueName"] = "Type";

            ManagementBaseObject outParams = reg.InvokeMethod("GetStringValue", inParams, null);
            return outParams["sValue"]?.ToString() ?? "Unknown";
        }

        public double TryGetTimeDriftHours()
        {
            DateTime localTime = DateTime.UtcNow;
            try
            {
                var ntp = new NtpConnection("time.windows.com");
                return (localTime - ntp.GetUtc()).TotalHours;
            }
            catch (Exception)
            {
                return 1000;
            }
        }

        public void ReportStatus()
        {
            bool isRunning = IsTimeServiceRunning();
            string syncType = GetTimeSyncType();
            double drift = TryGetTimeDriftHours();

            Console.WriteLine($"Windows Time Service Running: {isRunning}");
            Console.WriteLine($"Time Sync Type: {syncType}");
            Console.WriteLine($"Time Drift: {drift:F2} seconds");

            if (isRunning && syncType == "NTP" && Math.Abs(drift) < 5)
            {
                Console.WriteLine("✅ System time is healthy and synchronized.");
            }
            else
            {
                Console.WriteLine("⚠️ System time may be out of sync or misconfigured.");
            }
        }
    }
}
