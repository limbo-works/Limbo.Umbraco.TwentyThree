using Limbo.Umbraco.TwentyThree.Options;
using Newtonsoft.Json;

#pragma warning disable CS1591

namespace Limbo.Umbraco.TwentyThree.Models.Api {

    public class ApiVideoParameters {

        [JsonProperty("videoId")]
        public string VideoId { get; }

        [JsonProperty("token")]
        public string? Token { get; }

        [JsonProperty("playerId")]
        public string? PlayerId { get; }

        [JsonProperty("autoplay")]
        public bool? Autoplay { get; }

        [JsonProperty("endOn")]
        public TwentyThreeEndOn? EndOn { get; }

        public ApiVideoParameters(string videoId, TwentyThreeVideoOptions options) {
            VideoId = videoId;
            Token = options.Token;
            PlayerId = options.PlayerId;
            Autoplay = options.Autoplay;
            EndOn = options.EndOn;
        }

    }

}