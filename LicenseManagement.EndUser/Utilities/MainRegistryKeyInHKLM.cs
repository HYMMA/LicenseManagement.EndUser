using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace Hymma.Lm.EndUser.Utilities
{
    /// <summary>
    /// reads and writes to HKLM and fallbacks to HKCU 
    /// </summary>
    public static class MainRegistryKeyInHKLM
    {
        public static bool TryReadFrom(string subKey, string name, out string content)
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

        public static bool TryWriteTo(string subKey, KeyValuePair<string, string> pair)
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
            catch (Exception)
            {
                return false;
            }
        }
    }

}
