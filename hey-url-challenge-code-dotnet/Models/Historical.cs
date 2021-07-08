using System;
using System.ComponentModel.DataAnnotations;

namespace hey_url_challenge_code_dotnet.Models
{
    public class Historical
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string BrowserName { get; set; }

        [Required]
        [StringLength(50)]
        public string OS { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Url Url { get; set; }
    }
}
