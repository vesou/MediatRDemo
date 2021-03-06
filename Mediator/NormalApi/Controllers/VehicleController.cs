using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NormalApi.Entities;
using NormalApi.Interfaces;
using Polly;

namespace NormalApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehicleController : BaseController
    {
        private const int MaxRetryAttempts = 3;
        private readonly ILogger _logger;
        private readonly TimeSpan _pauseBetweenFailures = TimeSpan.FromSeconds(2);
        private readonly IShortlistManager _shortlistManager;
        private readonly IVehicleManager _vehicleManager;

        public VehicleController(IVehicleManager vehicleVehicleManager, ILogger<VehicleController> logger,
            IShortlistManager shortlistManager)
        {
            _vehicleManager = vehicleVehicleManager;
            _logger = logger;
            _shortlistManager = shortlistManager;
        }

        [HttpGet("{vehicleId}")]
        public async Task<IActionResult> GetVehicleInfo(int vehicleId)
        {
            VehicleInfo vehicleInfo = await _vehicleManager.GetInfo(vehicleId);
            _logger.LogInformation("GetVehicleInfo called , for vehicleId: {@VehicleId}, response: {@VehicleInfo}",
                vehicleId, vehicleInfo);

            return OkOrNotFound(vehicleInfo);
        }

        [HttpPost("Bid")]
        public async Task<IActionResult> PlaceBidOnVehicle([FromBody] BidRequest bidRequest)
        {
            BidResponse bidResponse = await _vehicleManager.Bid(bidRequest);
            _logger.LogInformation("Bid called , request: {@BiddingRequest}, response: {@BiddingResult}", bidRequest,
                bidResponse);

            return new OkObjectResult(bidResponse);
        }

        [HttpPost("Shortlist")]
        public async Task<IActionResult> Shortlist([FromBody] ShortlistRequest shortlistRequest)
        {
            await _shortlistManager.UpdateShortlist(shortlistRequest);
            _logger.LogInformation(
                "Update shortlist called for VehicleId: {@VehicleId}, with value of: {@IsShortlisted}",
                shortlistRequest.VehicleId, shortlistRequest.Shortlist);

            return new OkResult();
        }

        [HttpPost("Shortlist/Note")]
        public async Task<IActionResult> ShortlistNotes([FromBody] ShortlistNoteRequest noteRequest)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(MaxRetryAttempts, i => _pauseBetweenFailures,
                        (exception, span, retryCount, context) =>
                        {
                            _logger.LogWarning(exception, "{@Action} Retrying {@RetryCount}", "shortlistNotes",
                                retryCount);
                        });

                await retryPolicy.ExecuteAsync(async () => { await _shortlistManager.AddShortlistNotes(noteRequest); });

                sw.Stop();
                _logger.LogInformation(
                    "Shortlist notes for VehicleId: {@VehicleId}, set to: {@Notes} and took: {@ElapsedMilliseconds}",
                    noteRequest.VehicleId, noteRequest.Note, sw.ElapsedMilliseconds);

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Updating Shortlist notes failed. ");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}