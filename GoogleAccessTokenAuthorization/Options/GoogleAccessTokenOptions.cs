using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace HXRd.Google.AccessTokenAuthorization
{
    public class GoogleAccessTokenOptions : AuthenticationSchemeOptions
    {
        public bool UseMemoryCache { get; set; } = false;
        public int ExpieryWindowSeconds { get; set; } = 10;
        public bool RequireCorrectClientId { get; set; } = false;
        public string ClientId { get; set; }
        public List<string> RequiredScopes { get; set; }
        public bool RequireVerifiedEmail { get; set; } = true;
        public bool OnlyAllowOfflineTokens { get; set; } = false;
    }
}
