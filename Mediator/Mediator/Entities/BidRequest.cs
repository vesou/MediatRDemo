namespace Mediator.Entities
{
    public class BidRequest
    {
        public int VehicleId { get; set; }
        public int? SelectedGroupSiteId { get; set; }
        public decimal MaxBidAmount { get; set; }
    }
}