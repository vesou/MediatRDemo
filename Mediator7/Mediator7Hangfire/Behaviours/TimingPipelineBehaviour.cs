using System.Diagnostics;
using MediatR;

namespace Mediator7Hangfire.Behaviours
{
    public class TimingPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ILogger _logger;

        public TimingPipelineBehaviour(ILogger<TimingPipelineBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var response = await next();
            sw.Stop();
            _logger.LogInformation("Call to {@MethodName} finished in {@ElapsedMilliseconds}ms",
                request.GetType().FullName, sw.ElapsedMilliseconds);

            return response;
        }
    }
}