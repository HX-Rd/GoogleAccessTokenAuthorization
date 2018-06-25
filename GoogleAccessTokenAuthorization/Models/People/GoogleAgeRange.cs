using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HXRd.Google.AccessTokenAuthorization.Models.People
{
    public class GoogleAgeRange
    {
        [JsonProperty(PropertyName = "min")]
        public int Min { get; set; }
        [JsonProperty(PropertyName = "max")]
        public int Max { get; set; }
    }
}
