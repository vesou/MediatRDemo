using System.Threading.Tasks;
using Mediator.Entities;

namespace Mediator.Interfaces
{
    public interface IBiddingRepository
    {
        Task<BiddingInformation> GetBiddingInformationAsync(int vehicleId);
        Task<BidResult> PlaceBid(BidRequest bidRequest, BiddingInformation currentBidInformation);
        Task<ValidationResult> ValidateBidAsync(BidRequest request);
    }
}