using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VERPS.WebApp.Areas.ExactOnline.Models.Dashboard
{
    public class ImportFileVM
    {
        [DisplayName("Bestand")]
        public IFormFile File { get; set; }
        public List<SelectListItem> Suppliers { get; set; }
        [DisplayName("Leverancier")]
        public Guid SupplierId { get; set; }
    }
}
