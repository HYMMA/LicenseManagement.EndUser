using Bogus;
using Hymma.Lm.EndUser.Models;

namespace Hymma.Lm.EndUser.Test.Data
{
    public class Receipts
    {
        public static ReceiptModel ForProduct(ProductModel product, DateTime expire)
        {
            if (string.IsNullOrEmpty(product.Id))
                ThrowHelper.ThrowNoIdException(product.Id);

            var r = new Faker<ReceiptModel>().RuleFor(r => r.Code, f => f.Random.Replace("******-******-******-******"))
           .RuleFor(r => r.Qty, f => f.Random.Number(1, 10))
           .RuleFor(r => r.Expires, f => f.Date.Between(DateTime.Now, expire))
           .RuleFor(r => r.Product, product)
           .RuleFor(r => r.BuyerEmail, f => f.Internet.Email())
           .Generate(1).First();
            return r;
        }
    }
}
