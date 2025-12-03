using Hymma.Lm.EndUser.Computer;
using Hymma.Lm.EndUser.Exceptions;
using Hymma.Lm.EndUser.Models;
using Hymma.Lm.EndUser.Registrars;
using System;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Hymma.Lm.EndUser
{
    public sealed class LicenseHandlingInstall : LicenseHandlingStrategy
    {
        /// <summary>
        /// allows you to handle license during installation, if license is valid will be saved on disk via <see cref="DefaultLicFileRegistrar"/>
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="computerRegistrarTypes"></param>
        public LicenseHandlingInstall(LicHandlingContext context,Action<LicenseModel> OnLicenseHandledSuccessfully) : base(context, OnLicenseHandledSuccessfully)
        {
            HandlingContext.ContextEnvironment = HandlerStrategy.Install;
        }

        void SetNextHandler()
        {

            //write computer id (MacAddress) on computer registrar
            if (ComputerRegister.TryWrite(ComputerId.Instance.MachineId))
            {
                NextHandler = (new ApiPostComputerHandler());
            }
            else
            {
                //generating a unique identifier on the fly and not saving it somewhere on the hard-drive is meaningless.
                HandlingContext.Exception = new CouldNotWriteComputerIdException();
                NextHandler = (new ErrorHandler());
            }
        }
        public override void HandleLicense()
        {
            //write computer id (MacAddress) on computer registrar
            SetNextHandler();
            NextHandler.HandleContext(HandlingContext); 
        }

        public override Task HandleLicenseAsync()
        {
            //write computer id (MacAddress) on computer registrar
            SetNextHandler();
            return NextHandler.HandleContextAsync(HandlingContext); 
        }
    }
}
