using System;
namespace VERPS.WebApp.Areas.ExactOnline.Models.OrderManagement
{
    public class ItemVM
    {
        public int Id { get; set; }
        public Guid ExactId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
        public bool MessageSeen { get; set; }
    }
}
