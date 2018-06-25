using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HXRd.Google.AccessTokenAuthorization.Models
{
    public class GoogleCoverPhoto
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "height")]
        public int? Height { get; set; }
        [JsonProperty(PropertyName = "width")]
        public int? Width { get; set; }
    }
}
