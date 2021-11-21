using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mediator.Behaviours
{
    public class TimingPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;

        public TimingPipelineBehaviour(ILogger<TimingPipelineBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var response = await next();
            sw.Stop();
            _logger.LogInformation("Call to {@MethodName} finished in {@ElapsedMilliseconds}",
                request.GetType().FullName, sw.ElapsedMilliseconds);

            return response;
        }
    }
}