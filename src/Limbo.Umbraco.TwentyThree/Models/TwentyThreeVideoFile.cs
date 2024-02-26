using System.Linq;
using Limbo.Umbraco.Video.Models.Videos;
using Newtonsoft.Json;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Social.TwentyThree.Models.Photos;

namespace Limbo.Umbraco.TwentyThree.Models;

/// <summary>
/// Class representing a video file for a TwentyThree video.
/// </summary>
public class TwentyThreeVideoFile : IVideoFile {

    #region Properties

    /// <summary>
    /// Gets the alias of the video file.
    /// </summary>
    [JsonProperty("alias")]
    public string Alias { get; }

    /// <summary>
    /// Gets the width of the video file.
    /// </summary>
    [JsonProperty("width")]
    public int Width { get; }

    /// <summary>
    /// Gets the height of the video file.
    /// </summary>
    [JsonProperty("height")]
    public int Height { get; }

    /// <summary>
    /// Gets the URL of the video file.
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; }

    /// <summary>
    /// Gets the type of the video file, if available.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string? Type { get; }

    /// <summary>
    /// Gets the size of the video file, if available.
    /// </summary>
    [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
    public long? Size { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="video"/> and <paramref name="format"/>.
    /// </summary>
    /// <param name="video">The video.</param>
    /// <param name="format">The vide format the video file should represent.</param>
    public TwentyThreeVideoFile(TwentyThreePhoto video, TwentyThreeVideoFormat format) {
        Alias = format.Alias;
        Width = format.Width;
        Height = format.Height;
        Url = $"{video.AbsoluteUrl.Split('/').Take(2).Join("/")}{format.Url}";
        Size = format.Size;
    }

    #endregion

}