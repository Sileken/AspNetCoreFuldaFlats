using System;

namespace AspNetCoreFuldaFlats.Database.Models
{
    public partial class Mediaobject
    {
        public int Id { get; set; }
        public int? CreatedByUserId { get; set; }
        public virtual User CreatedByUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public string MainUrl { get; set; }
        public int? OfferId { get; set; }
        public virtual Offer Offer { get; set; }
        public string ThumbnailUrl { get; set; }
        public int? Type { get; set; }
        public int? UserId { get; set; }
    }
}
