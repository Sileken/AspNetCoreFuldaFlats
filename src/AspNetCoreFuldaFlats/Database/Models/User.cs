using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace AspNetCoreFuldaFlats.Database.Models
{
    public class User
    {
        private string _profileImage = string.Empty;
        [Key]
        public int Id { get; set; }
        public double? AverageRating { get; set; }
        public DateTime? Birthday { get; set; }
        public string City { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public string HouseNumber { get; set; }
        public string LastName { get; set; }
        public string OfficeAddress { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePicture
        {
            get { return string.IsNullOrWhiteSpace(_profileImage) ? "/uploads/cupcake.png" : _profileImage; }
            set { _profileImage = value; }
        }
        public string Street { get; set; }
        public int? Type { get; set; }
        public DateTime? UpgradeDate { get; set; }
        public string ZipCode { get; set; }
        [JsonIgnore]
        public sbyte? IsLocked { get; set; }
        [JsonIgnore]
        public int? LoginAttempts { get; set; }
        [JsonIgnore]
        [Column("Favorites")]
        public virtual ICollection<Favorite> DatabaseFavorites { get; set; }
        [NotMapped]
        public ICollection<Offer> Favorites
            =>
            (DatabaseFavorites != null) && (DatabaseFavorites.Count > 0)
                ? DatabaseFavorites.Where(d => d.Offer != null).Select(d => d.Offer).ToArray()
                : new Offer[0];
    }
}