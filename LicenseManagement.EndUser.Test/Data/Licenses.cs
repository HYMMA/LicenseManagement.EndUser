using LicenseManagement.EndUser.Models;

namespace LicenseManagement.EndUser.Test.Data
{
    /// <summary>
    /// Static test data for licenses matching the SQL seed script in WebApi.Test.
    /// IDs include the server-mandated LIC_ prefix.
    /// </summary>
    public static class Licenses
    {
        public static LicenseModel Create(ReceiptModel? receipt, ProductModel product, ComputerModel computer)
        {
            return new LicenseModel
            {
                Product = product,
                Computer = computer,
                Receipt = receipt
            };
        }

        /// <summary>Paid licenses (IDs 1-20) linked to valid receipts.</summary>
        public static LicenseModel[] PaidLicenses => new[]
        {
            new LicenseModel { Id = "LIC_01JTEZT11C00001PA1DAAAA001", Product = Products.Basic, Computer = Computers.PaidComputers[0], Receipt = Receipts.BasicReceipts[0] },
            new LicenseModel { Id = "LIC_01JTEZT11C00002PA1DAAAA002", Product = Products.Basic, Computer = Computers.PaidComputers[1], Receipt = Receipts.BasicReceipts[0] },
            new LicenseModel { Id = "LIC_01JTEZT11C00003PA1DAAAA003", Product = Products.Basic, Computer = Computers.PaidComputers[2], Receipt = Receipts.BasicReceipts[0] },
            new LicenseModel { Id = "LIC_01JTEZT11C00004PA1DAAAA004", Product = Products.Basic, Computer = Computers.PaidComputers[3], Receipt = Receipts.BasicReceipts[1] },
            new LicenseModel { Id = "LIC_01JTEZT11C00005PA1DAAAA005", Product = Products.Pro, Computer = Computers.PaidComputers[4], Receipt = Receipts.ProReceipts[0] },
            new LicenseModel { Id = "LIC_01JTEZT11C00006PA1DAAAA006", Product = Products.Pro, Computer = Computers.PaidComputers[5], Receipt = Receipts.ProReceipts[0] },
            new LicenseModel { Id = "LIC_01JTEZT11C00007PA1DAAAA007", Product = Products.Pro, Computer = Computers.PaidComputers[6], Receipt = Receipts.ProReceipts[0] },
            new LicenseModel { Id = "LIC_01JTEZT11C00008PA1DAAAA008", Product = Products.Pro, Computer = Computers.PaidComputers[7], Receipt = Receipts.ProReceipts[1] },
            new LicenseModel { Id = "LIC_01JTEZT11C00009PA1DAAAA009", Product = Products.Enterprise, Computer = Computers.PaidComputers[8], Receipt = Receipts.EnterpriseReceipts[0] },
            new LicenseModel { Id = "LIC_01JTEZT11C00010PA1DAAAA010", Product = Products.Enterprise, Computer = Computers.PaidComputers[9], Receipt = Receipts.EnterpriseReceipts[0] },
        };

        /// <summary>Trial licenses with FUTURE trial end (IDs 21-24) — for ValidTrial tests.</summary>
        public static LicenseModel[] ValidTrialLicenses => new[]
        {
            new LicenseModel { Id = "LIC_01JTEZT11C00021TR11AAAA021", Product = Products.Basic, Computer = Computers.TrialComputers[0], Receipt = null },
            new LicenseModel { Id = "LIC_01JTEZT11C00022TR11AAAA022", Product = Products.Basic, Computer = Computers.TrialComputers[1], Receipt = null },
            new LicenseModel { Id = "LIC_01JTEZT11C00023TR11AAAA023", Product = Products.Basic, Computer = Computers.TrialComputers[2], Receipt = null },
            new LicenseModel { Id = "LIC_01JTEZT11C00024TR11AAAA024", Product = Products.Basic, Computer = Computers.TrialComputers[3], Receipt = null },
        };

        /// <summary>Trial licenses with PAST trial end (IDs 25-28) — for InvalidTrial tests.</summary>
        public static LicenseModel[] InvalidTrialLicenses => new[]
        {
            new LicenseModel { Id = "LIC_01JTEZT11C00025TR11AAAA025", Product = Products.Pro, Computer = Computers.InvalidTrialComputers[0], Receipt = null },
            new LicenseModel { Id = "LIC_01JTEZT11C00026TR11AAAA026", Product = Products.Pro, Computer = Computers.InvalidTrialComputers[1], Receipt = null },
            new LicenseModel { Id = "LIC_01JTEZT11C00027TR11AAAA027", Product = Products.Pro, Computer = Computers.InvalidTrialComputers[2], Receipt = null },
            new LicenseModel { Id = "LIC_01JTEZT11C00028TR11AAAA028", Product = Products.Pro, Computer = Computers.InvalidTrialComputers[3], Receipt = null },
        };

        /// <summary>Legacy alias kept for backward compatibility — points to valid trial licenses.</summary>
        public static LicenseModel[] TrialLicenses => ValidTrialLicenses;

        /// <summary>Unregistered licenses (IDs 41-50) — receipt was removed.</summary>
        public static LicenseModel[] UnregisteredLicenses => new[]
        {
            new LicenseModel { Id = "LIC_01JTEZT11C00041VNRGAAAA041", Product = Products.Basic, Computer = Computers.UnregisteredComputers[0], Receipt = null },
            new LicenseModel { Id = "LIC_01JTEZT11C00042VNRGAAAA042", Product = Products.Basic, Computer = Computers.UnregisteredComputers[1], Receipt = null },
        };

        /// <summary>Licenses with expired receipts (IDs 51-52) — for ReceiptExpired tests.</summary>
        public static LicenseModel[] ExpiredReceiptLicenses => new[]
        {
            new LicenseModel { Id = "LIC_01JTEZT11C00051EXPRAAAA051", Product = Products.Basic, Computer = Computers.ExpiredReceiptComputers[0], Receipt = Receipts.ExpiredReceipts[0] },
            new LicenseModel { Id = "LIC_01JTEZT11C00052EXPRAAAA052", Product = Products.Basic, Computer = Computers.ExpiredReceiptComputers[1], Receipt = Receipts.ExpiredReceipts[0] },
        };

        public static LicenseModel GetPaid() => PaidLicenses[0];
        public static LicenseModel GetTrial() => ValidTrialLicenses[0];
        public static LicenseModel GetUnregistered() => UnregisteredLicenses[0];
    }
}
