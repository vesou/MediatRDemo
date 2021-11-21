using System.Threading.Tasks;
using NormalApi.Entities;

namespace NormalApi.Interfaces
{
    public interface IVehicleManager
    {
        Task<BidResponse> Bid(BidRequest bidRequest);
    }
}