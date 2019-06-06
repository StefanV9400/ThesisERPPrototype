using System.ComponentModel.DataAnnotations;

namespace VERPS.WebApp.Database.Models
{
    public class Provider
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Colour { get; set; }
        public string Address { get; set; }
    }

}
