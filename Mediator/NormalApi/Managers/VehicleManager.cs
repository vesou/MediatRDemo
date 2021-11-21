using System.Threading.Tasks;
using NormalApi.Entities;
using NormalApi.Interfaces;

namespace NormalApi.Managers
{
    public class VehicleManager : IVehicleManager
    {
        private readonly IBiddingRepository _biddingRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleManager(IBiddingRepository biddingRepository, IVehicleRepository vehicleRepository)
        {
            _biddingRepository = biddingRepository;
            _vehicleRepository = vehicleRepository;
        }

        public async Task<BidResponse> Bid(BidRequest bidRequest)
        {
            var validationResult = await _biddingRepository.ValidateBid(bidRequest);

            if (!validationResult.ValidationPassed)
            {
                return new BidResponse(validationResult.ValidationError);
            }

            var vehicleDetails = await _vehicleRepository.GetInfo(bidRequest.VehicleId);

            if (vehicleDetails == null)
            {
                return new BidResponse("Vehicle doesn't exist.");
            }

            if (!vehicleDetails.OnSale)
            {
                return new BidResponse("Vehicle no longer on sale.");
            }

            var currentBidInformation = await _biddingRepository.GetBiddingInformation(bidRequest.VehicleId);

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

        public Task<VehicleInfo> GetInfo(int vehicleId)
        {
            return _vehicleRepository.GetInfo(vehicleId);
        }
    }
}