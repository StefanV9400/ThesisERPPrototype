using System;
using System.Collections.Generic;
using ExactOnline.Models.Logistics;
using Microsoft.AspNetCore.Mvc.Rendering;
using VERPS.WebApp.Models.General;

namespace VERPS.WebApp.Areas.ExactOnline.Models.OrderManagement
{
    public class ItemsToBeCreatedVM
    {
        public StateMessage StateMessage { get; set; }
        public string SupplierName { get; set; }
        public int OrderId { get; set; }
        public List<Item> Items { get; set; }
        public Guid ItemGroupId { get; set; }
        public List<SelectListItem> ItemGroups { get; set; }
    }
}
