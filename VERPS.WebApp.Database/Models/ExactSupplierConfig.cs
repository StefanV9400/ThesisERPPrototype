using System;
using System.ComponentModel.DataAnnotations;

namespace VERPS.WebApp.Database.Models
{
    public class ExactSupplierConfig
    {
        [Required]
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public ExactSupplier Supplier { get; set; }
        public bool IsSet { get; set; }
        public User User { get; set; }
    }
}
