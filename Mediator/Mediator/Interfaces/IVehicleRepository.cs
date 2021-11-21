using System.Threading.Tasks;
using Mediator.Entities;

namespace Mediator.Interfaces
{
    public interface IVehicleRepository
    {
        Task<VehicleInfo> GetInfo(int vehicleId);
    }
}