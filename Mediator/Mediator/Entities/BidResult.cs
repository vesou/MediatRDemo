namespace Mediator.Entities
{
    public class BidResult
    {
        public int? BidId { get; set; }
        public bool IsHighestBidder { get; set; }
        public decimal? BidAmount { get; set; }
    }
}