using Microsoft.AspNetCore.Identity;
using System;

namespace VERPS.WebApp.Database.Models
{
    public class User : IdentityUser
    {
        public string CompanyName { get; set; }
        public string Token { get; set; }
        public Enums.ProviderEnum Provider { get; set; }
        public ExactConfiguration ExactConfiguration { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }
        public string UserID { get; set; }
        public bool HasConfig { get; set; }
        public bool IsExact { get; set; }
        public ExactToken ExactToken { get; set; }
    }
}
