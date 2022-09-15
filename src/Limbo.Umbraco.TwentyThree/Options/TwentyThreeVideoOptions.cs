using Limbo.Umbraco.TwentyThree.Models;

namespace Limbo.Umbraco.TwentyThree.Options;

/// <summary>
/// Class with options for embedding a TwnetyThree video.
/// </summary>
public class TwentyThreeVideoOptions : ITwentyThreeOptions {

    /// <summary>
    /// Gets a reference to the original source the options were parsed from.
    /// </summary>
    public string? Source { get; }

    /// <summary>
    /// Gets the scheme of the video.
    /// </summary>
    public string Scheme { get; }

    /// <summary>
    /// Gets the domain of the video.
    /// </summary>
    public string Domain { get; }

    /// <summary>
    /// Gets the ID of the video.
    /// </summary>
    public string VideoId { get; }

    /// <summary>
    /// Gets the token of the video.
    /// </summary>
    public string? Token { get; }

    /// <summary>
    /// Gets the ID of the player, if specified.
    /// </summary>
    public string? PlayerId { get; }

    /// <summary>
    /// Gets whether the video should autoplay.
    /// </summary>
    public bool? Autoplay { get; }

    /// <summary>
    /// Gets what happens when the video ends.
    /// </summary>
    public TwentyThreeEndOn? EndOn { get; init; }

    /// <summary>
    /// Initializes a new instance based on the specified parameters.
    /// </summary>
    /// <param name="source">The original source the options were parsed from.</param>
    /// <param name="scheme">The scheme of the video.</param>
    /// <param name="domain">The domain of the video.</param>
    /// <param name="videoId">The ID of the video.</param>
    /// <param name="token">The token of the video.</param>
    /// <param name="playerId">The ID of the player.</param>
    public TwentyThreeVideoOptions(string? source, string scheme, string domain, string videoId, string? token, string? playerId) {
        Source = source;
        Scheme = scheme;
        Domain = domain;
        VideoId = videoId;
        Token = token;
        PlayerId = playerId;
    }

    /// <summary>
    /// Initializes a new instance based on the specified parameters.
    /// </summary>
    /// <param name="source">The original source the options were parsed from.</param>
    /// <param name="scheme">The scheme of the video.</param>
    /// <param name="domain">The domain of the video.</param>
    /// <param name="videoId">The ID of the video.</param>
    /// <param name="token">The token of the video.</param>
    /// <param name="playerId">The ID of the player.</param>
    /// <param name="autoplay">Indicates whether the video should autoplay.</param>
    /// <param name="endOn">Indicates what happens when the video ends.</param>
    public TwentyThreeVideoOptions(string? source, string scheme, string domain, string videoId, string? token, string? playerId, bool? autoplay, TwentyThreeEndOn? endOn) {
        Source = source;
        Scheme = scheme;
        Domain = domain;
        VideoId = videoId;
        Token = token;
        PlayerId = playerId;
        Autoplay = autoplay;
        EndOn = endOn;
    }

}