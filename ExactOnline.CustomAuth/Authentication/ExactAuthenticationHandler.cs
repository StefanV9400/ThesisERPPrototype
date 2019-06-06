using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ExactOnline.CustomAuth.Authentication
{
    public class ExactAuthenticationHandler : AuthenticationHandler<ExactAuthenticationOptions>
    {
        public ExactAuthenticationHandler(IOptionsMonitor<ExactAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock) {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            return Task.FromResult(
                 AuthenticateResult.Success(
                    new AuthenticationTicket(
                        new ClaimsPrincipal(Options.Identity),
                        new AuthenticationProperties(),
                        this.Scheme.Name)));
        }
    }
}
