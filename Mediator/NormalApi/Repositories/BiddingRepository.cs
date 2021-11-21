using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NormalApi.DAL;
using NormalApi.DAL.Models;
using NormalApi.Entities;
using NormalApi.Interfaces;

namespace NormalApi.Repositories
{
    public class BiddingRepository : IBiddingRepository
    {
        private readonly ApiContext _dbContext;

        public BiddingRepository(ApiContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BiddingInformation> GetBiddingInformation(int vehicleId)
        {
            List<Bid> bids = await _dbContext.Bids.Where(b => b.VehicleId == vehicleId).ToListAsync();
            decimal currentBid = bids.Any() ? bids.Max(x => x.Amount) : 0;

            return new BiddingInformation(currentBid, currentBid + 100, bids.Count);
        }

        public async Task<BidResult> PlaceBid(BidRequest bidRequest, BiddingInformation currentBidInformation)
        {
            Bid bid = new Bid
            {
                Amount = bidRequest.BidAmount,
                VehicleId = bidRequest.VehicleId
            };

            await _dbContext.Bids.AddAsync(bid);
            await _dbContext.SaveChangesAsync();

            BidResult result = new BidResult
            {
                IsHighestBidder = bidRequest.BidAmount >= currentBidInformation.NextMinBidAmount,
                BidAmount = bidRequest.BidAmount,
                BidId = bid.Id
            };

            return result;
        }

        public async Task<ValidationResult> ValidateBid(BidRequest request)
        {
            return await Task.Run(() =>
            {
                bool validationPassed = request.BidAmount > 0;
                return validationPassed
                    ? new ValidationResult(true)
                    : new ValidationResult("Bid amount must be more than 0");
            });
        }
    }
}