using System;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Entities;
using Mediator.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mediator.Queries
{
    public static class GetVehicleInfo
    {
        public class Request : IRequest<Response>
        {
            public int VehicleId { get; set; }

            public Request(int vehicleId)
            {
                VehicleId = vehicleId;
            }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IVehicleManager _vehicleManager;

            public Handler(IVehicleManager vehicleManager, ILogger<Handler> logger)
            {
                _vehicleManager = vehicleManager;
                _logger = logger;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                VehicleInfo vehicleInfo = await _vehicleManager.GetInfo(request.VehicleId);
                return vehicleInfo is not null 
                    ? new Response(vehicleInfo)
                    : null;
            }
        }

        public record Response(VehicleInfo VehicleInfo);
    }
}