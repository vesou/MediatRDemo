using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mediator.Entities;
using Mediator.Helpers;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Mediator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehicleMediatorController : ControllerBase
    {
        private readonly IVehicleManager _manager;
        private readonly ILogger _logger;
        //private readonly IMediator _mediator;
        //private readonly IMapper _mapper;

        public VehicleMediatorController(IVehicleManager vehicleManager, ILogger<VehicleMediatorController> logger)
        {
            _manager = vehicleManager;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details([FromRoute] int id)
        {
            UserInfo userInfo = JWTHelper.GetUserInfoFromToken(User.Claims.ToList(), Request.Headers);
            MockResponse vehicleDetailsView = await _manager.GetVehicleDetails(userInfo.BuyerId.Value, id);
            _logger.LogInformation("Mock message, request: , response: ");

            return new OkObjectResult(vehicleDetailsView);
        }

        [HttpPost("Shortlist")]
        public async Task<IActionResult> ShortlistAction([FromBody] MockRequest mockInfo)
        {
            UserInfo userInfo = JWTHelper.GetUserInfoFromToken(User.Claims.ToList(), Request.Headers);
            await _manager.UpdateShortlist(userInfo.BuyerId.Value, userInfo.UserId.Value, mockInfo);
            _logger.LogInformation("Mock message, request: , response: ");

            return new OkResult();
        }

        [HttpPost("Purchasing")]
        public async Task<IActionResult> GetPurchasingOptions([FromBody] MockRequest purchasingRequest)
        {
            UserInfo userInfo = JWTHelper.GetUserInfoFromToken(User.Claims.ToList(), Request.Headers);
            MockResponse purchasingOptions = await _manager.GetPurchasingOptions(userInfo.BuyerId.Value, purchasingRequest);
            _logger.LogInformation("Mock message, request: , response: ");
            
            return new OkObjectResult(purchasingOptions);
        }

        [HttpPost("Purchase")]
        public async Task<IActionResult> PurchaseVehicle([FromBody] MockRequest purchaseRequest)
        {
            UserInfo userInfo = JWTHelper.GetUserInfoFromToken(User.Claims.ToList(), Request.Headers);
            MockResponse purchaseResponse = await _manager.PurchaseVehicle(userInfo.BuyerId.Value, userInfo.UserId.Value, purchaseRequest);
            _logger.LogInformation("Mock message, request: , response: ");
            
            return new OkObjectResult(purchaseResponse);
        }

        // [HttpPost("Bid")]
        // public async Task<IActionResult> PlaceBidOnVehicle([FromBody] BidRequest bidRequest)
        // {
        //     PlaceBid.Response bidResponse = await _mediator.Send(_mapper.Map<PlaceBid.Command>(bidRequest));
        //
        //     return new OkObjectResult(bidResponse);
        // }
    }
}