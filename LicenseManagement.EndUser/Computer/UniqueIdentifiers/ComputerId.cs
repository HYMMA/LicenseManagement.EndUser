using DeviceId;
using System;
using System.Collections.Generic;

namespace LicenseManagement.EndUser
{
    public sealed class ComputerId
    {
        string machineName;
        string machineId;
        string machineIdV2;

        /// <summary>SHA256("") in Crockford Base32 — what the DeviceId pipeline
        /// yields when the MachineGuid component is missing; treated as "no v2".</summary>
        const string EmptyComponentHash = "WERC8GMRZGE196QVYK49JVXS4GKTWGF4CJDS6K54JPCHPY2JQ1AG";
        private static readonly Lazy<ComputerId> lazy = new Lazy<ComputerId>(() => new ComputerId());
        public static ComputerId Instance { get { return lazy.Value; } }
        //public static ComputerId Instance { get { return new ComputerId(); } }
        private ComputerId()
        {
            machineName = new DeviceIdBuilder().AddMachineName().UseFormatter(new NullDeviceIdFormatter("MachineName")).ToString();
            machineId = new DeviceIdBuilder().OnWindows(b =>
            //b.AddMachineGuid()).ToString();
            b.AddProcessorId()
            .AddMotherboardSerialNumber())
               .ToString();
            try
            {
                //identity v2: the per-install Windows MachineGuid. The v1 sources
                //collide across unrelated machines (ProcessorId is per CPU model;
                //OEM boards ship placeholder baseboard serials), which let one
                //customer's fresh install adopt another customer's license row.
                //v2 goes out as MacAddress and v1 as LegacyMacAddress so the
                //server can re-key this machine's pre-v2 row (same name).
                machineIdV2 = new DeviceIdBuilder().OnWindows(b => b.AddMachineGuid()).ToString();
                if (string.IsNullOrWhiteSpace(machineIdV2) ||
                    string.Equals(machineIdV2, EmptyComponentHash, StringComparison.OrdinalIgnoreCase))
                    machineIdV2 = null;
            }
            catch
            {
                machineIdV2 = null; //no MachineGuid: v1 alone, the pre-v2 behaviour
            }
        }

        /// <summary>
        /// this value is used during test only
        /// </summary>
        public string MachineId { get => machineId; internal set => machineId = value; }
        public string MachineName { get => machineName; internal set => machineName = value; }

        /// <summary>identity v2 — fingerprint of the Windows MachineGuid; null when unavailable</summary>
        public string MachineIdV2 { get => machineIdV2; internal set => machineIdV2 = value; }

        /// <summary>the id this client presents as MacAddress: v2 when available, else v1</summary>
        public string EffectiveMachineId => machineIdV2 ?? machineId;

        /// <summary>v1, sent as LegacyMacAddress alongside a v2 MacAddress (null when v2 is unavailable)</summary>
        public string LegacyMachineId => machineIdV2 != null ? machineId : null;
    }
    internal class NullDeviceIdFormatter : IDeviceIdFormatter
    {
        private readonly string _key;

        internal NullDeviceIdFormatter(string key)
        {
            _key = key;
        }
        public string GetDeviceId(IDictionary<string, IDeviceIdComponent> components)
        {
            return components[_key].GetValue();
        }
    }
}