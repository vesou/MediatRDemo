namespace NormalApi.Entities
{
    public class BidResult
    {
        public decimal? BidAmount { get; set; }
        public int? BidId { get; set; }
        public bool IsHighestBidder { get; set; }
    }
}