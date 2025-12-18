using LicenseManagement.EndUser.Computer.Handlers;
using LicenseManagement.EndUser.Exceptions;
using System;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser.Product.Handlers
{
    internal class ProductNameValidatorHandler : LicenseValidationHandler
    {
        /// <summary>
        /// validates that the name of the product matches the one noted in license and sets the next handler to <see cref="ComputerIdValidatorHandler"/>
        /// </summary>
        public ProductNameValidatorHandler()
        {

        }
        bool ProductNameAreEqual(LicHandlingContext context) => string.Equals(context.PublisherPreferences.ProductId, context.LicenseModel.Product.Id, StringComparison.OrdinalIgnoreCase);
        string GetErrorText(LicHandlingContext context) => $"product names do not match. {context.PublisherPreferences.ProductId} and {context.LicenseModel.Product.Name}";
        void SetNextHandler(LicHandlingContext context)
        {
            if (ProductNameAreEqual(context))
            {
                SetNext(new ComputerIdValidatorHandler());
            }
            else
            {
                SetNextError(context, new ProductNameException(GetErrorText(context)));
            }
        }
        public override void HandleContext(LicHandlingContext context)
        {
            SetNextHandler(context);
            nextHandler.HandleContext(context);
        }

        public override Task HandleContextAsync(LicHandlingContext context)
        {
            SetNextHandler(context);
            return nextHandler.HandleContextAsync(context);
        }
    }
}
