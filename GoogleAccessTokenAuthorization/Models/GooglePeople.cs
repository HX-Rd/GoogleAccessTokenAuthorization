using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HXRd.Google.AccessTokenAuthorization.Models
{
    public class GooglePeople
    {
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }
        [JsonProperty(PropertyName = "etag")]
        public string Etag { get; set; }
        [JsonProperty(PropertyName = "objectType")]
        public string ObjectType { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "isPlusUser")]
        public bool? IsPlushUser { get; set; }
        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }
        [JsonProperty(PropertyName = "circledByCount")]
        public int? CircledByCount { get; set; }
        [JsonProperty(PropertyName = "nickname")]
        public string Nickname { get; set; }
        [JsonProperty(PropertyName = "occupation")]
        public string Occupation { get; set; }
        [JsonProperty(PropertyName = "skills")]
        public string Skills { get; set; }
        [JsonProperty(PropertyName = "birthday")]
        public string Birthday { get; set; }
        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }
        [JsonProperty(PropertyName = "tagline")]
        public string Tagline { get; set; }
        [JsonProperty(PropertyName = "braggingRights")]
        public string BraggingRights { get; set; }
        [JsonProperty(PropertyName = "aboutMe")]
        public string AboutMe { get; set; }
        [JsonProperty(PropertyName = "relationshipStatus")]
        public string RelationshipStatus { get; set; }
        [JsonProperty(PropertyName = "plusOneCount")]
        public int? PlusOneCount { get; set; }
        [JsonProperty(PropertyName = "verified")]
        public bool? Verified { get; set; }
        [JsonProperty(PropertyName = "domain")]
        public string Domain { get; set; }
        [JsonProperty(PropertyName = "emails")]
        public List<GoogleEmail> Emails { get; set; }
        [JsonProperty(PropertyName = "urls")]
        public List<GoogleUrl> Urls { get; set; }
        [JsonProperty(PropertyName = "name")]
        public GoogleName Name { get; set; }
        [JsonProperty(PropertyName = "image")]
        public GoogleImage Image { get; set; }
        [JsonProperty(PropertyName = "organizations")]
        public List<GoogleOrganizations> Organizations { get; set; }
        [JsonProperty(PropertyName = "placesLived")]
        public List<GooglePlacesLived> PlacesLived { get; set; }
        [JsonProperty(PropertyName = "cover")]
        public GoogleCover Cover { get; set; }
    }
}
