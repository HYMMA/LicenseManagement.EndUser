using Hymma.Lm.EndUser.Exceptions;
using Hymma.Lm.EndUser.Product.Handlers;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Hymma.Lm.EndUser.Computer
{
    internal class ApiPostComputerHandler : LicenseValidationHandler
    {
        /// <summary>
        /// post a new computer to the server and set next handler to <see cref="ApiGetProductHandler"/> 
        /// </summary>
        public ApiPostComputerHandler()
        {
        }

        public override void HandleContext(LicHandlingContext context)
        {
            try
            {
                var apiClient = new ComputerApiEndPoint(context.PublisherPreferences.ApiKey.ToString());

                var response = apiClient
                    .PostComputer(new Models.PostComputerModel()
                    {
                        Name = ComputerId.Instance.MachineName,
                        MacAddress = ComputerId.Instance.MachineId
                    });

                //if created a new one or if it already existed
                if (response == HttpStatusCode.Created || response == HttpStatusCode.Conflict)
                {
                    //update context product Id
                    SetNext(new ApiGetComputerHandler());
                }
                else
                {
                    context.Exception = new ApiException($"Could not create new computer {ComputerId.Instance.MachineId} on db.", response);
                    SetNext(new ErrorHandler());
                }
            }
            catch (Exception e)
            {
                context.Exception = e;
                SetNext(new ErrorHandler());
            }
            nextHandler.HandleContext(context);
        }

        public override async Task HandleContextAsync(LicHandlingContext context)
        {
            try
            {
                var apiClient = new ComputerApiEndPoint(context.PublisherPreferences.ApiKey);
                var response = await apiClient
                    .PostComputerAsync(new Models.PostComputerModel()
                    {
                        Name = ComputerId.Instance.MachineName,
                        MacAddress = ComputerId.Instance.MachineId
                    })
                    .ConfigureAwait(false);

                //if created a new one or if it already existed
                if (response == HttpStatusCode.Created || response == HttpStatusCode.Conflict)
                {
                    //update context product Id
                    SetNext(new ApiGetComputerHandler());
                }
                else
                {
                    context.Exception = new ApiException($"Could not create new computer {ComputerId.Instance.MachineId} on db. ", response);
                    SetNext(new ErrorHandler());
                }
            }
            catch (Exception e)
            {
                context.Exception = e;
                SetNext(new ErrorHandler());
            }
            await nextHandler.HandleContextAsync(context).ConfigureAwait(false);

        }
    }
}
