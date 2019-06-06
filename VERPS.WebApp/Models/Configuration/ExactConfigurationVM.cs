using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VERPS.WebApp.Database.Enums;

namespace VERPS.WebApp.Models.Configuration
{
    public class ExactConfigurationVM
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SupplierId { get; set; }
        public Guid ItemGroupId { get; set; }
        public ConfigType ConfigType { get; set; }
        public string PaymentConditionId { get; set; }
    }
}
