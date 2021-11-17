using System;
using System.Threading.Tasks;
using Mediator.Entities;
using Mediator.Interfaces;

namespace Mediator.Repositories
{
    public class BiddingRepository : IBiddingRepository
    {
        public Task<BiddingInformation> GetBiddingInformation(int vehicleId)
        {
            throw new NotImplementedException();
        }

        public Task<BidResult> PlaceBid(BidRequest bidRequest, BiddingInformation currentBidInformation)
        {
            throw new NotImplementedException();
        }

        public Task<ValidationResult> ValidateBid(BidRequest request)
        {
            throw new NotImplementedException();
        }
    }
}