using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AspNetCoreFuldaFlats.Models
{
    public class NormalUserUpdateError
    {
        [JsonIgnore]
        public bool HasError { get; set; } = false;
        public string[] FirstName { get; set; } = new string[1];
        public string[] LastName { get; set; } = new string[1];
        public string[] Gender { get; set; } = new string[1];
        public string[] Birthday { get; set; } = new string[1];
        public string[] Email { get; set; } = new string[1];
    }
}
