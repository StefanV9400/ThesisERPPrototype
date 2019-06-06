using VERPS.WebApp.Models.General;

namespace VERPS.WebApp.Areas.ExactOnline.Models.Dashboard
{
    public class IndexVM
    {
        public string CompanyName { get; set; }
        public int AmountOpenOrders { get; set; }
        public int AmountSentOrders { get; set; }
        public StateMessage StateMessage { get; set; }
        public ImportFileVM ImportFileVM { get; set; }
    }
}
