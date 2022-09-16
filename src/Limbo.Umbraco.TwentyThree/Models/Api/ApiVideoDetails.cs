using Limbo.Umbraco.TwentyThree.Models.Credentials;
using Limbo.Umbraco.TwentyThree.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Social.TwentyThree.Models.Photos;
using Skybrud.Social.TwentyThree.Models.Players;
using Skybrud.Social.TwentyThree.Models.Sites;

#pragma warning disable CS1591

namespace Limbo.Umbraco.TwentyThree.Models.Api {

    public class ApiVideoDetails {

        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("source")]
        public string? Source { get; }

        [JsonProperty("credentials")]
        public ApiCredentials Credentials { get; }

        [JsonProperty("parameters")]
        public ApiVideoParameters Parameters { get; }

        [JsonProperty("video")]
        public JObject Video { get; }

        [JsonProperty("player")]
        public ApiPlayer Player { get; }

        [JsonProperty("site")]
        public ApiSite Site { get; }

        public ApiVideoDetails(TwentyThreeVideoOptions options, TwentyThreeCredentials credentials, TwentyThreePhoto video, TwentyThreePlayer player, TwentyThreeSite site) {
            Type = "video";
            Source = options.Source;
            Credentials = new ApiCredentials(credentials);
            Parameters = new ApiVideoParameters(video.PhotoId, options);
            Video = video.JObject;
            Player = new ApiPlayer(player);
            Site = new ApiSite(site);
        }

    }

}