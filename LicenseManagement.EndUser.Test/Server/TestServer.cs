using Hymma.Lm.EndUser.License;
using Hymma.Lm.EndUser.Models;
using Hymma.Lm.EndUser.Test.Data;
using Hymma.Lm.EndUser.Test.Utilities;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace Hymma.Lm.EndUser.Test.Server
{
    /// <summary>
    /// Test server client for integration tests.
    /// Uses static test data from SQL seed script instead of random Bogus data.
    /// </summary>
    public class TestServer
    {
        public TestServer()
        {
            HttpClient = HttpClientFactory.Create(new AuthenticationHandler());
            HttpClient.BaseAddress = new Uri("http://localhost:7298/api/");
            // Reset the computer index at test server construction
            Computers.ResetIndex();
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
        /// For statuses that require creating new data (like new trial), uses existing seed data.
        /// </summary>
        public async Task<LicenseModel> GetLicenseAsync(LicenseStatusTitles licenseStatusTitles = LicenseStatusTitles.Valid, ProductType type = ProductType.ManyFeatures)
        {
            LicenseModel seedLicense;
            int validDays = 90;

            switch (licenseStatusTitles)
            {
                case LicenseStatusTitles.Expired:
                    seedLicense = Data.Licenses.PaidLicenses[0];
                    validDays = 0;
                    break;

                case LicenseStatusTitles.Valid:
                    seedLicense = Data.Licenses.PaidLicenses[0];
                    validDays = 90;
                    break;

                case LicenseStatusTitles.ValidTrial:
                    seedLicense = Data.Licenses.TrialLicenses[0];
                    validDays = 90;
                    break;

                case LicenseStatusTitles.InvalidTrial:
                    seedLicense = Data.Licenses.TrialLicenses[0];
                    validDays = 0;
                    break;

                case LicenseStatusTitles.ReceiptExpired:
                    // Use a license with an expired receipt
                    seedLicense = Data.Licenses.PaidLicenses[0];
                    validDays = 10;
                    break;

                case LicenseStatusTitles.ReceiptUnregistered:
                    seedLicense = Data.Licenses.UnregisteredLicenses[0];
                    validDays = 90;
                    break;

                default:
                    seedLicense = Data.Licenses.PaidLicenses[0];
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
