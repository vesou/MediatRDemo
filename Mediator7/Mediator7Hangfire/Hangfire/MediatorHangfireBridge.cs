using System.ComponentModel;
using MediatR;

namespace Mediator7Hangfire.Hangfire;

public class MediatorHangfireBridge
{
    private readonly IMediator _mediator;

    public MediatorHangfireBridge(IMediator mediator)
    {
        _mediator = mediator;
    }

    [DisplayName("{0}")]
    public async Task Send(string jobName, IRequest command)
    {
        await _mediator.Send(command);
    }
}
