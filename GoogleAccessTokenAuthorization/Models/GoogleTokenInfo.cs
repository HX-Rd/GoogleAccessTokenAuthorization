using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HXRd.Google.AccessTokenAuthorization.Models
{
    public class GoogleTokenInfo
    {
        [JsonProperty(PropertyName = "azp")]
        public string Azp { get; set; }
        [JsonProperty(PropertyName = "aud")]
        public string Aud { get; set; }
        [JsonProperty(PropertyName = "sub")]
        public string Sub { get; set; }
        [JsonProperty(PropertyName = "scope")]
        public string Scopes { get; set; }
        [JsonProperty(PropertyName = "exp")]
        public int ExpiresInSeconds { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "email_verified")]
        public bool EmailVerified { get; set; }
        [JsonProperty(PropertyName = "access_type")]
        public string AccessType { get; set; }
    }
}
