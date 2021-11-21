using System.Threading.Tasks;
using NormalApi.Entities;

namespace NormalApi.Interfaces
{
    public interface IVehicleRepository
    {
        Task<VehicleInfo> GetInfoAsync(int vehicleId);
    }
}