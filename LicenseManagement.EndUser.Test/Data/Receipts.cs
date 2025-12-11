using Hymma.Lm.EndUser.Models;

namespace Hymma.Lm.EndUser.Test.Data
{
    /// <summary>
    /// Static test data for receipts matching the SQL seed script in WebApi.Test
    /// </summary>
    public static class Receipts
    {
        /// <summary>
        /// Valid receipts for Product 1 (Basic)
        /// </summary>
        public static ReceiptModel[] BasicReceipts => new[]
        {
            new ReceiptModel { Id = "01JTESTRCP00001VALDAAAA001", Code = "VALID1-000001-000001-000001", Qty = 4, BuyerEmail = "buyer1@test.com", Product = Products.Basic, Expires = DateTime.UtcNow.AddMonths(3) },
            new ReceiptModel { Id = "01JTESTRCP00002VALDAAAA002", Code = "VALID2-000002-000002-000002", Qty = 2, BuyerEmail = "buyer2@test.com", Product = Products.Basic, Expires = DateTime.UtcNow.AddMonths(3) },
            new ReceiptModel { Id = "01JTESTRCP00003VALDAAAA003", Code = "VALID3-000003-000003-000003", Qty = 3, BuyerEmail = "buyer3@test.com", Product = Products.Basic, Expires = DateTime.UtcNow.AddMonths(3) },
        };

        /// <summary>
        /// Valid receipts for Product 2 (Pro)
        /// </summary>
        public static ReceiptModel[] ProReceipts => new[]
        {
            new ReceiptModel { Id = "01JTESTRCP00004VALDAAAA004", Code = "VALID4-000004-000004-000004", Qty = 4, BuyerEmail = "buyer4@test.com", Product = Products.Pro, Expires = DateTime.UtcNow.AddMonths(3) },
            new ReceiptModel { Id = "01JTESTRCP00005VALDAAAA005", Code = "VALID5-000005-000005-000005", Qty = 2, BuyerEmail = "buyer5@test.com", Product = Products.Pro, Expires = DateTime.UtcNow.AddMonths(3) },
        };

        /// <summary>
        /// Valid receipts for Product 3 (Enterprise)
        /// </summary>
        public static ReceiptModel[] EnterpriseReceipts => new[]
        {
            new ReceiptModel { Id = "01JTESTRCP00006VALDAAAA006", Code = "VALID6-000006-000006-000006", Qty = 3, BuyerEmail = "buyer6@test.com", Product = Products.Enterprise, Expires = DateTime.UtcNow.AddMonths(3) },
            new ReceiptModel { Id = "01JTESTRCP00007VALDAAAA007", Code = "VALID7-000007-000007-000007", Qty = 2, BuyerEmail = "buyer7@test.com", Product = Products.Enterprise, Expires = DateTime.UtcNow.AddMonths(3) },
        };

        /// <summary>
        /// Valid receipts for Product 4 (Developer)
        /// </summary>
        public static ReceiptModel[] DeveloperReceipts => new[]
        {
            new ReceiptModel { Id = "01JTESTRCP00008VALDAAAA008", Code = "VALID8-000008-000008-000008", Qty = 1, BuyerEmail = "buyer8@test.com", Product = Products.Developer, Expires = DateTime.UtcNow.AddMonths(3) },
            new ReceiptModel { Id = "01JTESTRCP00009VALDAAAA009", Code = "VALID9-000009-000009-000009", Qty = 2, BuyerEmail = "buyer9@test.com", Product = Products.Developer, Expires = DateTime.UtcNow.AddMonths(3) },
        };

        /// <summary>
        /// Valid receipts for Product 5 (Ultimate)
        /// </summary>
        public static ReceiptModel[] UltimateReceipts => new[]
        {
            new ReceiptModel { Id = "01JTESTRCP00010VALDAAAA010", Code = "VALDA-00000A-00000A-00000A", Qty = 3, BuyerEmail = "buyer10@test.com", Product = Products.Ultimate, Expires = DateTime.UtcNow.AddMonths(3) },
            new ReceiptModel { Id = "01JTESTRCP00011VALDAAAA011", Code = "VALDB-00000B-00000B-00000B", Qty = 2, BuyerEmail = "buyer11@test.com", Product = Products.Ultimate, Expires = DateTime.UtcNow.AddMonths(3) },
        };

        /// <summary>
        /// Expired receipts
        /// </summary>
        public static ReceiptModel[] ExpiredReceipts => new[]
        {
            new ReceiptModel { Id = "01JTESTRCP00012EXPDAAAA012", Code = "EXPRD1-000001-EXPRD-000001", Qty = 2, BuyerEmail = "expired1@test.com", Product = Products.Basic, Expires = DateTime.UtcNow.AddDays(-10) },
            new ReceiptModel { Id = "01JTESTRCP00013EXPDAAAA013", Code = "EXPRD2-000002-EXPRD-000002", Qty = 1, BuyerEmail = "expired2@test.com", Product = Products.Pro, Expires = DateTime.UtcNow.AddDays(-5) },
            new ReceiptModel { Id = "01JTESTRCP00014EXPDAAAA014", Code = "EXPRD3-000003-EXPRD-000003", Qty = 3, BuyerEmail = "expired3@test.com", Product = Products.Enterprise, Expires = DateTime.UtcNow.AddDays(-3) },
            new ReceiptModel { Id = "01JTESTRCP00015EXPDAAAA015", Code = "EXPRD4-000004-EXPRD-000004", Qty = 2, BuyerEmail = "expired4@test.com", Product = Products.Developer, Expires = DateTime.UtcNow.AddDays(-1) },
        };

        /// <summary>
        /// Gets a valid receipt for the specified product
        /// </summary>
        public static ReceiptModel ForProduct(ProductModel product)
        {
            return product.Id switch
            {
                "01JTESTPRD00001BASICAAA001" => BasicReceipts[0],
                "01JTESTPRD00002PROFAAAA002" => ProReceipts[0],
                "01JTESTPRD00003ENTRAAAA003" => EnterpriseReceipts[0],
                "01JTESTPRD00004DEVLAAAA004" => DeveloperReceipts[0],
                "01JTESTPRD00005ULTMAAAA005" => UltimateReceipts[0],
                _ => BasicReceipts[0],
            };
        }

        /// <summary>
        /// Gets an expired receipt for testing
        /// </summary>
        public static ReceiptModel GetExpired() => ExpiredReceipts[0];

        /// <summary>
        /// All valid receipts
        /// </summary>
        public static ReceiptModel[] AllValid => BasicReceipts
            .Concat(ProReceipts)
            .Concat(EnterpriseReceipts)
            .Concat(DeveloperReceipts)
            .Concat(UltimateReceipts)
            .ToArray();
    }
}
