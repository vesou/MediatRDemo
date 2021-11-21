namespace Mediator.Entities
{
    public class BidRequest
    {
        public decimal BidAmount { get; set; }
        public int VehicleId { get; set; }
        
        public override string ToString()
        {
            return $"BidAmount: {BidAmount}, VehicleId: {VehicleId}";
        }
    }
}