using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace VERPS.WebApp.Areas.ExactOnline.Models.Import
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
