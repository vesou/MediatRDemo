using System.Collections.Generic;

namespace Mediator.DAL.Models
{
    public class Bid
    {
        public int Id { get; set; }
        public string User { get; set; }
        public int VehicleId { get; set; }
        public decimal Amount { get; set; }
    }
}