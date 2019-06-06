using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VERPS.WebApp.Database.Models;

namespace VERPS.WebApp.Areas.ExactOnline.Models.SupplierConfig
{
    public class IndexVM
    {
        public string UserID { get; set; }
        public List<SelectListItem> SuppliersSelectList { get; set; }
        public List<ExactSupplierConfig> ExactSupplierConfigs { get; set; }
    }
}
