using System.Collections.Generic;
using Newtonsoft.Json;

namespace AspNetCoreFuldaFlats.Models
{
    public class ChangePasswordError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Password { get; set; }
    }
}
