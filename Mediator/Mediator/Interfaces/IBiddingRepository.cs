using System.Threading.Tasks;
using Mediator.Entities;

namespace Mediator.Interfaces
{
    public interface IBiddingRepository
    {
        Task<ValidationResult> ValidateBid(BidRequest request);
        Task<BiddingInformation> GetBiddingInformation(int vehicleId);
        Task<BidResult> PlaceBid(BidRequest bidRequest, BiddingInformation currentBidInformation);
    }
}