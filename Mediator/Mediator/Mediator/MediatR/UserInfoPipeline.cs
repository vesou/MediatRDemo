using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using OneLink.Shared.Entities;
using OneLink.Shared.Helpers;

namespace OneLink.Shared.MediatR
{
    public class UserInfoPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly HttpContext _httpContext;

        public UserInfoPipeline(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext.HttpContext;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            UserInfo userInfo = JWTHelper.GetUserInfoFromToken(_httpContext.User.Claims.ToList(), _httpContext.Request.Headers);

            var br = request as BaseRequest;
            if (br != null && br.UserInfo == null)
            {
                br.UserInfo = userInfo;
            }

            return await next();
        }
    }
}