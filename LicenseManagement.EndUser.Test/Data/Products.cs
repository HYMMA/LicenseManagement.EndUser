using Hymma.Lm.EndUser.Models;

namespace Hymma.Lm.EndUser.Test.Data
{
    /// <summary>
    /// Static test data for products matching the SQL seed script in WebApi.Test
    /// </summary>
    public static class Products
    {
        /// <summary>
        /// The test vendor used in all test data
        /// </summary>
        public static VendorModel TestVendor => new VendorModel
        {
            Id = "01JAP1JE7FRCJ63FHE5DQJGPY3",
            Name = "TestVendor@test.com"
        };

        /// <summary>
        /// Product 1 - Basic (no features)
        /// </summary>
        public static ProductModel Basic => new ProductModel
        {
            Id = "01JTESTPRD00001BASICAAA001",
            Name = "Test Product Basic",
            Vendor = TestVendor,
            Features = new List<string>()
        };

        /// <summary>
        /// Product 2 - Pro (one feature)
        /// </summary>
        public static ProductModel Pro => new ProductModel
        {
            Id = "01JTESTPRD00002PROFAAAA002",
            Name = "Test Product Pro",
            Vendor = TestVendor,
            Features = new List<string> { "Pro Feature" }
        };

        /// <summary>
        /// Product 3 - Enterprise (many features)
        /// </summary>
        public static ProductModel Enterprise => new ProductModel
        {
            Id = "01JTESTPRD00003ENTRAAAA003",
            Name = "Test Product Enterprise",
            Vendor = TestVendor,
            Features = new List<string> { "Enterprise Feature 1", "Enterprise Feature 2", "Enterprise Feature 3" }
        };

        /// <summary>
        /// Product 4 - Developer
        /// </summary>
        public static ProductModel Developer => new ProductModel
        {
            Id = "01JTESTPRD00004DEVLAAAA004",
            Name = "Test Product Developer",
            Vendor = TestVendor,
            Features = new List<string> { "Developer Feature 1", "Developer Feature 2" }
        };

        /// <summary>
        /// Product 5 - Ultimate
        /// </summary>
        public static ProductModel Ultimate => new ProductModel
        {
            Id = "01JTESTPRD00005ULTMAAAA005",
            Name = "Test Product Ultimate",
            Vendor = TestVendor,
            Features = new List<string> { "Ultimate Feature 1", "Ultimate Feature 2", "Ultimate Feature 3", "Ultimate Feature 4" }
        };

        /// <summary>
        /// Gets a product based on the ProductType enum
        /// </summary>
        public static ProductModel FromType(ProductType type)
        {
            return type switch
            {
                ProductType.NoFeatures => Basic,
                ProductType.OneFeature => Pro,
                ProductType.ManyFeatures => Enterprise,
                _ => Basic,
            };
        }

        /// <summary>
        /// All products as an array
        /// </summary>
        public static ProductModel[] All => new[] { Basic, Pro, Enterprise, Developer, Ultimate };
    }
}
