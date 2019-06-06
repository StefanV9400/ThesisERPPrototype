using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VERPS.WebApp.Database.Models;
using VERPS.WebApp.Models.General;

namespace VERPS.WebApp.Areas.ExactOnline.Models.OrderManagement
{
    public class IndexVM
    {
        public string UserID { get; set; }
        public List<DBOrderVM> Orders { get; set; }
        public StateMessage StateMessage { get; set; }
        public bool IsOpenOrderPage { get; set; }
    }
}
