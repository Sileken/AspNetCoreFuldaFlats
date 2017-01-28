using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreFuldaFlats.Database.Models
{
    public partial class Sessions
    {
        [Key]
        public string SessionId { get; set; }
        public string Data { get; set; }
        public int Expires { get; set; }
    }
}
