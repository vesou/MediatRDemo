using System.Threading.Tasks;
using Mediator.Entities;
using Mediator.Interfaces;

namespace Mediator.Managers
{
    public class VehicleManager : IVehicleManager
    {
        private readonly IBiddingRepository _biddingRepository;

        public VehicleManager(IBiddingRepository biddingRepository)
        {
            _biddingRepository = biddingRepository;
        }

        public async Task<BidResponse> Bid(BidRequest bidRequest)
        {
            var validationResultTask = _biddingRepository.ValidateBid(bidRequest);
            var currentBidInformationTask = _biddingRepository.GetBiddingInformation(bidRequest.VehicleId);
            ValidationResult validationResult = await validationResultTask;
            if (!validationResult.ValidationPassed)
            {
                return new BidResponse(validationResult.ValidationError);
            }

            BiddingInformation currentBidInformation = await currentBidInformationTask;
            if (currentBidInformation == null)
            {
                return new BidResponse("Error getting PlaceBidInfo.");
            }

            BidResult bidResult = await _biddingRepository.PlaceBid(bidRequest, currentBidInformation);

            if (!bidResult.BidId.HasValue || !bidResult.BidAmount.HasValue)
            {
                return new BidResponse("Failed to place a bid.");
            }

            return new BidResponse(bidResult.BidId.Value, bidResult.IsHighestBidder, bidResult.BidAmount.Value);
        }
    }
}