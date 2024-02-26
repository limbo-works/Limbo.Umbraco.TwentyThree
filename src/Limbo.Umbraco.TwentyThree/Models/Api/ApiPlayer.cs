using Newtonsoft.Json;
using Skybrud.Social.TwentyThree.Models.Players;

#pragma warning disable CS1591

namespace Limbo.Umbraco.TwentyThree.Models.Api;

public class ApiPlayer {

    [JsonProperty("id")]
    public string Id { get; }

    [JsonProperty("name")]
    public string Name { get; }

    [JsonProperty("default", NullValueHandling = NullValueHandling.Ignore)]
    public bool IsDefault { get; }

    public ApiPlayer(TwentyThreePlayer player) {
        Id = player.PlayerId;
        Name = player.PlayerName;
        IsDefault = player.IsDefault;
    }

}