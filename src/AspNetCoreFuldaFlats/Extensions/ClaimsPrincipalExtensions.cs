using System.Linq;
using System.Security.Claims;

namespace AspNetCoreFuldaFlats.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid).Value;
        }
    }
}
