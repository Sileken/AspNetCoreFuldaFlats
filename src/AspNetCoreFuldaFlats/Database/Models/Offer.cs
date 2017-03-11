using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace AspNetCoreFuldaFlats.Database.Models
{
    public partial class Offer
    {
        private string _thumbnailUrl = string.Empty;

        [Key]
        public int Id { get; set; }
        public bool? Accessability { get; set; }
        public string BathroomDescription { get; set; }
        public int? BathroomNumber { get; set; }
        public bool? Cellar { get; set; }
        public string City { get; set; }
        public double? Commission { get; set; }
        public DateTime? CreationDate { get; set; }
        public double? Deposit { get; set; }
        public string Description { get; set; }
        public bool? Dryer { get; set; }
        public bool? Elevator { get; set; }
        public int? Floor { get; set; }
        public double? FullPrice { get; set; }
        public bool? Furnished { get; set; }
        public string HeatingDescription { get; set; }
        public int? HouseNumber { get; set; }
        public int? InternetSpeed { get; set; }
        public string KitchenDescription { get; set; }
        public bool? Lan { get; set; }

        [JsonIgnore]
        public int? Landlord { get; set; }

        [JsonProperty("Landlord")]
        [ForeignKey("Landlord")]
        public virtual User DatabaseLandlord { get; set; }

        public DateTime? LastModified { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string OfferType { get; set; }
        public bool? Parking { get; set; }
        public bool? Pets { get; set; }
        public string PriceType { get; set; }
        public int? Rent { get; set; }
        public string RentType { get; set; }
        public int? Rooms { get; set; }
        public int? SideCosts { get; set; }
        public double? Size { get; set; }
        public int? Status { get; set; }
        public string Street { get; set; }
        public bool? Telephone { get; set; }
        public string Television { get; set; }
        public string Title { get; set; }
        public double? UniDistance { get; set; }
        public bool? WashingMachine { get; set; }
        public bool? Wlan { get; set; }
        public string ZipCode { get; set; }

        [NotMapped]
        public string ThumbnailUrl
        {
            get { return string.IsNullOrWhiteSpace(_thumbnailUrl) ? Mediaobjects.Count > 0 ? Mediaobjects.First().ThumbnailUrl : "/uploads/dummy.png" : _thumbnailUrl; }
            set { _thumbnailUrl = value; }
        }
        
        [InverseProperty("Offer")]
        public virtual ICollection<Mediaobject> Mediaobjects { get; set; }

        [InverseProperty("Offer")]
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
