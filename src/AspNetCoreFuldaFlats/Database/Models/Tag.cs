namespace AspNetCoreFuldaFlats.Database.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public int? OfferId { get; set; }
        public virtual Offer Offer { get; set; }
        public string Title { get; set; }
    }
}
