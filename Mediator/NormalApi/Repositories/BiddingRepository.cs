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

        public async Task<BiddingInformation> GetBiddingInformationAsync(int vehicleId)
        {
            Vehicle vehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(x => x.Id == vehicleId);
            List<Bid> bids = vehicle.Bids.ToList();
            decimal currentBid = bids.Max(x => x.Amount);

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

        public async Task<ValidationResult> ValidateBidAsync(BidRequest request)
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