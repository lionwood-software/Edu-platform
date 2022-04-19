using IdentityModel;
using System.Linq;
using System.Security.Claims;

namespace SchoolApi.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static string GetOwnRole(this ClaimsPrincipal claimsPrincipal)
        {
            return string.Empty;
        }

        public static string GetSub(this ClaimsPrincipal claimsPrincipal)
        {
            var sub = claimsPrincipal.Claims.First(c => c.Type == JwtClaimTypes.Subject).Value;

            return sub;
        }
    }
}
