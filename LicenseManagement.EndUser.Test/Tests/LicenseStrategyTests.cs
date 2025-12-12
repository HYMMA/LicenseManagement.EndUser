using Hymma.Lm.EndUser.Exceptions;
using Hymma.Lm.EndUser.License;
using Hymma.Lm.EndUser.Models;
using Hymma.Lm.EndUser.Registrars;
using Hymma.Lm.EndUser.Test.Data;
using Hymma.Lm.EndUser.Test.Server;
using Hymma.Lm.EndUser.Test.Utilities;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text.Json.Serialization.Metadata;
using System.Xml;
using System.Xml.Schema;
using Xunit;

namespace Hymma.Lm.EndUser.Test.Tests
{
    public class LicenseStrategyTests : DisposableTest, IClassFixture<TestServer>
    {
        readonly TestServer testServer;
        readonly LicGenerator licGenerator;

        public LicenseStrategyTests(TestServer server)
        {
            this.testServer = server;
            this.licGenerator = new LicGenerator(server);
        }

        [Fact]
        public async Task OnLaunchFile_IfFileIsModified_ShouldFailToVerify()
        {
            //arrange
            //GET a license from server
            var lic = await licGenerator.SaveNewLicOnDiskAsync(LicenseStatusTitles.Valid);
            var context = ContextManager.FromLic(lic);

            var register = new LicenseRegister(context);
            register.TryRead(out string signedLic);

            //modify the license string value
            XmlDocument doc = new();
            doc.LoadXml(signedLic);
            doc["License"]!["Id"]!.InnerText = (lic.Id + 1).ToString();
            //context.SetLicenseData(doc.OuterXml, false);

            var licFileName = context.PublisherPreferences.ProductId;
            var fullFileName = register.ResolveFileName(licFileName);

            //save it on computer
            doc.Save(fullFileName);

            //ACT
            var handler = new LicenseHandlingLaunch(context);

            await Assert.ThrowsAsync<CryptographicException>(async () => await handler.HandleLicenseAsync());
            Assert.Throws<CryptographicException>(() => handler.HandleLicense());
        }

        [Fact]
        public async Task OnLaunch_IfProductNameMismatch_ShouldFailToVerify()
        {
            //Arrange
            var lic = await testServer.RegisterRandomLicenseAsync();
            var signedLic = await testServer.GetSignedLicenseXmlAsync(lic);
            var context = ContextManager.GetContext(lic.Product!.Id! + "_changed", "VDR_vendor", 1, lic.Computer!.MacAddress!);
            context.SetLicenseData(signedLic, false);
            var handler = new LicenseHandlingLaunch(context);

            //here we insert wrong product name as preferences
            //ContextManager.SetEnv(HandlerStrategy.Launch);

            //ACT
            //save it on computer and registry
            var register = new LicenseRegister(context);
            Assert.True(register.TryWrite());


            //assert
            await Assert.ThrowsAsync<ProductNameException>(async () => { await handler.HandleLicenseAsync(); });
            Assert.Throws<ProductNameException>(() => { handler.HandleLicense(); });
        }

        [Fact]
        public async Task OnLaunch_IfComputerNameMismatch_ShouldFailToVerify()
        {
            var lic = await testServer.RegisterRandomLicenseAsync();
            //here we insert wrong computer name, string.empty, as preferences
            var context = ContextManager.GetContext(lic.Product!.Id!, "VDR_vendor", 1, string.Empty);
            var signedLic = await testServer.GetSignedLicenseXmlAsync(lic);
            context.SetLicenseData(signedLic, false);
            var handler = new LicenseHandlingLaunch(context);


            //save it on computer and registry
            //ACT
            var register = new LicenseRegister(context);
            register.TryWrite();

            //assert
            await Assert.ThrowsAsync<ComputerNameException>(async () => { await handler.HandleLicenseAsync(); });
        }

        [Fact]
        public async Task OnUninstall_ShouldGetLicOnline()
        {
            //first element is used in another test
            var lic = await testServer.RegisterRandomLicenseAsync();
            var context = ContextManager.GetContext(lic.Product!.Id!, "VDR_vendor", 4, lic.Computer!.MacAddress!);

            //we're uninstalling here
            var handler = new LicenseHandlingUninstall(context);
            await handler.HandleLicenseAsync();

            //ACT
            var expected = await testServer.GetSignedLicenseXmlAsync(lic);
            var expectedLic = LicenseModel.FromXml(expected);

            //ASSERT
            Assert.Null(expectedLic.Receipt);
        }

