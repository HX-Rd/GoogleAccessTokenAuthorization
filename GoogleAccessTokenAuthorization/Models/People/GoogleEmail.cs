using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HXRd.Google.AccessTokenAuthorization.Models
{
    public class GoogleEmail
    {
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}
