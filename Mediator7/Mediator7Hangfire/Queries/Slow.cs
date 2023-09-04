using MediatR;

namespace Mediator7Hangfire.Queries;

public static class Slow
{
    public class Query : IRequest
    {
    }

    public class Handler : IRequestHandler<Query>
    {
        private readonly ILogger<Handler> _logger;

        public Handler(ILogger<Handler> logger)
        {
            _logger = logger;
        }

        public Task Handle(Query request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("SlowQueryHandler called");
            var task = Task.Delay(5000, cancellationToken);
            task.Wait(cancellationToken);
            
            return Task.CompletedTask;
        }
    }
}