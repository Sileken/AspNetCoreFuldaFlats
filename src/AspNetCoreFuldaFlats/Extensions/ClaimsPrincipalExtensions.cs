using System.Linq;
using System.Security.Claims;

namespace AspNetCoreFuldaFlats.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            return int.Parse(principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid).Value);
        }
    }
}
