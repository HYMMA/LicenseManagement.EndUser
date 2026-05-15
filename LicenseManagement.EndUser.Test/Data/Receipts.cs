using LicenseManagement.EndUser.Models;

namespace LicenseManagement.EndUser.Test.Data
{
    /// <summary>
    /// Static test data for receipts matching the SQL seed script in WebApi.Test.
    /// IDs include the server-mandated RCT_ prefix.
    /// </summary>
    public static class Receipts
    {
        public static ReceiptModel[] BasicReceipts => new[]
        {
            new ReceiptModel { Id = "RCT_01JTEZTRC000001VA1DAAAA001", Code = "VALID1-000001-000001-000001", Qty = 4, BuyerEmail = "buyer1@test.com", Product = Products.Basic, Expires = DateTime.UtcNow.AddMonths(3) },
            new ReceiptModel { Id = "RCT_01JTEZTRC000002VA1DAAAA002", Code = "VALID2-000002-000002-000002", Qty = 2, BuyerEmail = "buyer2@test.com", Product = Products.Basic, Expires = DateTime.UtcNow.AddMonths(3) },
            new ReceiptModel { Id = "RCT_01JTEZTRC000003VA1DAAAA003", Code = "VALID3-000003-000003-000003", Qty = 3, BuyerEmail = "buyer3@test.com", Product = Products.Basic, Expires = DateTime.UtcNow.AddMonths(3) },
        };

        public static ReceiptModel[] ProReceipts => new[]
        {
            new ReceiptModel { Id = "RCT_01JTEZTRC000004VA1DAAAA004", Code = "VALID4-000004-000004-000004", Qty = 4, BuyerEmail = "buyer4@test.com", Product = Products.Pro, Expires = DateTime.UtcNow.AddMonths(3) },
            new ReceiptModel { Id = "RCT_01JTEZTRC000005VA1DAAAA005", Code = "VALID5-000005-000005-000005", Qty = 2, BuyerEmail = "buyer5@test.com", Product = Products.Pro, Expires = DateTime.UtcNow.AddMonths(3) },
        };

        public static ReceiptModel[] EnterpriseReceipts => new[]
        {
            new ReceiptModel { Id = "RCT_01JTEZTRC000006VA1DAAAA006", Code = "VALID6-000006-000006-000006", Qty = 3, BuyerEmail = "buyer6@test.com", Product = Products.Enterprise, Expires = DateTime.UtcNow.AddMonths(3) },
            new ReceiptModel { Id = "RCT_01JTEZTRC000007VA1DAAAA007", Code = "VALID7-000007-000007-000007", Qty = 2, BuyerEmail = "buyer7@test.com", Product = Products.Enterprise, Expires = DateTime.UtcNow.AddMonths(3) },
        };

        public static ReceiptModel[] DeveloperReceipts => new[]
        {
            new ReceiptModel { Id = "RCT_01JTEZTRC000008VA1DAAAA008", Code = "VALID8-000008-000008-000008", Qty = 1, BuyerEmail = "buyer8@test.com", Product = Products.Developer, Expires = DateTime.UtcNow.AddMonths(3) },
            new ReceiptModel { Id = "RCT_01JTEZTRC000009VA1DAAAA009", Code = "VALID9-000009-000009-000009", Qty = 2, BuyerEmail = "buyer9@test.com", Product = Products.Developer, Expires = DateTime.UtcNow.AddMonths(3) },
        };

        public static ReceiptModel[] UltimateReceipts => new[]
        {
            new ReceiptModel { Id = "RCT_01JTEZTRC000010VA1DAAAA010", Code = "VA1DA-00000A-00000A-00000A", Qty = 3, BuyerEmail = "buyer10@test.com", Product = Products.Ultimate, Expires = DateTime.UtcNow.AddMonths(3) },
            new ReceiptModel { Id = "RCT_01JTEZTRC000011VA1DAAAA011", Code = "VA1DB-00000B-00000B-00000B", Qty = 2, BuyerEmail = "buyer11@test.com", Product = Products.Ultimate, Expires = DateTime.UtcNow.AddMonths(3) },
        };

        public static ReceiptModel[] ExpiredReceipts => new[]
        {
            new ReceiptModel { Id = "RCT_01JTEZTRC000012EXPDAAAA012", Code = "EXPRD1-000001-EXPRD-000001", Qty = 2, BuyerEmail = "expired1@test.com", Product = Products.Basic, Expires = DateTime.UtcNow.AddDays(-10) },
            new ReceiptModel { Id = "RCT_01JTEZTRC000013EXPDAAAA013", Code = "EXPRD2-000002-EXPRD-000002", Qty = 1, BuyerEmail = "expired2@test.com", Product = Products.Pro, Expires = DateTime.UtcNow.AddDays(-5) },
            new ReceiptModel { Id = "RCT_01JTEZTRC000014EXPDAAAA014", Code = "EXPRD3-000003-EXPRD-000003", Qty = 3, BuyerEmail = "expired3@test.com", Product = Products.Enterprise, Expires = DateTime.UtcNow.AddDays(-3) },
            new ReceiptModel { Id = "RCT_01JTEZTRC000015EXPDAAAA015", Code = "EXPRD4-000004-EXPRD-000004", Qty = 2, BuyerEmail = "expired4@test.com", Product = Products.Developer, Expires = DateTime.UtcNow.AddDays(-1) },
        };

        public static ReceiptModel ForProduct(ProductModel product)
        {
            return product.Id switch
            {
                "PRD_01JTEZTPRD00001BA51CAAA001" => BasicReceipts[0],
                "PRD_01JTEZTPRD00002PR0FAAAA002" => ProReceipts[0],
                "PRD_01JTEZTPRD00003ENTRAAAA003" => EnterpriseReceipts[0],
                "PRD_01JTEZTPRD00004DEV1AAAA004" => DeveloperReceipts[0],
                "PRD_01JTEZTPRD00005V1TMAAAA005" => UltimateReceipts[0],
                _ => BasicReceipts[0],
            };
        }

        public static ReceiptModel GetExpired() => ExpiredReceipts[0];

        public static ReceiptModel[] AllValid => BasicReceipts
            .Concat(ProReceipts)
            .Concat(EnterpriseReceipts)
            .Concat(DeveloperReceipts)
            .Concat(UltimateReceipts)
            .ToArray();
    }
}
