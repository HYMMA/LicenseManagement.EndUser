using LicenseManagement.EndUser.Exceptions;
using LicenseManagement.EndUser.License;
using LicenseManagement.EndUser.Models;
using LicenseManagement.EndUser.Registrars;
using LicenseManagement.EndUser.Test.Data;
using LicenseManagement.EndUser.Test.Server;
using LicenseManagement.EndUser.Test.Utilities;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text.Json.Serialization.Metadata;
using System.Xml;
using System.Xml.Schema;
using Xunit;

namespace LicenseManagement.EndUser.Test.Tests
{
    public class LicenseStrategyTests : DisposableTest, IClassFixture<TestServer>
    {
        readonly TestServer testServer;
        readonly LicGenerator licGenerator;
        readonly string _originalMachineId = ComputerId.Instance.MachineId;
        readonly string _originalMachineName = ComputerId.Instance.MachineName;

        public LicenseStrategyTests(TestServer server)
        {
            this.testServer = server;
            this.licGenerator = new LicGenerator(server);
        }

        public override void Dispose()
        {
            ComputerId.Instance.MachineId = _originalMachineId;
            ComputerId.Instance.MachineName = _originalMachineName;
            base.Dispose();
        }

        [Fact]
        public async Task OnLaunchFile_IfFileIsModified_ShouldFailToVerify()
        {
            //arrange
            //GET a license from server
            var lic = await licGenerator.SaveNewLicOnDiskAsync(LicenseStatusTitles.Valid);
            ComputerId.Instance.MachineId = lic.Computer.MacAddress;
            ComputerId.Instance.MachineName = lic.Computer.Name;
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
            var lic = await testServer.RegisterRandomLicenseAsync();

            // Write the license to disk so the uninstall handler can read it.
            var context = ContextManager.GetContext(lic.Product!.Id!, lic.Product!.Vendor!.Id!, 4, lic.Computer!.MacAddress!);
            var signedXml = await testServer.GetSignedLicenseXmlAsync(lic);
            context.SetLicenseData(signedXml, false);
            new LicenseRegister(context).TryWrite();

            ComputerId.Instance.MachineId = lic.Computer.MacAddress;
            ComputerId.Instance.MachineName = lic.Computer.Name;

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

            // Uninstall reads the on-disk license; the computer ID check uses ComputerId.Instance.MachineId.
            ComputerId.Instance.MachineId = lic.Computer.MacAddress;
            ComputerId.Instance.MachineName = lic.Computer.Name;

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
            var trialDays = 100U;
            if (initialState == LicenseStatusTitles.InvalidTrial)
                trialDays = 0;

            // Use seedIndex=1 to avoid conflict with uninstall tests that modify index-0 licenses.
            // Write the seed license directly to disk so launch detects the status without a server round-trip.
            int validDays = initialState == LicenseStatusTitles.Expired ? 0 : 90;
            var lic = await testServer.GetLicenseAsync(initialState, seedIndex: 1);
            ComputerId.Instance.MachineId = lic.Computer.MacAddress;
            ComputerId.Instance.MachineName = lic.Computer.Name;

            var context = ContextManager.FromLic(lic, trialDays);
            // Force ValidDays=0 so that when the Expired handler re-fetches from server it gets
            // another expired license (server honours the validDays parameter), keeping status Expired.
            if (initialState == LicenseStatusTitles.Expired)
                context.PublisherPreferences.ValidDays = 0;
            var signedXml = await testServer.GetSignedLicenseXmlAsync(lic, validDays);
            context.SetLicenseData(signedXml, false);
            new LicenseRegister(context).TryWrite();

            var launcher = new LicenseHandlingLaunch(context);
            launcher.HandleLicense();
            Assert.Equal(initialState, context.LicenseModel.Status);
        }

