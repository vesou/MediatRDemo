using System.Threading.Tasks;
using NormalApi.Entities;

namespace NormalApi.Interfaces
{
    public interface IBiddingRepository
    {
        Task<BiddingInformation> GetBiddingInformationAsync(int vehicleId);
        Task<BidResult> PlaceBid(BidRequest bidRequest, BiddingInformation currentBidInformation);
        Task<ValidationResult> ValidateBidAsync(BidRequest request);
    }
}