﻿namespace AspNetCoreFuldaFlats.Database.Models
{
    public partial class Tag
    {
        public int Id { get; set; }
        public int? OfferId { get; set; }
        public string Title { get; set; }
    }
}
