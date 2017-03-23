using System.Collections.Generic;

namespace AspNetCoreFuldaFlats.Models
{
    public class SearchParamaters
    {
        public string OfferType { get; set; }
        public SearchRangeParameter UniDistance { get; set; }
        public SearchRangeParameter Rent { get; set; }
        public SearchRangeParameter Size { get; set; }
        public ICollection<string> Tags { get; set; }

        //Extended Search Parameter
        public SearchRangeParameter Rooms { get; set; }
        public bool Furnished { get; set; }
        public bool Pets { get; set; }
        public bool Cellar { get; set; }
        public bool Parking { get; set; }
        public bool Elevator { get; set; }
        public bool Accessibility { get; set; }
        public bool Dryer { get; set; }
        public bool Washingmachine { get; set; }
        public bool Television { get; set; }
        public bool Wlan { get; set; }
        public bool Lan { get; set; }
        public bool Telephone { get; set; }
        public SearchRangeParameter Internetspeed { get; set; }
    }
}