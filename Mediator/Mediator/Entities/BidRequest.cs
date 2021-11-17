namespace Mediator.Entities
{
    public class BidRequest
    {
        public decimal MaxBidAmount { get; set; }
        public int? SelectedGroupSiteId { get; set; }
        public int VehicleId { get; set; }
    }
}