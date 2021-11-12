using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OneLink.Microservices.Vehicles.Interfaces;
using OneLink.Microservices.Vehicles.Models;
using OneLink.Shared.Entities;
using OneLink.Shared.Enums;
using OneLink.Shared.Helpers.Logging;

namespace OneLink.Microservices.Vehicles.Queries
{
    public static class ValidateBid
    {
        public class Request : BaseRequest, IRequest<Response>
        {
            public int VehicleId { get; set; }
            public int? SelectedGroupSiteId { get; set; }
            public decimal MaxBidAmount { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly ILogger _logger;
            private readonly IBiddingQueryRepository _repository;

            public Handler(IBiddingQueryRepository repository, ILogger<Handler> logger)
            {
                _repository = repository;
                _logger = logger;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    if (request.UserInfo?.BuyerId == null || request.UserInfo?.UserId == null)
                    {
                        return new Response(false, EventTypeEnum.BadRequest);
                    }

                    if (!await _repository.IsPurchaseAllowed(request.VehicleId))
                    {
                        return new Response(false, EventTypeEnum.PurchaseNotAllowed);
                    }

                    if (!await _repository.IsBiddingEnabledAndAllowed(request.VehicleId))
                    {
                        return new Response(false, EventTypeEnum.BiddingNotAllowed);
                    }

                    Bids currentHighestBid = await _repository.GetHighestBid(request.VehicleId);
                    if (!await IsMoreThanCurrentBid(request.VehicleId, request.UserInfo.BuyerId.Value, request.MaxBidAmount, currentHighestBid))
                    {
                        return new Response(false, EventTypeEnum.Outbid);
                    }

                    if (!IsNewMaxBidHigherThanPreviousMaxAmount(request.UserInfo.BuyerId.Value,
                        request.MaxBidAmount,
                        currentHighestBid?.BuyerId,
                        currentHighestBid?.MaximumBid))
                    {
                        return new Response(false, EventTypeEnum.MaxBidLessThanPreviousMaxAmount);
                    }

                    if (request.SelectedGroupSiteId.HasValue)
                    {
                        if (!await _repository.IsSelectedGroupSiteValid(request.UserInfo.BuyerId.Value, request.SelectedGroupSiteId.Value))
                        {
                            return new Response(false, EventTypeEnum.SelectedGroupSiteIsNotValid);
                        }
                    }

                    if (await _repository.IsAnotherSisterSiteBidding(request.VehicleId, request.UserInfo.BuyerId.Value))
                    {
                        return new Response(false, EventTypeEnum.AnotherSiteInTheGroupIsBidding);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(EventTypeEnum.UnhandledException, "ValidateBid.Handler threw an Unhandled exception.", e);
                    return new Response(false, EventTypeEnum.UnhandledException);
                }

                return new Response(true);
            }

            private async Task<bool> IsMoreThanCurrentBid(int vehicleId, int buyerId, decimal maxBidAmount, Bids currentHighestBid)
            {
                return maxBidAmount >= await _repository.MinimumBidAmount(vehicleId, buyerId, currentHighestBid);
            }

            private bool IsNewMaxBidHigherThanPreviousMaxAmount(int buyerId, decimal maxBidAmount, int? currentHighestBidBuyerId,
                decimal? currentMaxBid)
            {
                return !currentMaxBid.HasValue || currentHighestBidBuyerId != buyerId || maxBidAmount > currentMaxBid;
            }
        }

        public class Response
        {
            public Response(bool validationPassed, EventTypeEnum? validationError = null)
            {
                ValidationError = validationError;
                ValidationPassed = validationPassed;
            }

            public EventTypeEnum? ValidationError { get; }

            public bool ValidationPassed { get; }
        }
    }
}