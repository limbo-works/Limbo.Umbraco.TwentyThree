using System.Collections.Generic;
using Limbo.Umbraco.TwentyThree.Json.Converters;
using Limbo.Umbraco.TwentyThree.Models.Settings;
using Limbo.Umbraco.TwentyThree.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Social.TwentyThree.Models.Sites;
using Skybrud.Social.TwentyThree.Models.Spots;

namespace Limbo.Umbraco.TwentyThree.Models.Intermediary;

#pragma warning disable CS1591

public class TwentyThreeIntermediarySpotValue : TwentyThreeIntermediaryValue {

    [JsonProperty("spot")]
    [JsonConverter(typeof(TwentyThreeJsonConverter))]
    public JObject Spot { get; }

    [JsonProperty("thumbnails")]
    public IReadOnlyList<TwentyThreeThumbnail> Thumbnails { get; }

    public TwentyThreeIntermediarySpotValue(TwentyThreeSpotOptions options, TwentyThreeCredentials credentials, TwentyThreeSpot spot, TwentyThreeThumbnail[] thumbnails, TwentyThreeSite site) : base("spot", options.Source, credentials, site, spot.SpotName) {
        Spot = spot.JObject!;
        Thumbnails = thumbnails;
    }

}