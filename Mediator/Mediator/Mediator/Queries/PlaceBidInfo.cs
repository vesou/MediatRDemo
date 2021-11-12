using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OneLink.Microservices.Vehicles.Entities;
using OneLink.Microservices.Vehicles.Interfaces;
using OneLink.Microservices.Vehicles.Models;
using OneLink.Shared.Interfaces;

namespace OneLink.Microservices.Vehicles.Queries
{
    public static class PlaceBidInfo
    {
        public class Request : IRequest<Response>, IRetryable
        {
            public int PlacedByBuyerId { get; set; }
            public int UserId { get; set; }
            public int VehicleId { get; set; }
            public int? SelectedGroupSiteId { get; set; }
            public int RetryCount { get; } = 3;
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IBiddingQueryRepository _repository;

            public Handler(IBiddingQueryRepository repository)
            {
                _repository = repository;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                BidData bidData =
                    await _repository.GetBidData(request.PlacedByBuyerId, request.UserId, request.VehicleId, request.SelectedGroupSiteId);
                Bids currentHighestBid = await _repository.GetHighestBid(request.VehicleId);
                decimal nextMinBidAmount = await _repository.MinimumBidAmount(request.VehicleId, bidData.BuyerId, currentHighestBid);

                return new Response(bidData, currentHighestBid, nextMinBidAmount);
            }
        }

        public class Response
        {
            public Response(BidData bidData, Bids currentHighestBid, decimal nextMinBidAmount)
            {
                BidData = bidData;
                CurrentHighestBid = currentHighestBid;
                NextMinBidAmount = nextMinBidAmount;
            }

            public BidData BidData { get; }
            public Bids CurrentHighestBid { get; }
            public decimal NextMinBidAmount { get; }
        }
    }
}