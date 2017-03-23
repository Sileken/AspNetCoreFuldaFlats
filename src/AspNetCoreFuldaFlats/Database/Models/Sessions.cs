using System.ComponentModel.DataAnnotations;

namespace AspNetCoreFuldaFlats.Database.Models
{
    public class Sessions
    {
        [Key]
        public string SessionId { get; set; }

        public string Data { get; set; }
        public int Expires { get; set; }
    }
}
