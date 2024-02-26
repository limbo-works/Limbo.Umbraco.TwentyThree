using Limbo.Umbraco.TwentyThree.PropertyEditors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Limbo.Umbraco.TwentyThree.Models;

/// <summary>
/// Class representing the value returned by the <see cref="TwentyThreeEditor"/> property editor.
/// </summary>
public class TwentyThreeVideoValue : TwentyThreeValue {

    #region Properties

    /// <summary>
    /// Gets a reference to the video parameters.
    /// </summary>
    [JsonIgnore]
    public TwentyThreeParameters Parameters { get; }

    /// <summary>
    /// Gets a reference to the video details.
    /// </summary>
    [JsonProperty("details")]
    public new TwentyThreeVideoDetails Details { get; }

    /// <summary>
    /// Gets a reference to the video embed information.
    /// </summary>
    [JsonProperty("embed")]
    public new TwentyThreeVideoEmbed Embed { get; }

    #endregion

    #region Constructors

    private TwentyThreeVideoValue(JObject json, TwentyThreeParameters parameters, TwentyThreeVideoDetails details, TwentyThreeVideoEmbed embed) : base(json, "video", details, embed) {
        Parameters = parameters;
        Details = details;
        Embed = embed;
    }

    #endregion

    #region Static methods

    /// <summary>
    /// Creates and returns a new <see cref="TwentyThreeVideoValue"/> instance based on the specified <paramref name="json"/> and <paramref name="config"/>.
    /// </summary>
    /// <param name="json">The JSOn object representing the video value.</param>
    /// <param name="config">The configuration of the TwentyThree data type.</param>
    /// <returns>An instance of <see cref="TwentyThreeVideoValue"/>.</returns>
    public static TwentyThreeVideoValue Create(JObject json, TwentyThreeConfiguration? config) {
        var parameters = json.GetObject("parameters", x => new TwentyThreeParameters(x))!;
        var details = json.GetObject("video", x => new TwentyThreeVideoDetails(x))!;
        var embed = json.GetObject("embed", x => new TwentyThreeVideoEmbed(x, details, parameters, config))!;
        return new TwentyThreeVideoValue(json, parameters, details, embed);
    }

    #endregion

}