using Hymma.Lm.EndUser.License;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Hymma.Lm.EndUser.Models{    [XmlType("License")]
    public class LicenseModel : DbModelWithDate
    {
        ProductModel _product;
        ReceiptModel _receipt;
        ComputerModel _computer;
        DateTime? _expires;
        string _id;

        public string Id
        {
            get => _id; set
            {
                if (_id != value)
                {
                    _id = value;
                }
            }
        }

        /// <summary>
        /// a safety threshold to stop dodgy users manipulating the trial days value in the App.Config, also allows you increase the trial days 
        /// </summary>
        public int MaxTrialDays { get; set; }

        public DateTime? Expires
        {
            get => _expires; set
            {
                if (_expires != value)
                {
                    _expires = value;
                }
            }
        }

        [XmlElement("Receipt")]
        public ReceiptModel Receipt
        {
            get => _receipt; set
            {
                if (_receipt != value)
                {
                    _receipt = value;
                }
            }
        }

        [XmlElement("Product")]
        public ProductModel Product
        {
            get => _product; set
            {
                if (_product != value)
                {
                    _product = value;
                }
            }
        }

        [XmlElement("Computer")]
        public ComputerModel Computer
        {
            get => _computer; set
            {
                if (_computer != value)
                {
                    _computer = value;
                }
            }
        }

        [XmlIgnore]
        public LicenseStatusTitles Status { get;internal set; }

        /// <summary>
        /// tool to deserialize xml to this type
        /// </summary>
        /// <param name="signedXml"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static LicenseModel FromXml(string signedXml)
        {
            if (string.IsNullOrEmpty(signedXml))
            {
                ArgumentNullException argumentNullException = new ArgumentNullException("signed xml was null");
                throw argumentNullException;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(LicenseModel));
            object obj = null;
            using (var reader = new StringReader(signedXml))
            {
                obj = serializer.Deserialize(reader);
            }
            if (obj is LicenseModel model)
            {
                return model;
            }
            else
            {
                throw new Exception("could not parse signed xml");
            }
        }
    }
}