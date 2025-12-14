// =============================================================================
// WiX Custom Action Sample for License Management
// =============================================================================
// This sample demonstrates how to use LicenseManagement.EndUser in a WiX installer
// to register and unregister licenses during installation/uninstallation.
//
// IMPORTANT: This code runs during MSI installation with elevated privileges.
// =============================================================================

using Hymma.Lm.EndUser;
using System;
using WixToolset.Dtf.WindowsInstaller;

namespace LicenseManagement.Sample.CustomAction
{
    /// <summary>
    /// WiX Custom Actions for handling license registration during install/uninstall.
    ///
    /// These custom actions:
    /// - InstallLicense: Registers the computer and downloads the license file during installation
    /// - UnInstallLicense: Unregisters the computer to free up a seat during uninstallation
    /// </summary>
    public class HandleLicenseAction
    {
        /// <summary>
        /// Creates publisher preferences from WiX custom action data.
        /// The data is passed from Package.wxs via CustomActionData.
        /// </summary>
        /// <param name="session">The installer session containing custom action data</param>
        /// <param name="productKey">Key name for the product ID in CustomActionData</param>
        /// <returns>Configured PublisherPreferences</returns>
        /// <example>
        /// CustomActionData format in Package.wxs:
        /// "ApiKey=PUB_xxx;ProductId=PRD_xxx;VendorId=VDR_xxx;ValidDays=14"
        /// </example>
        private static PublisherPreferences GetPreferences(Session session, string productKey)
        {
            // Read values from CustomActionData passed from WiX
            var apiKey = session.CustomActionData["ApiKey"];
            var vendorId = session.CustomActionData["VendorId"];
            var productId = session.CustomActionData[productKey];
            var validDays = uint.Parse(session.CustomActionData["ValidDays"]);

            return new PublisherPreferences(vendorId, productId, apiKey)
            {
                ValidDays = validDays
            };
        }

        /// <summary>
        /// Custom action to unregister the computer from the license during uninstallation.
        /// This frees up a seat so it can be used on another computer.
        /// </summary>
        /// <param name="session">The MSI installer session</param>
        /// <returns>ActionResult indicating success or failure</returns>
        /// <remarks>
        /// Configure in Package.wxs to run only during uninstall:
        /// Condition='REMOVE~="ALL"'
        /// </remarks>
        [CustomAction]
        public static ActionResult UnInstallLicense(Session session)
        {
            session.Log("=== Begin UnInstallLicense ===");
            session.Log("Time: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            // Support multiple products in the same installer
            // Add more product keys as needed: "Product2Id", "Product3Id", etc.
            var productKeys = new[] { "ProductId" };

            foreach (var productKey in productKeys)
            {
                try
                {
                    var preferences = GetPreferences(session, productKey);

                    session.Log("Product ID: {0}", preferences.ProductId);
                    session.Log("Vendor ID: {0}", preferences.VendorId);
                    session.Log("Valid Days: {0}", preferences.ValidDays);

                    // Create the handling context and handler
                    var context = new LicHandlingContext(preferences);
                    var handler = new LicenseHandlingUninstall(context);

                    // Execute the unregistration
                    // This calls the API to unregister this computer from the license
                    handler.HandleLicense();

                    session.Log("Successfully unregistered license for product: {0}", preferences.ProductId);
                }
                catch (Exception e)
                {
                    // Log the error but don't fail the uninstall
                    // User can manually unregister via the web portal if needed
                    session.Log("Warning: Could not unregister license for {0}", productKey);
                    session.Log("Error: {0}", e.Message);
                    session.Log("Stack trace: {0}", e.StackTrace);

                    // Return success anyway - we don't want to block uninstallation
                    // The user can manually release the seat via the dashboard
                }
            }

            session.Log("=== Finished UnInstallLicense ===");
            return ActionResult.Success;
        }

        /// <summary>
        /// Custom action to register the computer and download the license during installation.
        /// This creates or retrieves a license for this computer.
        /// </summary>
        /// <param name="session">The MSI installer session</param>
        /// <returns>ActionResult indicating success or failure</returns>
        /// <remarks>
        /// Configure in Package.wxs to run only during install (not uninstall):
        /// Condition="NOT REMOVE"
        /// </remarks>
        [CustomAction]
        public static ActionResult InstallLicense(Session session)
        {
            session.Log("=== Begin InstallLicense ===");
            session.Log("Time: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            // Support multiple products in the same installer
            var productKeys = new[] { "ProductId" };

            foreach (var productKey in productKeys)
            {
                try
                {
                    var preferences = GetPreferences(session, productKey);

                    session.Log("Product ID: {0}", preferences.ProductId);
                    session.Log("Vendor ID: {0}", preferences.VendorId);
                    session.Log("Valid Days: {0}", preferences.ValidDays);

                    // Create the handling context and handler
                    var context = new LicHandlingContext(preferences);

                    // The onSuccess callback is optional - useful for logging
                    var handler = new LicenseHandlingInstall(context,
                        OnLicenseHandledSuccessfully: (license) =>
                        {
                            session.Log("License downloaded successfully");
                            session.Log("License expires: {0}", license.Expires);
                            session.Log("License status: {0}", license.Status);
                        });

                    // Execute the installation
                    // This will:
                    // 1. Generate/read the computer's unique ID (MAC address)
                    // 2. Register the computer with the API
                    // 3. Download and save the license file locally
                    handler.HandleLicense();

                    session.Log("Successfully installed license for product: {0}", preferences.ProductId);
                }
                catch (Exception e)
                {
                    session.Log("ERROR: License installation failed for {0}", productKey);
                    session.Log("Error: {0}", e.Message);
                    session.Log("Stack trace: {0}", e.StackTrace);

                    // Decide whether to fail the installation or continue
                    // Option 1: Fail the installation (stricter)
                    // return ActionResult.Failure;

                    // Option 2: Continue anyway (more lenient - app can retry at launch)
                    // The application can handle missing license at startup
                    session.Log("Continuing installation - license can be obtained at first launch");
                }
            }

            session.Log("=== Finished InstallLicense ===");
            return ActionResult.Success;
        }
    }
}
