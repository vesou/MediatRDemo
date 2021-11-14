using System.Threading.Tasks;
using Mediator.Entities;

namespace Mediator.Interfaces
{
    public interface IVehicleManager
    {
        Task<BidResponse> Bid(BidRequest bidRequest);
    }
}