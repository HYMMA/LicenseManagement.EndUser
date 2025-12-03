using Hymma.Lm.EndUser.Models;
using Hymma.Lm.EndUser.Product.Handlers;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Hymma.Lm.EndUser.License.Handlers
{
    internal class LicenseParsingHandler : LicenseValidationHandler
    {
        /// <summary>
        /// Responsible to deserialize xml string to a <see cref="LicenseModel"/> and sets to next handler to <see cref="ProductNameValidatorHandler"/>
        /// </summary>
        ///<remarks>this handler is for testing only, DO NOT USE </remarks>
        public LicenseParsingHandler()
        {

        }

        public override void HandleContext(LicHandlingContext context)
        {
            try
            {
                context.LicenseModel = LicenseModel.FromXml(context.SignedLicense);
                SetNext(new ProductNameValidatorHandler());
            }
            catch (System.Exception e)
            {
                SetNextError(context, e);
            }
            nextHandler.HandleContext(context);
        }

        public override Task HandleContextAsync(LicHandlingContext context)
        {
            try
            {
                context.LicenseModel = LicenseModel.FromXml(context.SignedLicense);
                SetNext(new ProductNameValidatorHandler());
            }
            catch (System.Exception e)
            {
                SetNextError(context, e);
            }
            return nextHandler.HandleContextAsync(context);
        }
    }
}