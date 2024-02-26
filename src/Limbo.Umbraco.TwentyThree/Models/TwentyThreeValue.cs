using Limbo.Umbraco.TwentyThree.PropertyEditors;
using Limbo.Umbraco.Video.Models.Providers;
using Limbo.Umbraco.Video.Models.Videos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Limbo.Umbraco.TwentyThree.Models;

/// <summary>
/// Class representing the value returned by the <see cref="TwentyThreeEditor"/> property editor.
/// </summary>
public abstract class TwentyThreeValue : IVideoValue {

    /// <summary>
    /// Gets a reference to the underlying JSON the instance was parsed from.
    /// </summary>
    [JsonIgnore]
    public JObject Json { get; }

    /// <summary>
    /// Gets the source (URL or embed code) as entered by the user.
    /// </summary>
    [JsonIgnore]
    public string? Source { get; }

    /// <summary>
    /// Gets information about the video provider.
    /// </summary>
    [JsonProperty("provider")]
    public TwentyThreeProvider Provider { get; }

    /// <summary>
    /// Gets the type of the video or spot.
    /// </summary>
    [JsonProperty("type", Order = -99)]
    public string Type { get; }

    /// <summary>
    /// Gets a reference to the video or spot details.
    /// </summary>
    [JsonProperty("details")]
    public TwentyThreeDetails Details { get; protected set; }

    /// <summary>
    /// Gets embed information for the video or spot.
    /// </summary>
    [JsonProperty("embed")]
    public TwentyThreeEmbed Embed { get; protected set; }

    IVideoProvider IVideoValue.Provider => Provider;

    IVideoDetails IVideoValue.Details => Details;

    IVideoEmbed IVideoValue.Embed => Embed;

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="json"/> object.
    /// </summary>
    /// <param name="json">The JSON object representing the value.</param>
    /// <param name="type">The type.</param>
    /// <param name="details">The details about the video or spot.</param>
    /// <param name="embed">The embed information.</param>
    protected TwentyThreeValue(JObject json, string type, TwentyThreeDetails details, TwentyThreeEmbed embed) {
        Json = json;
        Source = json.GetString("source");
        Provider = TwentyThreeProvider.Default;
        Type = type;
        Details = details;
        Embed = embed;
    }

}