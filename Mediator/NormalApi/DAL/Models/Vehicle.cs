using System.Collections.Generic;

namespace NormalApi.DAL.Models
{
    public class Vehicle
    {
        public List<Bid> Bids { get; set; }
        public int Id { get; set; }

        public string Make { get; set; }
        public string Model { get; set; }

        public bool OnSale { get; set; }
    }
}