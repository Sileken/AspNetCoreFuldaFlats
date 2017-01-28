using AspNetCoreFuldaFlats.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreFuldaFlats.Database
{
    public partial class WebApiDataContext : DbContext
    { 
        public virtual DbSet<Favorite> Favorite { get; set; }
        public virtual DbSet<Mediaobject> Mediaobject { get; set; }
        public virtual DbSet<Offer> Offer { get; set; }
        public virtual DbSet<Review> Review { get; set; }
        public virtual DbSet<Sessions> Sessions { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<User> User { get; set; }

        public WebApiDataContext(DbContextOptions<WebApiDataContext> options)
            : base(options)
        {}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>()
            //    .HasMany(c => c.Favorites);

            //modelBuilder.Entity<Favorite>().HasOne(c => c.Offer);

            //    modelBuilder.Entity<Favorite>(entity =>
            //    {
            //        entity.ToTable("favorite");

            //        entity.Property(e => e.Id)
            //            .HasColumnName("id")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.OfferId)
            //            .HasColumnName("offerId")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.UserId)
            //            .HasColumnName("userId")
            //            .HasColumnType("int(11)");
            //    });

            //    modelBuilder.Entity<Mediaobject>(entity =>
            //    {
            //        entity.ToTable("mediaobject");

            //        entity.Property(e => e.Id)
            //            .HasColumnName("id")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.CreatedByUserId)
            //            .HasColumnName("createdByUserId")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.CreationDate)
            //            .HasColumnName("creationDate")
            //            .HasColumnType("datetime");

            //        entity.Property(e => e.MainUrl)
            //            .HasColumnName("mainUrl")
            //            .HasColumnType("varchar(255)");

            //        entity.Property(e => e.OfferId)
            //            .HasColumnName("offerId")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.ThumbnailUrl)
            //            .HasColumnName("thumbnailUrl")
            //            .HasColumnType("varchar(255)");

            //        entity.Property(e => e.Type)
            //            .HasColumnName("type")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.UserId)
            //            .HasColumnName("userId")
            //            .HasColumnType("int(11)");
            //    });

            //    modelBuilder.Entity<Offer>(entity =>
            //    {
            //        entity.ToTable("offer");

            //        entity.Property(e => e.Id)
            //            .HasColumnName("id")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.Accessability)
            //            .HasColumnName("accessability")
            //            .HasColumnType("tinyint(1)");

            //        entity.Property(e => e.BathroomDescription)
            //            .HasColumnName("bathroomDescription")
            //            .HasColumnType("varchar(255)");

            //        entity.Property(e => e.BathroomNumber)
            //            .HasColumnName("bathroomNumber")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.Cellar)
            //            .HasColumnName("cellar")
            //            .HasColumnType("tinyint(1)");

            //        entity.Property(e => e.City)
            //            .HasColumnName("city")
            //            .HasColumnType("varchar(255)");

            //        entity.Property(e => e.Commission).HasColumnName("commission");

            //        entity.Property(e => e.CreationDate)
            //            .HasColumnName("creationDate")
            //            .HasColumnType("datetime");

            //        entity.Property(e => e.Deposit).HasColumnName("deposit");

            //        entity.Property(e => e.Description)
            //            .HasColumnName("description")
            //            .HasColumnType("text");

            //        entity.Property(e => e.Dryer)
            //            .HasColumnName("dryer")
            //            .HasColumnType("tinyint(1)");

            //        entity.Property(e => e.Elevator)
            //            .HasColumnName("elevator")
            //            .HasColumnType("tinyint(1)");

            //        entity.Property(e => e.Floor)
            //            .HasColumnName("floor")
            //            .HasColumnType("int(2)");

            //        entity.Property(e => e.FullPrice).HasColumnName("fullPrice");

            //        entity.Property(e => e.Furnished)
            //            .HasColumnName("furnished")
            //            .HasColumnType("tinyint(1)");

            //        entity.Property(e => e.HeatingDescription)
            //            .HasColumnName("heatingDescription")
            //            .HasColumnType("varchar(255)");

            //        entity.Property(e => e.HouseNumber)
            //            .HasColumnName("houseNumber")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.InternetSpeed)
            //            .HasColumnName("internetSpeed")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.KitchenDescription)
            //            .HasColumnName("kitchenDescription")
            //            .HasColumnType("varchar(255)");

            //        entity.Property(e => e.Lan)
            //            .HasColumnName("lan")
            //            .HasColumnType("tinyint(1)");

            //        entity.Property(e => e.Landlord)
            //            .HasColumnName("landlord")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.LastModified)
            //            .HasColumnName("lastModified")
            //            .HasColumnType("datetime");

            //        entity.Property(e => e.Latitude).HasColumnName("latitude");

            //        entity.Property(e => e.Longitude).HasColumnName("longitude");

            //        entity.Property(e => e.OfferType)
            //            .HasColumnName("offerType")
            //            .HasColumnType("varchar(12)");

            //        entity.Property(e => e.Parking)
            //            .HasColumnName("parking")
            //            .HasColumnType("tinyint(1)");

            //        entity.Property(e => e.Pets)
            //            .HasColumnName("pets")
            //            .HasColumnType("tinyint(1)");

            //        entity.Property(e => e.PriceType)
            //            .HasColumnName("priceType")
            //            .HasColumnType("varchar(8)");

            //        entity.Property(e => e.Rent)
            //            .HasColumnName("rent")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.RentType)
            //            .HasColumnName("rentType")
            //            .HasColumnType("varchar(4)");

            //        entity.Property(e => e.Rooms)
            //            .HasColumnName("rooms")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.SideCosts)
            //            .HasColumnName("sideCosts")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.Size).HasColumnName("size");

            //        entity.Property(e => e.Status)
            //            .HasColumnName("status")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.Street)
            //            .HasColumnName("street")
            //            .HasColumnType("varchar(255)");

            //        entity.Property(e => e.Telephone)
            //            .HasColumnName("telephone")
            //            .HasColumnType("tinyint(1)");

            //        entity.Property(e => e.Television)
            //            .HasColumnName("television")
            //            .HasColumnType("varchar(255)");

            //        entity.Property(e => e.Title)
            //            .HasColumnName("title")
            //            .HasColumnType("varchar(255)");

            //        entity.Property(e => e.UniDistance).HasColumnName("uniDistance");

            //        entity.Property(e => e.WashingMachine)
            //            .HasColumnName("washingMachine")
            //            .HasColumnType("tinyint(1)");

            //        entity.Property(e => e.Wlan)
            //            .HasColumnName("wlan")
            //            .HasColumnType("tinyint(1)");

            //        entity.Property(e => e.ZipCode)
            //            .HasColumnName("zipCode")
            //            .HasColumnType("varchar(5)");
            //    });

            //    modelBuilder.Entity<Review>(entity =>
            //    {
            //        entity.ToTable("review");

            //        entity.Property(e => e.Id)
            //            .HasColumnName("id")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.Comment)
            //            .HasColumnName("comment")
            //            .HasColumnType("text");

            //        entity.Property(e => e.CreationDate)
            //            .HasColumnName("creationDate")
            //            .HasColumnType("datetime");

            //        entity.Property(e => e.OfferId)
            //            .HasColumnName("offerId")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.Rating)
            //            .HasColumnName("rating")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.Title)
            //            .HasColumnName("title")
            //            .HasColumnType("varchar(255)");

            //        entity.Property(e => e.UserId)
            //            .HasColumnName("userId")
            //            .HasColumnType("int(11)");
            //    });

            //    modelBuilder.Entity<Sessions>(entity =>
            //    {
            //        entity.HasKey(e => e.SessionId)
            //            .HasName("PK_sessions");

            //        entity.ToTable("sessions");

            //        entity.Property(e => e.SessionId)
            //            .HasColumnName("session_id")
            //            .HasColumnType("varchar(128)");

            //        entity.Property(e => e.Data)
            //            .HasColumnName("data")
            //            .HasColumnType("text");

            //        entity.Property(e => e.Expires)
            //            .HasColumnName("expires")
            //            .HasColumnType("int(11) unsigned");
            //    });

            //    modelBuilder.Entity<Tag>(entity =>
            //    {
            //        entity.ToTable("tag");

            //        entity.Property(e => e.Id)
            //            .HasColumnName("id")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.OfferId)
            //            .HasColumnName("offerId")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.Title)
            //            .HasColumnName("title")
            //            .HasColumnType("varchar(255)");
            //    });

            //    modelBuilder.Entity<User>(entity =>
            //    {
            //        entity.ToTable("user");

            //        entity.HasIndex(e => e.Email)
            //            .HasName("email");

            //        entity.Property(e => e.Id)
            //            .HasColumnName("id")
            //            .HasColumnType("int(11)");

            //        entity.Property(e => e.AverageRating).HasColumnName("averageRating");

            //        entity.Property(e => e.Birthday)
            //            .HasColumnName("birthday")
            //            .HasColumnType("datetime");

            //        entity.Property(e => e.City)
            //            .HasColumnName("city")
            //            .HasColumnType("varchar(255)");

            //        entity.Property(e => e.CreationDate)
            //            .HasColumnName("creationDate")
            //            .HasColumnType("datetime");

            //        entity.Property(e => e.Email)
            //            .HasColumnName("email")
            //            .HasColumnType("varchar(255)");

            //        entity.Property(e => e.FirstName)
            //            .HasColumnName("firstName")
            //            .HasColumnType("varchar(255)");

            //        entity.Property(e => e.Gender)
            //            .HasColumnName("gender")
            //            .HasColumnType("varchar(6)");

            //        entity.Property(e => e.HouseNumber)
            //            .HasColumnName("houseNumber")
            //            .HasColumnType("varchar(5)");

            //        entity.Property(e => e.IsLocked)
            //            .HasColumnName("isLocked")
            //            .HasColumnType("tinyint(1)");

            //        entity.Property(e => e.LastName)
            //            .HasColumnName("lastName")
            //            .HasColumnType("varchar(255)");

            //        entity.Property(e => e.LoginAttempts)
            //            .HasColumnName("loginAttempts")
            //            .HasColumnType("int(10)");

            //        entity.Property(e => e.OfficeAddress)
            //            .HasColumnName("officeAddress")
            //            .HasColumnType("varchar(4000)");

            //        entity.Property(e => e.Password)
            //            .HasColumnName("password")
            //            .HasColumnType("varchar(255)");

            //        entity.Property(e => e.PhoneNumber)
            //            .HasColumnName("phoneNumber")
            //            .HasColumnType("varchar(255)");

            //        entity.Property(e => e.ProfilePicture)
            //            .IsRequired()
            //            .HasColumnName("profilePicture")
            //            .HasColumnType("varchar(255)")
            //            .HasDefaultValueSql("/uploads/cupcake.png");

            //        entity.Property(e => e.Street)
            //            .HasColumnName("street")
            //            .HasColumnType("varchar(255)");

            //        entity.Property(e => e.Type)
            //            .HasColumnName("type")
            //            .HasColumnType("int(1)");

            //        entity.Property(e => e.UpgradeDate)
            //            .HasColumnName("upgradeDate")
            //            .HasColumnType("datetime");

            //        entity.Property(e => e.ZipCode)
            //            .HasColumnName("zipCode")
            //            .HasColumnType("varchar(5)");
            //    });
        }
    }
}