        [Theory]
        [InlineData(LicenseStatusTitles.InvalidTrial)]
        [InlineData(LicenseStatusTitles.Valid)]
        [InlineData(LicenseStatusTitles.ReceiptUnregistered)]
        [InlineData(LicenseStatusTitles.ReceiptExpired)]
        [InlineData(LicenseStatusTitles.ValidTrial)]
        public async Task OnUninstall_WrittenLicFile_ShouldBeInUnregisteredState(LicenseStatusTitles initialState)
        {
            var lic = await licGenerator.SaveNewLicOnDiskAsync(initialState);

            var trialDays = 100U;
            if (initialState == LicenseStatusTitles.InvalidTrial)
                trialDays = 0U;

            var context = ContextManager.FromLic(lic, trialDays);
            var handler = new LicenseHandlingUninstall(context, null);
            handler.HandleLicense();

            var register = new LicenseRegister(context);
            register.TryRead(out string content);
            var parsed = LicenseModel.FromXml(content);
            var st = new LicenseStatus(parsed, DateTime.Now);
            var status = st.GetLicenseStatus(context.PublisherPreferences);
            Assert.Equal(LicenseStatusTitles.ReceiptUnregistered, status);
        }

        [Theory]
        [InlineData(LicenseStatusTitles.InvalidTrial)]
        [InlineData(LicenseStatusTitles.Expired)]
        [InlineData(LicenseStatusTitles.Valid)]
        [InlineData(LicenseStatusTitles.ReceiptUnregistered)]
        [InlineData(LicenseStatusTitles.ReceiptExpired)]
        [InlineData(LicenseStatusTitles.ValidTrial)]
        public async Task OnLaunch_ShouldAlways_SetStatus(LicenseStatusTitles initialState)
        {

            var lic = await testServer.RegisterRandomLicenseAsync(initialState);
            var trialDays = 100U;
            if (initialState == LicenseStatusTitles.InvalidTrial)
            {
                trialDays = 0;
            }
            var context = ContextManager.FromLic(lic, trialDays);
            var launcher = new LicenseHandlingLaunch(context,
                OnLicFileNotFound: (c) =>
                {
                    var installer = new LicenseHandlingInstall(c, null);
                    installer.HandleLicense();
                });
            launcher.HandleLicense();
            Assert.Equal(initialState, context.LicenseModel.Status);
        }

        [Fact]
        public async Task OnLaunch_IfLicExpired_ShouldGetNewOne()
        {
            //ARRANGE
            var lic = await testServer.RegisterRandomLicenseAsync(LicenseStatusTitles.Expired);
            var context = ContextManager.GetContext(lic.Product!.Id!, "VDR_vendor", 4, lic.Computer!.MacAddress!);
            var signedLic = await testServer.GetSignedLicenseXmlAsync(lic, 0);
            context.SetLicenseData(signedLic, false);

            //ACT
            //register on computer
            var register = new LicenseRegister(context);
            register.TryWrite();

            var handler = new LicenseHandlingLaunch(context);
            await handler.HandleLicenseAsync();

            //Assert
            Assert.True(handler.HandlingContext.IsLicenseFreshOutOfServer);
        }

        //[Fact]
        /*public async Task OnLaunch_WhenReceiptIsExpired_ShouldUnregisterComputer()
        {
            //ARRANGE
            var lic = await server.GetLicenseAsync(LicenseStatusTitles.ReceiptExpired);
            var context = ContextManager.GetContext(lic.Product!.Name!, "Hymma", 4, lic.Computer!.MacAddress!);
            var signedLic = await server.SignLicenseAsync(lic);
            context.SetLicenseData(signedLic, false);

            //ACT
            //register on computer
            var register = new LicenseRegister(context);
            register.TryWrite();

            var launcher = new LicenseHandlingLaunch(context);
            await launcher.HandleLicenseAsync();

            //ASSERT
            var expectedLicXml = await server.SignLicenseAsync(lic);
            var expected = Parser.ParseSignedLicense(expectedLicXml);
            Assert.Null(expected.Receipt);
        }*/

