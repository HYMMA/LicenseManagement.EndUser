using DeviceId;
using System;
using System.Collections.Generic;

namespace LicenseManagement.EndUser
{
    public sealed class ComputerId
    {
        string machineName;
        string machineId;
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
        }

        /// <summary>
        /// this value is used during test only
        /// </summary>
        public string MachineId { get => machineId; private set => machineId = value; }
        public string MachineName { get => machineName; private set => machineName = value; }
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