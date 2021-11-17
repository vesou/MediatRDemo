namespace Mediator.DAL.Models
{
    public class Bid
    {
        public decimal Amount { get; set; }
        public int Id { get; set; }
        public string User { get; set; }
        public int VehicleId { get; set; }
    }
}