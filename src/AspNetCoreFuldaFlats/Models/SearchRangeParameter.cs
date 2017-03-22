using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreFuldaFlats.Models
{
    public class SearchRangeParameter
    {
        public double? Lte { get; set; }
        public double? Gte { get; set; }
    }
}
