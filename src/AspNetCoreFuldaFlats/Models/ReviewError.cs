﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AspNetCoreFuldaFlats.Models
{
    public class ReviewError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Title { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Rating { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> OfferType { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> OwnOffer { get; set; }
    }
}