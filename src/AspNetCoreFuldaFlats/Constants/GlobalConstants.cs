namespace AspNetCoreFuldaFlats.Constants
{
    public static class GlobalConstants
    {
        public static class Routes
        {
            public static readonly string RelativeUrlPath = "/api";
        }

        public static readonly string IdentityAuthenticationSchema = "Identity";
        public static readonly string CookieAuthenticationSchema = "Cookie";

        public enum OfferStatus
        {
            InCreation = 0,
            Active = 1,
            Inactive = 0
        }

        public static readonly string SearchParamtersSessionkey = "LastSearchParamters";

        public static string DefaultThumbnailUrl { get; set; }
    }
}
