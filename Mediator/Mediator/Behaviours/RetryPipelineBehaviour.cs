using System;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Polly;

namespace Mediator.Behaviours
{
    public class RetryPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;

        public RetryPipelineBehaviour(ILogger<RetryPipelineBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var maxRetryAttempts = 0;
            var pauseBetweenFailures = TimeSpan.FromSeconds(2);

            if (request is IRetryable req)
            {
                maxRetryAttempts = req.MaxRetryCount;
            }

            // we could handle different types of exceptions differently here
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures,
                    (exception, span, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, "{@MethodName} Retrying {@RetryCount}",
                            request.GetType().FullName, retryCount);
                    });

            TResponse response = default(TResponse);
            await retryPolicy.ExecuteAsync(async () =>
            {
                response = await next();
                return response;
            });

            return response;
        }
    }
}