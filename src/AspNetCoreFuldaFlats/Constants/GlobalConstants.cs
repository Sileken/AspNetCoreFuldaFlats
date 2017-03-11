using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Chunks;

namespace AspNetCoreFuldaFlats.Constants
{
    public static class GlobalConstants
    {
        public static class Routes
        {
            public static string RelativeUrlPath = "/api";
        }

        public static string IdentityAuthenticationSchema = "Identity";
        public static string CookieAuthenticationSchema = "Cookie";
    }
}
