using Hymma.Lm.EndUser.Models;
using System.Data;

namespace Hymma.Lm.EndUser.Test
{
    internal static class ContextManager
    {
        internal static string ApiKey => "MST_01JAP1JE7FRCJ63FHE5DQJGPY3_xYnvvm6HMOwMWtcGkn3NIrVI34LGTHm1i1tE4qQ7d5Y95iIR3L__vr-m4v6POxnF_JKsM23b-bYpLTM8zLzv3g";
        internal static LicHandlingContext FromLic(LicenseModel license, uint trial = 0)
        {
            int validDays = 90;
            if (license.Expires != null && license.Created != null)
            {
                validDays = (license.Expires - license.Created).Value.Days;
            }
            return GetContext(license.Product.Id, license.Product.Vendor.Id, trial, license.Computer.MacAddress, license.Computer.Name, ((uint)validDays));
        }

        internal static LicHandlingContext GetContext(string productId, string publisher, uint trial, string? macAddress = "", string? machineName = "", uint validDays = 90)
        {
            return new LicHandlingContext(new PublisherPreferences(publisher, productId, ApiKey)
            {
                TrialDays = trial,
                PublicKey = "<RSAKeyValue><Modulus>vE30e8HszHQbvKdejEwrGMiILh8E1wbciEdZxUVNrVM33WQCfxzdU5CtyviNm1837oCE3tetPq9nrtazSGXgCfiZY023pLdAaH6ExO3UeQv0hZBWJ4jPOhfUcFOGj4bkPH6EgaT4wjBVc+oRzXL00NxcIQTOFgWuqlBxnZ1NGSntdy8+AOiVaZ3tlV8iIO5J1pgt9NA1FS7Eh7icROJmJkDT/ZnfCn9PZdbBm/tT7LVaifi6rm+r1kPQ9Qp0jqX36pqYhES78Hs8nl5h/Ukfw+9J7KyWavey4NpYs1YqXGfKDYvVr1cJiwvsvh8h/rsFj/f/GhLfa9OwbH2bU/ZFrQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>",
                ValidDays = validDays
            });
        }
    }
}
