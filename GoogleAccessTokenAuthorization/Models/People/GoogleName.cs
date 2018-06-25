using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HXRd.Google.AccessTokenAuthorization.Models
{
    public class GoogleName
    {
        [JsonProperty(PropertyName = "formatted")]
        public string Formatted { get; set; }
        [JsonProperty(PropertyName = "familyName")]
        public string FamilyName { get; set; }
        [JsonProperty(PropertyName = "givenName")]
        public string GivenName { get; set; }
        [JsonProperty(PropertyName = "middleName")]
        public string MiddleName { get; set; }
        [JsonProperty(PropertyName = "honorificPrefix")]
        public string HonorificPrefix { get; set; }
        [JsonProperty(PropertyName = "honorificSuffix")]
        public string HonorificSuffix { get; set; }
    }
}
