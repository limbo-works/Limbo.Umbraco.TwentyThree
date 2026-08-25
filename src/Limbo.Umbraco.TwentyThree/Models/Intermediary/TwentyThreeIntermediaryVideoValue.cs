using Limbo.Umbraco.TwentyThree.Json.Converters;
using Limbo.Umbraco.TwentyThree.Models.Api;
using Limbo.Umbraco.TwentyThree.Models.Settings;
using Limbo.Umbraco.TwentyThree.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Social.TwentyThree.Models.Photos;
using Skybrud.Social.TwentyThree.Models.Players;
using Skybrud.Social.TwentyThree.Models.Sites;

namespace Limbo.Umbraco.TwentyThree.Models.Intermediary;

#pragma warning disable CS1591

public class TwentyThreeIntermediaryVideoValue : TwentyThreeIntermediaryValue {

    [JsonProperty("parameters")]
    public ApiVideoParameters Parameters { get; }

    [JsonProperty("video")]
    [JsonConverter(typeof(TwentyThreeJsonConverter))]
    public JObject Video { get; }

    [JsonProperty("player")]
    public ApiPlayer Player { get; }

    public TwentyThreeIntermediaryVideoValue(TwentyThreeVideoOptions options, TwentyThreeCredentials credentials, TwentyThreePhoto video, TwentyThreePlayer player, TwentyThreeSite site) : base("video", options.Source, credentials, site, video.Title) {
        Parameters = new ApiVideoParameters(video.PhotoId, options);
        Video = video.JObject!;
        Player = new ApiPlayer(player);
    }

}