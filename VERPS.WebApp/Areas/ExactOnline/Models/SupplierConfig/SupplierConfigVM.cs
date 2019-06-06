using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VERPS.WebApp.Models.General;

namespace VERPS.WebApp.Areas.ExactOnline.Models.SupplierConfig
{
    public class SupplierConfigVM
    {
        public int Id { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string VATNumber { get; set; }
        public bool IsSet { get; set; }
        public StateMessage StateMessage { get; set; }
    }
}
