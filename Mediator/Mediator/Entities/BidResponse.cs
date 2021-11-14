namespace Mediator.Entities
{
    public class BidResponse
    {
        public BidResponse(string error)
        {
            Error = error;
            Success = false;
            IsHighestBidder = false;
        }

        public BidResponse(int bidId, bool isHighestBidder, decimal bidAmount)
        {
            Error = null;
            Success = true;
            IsHighestBidder = isHighestBidder;
            BidAmount = bidAmount;
        }
        
        public bool Success { get; set; }
        public string Error { get; set; }
        public bool IsHighestBidder { get; set; }
        public decimal? BidAmount { get; set; }
    }
}