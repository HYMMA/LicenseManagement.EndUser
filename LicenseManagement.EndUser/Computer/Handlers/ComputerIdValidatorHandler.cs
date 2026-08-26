using LicenseManagement.EndUser.Exceptions;
using LicenseManagement.EndUser.License.Handlers;
using System;
using System.Threading.Tasks;

namespace LicenseManagement.EndUser.Computer.Handlers
{
    internal class ComputerIdValidatorHandler : LicenseValidationHandler
    {
        /// <summary>
        /// validates that the computer macAddress matches the one noted in license and sets the next handler to <see cref="LicenseStatusHandler"/> or <see cref="UnregisterReceiptHandler"/>
        /// </summary>
        public ComputerIdValidatorHandler()
        {

        }

        string GetErrText(LicHandlingContext context) =>
            $"computer id(s) do not match. {ComputerId.Instance.EffectiveMachineId} (legacy {ComputerId.Instance.MachineId}) and {context.LicenseModel.Computer.MacAddress}";

        private bool IsNameValid(LicHandlingContext context)
        {
            var inLicense = context.LicenseModel.Computer.MacAddress;
            //license data in the wild may carry either the v2 id (MachineGuid) or
            //the v1 id (ProcessorId+baseboard serial) — both identify this machine.
            return string.Equals(ComputerId.Instance.EffectiveMachineId, inLicense, StringComparison.OrdinalIgnoreCase)
                || string.Equals(ComputerId.Instance.MachineId, inLicense, StringComparison.OrdinalIgnoreCase);
        }

        private void SetNextHandler(LicHandlingContext context)
        {
            if (IsNameValid(context))
            {
                switch (context.ContextEnvironment)
                {
                    case HandlerStrategy.UnInstall:
                        SetNext(new UnregisterReceiptHandler()); //during unregister the validity does not matter. Just remove this computer form server.
                        return;
                    case HandlerStrategy.Install:
                        SetNext(new LastLicenseHandler());       //we do not care if license is valid during install. Just get it form server on this computer.
                        return;
                    default:
                        SetNext(new LicenseStatusHandler());      //this will check if license is valid during launch.
                        return;
                }
            }
            else
            {
                SetNextError(context, new ComputerNameException(GetErrText(context)));
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
