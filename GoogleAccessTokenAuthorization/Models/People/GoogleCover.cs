using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HXRd.Google.AccessTokenAuthorization.Models
{
    public class GoogleCover
    {
        [JsonProperty(PropertyName = "layout")]
        public string Layout { get; set; }
        [JsonProperty(PropertyName = "coverPhoto")]
        public GoogleCoverPhoto CoverPhoto { get; set; }
        [JsonProperty(PropertyName = "coverInfo")]
        public GoogleCoverInfo CoverInfo { get; set; }
    }
}
