using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LicenseManagement.EndUser.Utilities
{
    internal static class PathHelper
    {
        // Cache the invalid char set once per process so each call doesn't scan a 40-char array per char.
        private static readonly HashSet<char> InvalidChars = new HashSet<char>(Path.GetInvalidFileNameChars());

        /// <summary>
        /// removes invalid file name characters from file name
        /// </summary>
        internal static string RemoveInvalidFileNameChars(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return fileName;
            var sb = new StringBuilder(fileName.Length);
            foreach (char c in fileName)
            {
                if (!InvalidChars.Contains(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
