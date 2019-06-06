using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VERPS.WebApp.Database.Models
{
    public class ExactOrder
    {
        [Required]
        public int Id { get; set; }

        public User DBUser { get; set; }
        public DateTime TimeSend { get; set; }
        public List<ExactOrderLine> Lines { get; set; }

        public bool IsStoredInExact { get; set; }
        public bool ItemsAreInExact { get; set; }

        // EXACT DATA
        public Guid ExactId { get; set; }
        [DisplayName("Omschrijving")]
        public string Description { get; set; }
        [DisplayName("Ontvangst Datum")]
        public DateTime Created { get; set; }
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
        public string PaymentCondition { get; set; }
        [DisplayName("Inkoper")]
        public Guid User { get; set; }
        [DisplayName("Inkoper")]
        public string CreatorId { get; set; }
        public ExactSupplier Supplier { get; set; }
        
    }
}
