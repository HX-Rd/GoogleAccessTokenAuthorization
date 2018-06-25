using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace HXRd.Google.AccessTokenAuthorization
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddGoogleTokenAuthorization(this AuthenticationBuilder builder)
        {
            return AddGoogleTokenAuthorization(builder, GoogleAccessTokenDefaults.GOOGLE_TOKEN_SCHEME, null, o => { });
        }
        public static AuthenticationBuilder AddGoogleTokenAuthorization(this AuthenticationBuilder builder, Action<GoogleAccessTokenOptions> configurationOptions)
        {
            return AddGoogleTokenAuthorization(builder, GoogleAccessTokenDefaults.GOOGLE_TOKEN_SCHEME, null, configurationOptions);
        }

        public static AuthenticationBuilder AddGoogleTokenAuthorization(this AuthenticationBuilder builder, string authenticationScheme)
        {
            return AddGoogleTokenAuthorization(builder, authenticationScheme, null, o => { });
        }

        public static AuthenticationBuilder AddGoogleTokenAuthorization(this AuthenticationBuilder builder, string authenticationScheme, Action<GoogleAccessTokenOptions> configureOptions)
        {
            return AddGoogleTokenAuthorization(builder, authenticationScheme, null, configureOptions);
        }

        public static AuthenticationBuilder AddGoogleTokenAuthorization(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<GoogleAccessTokenOptions> configureOptions)
        {
            builder.Services.AddHttpClient<GoogleHttpClient>(client => client.BaseAddress = new Uri("https://www.googleapis.com"));
            var @return = null as AuthenticationBuilder;
            if (displayName == null)
                @return = builder.AddScheme<GoogleAccessTokenOptions, GoogleAccessTokenHandler>(authenticationScheme, configureOptions);
            @return = builder.AddScheme<GoogleAccessTokenOptions, GoogleAccessTokenHandler>(authenticationScheme, displayName, configureOptions);
            var provider = builder.Services.BuildServiceProvider();
            var options = provider.GetService<IOptionsMonitor<GoogleAccessTokenOptions>>().Get(GoogleAccessTokenDefaults.GOOGLE_TOKEN_SCHEME);
            if (options.UseMemoryCache) 
                builder.Services.AddSingleton<IAuthenticationTicketCache, AuthenticationTicketMemoryCache>();
            return @return;
        }
    }
}
