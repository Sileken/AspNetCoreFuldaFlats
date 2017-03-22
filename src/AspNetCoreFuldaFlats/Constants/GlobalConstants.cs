using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoCoordinatePortable;
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

        public enum OfferStatus
        {
            InCreation = 0,
            Active = 1,
            Inactive = 0
        }

        public static string OpenStreetMapSearchApi = "http://nominatim.openstreetmap.org/search";
        public static GeoCoordinate HsFuldaCoordinate = new GeoCoordinate(50.5648258, 9.6842798);

        public static string DefaultThumbnailUrl = "/uploads/dummy.png";

        public static string SearchParamtersSessionkey = "LastSearchParamters";
    }
}
