using Hymma.Lm.EndUser.License;
using Hymma.Lm.EndUser.Test.Server;
using Xunit;

namespace Hymma.Lm.EndUser.Test.Tests
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
            var lic = await server.RegisterRandomLicenseAsync();
            var licenseStatus = new LicenseStatus(lic, DateTime.Now);
            var context = ContextManager.GetContext(lic.Product.Name, "Hymma", 21, lic.Computer.MacAddress);
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

            var lic = await server.RegisterRandomLicenseAsync(LicenseStatusTitles.ValidTrial);
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
            var lic = await server.RegisterRandomLicenseAsync(LicenseStatusTitles.InValidTrial);
            var licenseStatus = new LicenseStatus(lic, DateTime.Now);
            var context= ContextManager.GetContext(lic.Product.Name, "Hymma", trialDays, lic.Computer.MacAddress);
            var status = licenseStatus.GetLicenseStatus(context.PublisherPreferences);
            Assert.Equal(LicenseStatusTitles.InValidTrial, status);
        }

        [Fact]
        public async Task ShouldDetect_ReceiptExpired()
        {

            var lic = await server.RegisterRandomLicenseAsync(LicenseStatusTitles.ReceiptExpired);
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
