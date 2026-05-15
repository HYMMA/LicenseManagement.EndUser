using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace LicenseManagement.EndUser.Utilities
{
    /// <summary>
    /// reads and writes to HKLM and fallbacks to HKCU
    /// </summary>
    /// <remarks>
    /// HKLM writes require the calling process to be elevated. WiX deferred custom actions run impersonated
    /// as the installing user by default unless `Impersonate="no"` is set on the CustomAction element, so
    /// HKLM writes from a CA will fail and the value will only end up under HKCU. Document the privilege
    /// requirement on the consumer side.
    /// </remarks>
    internal static class MainRegistryKeyInHKLM
    {
        internal static bool TryReadFrom(string subKey, string name, out string content)
        {
            var key = Registry.LocalMachine.OpenSubKey(Constants.RegKey)?.OpenSubKey(subKey);
            if (key ==null)
            {
                var innerRead = MainRegistryKeyInHkcu.TryReadFrom(subKey, name, out string innerContent);
                content = innerContent;
                return innerRead;
            }
            else
            {

                content = key.GetValue(name) as string;
                key.Dispose();
                return !string.IsNullOrEmpty(content);
            }
        }

        internal static bool TryWriteTo(string subKey, KeyValuePair<string, string> pair)
        {
            //write to HKCU first
            var innerWroteToHKCU = MainRegistryKeyInHkcu.TryWriteTo(subKey, pair);
            var outerWroteToHKLM = Write(subKey, pair);
            return innerWroteToHKCU || outerWroteToHKLM;
        }

        static bool Write(string subkey, KeyValuePair<string, string> pair)
        {
            try
            {
                using (var hySubKey = Registry.LocalMachine.CreateSubKey(Constants.RegKey).CreateSubKey(subkey))
                {
                    hySubKey.SetValue(pair.Key, pair.Value, RegistryValueKind.String);
                }
                return TryReadFrom(subkey, pair.Key, out _);
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (System.Security.SecurityException)
            {
                return false;
            }
        }
    }
}
