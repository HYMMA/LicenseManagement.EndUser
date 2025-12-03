using Hymma.Lm.EndUser.License;
using Hymma.Lm.EndUser.Models;
using Hymma.Lm.EndUser.Test.Data;
using Hymma.Lm.EndUser.Test.Utilities;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace Hymma.Lm.EndUser.Test.Server
{
    public class TestServer
    {
        //public static 
        public TestServer()
        {
            HttpClient = HttpClientFactory.Create(new AuthenticationHandler());
            HttpClient.BaseAddress = new Uri("http://localhost:7298/api/");
        }
        public HttpClient HttpClient;

        /// <summary>
        /// gets the public key
        /// </summary>
        /// <returns></returns>
        internal Task<string> GetPublicKeyAsync()
        {
            return HttpClient.GetStringAsync("PublicKey");
        }
        /// <summary>
        /// returns a signed xml of the license form server
        /// </summary>
        /// <param name="lic"></param>
        /// <returns></returns>
        public async Task<string> GetSignedLicenseXmlAsync(LicenseModel lic)
            => await HttpClient.GetStringAsync($"license?computer={lic.Computer.Id}&product={lic.Product.Id}");


        /// <summary>
        /// gets a license with valid days set to a specific number 
        /// </summary>
        /// <param name="lic">the license that needs to be retrieved from server</param>
        /// <param name="validDays">days a license file could be valid</param>
        /// <returns>signed license</returns>
        public async Task<string> GetSignedLicenseXmlAsync(LicenseModel lic, int validDays)
            => await HttpClient.GetStringAsync($"license?computer={lic.Computer.Id}&product={lic.Product.Id}&validDays={validDays}");

        public async Task<ComputerModel> RegisterRandomComputer()
        {
            var c = Computers.FromRandom();
            var msg = await HttpClient.PostAsJsonAsync("computer", new { c.MacAddress, c.Name });
            if (!msg.IsSuccessStatusCode)
                ThrowHelper.ThrowUnSuccessfulRequest(HttpClient.BaseAddress + "computer");
            return await HttpClient.GetFromJsonAsync<ComputerModel>(msg.Headers.Location!)
                ?? throw new Exception("no computer was found");
        }

        public async Task<ProductModel> RegisterRandomProductAsync(ProductType type)
        {
            var p = Products.FromType(type);
            var msg = await HttpClient.PostAsJsonAsync("product", new { p.Name })
                ?? throw new NullReferenceException("message from server was null");

            //even bogus makes the same product twice sometimes
            if (msg.StatusCode == HttpStatusCode.Conflict)
                return await HttpClient.GetFromJsonAsync<ProductModel>($"product?id={p.Id}")
                    ?? throw new NullReferenceException($"Could not get {p.Id} from server ");

            if (!msg.IsSuccessStatusCode)
                ThrowHelper.ThrowUnSuccessfulRequest(HttpClient.BaseAddress + "product");


            if (msg.Headers.Location == null)
                throw new NullReferenceException($"response from product has no location header. for product {p.Name}");

            var val = await HttpClient.GetFromJsonAsync<ProductModel>(Uri.EscapeUriString(msg.Headers.Location.ToString()));
            return val ?? throw new Exception("Product was null");
        }

        /// <summary>
        /// creates a new receipt for a product , as if someone has just purchased it
        /// </summary>
        /// <param name="p">the product to make the receipt for</param>
        /// <param name="expires">the time the receipt will expire</param>
        /// <returns></returns>
        /// <exception cref="Exception">When the server response is not successful</exception>
        public async Task<ReceiptModel> RegisterRandomReceiptForProduct(ProductModel p, DateTime expires)
        {
            var r = Receipts.ForProduct(p, expires);
            var msg = await HttpClient.PostAsJsonAsync("receipt", new { r.Qty, Product = r.Product.Id, r.Code, r.BuyerEmail, r.Expires });
            if (!msg.IsSuccessStatusCode)
                ThrowHelper.ThrowUnSuccessfulRequest(HttpClient.BaseAddress + "receipt");
            var val = await HttpClient.GetFromJsonAsync<ReceiptModel>(Uri.EscapeUriString(msg.Headers.Location.ToString()));
            return val ?? throw new Exception("returned receipt was null");
        }

        /// <summary>
        /// creates a bogus license in the server
        /// </summary>
        /// <param name="licenseStatusTitles"></param>
        /// <param name="type"></param>
        /// <returns>the license that was created</returns>
        public async Task<LicenseModel> RegisterRandomLicenseAsync(LicenseStatusTitles licenseStatusTitles = LicenseStatusTitles.Valid, ProductType type = ProductType.ManyFeatures)
        {
            var c = await RegisterRandomComputer();
            var p = await RegisterRandomProductAsync(type);
            var r = await RegisterRandomReceiptForProduct(p, DateTime.Now.AddDays(99));
            LicenseModel l = new();
            switch (licenseStatusTitles)
            {
                case LicenseStatusTitles.Expired:
                    l = await PostLicenseAsync(r, p, c, 0);
                    break;
                case LicenseStatusTitles.Valid:
                    l = await PostLicenseAsync(r, p, c, 90);
                    break;
                case LicenseStatusTitles.ValidTrial:
                    l = await PostLicenseAsync(null, p, c, 90);
                    break;
                case LicenseStatusTitles.InValidTrial:
                    l = await PostLicenseAsync(null, p, c, 90);
                    break;
                case LicenseStatusTitles.ReceiptExpired:
                    r = await RegisterRandomReceiptForProduct(p, DateTime.Now.Subtract(TimeSpan.FromDays(5)));
                    l = await PostLicenseAsync(r, p, c, 10);
                    break;
                case LicenseStatusTitles.ReceiptUnregistered:
                    l = await PostLicenseAsync(r, p, c, 90);
                    var response = await HttpClient.PatchAsJsonAsync("license", new { License = l.Id });
                    response.EnsureSuccessStatusCode();

                    //get un-registered receipt
                    l = LicenseModel.FromXml(await GetSignedLicenseXmlAsync(l));
                    break;
                default:
                    break;
            }
            return l;
        }

        async Task<LicenseModel> PostLicenseAsync(ReceiptModel? r, ProductModel p, ComputerModel c, int validDays = 90)
        {
            var lic = Licenses.FromType(r, p, c);
            var response = await HttpClient.PostAsJsonAsync("license", new { Product = p.Id, Computer = c.Id });
            response.EnsureSuccessStatusCode();

            lic = LicenseModel.FromXml(await GetSignedLicenseXmlAsync(lic, validDays));
            if (r != null)
            {
                var patchResponse = await HttpClient.PatchAsJsonAsync("license", new { License = lic.Id, Code = r.Code });
                patchResponse.EnsureSuccessStatusCode();
            }

            var xml = await GetSignedLicenseXmlAsync(lic, validDays);
            var val = LicenseModel.FromXml(xml);
            return val;
        }
    }
}
