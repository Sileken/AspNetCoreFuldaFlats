using System.Collections.Generic;
using Newtonsoft.Json;

namespace AspNetCoreFuldaFlats.Models
{
    public class SignUpError : NormalUserUpdateError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Password { get; set; }
    }
}