using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VERPS.WebApp.Database.Enums;
using VERPS.WebApp.Models.General;

namespace VERPS.WebApp.Areas.ExactOnline.Models.Configuration
{
    public class ConfigVM
    {
        public int ConfigurationId { get; set; }

        public string UserId { get; set; }
        [Display(Name = "Leveranciers")]
        public string SupplierId { get; set; }
        public List<SelectListItem> Suppliers { get; set; }
        [Display(Name = "Administraties")]
        public string DivisionId { get; set; }
        public List<SelectListItem> Divisions { get; set; }
        [Display(Name = "Productgroepen")]
        public string ItemGroupId { get; set; }
        public List<SelectListItem> ItemGroups { get; set; }
        [Display(Name = "Betalingscondities")]
        public string PaymentConditionId { get; set; }
        public List<SelectListItem> PaymentConditions { get; set; }
        [Display(Name = "Opslaan methode")]
        public ConfigType ConfigType { get; set; }
        public StateMessage StateMessage { get; set; }
    }
}
