using Hymma.Lm.EndUser.Extensions;
using Hymma.Lm.EndUser.Models;
using Hymma.Lm.EndUser.Test.Server;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net;
using Xunit;
using Hymma.Lm.EndUser.Test.Data;
using Hymma.Lm.EndUser.License.EndPoint;
using Hymma.Lm.EndUser.Time.EndPoint;
using Hymma.Lm.EndUser.Product.EndPoint;

namespace Hymma.Lm.EndUser.Test.Tests

{
    [Collection(CollectionNames.API_ASYNC)]
    public class ApiEndPointTests : IClassFixture<TestServer>
    {
        private readonly TestServer tesetServer;
        public ApiEndPointTests(TestServer testServer)
        {
            tesetServer = testServer;
        }

        [Fact]
        public async Task LicenseEndPoint_WhenUnregistering_ShouldUnregisterLicense()
        {
            var lic = await tesetServer.RegisterRandomLicenseAsync();

            var licEndPoint = new LicenseApiEndPoint(ContextManager.ApiKey);
            var status = await licEndPoint.PatchLicenseAsync(new PatchLicenseModel() { License = lic.Id, Code = null });

            Assert.Equal(HttpStatusCode.NoContent, status);
        }

        [Fact]
        public async Task LicenseEndpoint_WhenGettingAvailableLic_ShouldNotBeNull()
        {
            //arrange
            var lic = await tesetServer.RegisterRandomLicenseAsync();

            var licEndPoint = new LicenseApiEndPoint(ContextManager.ApiKey);

            //act
            var result = await licEndPoint.GetLicenseAsync(lic.Computer!.Id, lic.Product!.Id,null, validDays: 2);

            //assert
            //Assert.True(result.IsSuccessStatusCode);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task LicenseEndPoint_WhenPostingNewLic_ShouldReturnCreatedStatusCode()
        {
            //arrange
            var comp = await tesetServer.RegisterRandomComputer();

            var product = await tesetServer.RegisterRandomProductAsync(Data.ProductType.NoFeatures);
            var licEndPoint = new LicenseApiEndPoint(ContextManager.ApiKey);

            //act
            var model = new PostLicenseModel() { Computer = comp.Id, Product = product.Id };
            var result = await licEndPoint.PostLicenseAsync(model);

            //Assert
            Assert.Equal(HttpStatusCode.Created, result);
        }

        [Fact]
        public async Task LicenseEndPoint_WhenPostingExistingLic_ShouldReturnConflictStatus()
        {
            //arrange
            var lic = await tesetServer.RegisterRandomLicenseAsync();
            var licEndPoint = new LicenseApiEndPoint(ContextManager.ApiKey);

            //act
            var model = new PostLicenseModel() { Computer = lic.Computer.Id, Product = lic.Product.Id };
            var result = await licEndPoint.PostLicenseAsync(model);

            //assert
            Assert.Equal(HttpStatusCode.Conflict, result);
        }

        [Fact]
        public async Task ProductEndPoint_WhenGettingAvailableProduct_ShouldReturnProduct()
        {
            //arrange
            var expected = await tesetServer.RegisterRandomProductAsync(ProductType.OneFeature);

            //act
            var endPoint = new ProductApiEndPoint(ContextManager.ApiKey);
            var actual = await endPoint.GetProductAsync(expected.Id);

            //assert
            Assert.Equal(expected, actual, (e, a) =>  e.Equals(a) );
        }

        [Fact]
        public async Task DateTimeEndPoint_WhenGetting_ShouldReturnCurrentDateTime()
        {
            //arrange
            var endpoint = new DateTimeApiEndPoint(ContextManager.ApiKey);
            var defaultTime = new DateTime();

            //act
            var time = await endpoint.GetCurrentUtcTimeAsync();

            //assert
            Assert.IsAssignableFrom<DateTime>(time);
            Assert.NotEqual(defaultTime, time);
        }

        //[Fact]
        //public async Task ComputerEndPoint_WhenGetting_ShouldReturnComputer()
        //{
        //    //arrange
        //    var computer = await tesetServer.RegisterRandomComputer();
        //    var endpoint = new ComputerApiEndPoint(ContextManager.ApiKey);
        //    ComputerId.Instance.MachineId = computer.MacAddress;
        //    ComputerId.Instance.MachineName= computer.Name;

        //    //act
        //    var actual = endpoint.GetComputer();

        //    //assert
        //    Assert.Equal(computer, actual);
        //}

        //[Fact]
        //public async Task ComputerEndPoint_WhenPostingNewComputer_ShouldReturnCreatedStatusCode()
        //{
        //    //arrange
        //    var computer = Computers.FromRandom();
        //    ComputerId.Instance.MachineId = computer.MacAddress;
        //    ComputerId.Instance.MachineName= computer.Name;
        //    var endpoint = new ComputerApiEndPoint(ContextManager.ApiKey);

        //    //act
        //    var statusCode = await endpoint.PostComputerAsync();

        //    //assert
        //    Assert.Equal(HttpStatusCode.Created, statusCode);
        //}

        [Theory]
        [InlineData("", "")]
        [InlineData("product", "2")]
        [InlineData("premium product", "best in the market")]
        [InlineData("5NewMen%", "OverInTheBar")]
        public void UriExtension_WhenProvideNameValuePairs_ShouldConvertToQueryString(string key, string value)
        {
            //arrange
            var uri = new Uri("http://hymma.net/");
            var collection = new List<KeyValuePair<string, string>>();
            var qbuilder = new QueryBuilder
            {
                { key, value }
            };
            collection.Add(new KeyValuePair<string, string>(key, value));
            var expected = uri + qbuilder.AsQueryable().ToString();

            //act
            var actual = uri.WithQueryString(collection);

            //assert
            Assert.Equal(expected, actual);
        }
    }

}
