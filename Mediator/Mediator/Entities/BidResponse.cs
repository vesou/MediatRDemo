namespace Mediator.Entities
{
    public class BidResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public bool IsHighestBidder { get; set; }
    }
}