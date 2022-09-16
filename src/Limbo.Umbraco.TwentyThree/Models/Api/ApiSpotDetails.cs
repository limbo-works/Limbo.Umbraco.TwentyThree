using System.Collections.Generic;
using Limbo.Umbraco.TwentyThree.Models.Credentials;
using Limbo.Umbraco.TwentyThree.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Social.TwentyThree.Models.Sites;
using Skybrud.Social.TwentyThree.Models.Spots;

#pragma warning disable CS1591

namespace Limbo.Umbraco.TwentyThree.Models.Api {

    public class ApiSpotDetails {

        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("source")]
        public string? Source { get; }

        [JsonProperty("credentials")]
        public ApiCredentials Credentials { get; }

        [JsonProperty("spot")]
        public JObject Spot { get; }

        [JsonProperty("thumbnails")]
        public IReadOnlyList<TwentyThreeThumbnail> Thumbnails { get; }

        [JsonProperty("site")]
        public ApiSite Site { get; }

        public ApiSpotDetails(TwentyThreeSpotOptions options, TwentyThreeCredentials credentials, TwentyThreeSpot spot, TwentyThreeThumbnail[] thumbnails, TwentyThreeSite site) {
            Thumbnails = thumbnails;
            Type = "spot";
            Source = options.Source;
            Credentials = new ApiCredentials(credentials);
            Spot = spot.JObject;
            Site = new ApiSite(site);
        }

    }

}