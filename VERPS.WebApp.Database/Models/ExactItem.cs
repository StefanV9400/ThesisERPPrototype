using System;
namespace VERPS.WebApp.Database.Models
{
    public class ExactItem
    {
        public int Id { get; set; }

        // Exact DATA
        public Guid ExactID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public Guid ExactUserId { get; set; }
        public bool IsComplete { get; set; }
        public bool MessageSeen { get; set; }
        public Guid SupplierId { get; set; }
    }
}
