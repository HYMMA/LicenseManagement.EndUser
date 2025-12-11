using Hymma.Lm.EndUser.Models;

namespace Hymma.Lm.EndUser.Test.Data
{
    public enum ProductType
    {
        NoFeatures = 0,
        OneFeature = 1,
        ManyFeatures = 2
    }

    /// <summary>
    /// Static test data for computers matching the SQL seed script in WebApi.Test
    /// </summary>
    public static class Computers
    {
        // Index to track which computer to use next for new registrations
        private static int _noLicenseIndex = 0;

        /// <summary>
        /// Gets a computer from the "no license" group (IDs 51-60) for POST tests.
        /// These computers don't have licenses yet.
        /// </summary>
        public static ComputerModel ForNewLicense()
        {
            var computers = NoLicenseComputers;
            var computer = computers[_noLicenseIndex % computers.Length];
            _noLicenseIndex++;
            return computer;
        }

        /// <summary>
        /// Resets the index for new license computers (call at start of test)
        /// </summary>
        public static void ResetIndex() => _noLicenseIndex = 0;

        /// <summary>
        /// Computers with paid licenses (IDs 1-20)
        /// </summary>
        public static ComputerModel[] PaidComputers => new[]
        {
            new ComputerModel { Id = "01JTESTCMP00001PAIDAAAA001", MacAddress = "AA:BB:CC:DD:EE:01", Name = "PaidComputer001" },
            new ComputerModel { Id = "01JTESTCMP00002PAIDAAAA002", MacAddress = "AA:BB:CC:DD:EE:02", Name = "PaidComputer002" },
            new ComputerModel { Id = "01JTESTCMP00003PAIDAAAA003", MacAddress = "AA:BB:CC:DD:EE:03", Name = "PaidComputer003" },
            new ComputerModel { Id = "01JTESTCMP00004PAIDAAAA004", MacAddress = "AA:BB:CC:DD:EE:04", Name = "PaidComputer004" },
            new ComputerModel { Id = "01JTESTCMP00005PAIDAAAA005", MacAddress = "AA:BB:CC:DD:EE:05", Name = "PaidComputer005" },
            new ComputerModel { Id = "01JTESTCMP00006PAIDAAAA006", MacAddress = "AA:BB:CC:DD:EE:06", Name = "PaidComputer006" },
            new ComputerModel { Id = "01JTESTCMP00007PAIDAAAA007", MacAddress = "AA:BB:CC:DD:EE:07", Name = "PaidComputer007" },
            new ComputerModel { Id = "01JTESTCMP00008PAIDAAAA008", MacAddress = "AA:BB:CC:DD:EE:08", Name = "PaidComputer008" },
            new ComputerModel { Id = "01JTESTCMP00009PAIDAAAA009", MacAddress = "AA:BB:CC:DD:EE:09", Name = "PaidComputer009" },
            new ComputerModel { Id = "01JTESTCMP00010PAIDAAAA010", MacAddress = "AA:BB:CC:DD:EE:0A", Name = "PaidComputer010" },
        };

        /// <summary>
        /// Computers with trial licenses (IDs 21-40)
        /// </summary>
        public static ComputerModel[] TrialComputers => new[]
        {
            new ComputerModel { Id = "01JTESTCMP00021TRILAAAA021", MacAddress = "AA:BB:CC:DD:EE:15", Name = "TrialComputer021" },
            new ComputerModel { Id = "01JTESTCMP00022TRILAAAA022", MacAddress = "AA:BB:CC:DD:EE:16", Name = "TrialComputer022" },
            new ComputerModel { Id = "01JTESTCMP00023TRILAAAA023", MacAddress = "AA:BB:CC:DD:EE:17", Name = "TrialComputer023" },
            new ComputerModel { Id = "01JTESTCMP00024TRILAAAA024", MacAddress = "AA:BB:CC:DD:EE:18", Name = "TrialComputer024" },
        };

        /// <summary>
        /// Computers with unregistered licenses (IDs 41-50)
        /// </summary>
        public static ComputerModel[] UnregisteredComputers => new[]
        {
            new ComputerModel { Id = "01JTESTCMP00041UNRGAAAA041", MacAddress = "AA:BB:CC:DD:EE:29", Name = "UnregComputer041" },
            new ComputerModel { Id = "01JTESTCMP00042UNRGAAAA042", MacAddress = "AA:BB:CC:DD:EE:2A", Name = "UnregComputer042" },
        };

        /// <summary>
        /// Computers without licenses (IDs 51-60) - for POST tests
        /// </summary>
        public static ComputerModel[] NoLicenseComputers => new[]
        {
            new ComputerModel { Id = "01JTESTCMP00051NOLIAAAA051", MacAddress = "AA:BB:CC:DD:EE:33", Name = "NoLicComputer051" },
            new ComputerModel { Id = "01JTESTCMP00052NOLIAAAA052", MacAddress = "AA:BB:CC:DD:EE:34", Name = "NoLicComputer052" },
            new ComputerModel { Id = "01JTESTCMP00053NOLIAAAA053", MacAddress = "AA:BB:CC:DD:EE:35", Name = "NoLicComputer053" },
            new ComputerModel { Id = "01JTESTCMP00054NOLIAAAA054", MacAddress = "AA:BB:CC:DD:EE:36", Name = "NoLicComputer054" },
            new ComputerModel { Id = "01JTESTCMP00055NOLIAAAA055", MacAddress = "AA:BB:CC:DD:EE:37", Name = "NoLicComputer055" },
            new ComputerModel { Id = "01JTESTCMP00056NOLIAAAA056", MacAddress = "AA:BB:CC:DD:EE:38", Name = "NoLicComputer056" },
            new ComputerModel { Id = "01JTESTCMP00057NOLIAAAA057", MacAddress = "AA:BB:CC:DD:EE:39", Name = "NoLicComputer057" },
            new ComputerModel { Id = "01JTESTCMP00058NOLIAAAA058", MacAddress = "AA:BB:CC:DD:EE:3A", Name = "NoLicComputer058" },
            new ComputerModel { Id = "01JTESTCMP00059NOLIAAAA059", MacAddress = "AA:BB:CC:DD:EE:3B", Name = "NoLicComputer059" },
            new ComputerModel { Id = "01JTESTCMP00060NOLIAAAA060", MacAddress = "AA:BB:CC:DD:EE:3C", Name = "NoLicComputer060" },
        };
    }
}
