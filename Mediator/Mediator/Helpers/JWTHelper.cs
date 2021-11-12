using System.Collections.Generic;
using System.Security.Claims;
using Mediator.Entities;
using Microsoft.AspNetCore.Http;

namespace Mediator.Helpers
{
    public class JWTHelper
    {
        public static UserInfo GetUserInfoFromToken(List<Claim> toList, IHeaderDictionary requestHeaders)
        {
            return new UserInfo()
            {
                BuyerId = 123,
                SiteId = 234,
                UserType = 1,
                UserId = 345
            };
        }
    }
}