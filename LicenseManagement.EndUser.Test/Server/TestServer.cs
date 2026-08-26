using LicenseManagement.EndUser.License;
using LicenseManagement.EndUser.Models;
using LicenseManagement.EndUser.Test.Data;
using LicenseManagement.EndUser.Test.Utilities;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace LicenseManagement.EndUser.Test.Server
{
    /// <summary>
    /// Test server client for integration tests.
    /// Uses static test data from SQL seed script instead of random Bogus data.
    /// </summary>
    public class TestServer
    {
        public TestServer()
        {
            // Redirect the library's shared HttpClient to the local test webapp (must happen before
            // the first ApiHttp call, since WebApiClient uses a Lazy<HttpClient>).
            LicenseHandlingOptions.ServerBaseAddress = "http://localhost:7298/api/";

            HttpClient = HttpClientFactory.Create(new AuthenticationHandler());
            HttpClient.BaseAddress = new Uri("http://localhost:7298/api/");
            // Reset the computer index at test server construction
            Computers.ResetIndex();

            // Ensure real machine's computer is on the server so install/launch/uninstall tests work.
            // Returns 201 (created) or 409 (already exists) — both are success states.
            HttpClient.PostAsJsonAsync("computer", new
            {
                MacAddress = ComputerId.Instance.EffectiveMachineId,
                LegacyMacAddress = ComputerId.Instance.LegacyMachineId,
                Name = ComputerId.Instance.MachineName
            }).GetAwaiter().GetResult();

            // Fetch and cache the server's current RSA public key so ContextManager always has
            // the correct key, even after TestSetup regenerates it.
            try { ContextManager.ServerPublicKey = GetPublicKeyAsync().GetAwaiter().GetResult(); }
            catch { /* server not running — tests will self-recover via GetPublicKeyFromServer */ }
        }

        public HttpClient HttpClient;

        /// <summary>
        /// Gets the public key
        /// </summary>
        internal Task<string> GetPublicKeyAsync()
        {
            return HttpClient.GetStringAsync("PublicKey");
        }

        /// <summary>
        /// Returns a signed xml of the license from server
        /// </summary>
        public async Task<string> GetSignedLicenseXmlAsync(LicenseModel lic)
            => await HttpClient.GetStringAsync($"license?computer={lic.Computer.Id}&product={lic.Product.Id}");

        /// <summary>
        /// Gets a license with valid days set to a specific number
        /// </summary>
        public async Task<string> GetSignedLicenseXmlAsync(LicenseModel lic, int validDays)
            => await HttpClient.GetStringAsync($"license?computer={lic.Computer.Id}&product={lic.Product.Id}&validDays={validDays}");

        /// <summary>
        /// Gets a computer from the seed data that doesn't have a license yet.
        /// Uses the NoLicenseComputers group (IDs 51-60).
        /// </summary>
        public ComputerModel GetComputerWithoutLicense()
        {
            return Computers.ForNewLicense();
        }

        /// <summary>
        /// Gets a product from the seed data
        /// </summary>
        public ProductModel GetProduct(ProductType type = ProductType.ManyFeatures)
        {
            return Products.FromType(type);
        }

        /// <summary>
        /// Gets an existing receipt from the seed data for a product
        /// </summary>
        public ReceiptModel GetReceiptForProduct(ProductModel product)
        {
            return Receipts.ForProduct(product);
        }

        /// <summary>
        /// Gets an existing paid license from the seed data
        /// </summary>
        public LicenseModel GetPaidLicense()
        {
            return Data.Licenses.GetPaid();
        }

        /// <summary>
        /// Gets an existing trial license from the seed data
        /// </summary>
        public LicenseModel GetTrialLicense()
        {
            return Data.Licenses.GetTrial();
        }

        /// <summary>
        /// Gets an existing unregistered license from the seed data
        /// </summary>
        public LicenseModel GetUnregisteredLicense()
        {
            return Data.Licenses.GetUnregistered();
        }

        /// <summary>
        /// Gets a license based on status from seed data, then fetches the signed version from API.
        /// <paramref name="seedIndex"/> selects which seed entry to use (0 = default, 1 = alternate).
        /// Use index 1 for tests that must not conflict with tests that modify index 0 server-side.
        /// </summary>
        public async Task<LicenseModel> GetLicenseAsync(LicenseStatusTitles licenseStatusTitles = LicenseStatusTitles.Valid, ProductType type = ProductType.ManyFeatures, int seedIndex = 0)
        {
            LicenseModel seedLicense;
            int validDays = 90;

            switch (licenseStatusTitles)
            {
                case LicenseStatusTitles.Expired:
                    seedLicense = Data.Licenses.PaidLicenses[seedIndex];
                    validDays = 0;
                    break;

                case LicenseStatusTitles.Valid:
                    seedLicense = Data.Licenses.PaidLicenses[seedIndex];
                    validDays = 90;
                    break;

                case LicenseStatusTitles.ValidTrial:
                    // Use a license whose TrialEndDate is in the future (IDs 21-24)
                    seedLicense = Data.Licenses.ValidTrialLicenses[seedIndex];
                    validDays = 90;
                    break;

                case LicenseStatusTitles.InvalidTrial:
                    // Use a license whose TrialEndDate is in the past (IDs 25-28).
                    // validDays > 0 so the file itself isn't expired — only the trial period is over.
                    seedLicense = Data.Licenses.InvalidTrialLicenses[seedIndex];
                    validDays = 90;
                    break;

                case LicenseStatusTitles.ReceiptExpired:
                    // Use a license that is linked to an expired receipt (IDs 51-52)
                    seedLicense = Data.Licenses.ExpiredReceiptLicenses[seedIndex];
                    validDays = 90;
                    break;

                case LicenseStatusTitles.ReceiptUnregistered:
                    seedLicense = Data.Licenses.UnregisteredLicenses[seedIndex];
                    validDays = 90;
                    break;

                default:
                    seedLicense = Data.Licenses.PaidLicenses[seedIndex];
                    validDays = 90;
                    break;
            }

            // Get the signed license from the API
            var xml = await GetSignedLicenseXmlAsync(seedLicense, validDays);
            return LicenseModel.FromXml(xml);
        }

        #region Legacy methods for backward compatibility - these POST/register new data

        /// <summary>
        /// Registers a new computer via the API (uses static data from NoLicenseComputers)
        /// </summary>
        [Obsolete("Use GetComputerWithoutLicense() for seed data or ensure database is reset between tests")]
        public async Task<ComputerModel> RegisterRandomComputer()
        {
            var c = Computers.ForNewLicense();
            var msg = await HttpClient.PostAsJsonAsync("computer", new { c.MacAddress, c.Name });
            if (msg.StatusCode == HttpStatusCode.Conflict)
            {
                // Computer already exists from seed data, just return it
                return c;
            }
            if (!msg.IsSuccessStatusCode)
                ThrowHelper.ThrowUnSuccessfulRequest(HttpClient.BaseAddress + "computer");
            return await HttpClient.GetFromJsonAsync<ComputerModel>(msg.Headers.Location!)
                ?? throw new Exception("no computer was found");
        }

        /// <summary>
        /// Gets a product from seed data (no longer creates random products)
        /// </summary>
        [Obsolete("Use GetProduct() instead - products come from seed data")]
        public Task<ProductModel> RegisterRandomProductAsync(ProductType type)
        {
            return Task.FromResult(Products.FromType(type));
        }

        /// <summary>
        /// Gets a receipt from seed data for a product
        /// </summary>
        [Obsolete("Use GetReceiptForProduct() instead - receipts come from seed data")]
        public Task<ReceiptModel> RegisterRandomReceiptForProduct(ProductModel p, DateTime expires)
        {
            return Task.FromResult(Receipts.ForProduct(p));
        }

        /// <summary>
        /// Gets a license based on the status from seed data.
        /// Note: This method now uses pre-seeded data instead of creating random licenses.
        /// </summary>
        public async Task<LicenseModel> RegisterRandomLicenseAsync(LicenseStatusTitles licenseStatusTitles = LicenseStatusTitles.Valid, ProductType type = ProductType.ManyFeatures)
        {
            return await GetLicenseAsync(licenseStatusTitles, type);
        }

        #endregion
    }
}
