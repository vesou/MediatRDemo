using System.Collections.Generic;

namespace NormalApi.DAL.Models
{
    public class Vehicle
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public bool OnSale { get; set; }
        public int VehicleId { get; set; }
    }
}