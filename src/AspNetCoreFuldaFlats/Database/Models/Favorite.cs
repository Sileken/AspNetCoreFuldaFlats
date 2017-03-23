using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCoreFuldaFlats.Database.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int OfferId { get; set; }

        [ForeignKey("OfferId")]
        public virtual Offer Offer { get; set; }
    }
}
