using DeviceId;
using LicenseManagement.EndUser.Registrars;
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
        public string MachineId { get => machineId;private set => machineId = value; }
        public string MachineName{ get => machineName;private set => machineName= value; }
        /*string GetMacAddressFromHardware()
        {
            if (string.IsNullOrEmpty(MachineId))
            {
                MachineId = new DeviceIdBuilder().OnWindows(b =>
                //b.AddMachineGuid()).ToString();
                b.AddProcessorId()
                .AddMotherboardSerialNumber())
                   .ToString();
                machineName = new DeviceIdBuilder().AddMachineName().UseFormatter(new NullDeviceIdFormatter("MachineName")).ToString();
                MachineId = MachineId + Constants.MachineNameSeparator + machineName;
            }
            return MachineId.ToString();
        }*/

        /// <summary>
        /// tries finding the macAddress form registry. If not found, will elicit it from the hardware.
        /// </summary>
        /// <returns></returns>
        /*public string ResolveMacAddress()
        {
            string mac;
            try
            {
                mac = Instance.GetMacAddressFromHardware();
            }
            catch (Exception)
            {
                ComputerRegister.TryRead(out mac);
            }
            return mac;
        }*/
    }
    public class NullDeviceIdFormatter : IDeviceIdFormatter
    {
        private string _key;

        public NullDeviceIdFormatter(string key)
        {
            _key = key;
        }
        public string GetDeviceId(IDictionary<string, IDeviceIdComponent> components)
        {
            return components[_key].GetValue();
        }
    }
}