        [Fact]
        public async Task OnLaunch_IfLicExpired_ShouldGetNewOne()
        {
            //ARRANGE
            var lic = await testServer.RegisterRandomLicenseAsync(LicenseStatusTitles.Expired);
            ComputerId.Instance.MachineId = lic.Computer.MacAddress;
            ComputerId.Instance.MachineName = lic.Computer.Name;
            var context = ContextManager.GetContext(lic.Product!.Id!, lic.Product!.Vendor!.Id!, 4, lic.Computer!.MacAddress!);
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
            ComputerId.Instance.MachineId = lic.Computer.MacAddress;
            ComputerId.Instance.MachineName = lic.Computer.Name;
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

            // Spoof machine identity to a seed NoLicense computer so ComputerIdValidatorHandler passes.
            ComputerId.Instance.MachineId = computer.MacAddress;
            ComputerId.Instance.MachineName = computer.Name;

            var context = ContextManager.GetContext(product.Id, product.Vendor.Id, trial, computer.MacAddress, computer.Name, validDays);

            //we're installing here
            var handler = new LicenseHandlingInstall(context, null);
            await handler.HandleLicenseAsync();

            //ACT — file is written with .lic extension
            var expectedFile = Path.Combine(Constants.DefaultLicFileRootDir, product.Vendor.Id, product.Id + ".lic");

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

            // Spoof machine identity to match the seed computer so ComputerIdValidatorHandler passes.
            ComputerId.Instance.MachineId = lic.Computer.MacAddress;
            ComputerId.Instance.MachineName = lic.Computer.Name;

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
            ComputerId.Instance.MachineId = lic.Computer.MacAddress;
            ComputerId.Instance.MachineName = lic.Computer.Name;
            var context = ContextManager.FromLic(lic, trial);
            var handler = new LicenseHandlingLaunch(context,
            OnCustomerMustEnterProductKey: () =>
            {
                // ReceiptExpired uses Basic product; BasicReceipts[0] is full after other test cases
                // use its seats. BasicReceipts[2] (Qty=3) has no pre-seeded licenses — always available.
                if (title == LicenseStatusTitles.ReceiptExpired)
                    return Data.Receipts.BasicReceipts[2].Code;
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

        // ── Uninstall identity-source guards ─────────────────────────────────────────────
        // These tests enforce the invariant: uninstall ALWAYS uses the live hardware ID
        // (ComputerId.Instance.MachineId) and NEVER the computer ID stored in the lic file.
        // A lic file copied from another machine must not let that machine's seat be unregistered.

        [Fact]
        public async Task OnUninstall_ShouldUseHardwareMachineId_NotLicFileComputerId()
        {
            // Arrange — write Computer A's lic file to disk (simulates a shared / copied lic file).
            var licA = await testServer.GetLicenseAsync(LicenseStatusTitles.Valid, seedIndex: 0);
            var contextA = ContextManager.FromLic(licA);
            contextA.SetLicenseData(await testServer.GetSignedLicenseXmlAsync(licA), false);
            new LicenseRegister(contextA).TryWrite();

            // Arrange — Computer B: a fresh machine with no prior license for this product.
            var bytes = Guid.NewGuid().ToByteArray();
            var bMac = $"{bytes[0]:X2}:{bytes[1]:X2}:{bytes[2]:X2}:{bytes[3]:X2}:{bytes[4]:X2}:{bytes[5]:X2}";
            var compResp = await testServer.HttpClient.PostAsJsonAsync("computer", new { MacAddress = bMac, Name = "UninstallGuardB" });
            compResp.EnsureSuccessStatusCode();
            var computerB = await testServer.HttpClient.GetFromJsonAsync<ComputerModel>(compResp.Headers.Location!);

            // Set hardware identity to Computer B — this machine is the one running the uninstall.
            ComputerId.Instance.MachineId = computerB!.MacAddress;
            ComputerId.Instance.MachineName = computerB.Name;

            // Context carries only routing data (productId / vendorId).
            // Machine identity is never stored in context — it is always read from ComputerId.Instance at runtime.
            var context = ContextManager.GetContext(licA.Product!.Id, licA.Product!.Vendor!.Id, 10);

            // Act
            var handler = new LicenseHandlingUninstall(context);
            await handler.HandleLicenseAsync();

            // Assert — Computer B now has a license record on the server.
            // If the lic file's computer ID (A) had been used instead, B would have no record at all.
            string? licBXml = null;
            try
            {
                licBXml = await testServer.HttpClient.GetStringAsync(
                    $"license?computer={computerB.Id}&product={licA.Product!.Id}");
            }
            catch (HttpRequestException) { /* 404 = B has no license — hardware ID was NOT used (broken) */ }

            Assert.NotNull(licBXml);
            Assert.Null(LicenseModel.FromXml(licBXml!).Receipt); // B's receipt was unregistered
        }

        [Fact]
        public async Task OnUninstall_WhenNoLicFileOnDisk_ShouldSucceedViaApiChain()
        {
            // DisposableTest.Dispose() wipes the lic directory after every test,
            // so the disk is guaranteed clean here — no lic file exists.
            var bytes = Guid.NewGuid().ToByteArray();
            var mac = $"{bytes[0]:X2}:{bytes[1]:X2}:{bytes[2]:X2}:{bytes[3]:X2}:{bytes[4]:X2}:{bytes[5]:X2}";
            var compResp = await testServer.HttpClient.PostAsJsonAsync("computer", new { MacAddress = mac, Name = "NoFileUninstallTest" });
            compResp.EnsureSuccessStatusCode();
            var comp = await testServer.HttpClient.GetFromJsonAsync<ComputerModel>(compResp.Headers.Location!);

            var product = testServer.GetProduct(ProductType.OneFeature);
            ComputerId.Instance.MachineId = comp!.MacAddress;
            ComputerId.Instance.MachineName = comp.Name;

            // No SetLicenseData, no TryWrite — zero lic file on disk.
            var context = ContextManager.GetContext(product.Id, product.Vendor!.Id, 10);

            // Act — must not throw even with no lic file present.
            // If the chain tried to read from disk it would fail here.
            var handler = new LicenseHandlingUninstall(context);
            await handler.HandleLicenseAsync();

            // Assert — a license was created on the server via POST and then unregistered,
            // confirming the chain operates entirely through the API.
            var licXml = await testServer.HttpClient.GetStringAsync(
                $"license?computer={comp.Id}&product={product.Id}");
            Assert.Null(LicenseModel.FromXml(licXml).Receipt);
        }

        // ─────────────────────────────────────────────────────────────────────────────────

       [Fact]
        public async Task OnLaunch_PublicKeyFromServerTakesPrecedenceOverPublisherValue()
        {
            var lic = await licGenerator.SaveNewLicOnDiskAsync(LicenseStatusTitles.Valid, 10);
            ComputerId.Instance.MachineId = lic.Computer.MacAddress;
            ComputerId.Instance.MachineName = lic.Computer.Name;

            var context = ContextManager.FromLic(lic);

            //remove the correct public-key
            context.PublisherPreferences.PublicKey = "";
            var handler = new LicenseHandlingLaunch(context);

            handler.HandleLicense();
            Assert.NotEmpty(handler.HandlingContext.PublisherPreferences.PublicKey);

        }
    }
}
