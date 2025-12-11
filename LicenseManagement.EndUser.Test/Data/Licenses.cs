using Hymma.Lm.EndUser.Models;

namespace Hymma.Lm.EndUser.Test.Data
{
    /// <summary>
    /// Static test data for licenses matching the SQL seed script in WebApi.Test
    /// </summary>
    public static class Licenses
    {
        /// <summary>
        /// Creates a license model from the provided components (for test assertions)
        /// </summary>
        public static LicenseModel Create(ReceiptModel? receipt, ProductModel product, ComputerModel computer)
        {
            return new LicenseModel
            {
                Product = product,
                Computer = computer,
                Receipt = receipt
            };
        }

        /// <summary>
        /// Paid licenses (IDs 1-20) - licenses linked to valid receipts
        /// </summary>
        public static LicenseModel[] PaidLicenses => new[]
        {
            // Product 1 (Basic) licenses
            new LicenseModel { Id = "01JTESTLIC00001PAIDAAAA001", Product = Products.Basic, Computer = Computers.PaidComputers[0], Receipt = Receipts.BasicReceipts[0] },
            new LicenseModel { Id = "01JTESTLIC00002PAIDAAAA002", Product = Products.Basic, Computer = Computers.PaidComputers[1], Receipt = Receipts.BasicReceipts[0] },
            new LicenseModel { Id = "01JTESTLIC00003PAIDAAAA003", Product = Products.Basic, Computer = Computers.PaidComputers[2], Receipt = Receipts.BasicReceipts[0] },
            new LicenseModel { Id = "01JTESTLIC00004PAIDAAAA004", Product = Products.Basic, Computer = Computers.PaidComputers[3], Receipt = Receipts.BasicReceipts[1] },

            // Product 2 (Pro) licenses
            new LicenseModel { Id = "01JTESTLIC00005PAIDAAAA005", Product = Products.Pro, Computer = Computers.PaidComputers[4], Receipt = Receipts.ProReceipts[0] },
            new LicenseModel { Id = "01JTESTLIC00006PAIDAAAA006", Product = Products.Pro, Computer = Computers.PaidComputers[5], Receipt = Receipts.ProReceipts[0] },
            new LicenseModel { Id = "01JTESTLIC00007PAIDAAAA007", Product = Products.Pro, Computer = Computers.PaidComputers[6], Receipt = Receipts.ProReceipts[0] },
            new LicenseModel { Id = "01JTESTLIC00008PAIDAAAA008", Product = Products.Pro, Computer = Computers.PaidComputers[7], Receipt = Receipts.ProReceipts[1] },

            // Product 3 (Enterprise) licenses
            new LicenseModel { Id = "01JTESTLIC00009PAIDAAAA009", Product = Products.Enterprise, Computer = Computers.PaidComputers[8], Receipt = Receipts.EnterpriseReceipts[0] },
            new LicenseModel { Id = "01JTESTLIC00010PAIDAAAA010", Product = Products.Enterprise, Computer = Computers.PaidComputers[9], Receipt = Receipts.EnterpriseReceipts[0] },
        };

        /// <summary>
        /// Trial licenses (IDs 21-40) - licenses without receipts
        /// </summary>
        public static LicenseModel[] TrialLicenses => new[]
        {
            new LicenseModel { Id = "01JTESTLIC00021TRILAAAA021", Product = Products.Basic, Computer = Computers.TrialComputers[0], Receipt = null },
            new LicenseModel { Id = "01JTESTLIC00022TRILAAAA022", Product = Products.Basic, Computer = Computers.TrialComputers[1], Receipt = null },
            new LicenseModel { Id = "01JTESTLIC00023TRILAAAA023", Product = Products.Basic, Computer = Computers.TrialComputers[2], Receipt = null },
            new LicenseModel { Id = "01JTESTLIC00024TRILAAAA024", Product = Products.Basic, Computer = Computers.TrialComputers[3], Receipt = null },
        };

        /// <summary>
        /// Unregistered licenses (IDs 41-50) - licenses that had receipts but were unregistered
        /// </summary>
        public static LicenseModel[] UnregisteredLicenses => new[]
        {
            new LicenseModel { Id = "01JTESTLIC00041UNRGAAAA041", Product = Products.Basic, Computer = Computers.UnregisteredComputers[0], Receipt = null },
            new LicenseModel { Id = "01JTESTLIC00042UNRGAAAA042", Product = Products.Basic, Computer = Computers.UnregisteredComputers[1], Receipt = null },
        };

        /// <summary>
        /// Gets a paid license for testing
        /// </summary>
        public static LicenseModel GetPaid() => PaidLicenses[0];

        /// <summary>
        /// Gets a trial license for testing
        /// </summary>
        public static LicenseModel GetTrial() => TrialLicenses[0];

        /// <summary>
        /// Gets an unregistered license for testing
        /// </summary>
        public static LicenseModel GetUnregistered() => UnregisteredLicenses[0];
    }
}
