using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
            private readonly IMapper _mapper;
            private readonly IVehicleRepository _vehicleRepository;

            public Handler(IVehicleRepository vehicleRepository, ILogger<Handler> logger, IMapper mapper)
            {
                _vehicleRepository = vehicleRepository;
                _logger = logger;
                _mapper = mapper;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                VehicleInfo vehicleInfo = await _vehicleRepository.GetInfoAsync(request.VehicleId);
                return vehicleInfo is not null 
                    ? _mapper.Map<Response>(vehicleInfo)
                    : null;
            }
        }

        public class Response
        {
            public string Make { get; set; }
            public string Model { get; set; }
            public bool OnSale { get; set; }
        }
    }
}