using System;
using System.ComponentModel.DataAnnotations;

namespace VERPS.WebApp.Database.Models
{
    public class ExactToken
    {
        [Required]
        public int Id { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTime { get; set; }
    }
}
