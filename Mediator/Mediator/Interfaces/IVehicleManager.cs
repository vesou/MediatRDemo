using System.Threading.Tasks;
using Mediator.Entities;

namespace Mediator.Interfaces
{
    public interface IVehicleManager
    {
        Task<MockResponse> GetVehicleDetails(int userInfoBuyerId, int id);
        Task UpdateShortlist(int userInfoBuyerId, int userInfoUserId, MockRequest mockInfo);
        Task AddShortlistNotes(int userInfoBuyerId, int userInfoUserId, MockRequest request);
        Task<MockResponse> GetPurchasingOptions(int userInfoBuyerId, MockRequest purchasingRequest);
        Task<MockResponse> PurchaseVehicle(int userInfoBuyerId, int userInfoUserId, MockRequest purchaseRequest);
        Task<BidResponse> Bid(int userInfoBuyerId, BidRequest bidRequest);
    }
}