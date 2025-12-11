using System.Collections.Generic;

namespace Hymma.Lm.EndUser.Models
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
    }
}
