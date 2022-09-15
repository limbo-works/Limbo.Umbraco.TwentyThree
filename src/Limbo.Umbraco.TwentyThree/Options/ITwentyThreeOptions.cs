namespace Limbo.Umbraco.TwentyThree.Options;

/// <summary>
/// Interface describing a generic TwentyThree video.
/// </summary>
public interface ITwentyThreeOptions {

    /// <summary>
    /// Gets the scheme of the video.
    /// </summary>
    public string Scheme { get; }

    /// <summary>
    /// Gets the domain of the video.
    /// </summary>
    public string Domain { get; }

}