        [Fact]
        public async Task OnLaunch_WhenTrialExpired_ShouldRaiseTrialEndedEvent()
        {
            var lic = await licGenerator.SaveNewLicOnDiskAsync(LicenseStatusTitles.InvalidTrial, 0U);
            var handler = new LicenseHandlingLaunch(ContextManager.FromLic(lic, 0U), OnTrialEnded: (t) => Assert.True(true));
            handler.HandleLicense();
        }
        [Theory]
        [InlineData(LicenseStatusTitles.InvalidTrial)]
        [InlineData(LicenseStatusTitles.Expired)]
        [InlineData(LicenseStatusTitles.Valid)]
        [InlineData(LicenseStatusTitles.ReceiptUnregistered)]
        [InlineData(LicenseStatusTitles.ReceiptExpired)]
        [InlineData(LicenseStatusTitles.ValidTrial)]
        public async Task OnInstall_RegardlessOfLicStatus_ShouldWriteItOnDisk(LicenseStatusTitles title)
        {
            var product = testServer.GetProduct(ProductType.NoFeatures);
            var computer = Computers.ForNewLicense();
            var trial = 100U;
            var validDays = 90U;
            var context = ContextManager.GetContext(product.Id, product.Vendor.Id, trial, computer.MacAddress, computer.Name, validDays);

            //we're installing here
            var handler = new LicenseHandlingInstall(context, null);
            await handler.HandleLicenseAsync();

            //ACT
            var expectedFile = Path.Combine(Constants.DefaultLicFileRootDir, product.Vendor.Id, product.Id);

            //ASSERT
            Assert.True(File.Exists(expectedFile));
        }

        [Theory]
        [InlineData(LicenseStatusTitles.InvalidTrial)]
        [InlineData(LicenseStatusTitles.Expired)]
        [InlineData(LicenseStatusTitles.Valid)]
        [InlineData(LicenseStatusTitles.ReceiptUnregistered)]
        [InlineData(LicenseStatusTitles.ReceiptExpired)]
        [InlineData(LicenseStatusTitles.ValidTrial)]
        public async Task OnUninstall_FromServer_shouldUnregisterComputer(LicenseStatusTitles initialState)
        {
            var trialDays = 100U;
            if (initialState == LicenseStatusTitles.InvalidTrial)
                trialDays = 0U;

            var lic = await licGenerator.SaveNewLicOnDiskAsync(initialState, trialDays);

            //we're uninstalling here
            var handler = new LicenseHandlingUninstall(ContextManager.FromLic(lic, trialDays));
            await handler.HandleLicenseAsync();

            //ACT
            //var expected = await _server.FromApi.GetAsync($"License/?productId={lic.Product.Id}&computerId={lic.Computer.Id}");

            var expected = await testServer.GetSignedLicenseXmlAsync(lic);
            var expectedLic = LicenseModel.FromXml(expected);

            //ASSERT
            Assert.Null(expectedLic.Receipt);
        }

        [Theory]
        [InlineData(LicenseStatusTitles.Expired)]
        [InlineData(LicenseStatusTitles.ReceiptUnregistered)]
        [InlineData(LicenseStatusTitles.InvalidTrial)]
        [InlineData(LicenseStatusTitles.ReceiptExpired)]
        public async Task OnLaunch_AfterCustomerUpdatedProductKey_ShouldGrantAccess(LicenseStatusTitles title)
        {
            var trial = 100U;
            if (title == LicenseStatusTitles.InvalidTrial)
            {
                trial = 0;
            }
            var lic = await licGenerator.SaveNewLicOnDiskAsync(title, trial);
            var context = ContextManager.FromLic(lic, trial);
            var handler = new LicenseHandlingLaunch(context,
            OnCustomerMustEnterProductKey: () =>
            {
                var newRec = testServer.GetReceiptForProduct(lic.Product);
                return newRec.Code;
            });

            handler.HandleLicense();

            var signedLic = await testServer.GetSignedLicenseXmlAsync(lic, 90);
            var newlic = LicenseModel.FromXml(signedLic);

            var statusHandler = new LicenseStatus(newlic, DateTime.Now);
            var actual = statusHandler.GetLicenseStatus(context.PublisherPreferences);
            Assert.Equal(LicenseStatusTitles.Valid, actual);
        }

       [Fact]
        public async Task OnLaunch_PublicKeyFromServerTakesPrecedenceOverPublisherValue()
        {
            var lic = await licGenerator.SaveNewLicOnDiskAsync(LicenseStatusTitles.Valid, 10);

            var context = ContextManager.FromLic(lic);

            //remove the correct public-key 
            context.PublisherPreferences.PublicKey = "";
            var handler = new LicenseHandlingLaunch(context);

            handler.HandleLicense();
            Assert.NotEmpty(handler.HandlingContext.PublisherPreferences.PublicKey);

        }
    }
}
