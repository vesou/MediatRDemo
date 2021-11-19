using System.Threading.Tasks;
using Mediator.Entities;

namespace Mediator.Interfaces
{
    public interface IVehicleRepository
    {
        Task<VehicleInfo> GetInfoAsync(int vehicleId);
    }
}