﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AspNetCoreFuldaFlats.Models
{
    public class UpgradeError
    {
        [JsonIgnore]
        public bool HasError { get; set; } = false;
        public string[] Upgrade { get; set; } = new string[1];
        public string[] PhoneNumber { get; set; } = new string[1];
        public string[] ZipCode { get; set; } = new string[1];
        public string[] City { get; set; } = new string[1];
        public string[] Street { get; set; } = new string[1];
        public string[] HouseNumber { get; set; } = new string[1];
        public string[] Email { get; set; } = new string[1];
    }
}
