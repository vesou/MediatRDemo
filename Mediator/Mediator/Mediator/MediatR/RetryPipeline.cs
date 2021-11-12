using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OneLink.Shared.Enums;
using OneLink.Shared.Helpers.Logging;
using OneLink.Shared.Interfaces;
using Polly;

namespace OneLink.Shared.MediatR
{
    public class RetryPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;

        public RetryPipeline(ILogger<RetryPipeline<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var maxRetryAttempts = 0;
            var pauseBetweenFailures = TimeSpan.FromSeconds(2);

            if (request is IRetryable req)
            {
                maxRetryAttempts = req.RetryCount;
            }

            // we could handle InternalApiException, ApplicationException differently here
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures,
                    (exception, span, retryCount, context) =>
                    {
                        _logger.LogWarn(EventTypeEnum.CallRetry, $"{request.GetType().FullName} Retrying {retryCount}", exception);
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