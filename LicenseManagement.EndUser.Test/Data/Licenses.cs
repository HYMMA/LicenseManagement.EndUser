using Bogus;
using Hymma.Lm.EndUser.Models;

namespace Hymma.Lm.EndUser.Test.Data
{
    public class Licenses
    {
        public static LicenseModel FromType(ReceiptModel? receipt, ProductModel product, ComputerModel computer)
        {
            var l = new Faker<LicenseModel>()
                             .RuleFor(l => l.Product, product)
                             .RuleFor(l => l.Computer, computer)
                             .RuleFor(l => l.Receipt, receipt)
                             .Generate(1)
                             .First();
            return l;
        }
    }
}
