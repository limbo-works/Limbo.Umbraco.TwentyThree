namespace Limbo.Umbraco.TwentyThree.Options;

/// <summary>
/// Interface describing a generic TwentyThree video.
/// </summary>
public interface ITwentyThreeOptions {

    /// <summary>
    /// Gets a reference to the original source the options were parsed from.
    /// </summary>
    string? Source { get; }

    /// <summary>
    /// Gets the scheme of the video.
    /// </summary>
    string Scheme { get; }

    /// <summary>
    /// Gets the domain of the video.
    /// </summary>
    string Domain { get; }

}