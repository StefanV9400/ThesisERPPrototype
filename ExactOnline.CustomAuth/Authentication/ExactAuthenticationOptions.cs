using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;

namespace ExactOnline.CustomAuth.Authentication
{
    public class ExactAuthenticationOptions : AuthenticationSchemeOptions
    {
        public ClaimsIdentity Identity { get; set; }
        // Other authentication options propters
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
