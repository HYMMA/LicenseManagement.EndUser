using LicenseManagement.EndUser.Models;

namespace LicenseManagement.EndUser.Test.Data
{
    /// <summary>
    /// Static test data for products matching the SQL seed script in WebApi.Test.
    /// IDs include the server-mandated prefix (PRD_ for products, VDR_ for vendors).
    /// </summary>
    public static class Products
    {
        public static VendorModel TestVendor => new VendorModel
        {
            Id = "VDR_01JAP1JE7FRCJ63FHE5DQJGPY3",
            Name = "TestVendor@test.com"
        };

        public static ProductModel Basic => new ProductModel
        {
            Id = "PRD_01JTEZTPRD00001BA51CAAA001",
            Name = "Test Product Basic",
            Vendor = TestVendor,
            Features = new List<string>()
        };

        public static ProductModel Pro => new ProductModel
        {
            Id = "PRD_01JTEZTPRD00002PR0FAAAA002",
            Name = "Test Product Pro",
            Vendor = TestVendor,
            Features = new List<string> { "Pro Feature" }
        };

        public static ProductModel Enterprise => new ProductModel
        {
            Id = "PRD_01JTEZTPRD00003ENTRAAAA003",
            Name = "Test Product Enterprise",
            Vendor = TestVendor,
            Features = new List<string> { "Enterprise Feature 1", "Enterprise Feature 2", "Enterprise Feature 3" }
        };

        public static ProductModel Developer => new ProductModel
        {
            Id = "PRD_01JTEZTPRD00004DEV1AAAA004",
            Name = "Test Product Developer",
            Vendor = TestVendor,
            Features = new List<string> { "Developer Feature 1", "Developer Feature 2" }
        };

        public static ProductModel Ultimate => new ProductModel
        {
            Id = "PRD_01JTEZTPRD00005V1TMAAAA005",
            Name = "Test Product Ultimate",
            Vendor = TestVendor,
            Features = new List<string> { "Ultimate Feature 1", "Ultimate Feature 2", "Ultimate Feature 3", "Ultimate Feature 4" }
        };

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

        public static ProductModel[] All => new[] { Basic, Pro, Enterprise, Developer, Ultimate };
    }
}
