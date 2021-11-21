namespace NormalApi.Entities
{
    public record BiddingInformation(decimal? HighestBid, decimal NextMinBidAmount, int NumberOfBids);
}