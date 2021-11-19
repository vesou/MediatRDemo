namespace Mediator.Entities
{
    public record BiddingInformation(decimal? HighestBid, decimal NextMinBidAmount, int NumberOfBids);
}