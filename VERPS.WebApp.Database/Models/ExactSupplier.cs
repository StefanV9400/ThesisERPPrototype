using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VERPS.WebApp.Database.Models
{
    public class ExactSupplier
    {
        [Required]
        public int Id { get; set; }
        public Guid ExactId { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Zipcode { get; set; }
        public string VATNumber { get; set; }
        public string Website { get; set; }
        public Guid ExactUser { get; set; }
    }
}
