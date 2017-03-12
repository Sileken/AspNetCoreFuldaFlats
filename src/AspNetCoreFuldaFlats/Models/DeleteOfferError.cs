using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
