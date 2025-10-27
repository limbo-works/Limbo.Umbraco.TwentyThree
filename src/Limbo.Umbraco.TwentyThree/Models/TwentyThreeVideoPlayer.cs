namespace Limbo.Umbraco.TwentyThree.Models;

/// <summary>
/// Class representing a TwentyThree video player.
/// </summary>
public class TwentyThreeVideoPlayer {

    #region Properties

    /// <summary>
    /// Gets the ID of the player.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the name of the player.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets whether the player is the default player.
    /// </summary>
    public bool IsDefault { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="TwentyThreeVideoPlayer"/> with the specified <paramref name="id"/>, <paramref name="name"/> and <paramref name="isDefault"/> values.
    /// </summary>
    /// <param name="id">The ID of the player.</param>
    /// <param name="name">The name of the player.</param>
    /// <param name="isDefault">Whether this is the default player.</param>
    public TwentyThreeVideoPlayer(string id, string name, bool isDefault) {
        Id = id;
        Name = name;
        IsDefault = isDefault;
    }

    #endregion

}