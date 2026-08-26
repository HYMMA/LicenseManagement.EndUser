using System.Collections.Generic;

namespace LicenseManagement.EndUser.Models
{
    public class PostLicenseModel
    {
        public string Product { get; set; }
        public string Computer { get; set; }

        /// <summary>
        /// Optional metadata to attach to the license (key-value pairs)
        /// </summary>
        /// <remarks>Keys and values are limited to 100 characters each</remarks>
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class PostComputerModel
    {
        public string MacAddress { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// the v1 device id when <see cref="MacAddress"/> carries the v2 id —
        /// lets the server re-key this machine's pre-v2 row instead of
        /// treating it as a brand-new computer. Null (and serialized as null)
        /// when this client has no v2 id; the server treats null as absent.
        /// </summary>
        public string LegacyMacAddress { get; set; }
    }
}
