using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Mediator.Entities;
using Mediator.Interfaces;
using Mediator.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mediator.Commands
{
    public static class PlaceBid
    {
        public class Command : IRequest<Response>
        {
            public Command(int vehicleId, decimal bidAmount)
            {
                VehicleId = vehicleId;
                BidAmount = bidAmount;
            }

            public decimal BidAmount { get; set; }
            public int VehicleId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IBiddingRepository _biddingRepository;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;
            private readonly IMediator _mediator;

            public Handler(ILogger<Handler> logger,
                IBiddingRepository biddingRepository, IMapper mapper, IMediator mediator)
            {
                _logger = logger;
                _biddingRepository = biddingRepository;
                _mapper = mapper;
                _mediator = mediator;
            }

            public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
            {
                BidRequest request = _mapper.Map<BidRequest>(command);
                var validationResult = await _biddingRepository.ValidateBid(request);

                if (!validationResult.ValidationPassed)
                {
                    return Response.Failed(validationResult.ValidationError);
                }

                var vehicleDetails =
                    await _mediator.Send(new GetVehicleInfo.Request(command.VehicleId), cancellationToken);

                if (vehicleDetails == null)
                {
                    return Response.Failed("Vehicle doesn't exist.");
                }

                if (!vehicleDetails.OnSale)
                {
                    return Response.Failed("Vehicle no longer on sale.");
                }

                var currentBidInformation = await _biddingRepository.GetBiddingInformation(command.VehicleId);
                if (currentBidInformation == null)
                {
                    return Response.Failed("Error getting PlaceBidInfo.");
                }

                BidResult bidResult = await _biddingRepository.PlaceBid(request, currentBidInformation);

                if (!bidResult.BidId.HasValue || !bidResult.BidAmount.HasValue)
                {
                    return Response.Failed("Failed to place a bid.");
                }

                return Response.Succeeded(bidResult.BidId.Value, bidResult.IsHighestBidder, bidResult.BidAmount.Value);
            }
        }

        public class Response
        {
            public decimal? BidAmount { get; set; }
            public int BidId { get; set; }
            public string Error { get; set; }
            public bool IsHighestBidder { get; set; }

            public bool Success { get; set; }

            public static Response Failed(string message)
            {
                return new Response { Error = message };
            }

            public static Response Succeeded(int bidId, bool isHighestBidder, decimal bidAmount)
            {
                return new Response
                {
                    Error = null,
                    Success = true,
                    IsHighestBidder = isHighestBidder,
                    BidAmount = bidAmount,
                    BidId = bidId
                };
            }
        }
    }
}