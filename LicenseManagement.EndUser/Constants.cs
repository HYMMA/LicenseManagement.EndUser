using System;
using System.IO;

namespace Hymma.Lm.EndUser
{
    public static class Constants
    {
        /// <summary>
        /// the api base address
        /// </summary>
        public const string BaseAddress = "https://license-management.com/api/";

        /// <summary>
        /// the computer key in the register
        /// </summary>
        internal const string COMPUTER_KEY = "{0A3D961C-5571-409C-872C-10A31FCE7852}";

        /// <summary>
        /// the name of the key where the computer will be saved to
        /// </summary>
        internal const string COMPUTER_ID = "computerId";

        /// <summary>
        /// title of the header in the header api calls
        /// </summary>
        public const string ApiKeyHeader = "X-API-KEY";

        /// <summary>
        /// main key to save the address of the license file
        /// </summary>
        public const string RegKey = @"Software\Hymma\LicenseManagement\";

        /// <summary>
        /// this is the path to where the license files gets saved to
        /// </summary>
        ///<remarks>each publisher will have their own sub-dir under this folder, and their respective lic files will be saved there</remarks>
        public static string DefaultLicFileRootDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "License-Management.com");
    }
}
