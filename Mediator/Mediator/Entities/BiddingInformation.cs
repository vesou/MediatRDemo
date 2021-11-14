namespace Mediator.Entities
{
    public class BiddingInformation
    {
        public decimal? HighestBid { get; set; }
        public decimal NextMinBidAmount { get; set; }
        public int NumberOfBids { get; set; }
    }
}