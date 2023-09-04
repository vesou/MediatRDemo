using Mediator7Hangfire.Interfaces;
using MediatR;
using Polly;

namespace Mediator7Hangfire.Behaviours;

public class RetryPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRetryable
{
    private readonly ILogger _logger;

    public RetryPipelineBehaviour(ILogger<RetryPipelineBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var maxRetryAttempts = 0;
        var pauseBetweenFailures = TimeSpan.FromSeconds(2);

        maxRetryAttempts = request.MaxRetryCount;

        // we could handle different types of exceptions differently here
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures,
                (exception, span, retryCount, context) =>
                {
                    _logger.LogWarning(exception, "{@MethodName} Retrying {@RetryCount}",
                        request.GetType().FullName, retryCount);
                });

        var response = default(TResponse)!;
        await retryPolicy.ExecuteAsync(async () =>
        {
            response = await next();
            return response;
        });

        return response;
    }
}