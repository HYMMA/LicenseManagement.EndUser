using LicenseManagement.EndUser.Models;

namespace LicenseManagement.EndUser.Test.Data
{
    public enum ProductType
    {
        NoFeatures = 0,
        OneFeature = 1,
        ManyFeatures = 2
    }

    /// <summary>
    /// Static test data for computers matching the SQL seed script in WebApi.Test.
    /// IDs include the server-mandated PC_ prefix.
    /// </summary>
    public static class Computers
    {
        private static int _noLicenseIndex = 0;

        public static ComputerModel ForNewLicense()
        {
            var computers = NoLicenseComputers;
            var computer = computers[_noLicenseIndex % computers.Length];
            _noLicenseIndex++;
            return computer;
        }

        public static void ResetIndex() => _noLicenseIndex = 0;

        public static ComputerModel[] PaidComputers => new[]
        {
            new ComputerModel { Id = "PC_01JTEZTCMP00001PA1DAAAA001", MacAddress = "AA:BB:CC:DD:EE:01", Name = "PaidComputer001" },
            new ComputerModel { Id = "PC_01JTEZTCMP00002PA1DAAAA002", MacAddress = "AA:BB:CC:DD:EE:02", Name = "PaidComputer002" },
            new ComputerModel { Id = "PC_01JTEZTCMP00003PA1DAAAA003", MacAddress = "AA:BB:CC:DD:EE:03", Name = "PaidComputer003" },
            new ComputerModel { Id = "PC_01JTEZTCMP00004PA1DAAAA004", MacAddress = "AA:BB:CC:DD:EE:04", Name = "PaidComputer004" },
            new ComputerModel { Id = "PC_01JTEZTCMP00005PA1DAAAA005", MacAddress = "AA:BB:CC:DD:EE:05", Name = "PaidComputer005" },
            new ComputerModel { Id = "PC_01JTEZTCMP00006PA1DAAAA006", MacAddress = "AA:BB:CC:DD:EE:06", Name = "PaidComputer006" },
            new ComputerModel { Id = "PC_01JTEZTCMP00007PA1DAAAA007", MacAddress = "AA:BB:CC:DD:EE:07", Name = "PaidComputer007" },
            new ComputerModel { Id = "PC_01JTEZTCMP00008PA1DAAAA008", MacAddress = "AA:BB:CC:DD:EE:08", Name = "PaidComputer008" },
            new ComputerModel { Id = "PC_01JTEZTCMP00009PA1DAAAA009", MacAddress = "AA:BB:CC:DD:EE:09", Name = "PaidComputer009" },
            new ComputerModel { Id = "PC_01JTEZTCMP00010PA1DAAAA010", MacAddress = "AA:BB:CC:DD:EE:0A", Name = "PaidComputer010" },
        };

        /// <summary>Computers with FUTURE trial end dates (IDs 21-24) — for ValidTrial tests.</summary>
        public static ComputerModel[] TrialComputers => new[]
        {
            new ComputerModel { Id = "PC_01JTEZTCMP00021TR11AAAA021", MacAddress = "AA:BB:CC:DD:EE:15", Name = "TrialComputer021" },
            new ComputerModel { Id = "PC_01JTEZTCMP00022TR11AAAA022", MacAddress = "AA:BB:CC:DD:EE:16", Name = "TrialComputer022" },
            new ComputerModel { Id = "PC_01JTEZTCMP00023TR11AAAA023", MacAddress = "AA:BB:CC:DD:EE:17", Name = "TrialComputer023" },
            new ComputerModel { Id = "PC_01JTEZTCMP00024TR11AAAA024", MacAddress = "AA:BB:CC:DD:EE:18", Name = "TrialComputer024" },
        };

        /// <summary>Computers with PAST trial end dates (IDs 25-28) — for InvalidTrial tests.</summary>
        public static ComputerModel[] InvalidTrialComputers => new[]
        {
            new ComputerModel { Id = "PC_01JTEZTCMP00025TR11AAAA025", MacAddress = "AA:BB:CC:DD:EE:19", Name = "TrialComputer025" },
            new ComputerModel { Id = "PC_01JTEZTCMP00026TR11AAAA026", MacAddress = "AA:BB:CC:DD:EE:1A", Name = "TrialComputer026" },
            new ComputerModel { Id = "PC_01JTEZTCMP00027TR11AAAA027", MacAddress = "AA:BB:CC:DD:EE:1B", Name = "TrialComputer027" },
            new ComputerModel { Id = "PC_01JTEZTCMP00028TR11AAAA028", MacAddress = "AA:BB:CC:DD:EE:1C", Name = "TrialComputer028" },
        };

        /// <summary>Computers with expired-receipt licenses (IDs 61-62) — for ReceiptExpired tests.</summary>
        public static ComputerModel[] ExpiredReceiptComputers => new[]
        {
            new ComputerModel { Id = "PC_01JTEZTCMP00061EXPRAAAA061", MacAddress = "AA:BB:CC:DD:EE:3D", Name = "ExpRcptComputer061" },
            new ComputerModel { Id = "PC_01JTEZTCMP00062EXPRAAAA062", MacAddress = "AA:BB:CC:DD:EE:3E", Name = "ExpRcptComputer062" },
        };

        public static ComputerModel[] UnregisteredComputers => new[]
        {
            new ComputerModel { Id = "PC_01JTEZTCMP00041VNRGAAAA041", MacAddress = "AA:BB:CC:DD:EE:29", Name = "UnregComputer041" },
            new ComputerModel { Id = "PC_01JTEZTCMP00042VNRGAAAA042", MacAddress = "AA:BB:CC:DD:EE:2A", Name = "UnregComputer042" },
        };

        public static ComputerModel[] NoLicenseComputers => new[]
        {
            new ComputerModel { Id = "PC_01JTEZTCMP00051N011AAAA051", MacAddress = "AA:BB:CC:DD:EE:33", Name = "NoLicComputer051" },
            new ComputerModel { Id = "PC_01JTEZTCMP00052N011AAAA052", MacAddress = "AA:BB:CC:DD:EE:34", Name = "NoLicComputer052" },
            new ComputerModel { Id = "PC_01JTEZTCMP00053N011AAAA053", MacAddress = "AA:BB:CC:DD:EE:35", Name = "NoLicComputer053" },
            new ComputerModel { Id = "PC_01JTEZTCMP00054N011AAAA054", MacAddress = "AA:BB:CC:DD:EE:36", Name = "NoLicComputer054" },
            new ComputerModel { Id = "PC_01JTEZTCMP00055N011AAAA055", MacAddress = "AA:BB:CC:DD:EE:37", Name = "NoLicComputer055" },
            new ComputerModel { Id = "PC_01JTEZTCMP00056N011AAAA056", MacAddress = "AA:BB:CC:DD:EE:38", Name = "NoLicComputer056" },
            new ComputerModel { Id = "PC_01JTEZTCMP00057N011AAAA057", MacAddress = "AA:BB:CC:DD:EE:39", Name = "NoLicComputer057" },
            new ComputerModel { Id = "PC_01JTEZTCMP00058N011AAAA058", MacAddress = "AA:BB:CC:DD:EE:3A", Name = "NoLicComputer058" },
            new ComputerModel { Id = "PC_01JTEZTCMP00059N011AAAA059", MacAddress = "AA:BB:CC:DD:EE:3B", Name = "NoLicComputer059" },
            new ComputerModel { Id = "PC_01JTEZTCMP00060N011AAAA060", MacAddress = "AA:BB:CC:DD:EE:3C", Name = "NoLicComputer060" },
        };
    }
}
