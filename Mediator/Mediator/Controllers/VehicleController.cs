using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Mediator.Entities;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;

namespace Mediator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehicleController : ControllerBase
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