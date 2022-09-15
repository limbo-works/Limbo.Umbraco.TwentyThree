using Newtonsoft.Json;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Social.TwentyThree.Models.Sites;

#pragma warning disable CS1591

namespace Limbo.Umbraco.TwentyThree.Models.Api {

    public class ApiSite {

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("domain")]
        public string Domain { get; }

        [JsonProperty("secureDomain")]
        public string SecureDomain { get; }

        public ApiSite(TwentyThreeSite site) {
            Id = site.JObject.GetString("site_id");
            Name = site.SiteName;
            Domain = site.Domain;
            SecureDomain = site.SecureDomain;
        }

    }

}