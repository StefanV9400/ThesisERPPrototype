namespace VERPS.WebApp.Models
{
    public class UserVM
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }

        public string UserName { get; set; }

        public int ConfigID { get; set; }
        public int DivisionId { get; set; }

        public string CreatorId { get; set; }
        public string ItemGroupId { get; set; }
        public string SupplierId { get; set; }

        public string Token { get; set; }

    }
}
