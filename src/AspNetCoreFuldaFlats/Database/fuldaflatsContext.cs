using AspNetCoreFuldaFlats.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreFuldaFlats.Database
{
    public class WebApiDataContext : DbContext
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
        {
        }
    }
}