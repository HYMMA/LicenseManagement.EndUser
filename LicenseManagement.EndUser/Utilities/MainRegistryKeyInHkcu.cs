using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace LicenseManagement.EndUser.Utilities
{
    public class MainRegistryKeyInHkcu
    {
        public static bool TryReadFrom(string subKey, string name, out string content)
        {
            var key = Registry.CurrentUser.OpenSubKey(Constants.RegKey)?.OpenSubKey(subKey);
            if (key == null)
            {
                content = null;
                return false;
            }
            try
            {
                content = key.GetValue(name) as string;
                return !string.IsNullOrEmpty(content);
            }
            catch (Exception)
            {
                content = null;
                return false;
            }
            finally
            {
                key?.Dispose();
            }
        }

        public static bool TryWriteTo(string subKey, KeyValuePair<string, string> pair)
        {
            try
            {
                using (var inner = Registry.CurrentUser.CreateSubKey(Constants.RegKey).CreateSubKey(subKey))
                {
                    inner.SetValue(pair.Key, pair.Value, RegistryValueKind.String);
                    //inner.GetAccessControl().AddAccessRule(new System.Security.AccessControl.RegistryAccessRule(new IdentityReference()); 
                }
                return TryReadFrom(subKey, pair.Key, out _);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
