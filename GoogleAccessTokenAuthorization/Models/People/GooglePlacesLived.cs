using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HXRd.Google.AccessTokenAuthorization.Models
{
    public class GooglePlacesLived
    {
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
        [JsonProperty(PropertyName = "primary")]
        public bool? Primary { get; set; }
    }
}
