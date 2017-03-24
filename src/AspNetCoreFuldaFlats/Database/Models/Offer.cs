using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using AspNetCoreFuldaFlats.AuthorizedJsonSerialization;
using AspNetCoreFuldaFlats.Constants;
using AspNetCoreFuldaFlats.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AspNetCoreFuldaFlats.Database.Models
{
    public class Offer
    {
        private string _thumbnailUrl = string.Empty;

        [Key]
        public int Id { get; set; }

        [JsonAuthorized]
        public bool? Accessability { get; set; }

        [JsonAuthorized]
        public string BathroomDescription { get; set; }

        [JsonAuthorized]
        public int? BathroomNumber { get; set; }

        [JsonAuthorized]
        public bool? Cellar { get; set; }

        [JsonAuthorized]
        public string City { get; set; }

        public double? Commission { get; set; }
        public DateTime? CreationDate { get; set; }
        public double? Deposit { get; set; }

        [JsonAuthorized]
        public string Description { get; set; }

        [JsonAuthorized]
        public bool? Dryer { get; set; }

        [JsonAuthorized]
        public bool? Elevator { get; set; }

        [JsonAuthorized]
        public int? Floor { get; set; }

        public double? FullPrice { get; set; }

        [JsonAuthorized]
        public bool? Furnished { get; set; }

        [JsonAuthorized]
        public string HeatingDescription { get; set; }

        [JsonAuthorized]
        public int? HouseNumber { get; set; }

        [JsonAuthorized]
        public int? InternetSpeed { get; set; }

        [JsonAuthorized]
        public string KitchenDescription { get; set; }

        [JsonAuthorized]
        public bool? Lan { get; set; }

        [JsonIgnore]
        public int? Landlord { get; set; }

        [JsonAuthorized]
        [JsonProperty("landlord")]
        [ForeignKey("Landlord")]
        public virtual User DatabaseLandlord { get; set; }

        public DateTime? LastModified { get; set; }

        [JsonAuthorized]
        public double? Latitude { get; set; }

        [JsonAuthorized]
        public double? Longitude { get; set; }

        public string OfferType { get; set; }

        [JsonAuthorized]
        public bool? Parking { get; set; }

        [JsonAuthorized]
        public bool? Pets { get; set; }

        public string PriceType { get; set; }
        public int? Rent { get; set; }
        public string RentType { get; set; }
        public int? Rooms { get; set; }
        public int? SideCosts { get; set; }
        public double? Size { get; set; }
        public int? Status { get; set; }

        [JsonAuthorized]
        public string Street { get; set; }

        [JsonAuthorized]
        public bool? Telephone { get; set; }

        [JsonAuthorized]
        public string Television { get; set; }

        public string Title { get; set; }
        public double? UniDistance { get; set; }

        [JsonAuthorized]
        public bool? WashingMachine { get; set; }

        [JsonAuthorized]
        public bool? Wlan { get; set; }

        [JsonAuthorized]
        public string ZipCode { get; set; }

        [NotMapped]
        public string ThumbnailUrl
        {
            get
            {
                return string.IsNullOrWhiteSpace(_thumbnailUrl)
                    ? MediaObjects?.Count > 0 ? MediaObjects.First().ThumbnailUrl : GlobalConstants.DefaultThumbnailUrl
                    : _thumbnailUrl;
            }
            set { _thumbnailUrl = value; }
        }

        [InverseProperty("Offer")]
        public virtual ICollection<Mediaobject> MediaObjects { get; set; }

        [InverseProperty("Offer")]
        public virtual ICollection<Tag> Tags { get; set; }

        [NotMapped]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public  List<Favorite> Favorite { get; set; }
    }
}