using System;
using System.ComponentModel.DataAnnotations;
using VERPS.WebApp.Database.Enums;

namespace VERPS.WebApp.Database.Models
{
    public class ExactConfiguration
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string SupplierId { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }
        public int DivsionId { get; set; }
        public string PaymentConditionId { get; set; }
        public Guid BuyerId { get; set; }
        public Guid ItemGroupId { get; set; }
        public ConfigType ConfigType { get; set; }
        public string ItemCodePreset { get; set; }
    }
}
