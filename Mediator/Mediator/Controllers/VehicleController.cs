using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Mediator.Commands;
using Mediator.Entities;
using Mediator.Interfaces;
using Mediator.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;

namespace Mediator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehicleController : BaseController
    {
        private const int MaxRetryAttempts = 3;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly TimeSpan _pauseBetweenFailures = TimeSpan.FromSeconds(2);
        private readonly IShortlistManager _shortlistManager;
        private readonly IVehicleManager _vehicleManager;

        public VehicleController(IVehicleManager vehicleVehicleManager, ILogger<VehicleController> logger,
            IShortlistManager shortlistManager, IMediator mediator, IMapper mapper)
        {
            _vehicleManager = vehicleVehicleManager;
            _logger = logger;
            _shortlistManager = shortlistManager;
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet("{vehicleId:int}")]
        public async Task<IActionResult> GetVehicleInfo(int vehicleId)
        {
            GetVehicleInfo.Response vehicleInfo = await _mediator.Send(new GetVehicleInfo.Request(vehicleId));
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

        [HttpPost("Bid2")]
        public async Task<IActionResult> PlaceBidOnVehicle2([FromBody] BidRequest bidRequest)
        {
            PlaceBid.Response bidResponse = await _mediator.Send(_mapper.Map<PlaceBid.Command>(bidRequest));
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