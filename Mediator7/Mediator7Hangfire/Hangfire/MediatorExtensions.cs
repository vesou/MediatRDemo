using Hangfire;
using Hangfire.States;
using MediatR;

namespace Mediator7Hangfire.Hangfire;

public static class MediatorExtensions
{
    public static void Enqueue(this IMediator mediator, string jobName, IRequest request, string queueName)
    {
        var backgroundJobClient = new BackgroundJobClient();
        backgroundJobClient.Create<MediatorHangfireBridge>(bridge => bridge.Send(jobName, request), new EnqueuedState(queueName));
    }
}
