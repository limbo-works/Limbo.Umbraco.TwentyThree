using System;
using System.Diagnostics.CodeAnalysis;
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
    public required string Alias { get; init; }

    /// <summary>
    /// Gets the width of the video file.
    /// </summary>
    [JsonProperty("width")]
    public required int Width { get; init; }

    /// <summary>
    /// Gets the height of the video file.
    /// </summary>
    [JsonProperty("height")]
    public required int Height { get; init; }

    /// <summary>
    /// Gets the URL of the video file.
    /// </summary>
    [JsonProperty("url")]
    public required string Url { get; init; }

    /// <summary>
    /// Gets the type of the video file, if available.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public required string? Type { get; init; }

    /// <summary>
    /// Gets the size of the video file, if available.
    /// </summary>
    [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
    public required long? Size { get; init; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new video file. When using the parameterless constructor, properties most be initialized via property initializers.
    /// </summary>
    public TwentyThreeVideoFile() { }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="video"/> and <paramref name="format"/>.
    /// </summary>
    /// <param name="video">The video.</param>
    /// <param name="format">The vide format the video file should represent.</param>
    [SetsRequiredMembers]
    [Obsolete("Use the 'TwentyThreeModelFactory.CreateVideoFile' method instead.")]
    public TwentyThreeVideoFile(TwentyThreePhoto video, TwentyThreeVideoFormat format) {
        Alias = format.Alias;
        Width = format.Width;
        Height = format.Height;
        Url = $"{video.AbsoluteUrl.Split('/').Take(3).Join("/")}{format.Url}";
        Size = format.Size;
    }

    /// <summary>
    /// Initialized a new instance based on the specified parameters.
    /// </summary>
    /// <param name="alias">The alias of the video file.</param>
    /// <param name="width">The width of the video file.</param>
    /// <param name="height">The height of the video file.</param>
    /// <param name="url">The URL of the video file.</param>
    /// <param name="size">The size of the video file.</param>
    [SetsRequiredMembers]
    public TwentyThreeVideoFile(string alias, int width, int height, string url, long? size) {
        Alias = alias;
        Width = width;
        Height = height;
        Url = url;
        Size = size;
    }

    #endregion

}