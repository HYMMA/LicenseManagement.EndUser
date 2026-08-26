using LicenseManagement.EndUser;
using Microsoft.Win32;
using System;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace LicenseManagement.EndUser.Test
{
    public class DeviceIdTests
    {
        // Independent re-implementation of the fingerprint spec:
        // Crockford Base32 of SHA-256(input). Deliberately NOT the SDK's code,
        // so these tests catch a drift in the pipeline rather than mirroring it.
        private static string Fingerprint(string input)
        {
            const string alphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
            byte[] hash;
            using (var sha = SHA256.Create())
                hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            int bits = 0;
            int acc = 0;
            foreach (var b in hash)
            {
                acc = (acc << 8) | b;
                bits += 8;
                while (bits >= 5)
                {
                    bits -= 5;
                    sb.Append(alphabet[(acc >> bits) & 31]);
                }
            }
            if (bits > 0)
                sb.Append(alphabet[(acc << (5 - bits)) & 31]);
            return sb.ToString();
        }

        private static string ReadMachineGuid()
        {
            using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            using (var key = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
                return key?.GetValue("MachineGuid") as string;
        }

        [Fact]
        public void machineIdV2_isTheFingerprintOfTheWindowsMachineGuid()
        {
            var guid = ReadMachineGuid();
            Assert.False(string.IsNullOrWhiteSpace(guid), "test machine has no MachineGuid");

            // spec: v2 = CrockfordBase32(SHA256(MachineGuid)) — byte-identical to
            // license-management-core's hlm_machine_id_win_v2
            Assert.Equal(Fingerprint(guid), ComputerId.Instance.MachineIdV2);
        }

        [Fact]
        public void effectiveMachineId_prefersV2_andCarriesV1AsLegacy()
        {
            var ci = ComputerId.Instance;
            Assert.NotNull(ci.MachineIdV2); // this test machine has a MachineGuid
            Assert.Equal(ci.MachineIdV2, ci.EffectiveMachineId);
            Assert.Equal(ci.MachineId, ci.LegacyMachineId);
        }

        [Fact]
        public void machineIdV1_shape_isUnchanged_52CharCrockford()
        {
            // v1 must keep producing the historical value: existing rows in the
            // Computer table are keyed by it and the server re-key match depends
            // on the client resending the exact same string.
            var v1 = ComputerId.Instance.MachineId;
            Assert.Equal(52, v1.Length);
            foreach (var c in v1)
                Assert.True("0123456789ABCDEFGHJKMNPQRSTVWXYZ".IndexOf(c) >= 0,
                            $"non-Crockford char '{c}' in v1 id");
        }

        [Fact]
        public void knownVector_fingerprintOfAGuidString()
        {
            // same vector as license-management-core tests/vectors/vectors.json
            Assert.Equal("HX00R9BP27PNTC60WSG7NHGGEGR7VYH4SXRAHT9C7T0MFNKX5HR0",
                         Fingerprint("f47ac10b-58cc-4372-a567-0e02b2c3d479"));
        }
    }
}
