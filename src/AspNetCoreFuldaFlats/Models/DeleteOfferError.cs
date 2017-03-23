using System.Collections.Generic;
using Newtonsoft.Json;

namespace AspNetCoreFuldaFlats.Models
{
    public class DeleteOfferError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Offer { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Auth { get; set; }
    }
}
