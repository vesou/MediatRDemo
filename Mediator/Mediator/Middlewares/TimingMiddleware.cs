using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mediator.Middlewares
{
    public class TimingMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public TimingMiddleware(RequestDelegate next, ILogger<TimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            await _next(context);
            sw.Stop();

            _logger.LogInformation("call to {RequestPath} took {ElapsedMilliseconds}ms", context.Request.Path,
                sw.ElapsedMilliseconds);
        }
    }
}