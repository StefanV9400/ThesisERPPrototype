using System;
using System.ComponentModel;

namespace VERPS.WebApp.Areas.ExactOnline.Models.OrderManagement
{
    public class OrderLineVM
    {
        public int Id { get; set; }


        public ItemVM Item { get; set; }
        public string ItemID { get; set; }
      
        [DisplayName("Omschrijving")]
        public string Description { get; set; }

        [DisplayName("Aantal")]
        public int Quantity { get; set; }

        [DisplayName("Eenheid")]
        public string Unit { get; set; }

        [DisplayName("Netto")]
        public double? NetPrice { get; set; }

        [DisplayName("Ontvangen-Datum")]
        public DateTime? ReceiptDate { get; set; }

        [DisplayName("BTW")]
        public string VATCode { get; set; }

        [DisplayName("BTW %")]
        public double? VATPercentage { get; set; }

        [DisplayName("Totaal")]
        public double? AmountDC { get; set; }

        [DisplayName("Totaal BTW")]
        public double? VATAmount { get; set; }

        [DisplayName("Project")]
        public Guid? Project { get; set; }  
    }
}
