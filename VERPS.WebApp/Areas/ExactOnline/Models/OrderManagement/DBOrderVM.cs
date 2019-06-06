using System;
namespace VERPS.WebApp.Areas.ExactOnline.Models.OrderManagement
{
    public class DBOrderVM
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public int AmountofLines { get; set; }
        public double Total { get; set; }
        public string SupplierName { get; set; }
    }
}
