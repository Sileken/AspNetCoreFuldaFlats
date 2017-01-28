using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace AspNetCoreFuldaFlats.Database.Models
{
    public partial class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [ForeignKey("OfferId")]
        public virtual Offer Offer { get; set; }
    }
}
