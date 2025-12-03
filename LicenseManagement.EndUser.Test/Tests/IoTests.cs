using Hymma.Lm.EndUser.Registrars;
using Xunit;
namespace Hymma.Lm.EndUser.Test.Tests
{

    public class InputOutputTests : DisposableTest
    {

        /*public void CreateApiKeyFile()
        {
            var bytes = Encoding.UTF8.GetBytes("");
            var protectedBytes = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(@"protectedBytes", protectedBytes);
        }*/

        [Theory]
        [InlineData("test\\project")]
        [InlineData("tested/backslash")]
        [InlineData("*(_|someO~`.files")]
        public void OnLicRegistration_WhenThereIsInvalidChars_ShouldRegister(string productNameInvalidChars)
        {
            //ARRANGE
            var context = ContextManager.GetContext(productNameInvalidChars, "VDR_publisherId", 1);


            context.SetLicenseData(Guid.NewGuid().ToString(), false);
            var register = new LicenseRegister(context);

            //ACT
            register.TryWrite();
            register.TryRead(out string expectedSignedLic);

            //ASSERT
            Assert.Equal(expectedSignedLic, context.SignedLicense);
        }

        [Fact]
        public void OnLicRegistration_WhenMultipleProducts_ShouldRegisterAll()
        {
            //arrange
            var productContentPairs = new Dictionary<string, string>()
            {
                ["productOne"] = Guid.NewGuid().ToString(),
                ["productTwo"] = Guid.NewGuid().ToString(),
                ["productThree"] = Guid.NewGuid().ToString(),
            };

            //act
            var contexts = new List<LicHandlingContext>();
            var pairs = new List<KeyValuePair<string, string>>();
            foreach (var item in productContentPairs)
            {
                var context = ContextManager.GetContext(item.Key, "VDR_vendor", 29);
                context.SetLicenseData(item.Value, false);
                contexts.Add(context);
                var rego = new LicenseRegister(context);
                rego.TryWrite();
            }

            //now read what is written on disk
            foreach (var context in contexts)
            {
                var rego = new LicenseRegister(context);
                rego.TryRead(out string result);
                pairs.Add(new KeyValuePair<string, string>(context.SignedLicense, result));
            }


            //assert that all of them exist, and not over-written
            Assert.All(pairs, (pair) =>
            {
                Assert.True(pair.Key == pair.Value);
            });
        }

        [Fact]
        public void OnLicRegistration_SameProductFromMultiplePublishers_ShouldRegisterAll()
        {
            //arrange
            var productName = "product";
            var publishers = new string[] { "Hymma", "HymmaCompetitor" };

            //Act

            var contexts = new List<LicHandlingContext>();
            var pairs = new List<KeyValuePair<string, string>>();

            foreach (var publisher in publishers)
            {
                var context = ContextManager.GetContext(productName, publisher, 1, "testMacAddress");
                context.SetLicenseData(Guid.NewGuid().ToString(), false);
                contexts.Add(context);
                var register = new LicenseRegister(context);
                register.TryWrite();
            }

            //now read what is written on disk
            foreach (var context in contexts)
            {
                var rego = new LicenseRegister(context);
                rego.TryRead(out string result);
                pairs.Add(new KeyValuePair<string, string>(context.SignedLicense, result));
            }
            //assert
            Assert.All(pairs, (pair) =>
            {
                Assert.True(pair.Key == pair.Value);
            });
        }

        [Fact]
        public void OnLicRegistration_WhenLicFileExists_ShouldOverwrite()
        {
            //arrange
            var productName = "product";
            var publishers = new string[] { "Hymma", "Hymma" };

            //Act
            var licFileNames = new HashSet<string>();
            foreach (var publisher in publishers)
            {
                var context = ContextManager.GetContext(productName, publisher, 1, "testMacAddress");
                var register = new LicenseRegister(context);
                licFileNames.Add(register.ResolveFileName(productName));
            }

            //assert
            Assert.Single(licFileNames);
        }

        [Fact]
        public void OnLicRegistration_DefaultDirectory_ShouldBeInLocalAppData()
        {
            var context = ContextManager.GetContext("product", "VDR_vendor", 1);
            var register = new LicenseRegister(context);
            var fullFileName = register.ResolveFileName(context.PublisherPreferences.ProductId);
            var parent = Directory.GetParent(fullFileName)?.FullName;
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Assert.Contains(localAppData, parent);
        }
    }
}