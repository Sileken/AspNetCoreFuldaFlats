using System;

namespace AspNetCoreFuldaFlats.Database.Models
{
    public partial class Review
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? OfferId { get; set; }
        public int? Rating { get; set; }
        public string Title { get; set; }
        public int? UserId { get; set; }
    }
}
