using System.Threading.Tasks;
using Mediator.Entities;

namespace Mediator.Interfaces
{
    public interface IBiddingRepository
    {
        Task<BiddingInformation> GetBiddingInformation(int vehicleId);
        Task<BidResult> PlaceBid(BidRequest bidRequest, BiddingInformation currentBidInformation);
        Task<ValidationResult> ValidateBid(BidRequest request);
    }
}