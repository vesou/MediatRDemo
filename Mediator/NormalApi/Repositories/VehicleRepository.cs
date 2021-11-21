using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NormalApi.DAL;
using NormalApi.Entities;
using NormalApi.Interfaces;

namespace NormalApi.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly ApiContext _dbContext;

        public VehicleRepository(ApiContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<VehicleInfo> GetInfoAsync(int vehicleId)
        {
            var vehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(x => x.Id == vehicleId);
            if (vehicle == null) return null;

            return new VehicleInfo
            {
                Make = vehicle.Make,
                Model = vehicle.Model,
                OnSale = vehicle.OnSale
            };
        }
    }
}