using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HXRd.Google.AccessTokenAuthorization.Models
{
    public class GoogleCoverInfo
    {
        [JsonProperty(PropertyName = "topImageOffset")]
        public int? TopImageOffset { get; set; }
        [JsonProperty(PropertyName = "leftImageOffset")]
        public int? LeftImageOffset { get; set; }
    }
}
