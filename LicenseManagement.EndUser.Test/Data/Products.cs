using Bogus;
using Hymma.Lm.EndUser.Models;

namespace Hymma.Lm.EndUser.Test.Data
{
    public class Products
    {
        public static ProductModel WithOneFeatureOnly()
        {
            var f = new Faker();
            var p = WithNoFeatures();
            p.Features = new List<string>
            {
                f.Commerce.ProductMaterial()
            };
            return p;
        }
        public static ProductModel WithNoFeatures()
        {
            return new Faker<ProductModel>()
                .RuleFor(p=>p.Vendor,new VendorModel() { Name="W@H.com",Id= "VDR_01JAP1JE7FRCJ63FHE5DQJGPY3" })
                .RuleFor(p => p.Name, f => f.Commerce.ProductName());
        }
        public static ProductModel WithManyFeatures(int features = 2)
        {
            var p = WithNoFeatures();
            var f = new Faker();
            p.Features = new List<string>();
            for (int i = 0; i < features; i++)
            {
                p.Features.Add(f.Commerce.ProductDescription());
            }
            return p;
        }
        public static ProductModel FromType(ProductType type)
        {
            switch (type)
            {
                case ProductType.NoFeatures:
                    return WithNoFeatures();
                case ProductType.OneFeature:
                    return WithOneFeatureOnly();
                case ProductType.ManyFeatures:
                    return WithManyFeatures();
                default:
                    return WithNoFeatures();
            }
        }
    }
}
