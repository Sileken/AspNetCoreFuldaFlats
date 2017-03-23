using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoCoordinatePortable;

namespace AspNetCoreFuldaFlats.Models
{
    public class AppSettings
    {
        public string AzureMySqlConnectionString { get; set; }
        public string DefaultMySqlConnectionString { get; set; }
        
        public string OpenStreetMapSearchApi { get; set; }

        public GeoCoordinate HsFuldaCoordinate { get; set; }

        public string DefaultThumbnailUrl { get; set; }

        public List<string> SupportedTags { get; set; }

        public string DefaultUserProfilePicture { get; set; }
        public string PasswordSalt { get; set; }
        public int MaxSignInAttempts { get; set; }
        public int MinPasswordLength { get; set; }
    }
}
