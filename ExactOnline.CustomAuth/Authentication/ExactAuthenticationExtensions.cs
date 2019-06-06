using Microsoft.AspNetCore.Authentication;
using System;

namespace ExactOnline.CustomAuth.Authentication
{
    public static class ExactAuthenticationExtensions
    {
        public static AuthenticationBuilder AddCustomAuthentication(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<ExactAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<ExactAuthenticationOptions, ExactAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
