using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OneLink.Microservices.Vehicles.Enums;
using OneLink.Microservices.Vehicles.Interfaces;
using OneLink.Microservices.Vehicles.Models;
using OneLink.Microservices.Vehicles.Queries;
using OneLink.Shared.Entities;
using OneLink.Shared.Entities.EventData;
using OneLink.Shared.Enums;
using OneLink.Shared.Helpers;
using OneLink.Shared.Helpers.Logging;
using OneLink.Shared.Interfaces;

namespace OneLink.Microservices.Vehicles.Commands
{
    public static class PlaceBid
    {
        public class Command : BaseRequest, IRequest<Response>, ILoggable
        {
            public int VehicleId { get; set; }
            public int? SelectedGroupSiteId { get; set; }
            public decimal MaxBidAmount { get; set; }
            public SourceTypeEnum SourceType { get; set; }

            public string ToLogMessage()
            {
                return $"VehicleId: {VehicleId}, SelectedGroupSiteId: {SelectedGroupSiteId?.ToString() ?? "null"}, MaxBidAmount: {MaxBidAmount}" +
                       $"SourceType: {SourceType}, BuyerId: {UserInfo?.BuyerId}, UserId: {UserInfo?.UserId}";
            }
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IBiddingCommandRepository _commandRepository;
            private readonly ILogger _logger;
            private readonly IMapper _mapper;
            private readonly IMediator _mediator;

            public Handler(IBiddingCommandRepository commandRepository, IMediator mediator, IMapper mapper, ILogger<Handler> logger)
            {
                _commandRepository = commandRepository;
                _mediator = mediator;
                _mapper = mapper;
                _logger = logger;
            }

            // TODO: propagate cancellation token to all async calls that we want to allow a cancel 
            // - if it's after the point of "no going back" don't pass it anymore
            // let's discuss if it's worth using it - this might be useful with MediatR to handle graceful shutdown etc. 
            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                ValidateBid.Request validationRequest = _mapper.Map<ValidateBid.Request>(request);
                ValidateBid.Response validationResponse = await _mediator.Send(validationRequest, cancellationToken);

                if (!validationResponse.ValidationPassed)
                {
                    return new Response
                    {
                        MethodResult = new MethodResult(new BaseEventType(validationResponse.ValidationError ?? EventTypeEnum.InternalError))
                    };
                }

                PlaceBidInfo.Response placeBidInfo = await _mediator.Send(new PlaceBidInfo.Request
                {
                    // ReSharper disable once PossibleInvalidOperationException - it's checked in ValidateBid method
                    PlacedByBuyerId = request.UserInfo.BuyerId.Value,
                    // ReSharper disable once PossibleInvalidOperationException - it's checked in ValidateBid method
                    UserId = request.UserInfo.UserId.Value,
                    VehicleId = request.VehicleId,
                    SelectedGroupSiteId = request.SelectedGroupSiteId
                }, cancellationToken);

                if (placeBidInfo == null)
                {
                    return new Response
                    {
                        MethodResult = new MethodResult(new BaseEventType(EventTypeEnum.InternalError, "Error getting PlaceBidInfo."))
                    };
                }

                (int? bidId, MethodResult result) = await PlaceBidOnVehicle(
                    request.UserInfo.BuyerId.Value,
                    request.UserInfo.UserId.Value,
                    request.VehicleId,
                    request.MaxBidAmount,
                    request.SelectedGroupSiteId,
                    request.SourceType,
                    placeBidInfo);

                var response = new Response
                {
                    BidId = bidId,
                    MethodResult = result
                };

                return response;
            }

