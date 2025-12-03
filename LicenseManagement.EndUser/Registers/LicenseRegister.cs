using Hymma.Lm.EndUser.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hymma.Lm.EndUser.Registrars
{
    public class LicenseRegister
    {
        private LicHandlingContext context;

        public LicenseRegister(LicHandlingContext context)
        {
            this.context = context;
        }

        static string GetDefaultFullFileName(string subFolderName, string fileName) => Path.Combine(Constants.DefaultLicFileRootDir, subFolderName, fileName);
        /// <summary>
        /// this method is public for testing only, this method will return the default full file name of a license file which is based on the product name and the publisher id
        /// </summary>
        /// <param name="productName">the name of the product</param>
        /// <returns>full file name of the lic file</returns>
        public string ResolveFileName(string productName)
        {
            var validLicFileName = PathHelper.RemoveInvalidFileNameChars(productName);
            //Get File name from registry
            var read = MainRegistryKeyInHKLM.TryReadFrom(context.PublisherPreferences.VendorId, validLicFileName, out string fullFileName);
            if (!read || !File.Exists(fullFileName))
            {
                fullFileName = GetDefaultFullFileName(context.PublisherPreferences.VendorId, validLicFileName);
            }
            return fullFileName;
        }

        /// <summary>
        /// reads the contents of the license
        /// </summary>
        /// <param name="signedLic">content</param>
        /// <returns></returns>
        ///<remarks>tries to read the contents of the lic file. if was not there raises the <see cref="LicHandlingContext.OnLicenseFileNotFound"/> and then tries to read the value of the <see cref="LicHandlingContext.LicenseModel"/></remarks>
        public bool TryRead(out string signedLic)
        {
            signedLic = null;
            var licFileName = context.PublisherPreferences.ProductId;
            if (string.IsNullOrEmpty(licFileName))
                return false;

            var validName = PathHelper.RemoveInvalidFileNameChars(licFileName);
            var fullFileName = ResolveFileName(validName);

            if (!File.Exists(fullFileName))
            {
                //raise event
                context.RaiseOnLicenseFileNotFound();

                //the client has the window to write the license on the disk
                //they can do it by calling LicenseHandlingInstall which makes sure license is written on disk
                if (!File.Exists(fullFileName))
                    return false;
                
                //if (context.LicenseModel == null || !File.Exists(context.LicenseModel.FullFileName))
                //    return false;
            }

            try
            {
                signedLic = File.ReadAllText(fullFileName);
            }
            catch (Exception)
            {
                return false;
            }
            return !string.IsNullOrEmpty(signedLic);
        }

        /// <summary>
        /// write the content to file and the file address to registry 
        /// </summary>
        /// <param name="content">the signed license as string that needs to be writer to file</param>
        /// <returns>true if managed to write to file</returns>
        /// <remarks>file name is driven by the product name</remarks>
        public bool TryWrite()
        {
            var licFileName = context.PublisherPreferences.ProductId;
            var validName = PathHelper.RemoveInvalidFileNameChars(licFileName);
            var fullFileName = ResolveFileName(validName);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));
                File.WriteAllText(fullFileName, context.SignedLicense);
                var wrote = File.Exists(fullFileName);

                //we don't know if user has write access to registry, this function might be called during launch
                //all that matters is that we try to write it in there.
                if (wrote)
                {
                    _ = MainRegistryKeyInHKLM.TryWriteTo(context.PublisherPreferences.VendorId,
                        new KeyValuePair<string, string>(validName, fullFileName));
                }

                return wrote;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
