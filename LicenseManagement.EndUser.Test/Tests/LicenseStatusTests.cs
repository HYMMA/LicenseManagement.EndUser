using LicenseManagement.EndUser.License;
using LicenseManagement.EndUser.Models;
using LicenseManagement.EndUser.Test.Data;
using LicenseManagement.EndUser.Test.Server;
using Xunit;

namespace LicenseManagement.EndUser.Test.Tests
{
    [Collection(CollectionNames.HANDLER_ASYNC)]
    public class LicenseStatusTests :IClassFixture<TestServer>
    {
        private TestServer server;

        public LicenseStatusTests(TestServer server)
        {
            this.server = server;
        }

        [Fact]
        public async Task ShouldDetect_ExpiredLicense()
        {
            var lic = await server.RegisterRandomLicenseAsync(LicenseStatusTitles.Expired);
            var licenseStatus = new LicenseStatus(lic, DateTime.Now);
            var context = ContextManager.FromLic(lic);
            var status = licenseStatus.GetLicenseStatus(context.PublisherPreferences);
            Assert.Equal(LicenseStatusTitles.Expired, status);
        }

        [Fact]
        public async Task ShouldDetect_ValidLicense()
        {
            // Use PaidLicenses[1] (not [0]) to avoid state pollution from tests that PATCH PaidLicenses[0]
            var seedLic = Licenses.PaidLicenses[1];
            var xml = await server.GetSignedLicenseXmlAsync(seedLic, 90);
            var lic = LicenseModel.FromXml(xml);
            var licenseStatus = new LicenseStatus(lic, DateTime.Now);
            var context = ContextManager.GetContext(lic.Product.Id, lic.Product.Vendor.Id, 21, lic.Computer.MacAddress);
            var status = licenseStatus.GetLicenseStatus(context.PublisherPreferences);
            Assert.Equal(LicenseStatusTitles.Valid, status);
        }

        [Fact]
        public void PublisherPreferences_ShouldHave_NonZeroDefaultValidDays()
        {
            var preferences = new PublisherPreferences("","","");
            Assert.True(preferences.ValidDays > 0);
        }

        [Theory]
        [InlineData(10U)]
        [InlineData(20U)]
        [InlineData(1U)]
        public async Task ShouldDetect_ValidTrial(uint trialDays)
        {
            // Use seedIndex=2 to avoid state pollution from uninstall tests (index 0) and launch tests (index 1).
            var lic = await server.GetLicenseAsync(LicenseStatusTitles.ValidTrial, seedIndex: 2);
            var licenseStatus = new LicenseStatus(lic, DateTime.Now);
            var context = ContextManager.GetContext(lic.Product.Name, "Hymma", trialDays, lic.Computer.MacAddress);
            var status = licenseStatus.GetLicenseStatus(context.PublisherPreferences);
            Assert.Equal(LicenseStatusTitles.ValidTrial, status);
        }

        [Theory]
        [InlineData(0U)]
        [InlineData(21U)]
        [InlineData(210U)]
        public async Task ShouldDetect_InValidTrial(uint trialDays)
        {
            // Use seedIndex=2 to avoid state pollution from uninstall tests (index 0) and launch tests (index 1).
            var lic = await server.GetLicenseAsync(LicenseStatusTitles.InvalidTrial, seedIndex: 2);
            var licenseStatus = new LicenseStatus(lic, DateTime.Now);
            var context= ContextManager.GetContext(lic.Product.Name, "Hymma", trialDays, lic.Computer.MacAddress);
            var status = licenseStatus.GetLicenseStatus(context.PublisherPreferences);
            Assert.Equal(LicenseStatusTitles.InvalidTrial, status);
        }

        [Fact]
        public async Task ShouldDetect_ReceiptExpired()
        {
            // Use seedIndex=1 (License 52) to avoid state pollution from uninstall tests (index 0).
            var lic = await server.GetLicenseAsync(LicenseStatusTitles.ReceiptExpired, seedIndex: 1);
            var licenseStatus = new LicenseStatus(lic, DateTime.Now);
            var context = ContextManager.GetContext(lic.Product.Name, "Hymma", 0, lic.Computer.MacAddress);
            var status = licenseStatus.GetLicenseStatus(context.PublisherPreferences);
            Assert.Equal(LicenseStatusTitles.ReceiptExpired, status);
        }

        [Fact]
        public async Task ShouldDetect_ReceiptUnregistered()
        {
            var lic = await server.RegisterRandomLicenseAsync(LicenseStatusTitles.ReceiptUnregistered);
            var licenseStatus = new LicenseStatus(lic, DateTime.Now);
            var context = ContextManager.GetContext(lic.Product.Name, "Hymma", 21, lic.Computer.MacAddress);
            var status = licenseStatus.GetLicenseStatus(context.PublisherPreferences);
            Assert.Equal(LicenseStatusTitles.ReceiptUnregistered, status);
        }
    }
}
