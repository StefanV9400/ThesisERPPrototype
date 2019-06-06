using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using VERPS.WebApp.Models.General;

namespace VERPS.WebApp.Areas.ExactOnline.Models.SupplierConfig
{
    public class OrderConfigVM
    {
        public int Id { get; set; }
        [DisplayName("Ontvangst Datum")]
        public DateTime Created { get; set; }
        [DisplayName("Omschrijving")]
        public string Description { get; set; }
        [DisplayName("Wisselkoers")]
        public string Currency { get; set; }
        [DisplayName("Project")]
        public Guid Project { get; set; }
        [DisplayName("Bestelnummer")]
        public int OrderNumber { get; set; }
        [DisplayName("Referentie")]
        public string YourRef { get; set; }
        [DisplayName("Document")]
        public Guid? Document { get; set; }
        [DisplayName("Bestel datum")]
        public DateTime OrderDate { get; set; }
        [DisplayName("Betalings Conditie")]
        public string PaymentConditionId { get; set; }
        public string PaymentConditionName { get; set; }
        [DisplayName("Inkoper")]
        public string CreatorId { get; set; }
        public string CreatorName { get; set; }
        public bool HasBeenSend { get; set; }
        public List<string> CreatedItems { get; set; }

        public bool IsStoredInExact { get; set; }
        public bool ItemsAreInExact { get; set; }

        public DateTime TimeSend { get; set; }
        public string SupplierName { get; set; }


        public StateMessage StateMessage { get; set; }


        public List<string> ListOfElements { get; set; }
    }
}
