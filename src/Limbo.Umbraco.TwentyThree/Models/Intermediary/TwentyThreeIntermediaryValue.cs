using Limbo.Umbraco.TwentyThree.Models.Api;
using Limbo.Umbraco.TwentyThree.Models.Credentials;
using Newtonsoft.Json;
using Skybrud.Social.TwentyThree.Models.Sites;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.TwentyThree.Models.Intermediary;

public abstract class TwentyThreeIntermediaryValue {

    [JsonProperty("type", Order = -999)]
    public string Type { get; }

    [JsonProperty("source", Order = -998)]
    public string? Source { get; }

    [JsonProperty("site", Order = -997)]
    public ApiSite Site { get; }

    [JsonProperty("credentials", Order = -996)]
    public ApiCredentials Credentials { get; }

    [JsonIgnore]
    public string Title { get; }

    protected TwentyThreeIntermediaryValue(string type, string? source, TwentyThreeCredentials credentials, TwentyThreeSite site, string title) {
        Type = type;
        Source = source;
        Title = title;
        Credentials = new ApiCredentials(credentials);
        Site = new ApiSite(site);
    }

}