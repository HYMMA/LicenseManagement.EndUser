using System;
using System.Collections.Generic;

namespace Hymma.Lm.EndUser
{
    /// <summary>
    /// Event arguments for the BeforeLicensePost event
    /// </summary>
    public class BeforeLicensePostEventArgs : EventArgs
    {
        /// <summary>
        /// The computer MAC address
        /// </summary>
        public string MacAddress { get; }

        /// <summary>
        /// The computer name
        /// </summary>
        public string ComputerName { get; }

        /// <summary>
        /// The product ID
        /// </summary>
        public string ProductId { get; }

        /// <summary>
        /// Metadata to attach to the license. Add key-value pairs here.
        /// </summary>
        /// <remarks>Keys and values are limited to 100 characters each</remarks>
        public Dictionary<string, string> Metadata { get; } = new Dictionary<string, string>();

        public BeforeLicensePostEventArgs(string macAddress, string computerName, string productId)
        {
            MacAddress = macAddress;
            ComputerName = computerName;
            ProductId = productId;
        }
    }

    public class PublisherPreferences
    {
        /// <summary>
        /// Allows you to control how the license file is validated and retrieved from the servers
        /// </summary>
        /// <param name="vendorId">the id provided to you by the server</param>
        /// <param name="productId">the Id of the product</param>
        /// <param name="apiKey">your client api-key, DO NOT USE YOUR MASTER API KEY</param>
        public PublisherPreferences(string vendorId, string productId, string apiKey)
        {
            VendorId = vendorId;
            ProductId = productId;
            ApiKey = apiKey;
        }

        /// <summary>
        /// Event fired before a license is posted to the server.
        /// Use this to attach metadata to the license (e.g., end-user email).
        /// </summary>
        public event EventHandler<BeforeLicensePostEventArgs> BeforeLicensePost;

        /// <summary>
        /// this is the name of the product that will be provided by the assessor , or read from local license file
        /// </summary>
        public string ProductId { get; }

        /// <summary>
        /// the id of the publisher provided to you by Hymma license management system
        /// </summary>
        public string VendorId { get; }

        /// <summary>
        /// public key to use to check the signature of a license
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// List of features to include in a license file
        /// </summary>
        public List<string> IncludeFeatures { get; set; }

        /// <summary>
        /// your api key to call api sever
        /// </summary>
        public string  ApiKey { get; }

        /// <summary>
        /// the period of time a license file can be valid, server uses this number to set the <see cref="Models.LicenseModel.Expires"/>
        /// </summary>
        ///<remarks>default is 90 days</remarks>
        public uint ValidDays { get; set; } = 90;

        /// <summary>
        /// Invokes the BeforeLicensePost event and returns any metadata provided by the handler
        /// </summary>
        internal Dictionary<string, string> OnBeforeLicensePost(string macAddress, string computerName, string productId)
        {
            var args = new BeforeLicensePostEventArgs(macAddress, computerName, productId);
            BeforeLicensePost?.Invoke(this, args);
            return args.Metadata;
        }
    }
}
