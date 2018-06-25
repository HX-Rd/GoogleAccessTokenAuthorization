using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HXRd.Google.AccessTokenAuthorization.Models
{
    public class GoogleOrganizations
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "department")]
        public string Department { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "startDate")]
        public string StartDate { get; set; }
        [JsonProperty(PropertyName = "endDate")]
        public string EndDate { get; set; }
        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "primary")]
        public bool? Primary { get; set; }
    }
}