            private async Task<(int? bidId, MethodResult result)> PlaceBidOnVehicle(int placedByBuyerId, int userId, int vehicleId,
                decimal maxBidAmount,
                int? selectedGroupSiteId, SourceTypeEnum sourceType, PlaceBidInfo.Response placeBidInfo)
            {
                try
                {
                    await _commandRepository.CancelPreviousBid(vehicleId, placeBidInfo.BidData.BuyerId, placeBidInfo.BidData.BiddingSessionId);
                    int bidId = await _commandRepository.CreateBid(vehicleId, placeBidInfo.NextMinBidAmount, maxBidAmount,
                        placeBidInfo.BidData.BuyerId, placedByBuyerId, userId,
                        placeBidInfo.BidData.BiddingSessionId, sourceType);

                    List<(Func<Task> method, string stepName)> steps = new List<(Func<Task>, string stepName)>
                    {
                        (async () => await _commandRepository.UpdateBuyer(placeBidInfo.BidData.BuyerId), "UpdateBuyer"),
                        (async () => await _commandRepository.UpdateVehicleFlags(vehicleId, placeBidInfo.BidData.BidsReceived, placeBidInfo.NextMinBidAmount,
                            placeBidInfo.BidData.OnlineReservePrice), "UpdateVehicleFlags"),
                        (async () => await LogVehicleEvent(vehicleId, placeBidInfo.BidData.BuyerId, userId, placeBidInfo.NextMinBidAmount,
                            placeBidInfo.BidData.BuyerName, placeBidInfo.BidData.OwnerVendorId, placeBidInfo.BidData.ContactName, maxBidAmount), "LogVehicleEvent"),
                        (async () => await _commandRepository.DeleteMessages(vehicleId, placeBidInfo.BidData.BuyerId), "DeleteMessages"),
                        (async () => await _commandRepository.Buyer_DailyStats(placeBidInfo.BidData.BuyerId), "Buyer_DailyStats"),
                        (async () => await _commandRepository.Vehicle_BuyerTrayInfo_PlaceBid(vehicleId, placedByBuyerId, bidId, selectedGroupSiteId), "Vehicle_BuyerTrayInfo_PlaceBid"),
                        (async () => await _commandRepository.SP_CheckBidActionRequired(vehicleId, placeBidInfo.BidData.OwnerVendorId), "SP_CheckBidActionRequired")
                    };

                    if (!await PollyHelper.ExecuteMultipleStepsAsync(steps, "PlaceBid", _logger))
                    {
                        return (null, new MethodResult(new BaseEventType(EventTypeEnum.BidDeclined)));
                    }

                    bool isHighestBid = await UpdateVehicleTopBid(vehicleId, placeBidInfo.CurrentHighestBid, bidId, maxBidAmount,
                        placeBidInfo.BidData.BuyerId, placeBidInfo.NextMinBidAmount, placeBidInfo.BidData.OwnerVendorId,
                        placeBidInfo.BidData.OwnerBuyerId, placeBidInfo.BidData.RegistrationNumber);
                    await CheckAutoExtendBiddingEndDate(placeBidInfo.BidData.BiddingType, vehicleId, placeBidInfo.BidData.OwnerVendorId,
                        placeBidInfo.BidData.BiddingSessionId, placeBidInfo.BidData.BiddingEndDate, placeBidInfo.NextMinBidAmount,
                        placeBidInfo.BidData.BuyerName);

                    return (bidId,
                        isHighestBid ? new MethodResult() : new MethodResult(new BaseEventType(EventTypeEnum.Outbid)));
                }
                catch (Exception ex)
                {
                    _logger.LogError(EventTypeEnum.UnhandledException, "PlaceBidOnVehicle failed unexpectedly.", ex);
                }

                return (null, new MethodResult(new BaseEventType(EventTypeEnum.UnhandledException)));
            }

            private async Task<bool> UpdateVehicleTopBid(int vehicleId, Bids currentHighestBid, int newBidId, decimal newMaxBidAmount,
                int newBidBuyerId,
                decimal nextMinBidAmount, int ownerVendorId, int? ownerBuyerId, string registrationNumber)
            {
                (decimal bidAmount, Bids highestBid, Bids losingBId) =
                    await _commandRepository.CalculateBidAmount(currentHighestBid, newBidId, newMaxBidAmount, newBidBuyerId, nextMinBidAmount);
                await _commandRepository.UpdateHighestBidIdInVehicleDetails(vehicleId, highestBid.BidId);
                await _commandRepository.UpdateWinningAndLosingBids(bidAmount, highestBid, losingBId);
                if (losingBId?.BuyerId != null)
                {
                    await _commandRepository.SendOutbidMessage(losingBId.BuyerId.Value, ownerVendorId, ownerBuyerId, vehicleId,
                        losingBId.BuyerRequisitionId,
                        registrationNumber);
                }

                return newBidId == highestBid.BidId;
            }

            private async Task LogVehicleEvent(int vehicleId, int buyerId, int userId, decimal bidAmount, string buyerName, int vendorId,
                string contactName, decimal maxBidAmount)
            {
                string bidAmountString = bidAmount.ToString("F2");
                string maxBidAmountString = maxBidAmount.ToString("F2");
                                
                // Add history line for the seller
                await _commandRepository.Vehicle_AddVehicleEventHistory(vehicleId, EventBuilder.GetCode(EventTypeEnum.BidPlacedVendorEvent),
                    vendorId, UserType.Vendor, userId, $"{bidAmountString}, {contactName}, {buyerName}");

                // Add the history line for the buyer
                await _commandRepository.Vehicle_AddVehicleEventHistory(vehicleId, EventBuilder.GetCode(EventTypeEnum.BidPlacedBuyerEventWithMaxBid), buyerId,
                    UserType.Buyer, userId, $"{bidAmountString}, {maxBidAmountString}");
            }

            private async Task CheckAutoExtendBiddingEndDate(BiddingType biddingType, int vehicleId, int vendorId, int biddingSessionId,
                DateTime? biddingEndDate, decimal newBidAmount,
                string buyerName)
            {
                if (biddingType != BiddingType.Timed)
                {
                    return;
                }

                if (biddingEndDate == null || biddingEndDate < DateTime.Now)
                {
                    return;
                }

                await _commandRepository.ExtendBiddingEndDate(vehicleId, vendorId, biddingSessionId, biddingEndDate.Value, newBidAmount, buyerName);
            }
        }

        public class Response : ILoggable
        {
            public MethodResult MethodResult { get; set; }
            public int? BidId { get; set; }

            public string ToLogMessage()
            {
                return $"Successful: {MethodResult.CallSuccessful}, BidId: {BidId?.ToString() ?? "null"}, " +
                       $"ErrorCode: {MethodResult.ErrorCode}, ErrorMessage: {MethodResult.ErrorMessage}";
            }
        }
    }
}