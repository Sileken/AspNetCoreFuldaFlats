using System.Threading.Tasks;
using AspNetCoreFuldaFlats.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreFuldaFlats.Middlwares.HtmlFileExtensionMiddleware
{
    public class HtmlFileExtensionMiddleware
    {
        private readonly RequestDelegate _next;

        public HtmlFileExtensionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            if (!IsApiCall(httpContext))
            {
                ResolveRequestFileExtension(httpContext);
            }

            return _next(httpContext);
        }

        private bool IsApiCall(HttpContext httpContext)
        {
            return httpContext.Request.Path.StartsWithSegments(new PathString(GlobalConstants.Routes.RelativeUrlPath));
        }

        private void ResolveRequestFileExtension(HttpContext httpContext)
        {
            var requestPath = httpContext.Request.Path.ToString();
            requestPath = requestPath.Trim().TrimEnd('/');

            var lastIndexOfSlash = requestPath.LastIndexOf('/');
            if (lastIndexOfSlash + 1 < requestPath.Length)
            {
                var file = requestPath.Substring(lastIndexOfSlash + 1);
                if (!file.Contains("."))
                {
                    requestPath += ".html";
                    httpContext.Request.Path = requestPath;
                }
            }
        }
    }

    public static class HtmlFileExtensionMiddlewareExtensions
    {
        public static IApplicationBuilder UseHtmlFileExtensionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HtmlFileExtensionMiddleware>();
        }
    }
}