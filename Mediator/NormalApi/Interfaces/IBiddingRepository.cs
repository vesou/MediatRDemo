using System.Threading.Tasks;
using NormalApi.Entities;

namespace NormalApi.Interfaces
{
    public interface IBiddingRepository
    {
        Task<BiddingInformation> GetBiddingInformation(int vehicleId);
        Task<BidResult> PlaceBid(BidRequest bidRequest, BiddingInformation currentBidInformation);
        Task<ValidationResult> ValidateBid(BidRequest request);
    }
}