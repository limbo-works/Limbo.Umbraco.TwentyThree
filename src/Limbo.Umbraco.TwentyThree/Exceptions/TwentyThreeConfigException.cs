using System.Diagnostics.CodeAnalysis;
using Limbo.Umbraco.TwentyThree.PropertyEditors;

namespace Limbo.Umbraco.TwentyThree.Exceptions;

/// <summary>
/// Class representing exceptions related to the configuration of the <see cref="TwentyThreeEditor"/> data type.
/// </summary>
public class TwentyThreeConfigException : TwentyThreeUserException {

    #region Properties

    /// <summary>
    /// Gets a reference to the configuration.
    /// </summary>
    public TwentyThreeConfiguration Config { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TwentyThreeConfigException"/> class with a specified configuration, error message and user message key.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="message">The developer-focused error message.</param>
    /// <param name="userMessageKey">The key of the user-focused error message.</param>
    [SetsRequiredMembers]
    public TwentyThreeConfigException(TwentyThreeConfiguration config, string message, string userMessageKey) : base(message, userMessageKey) {
        Config = config;
    }

    #endregion

    #region Static methods

    /// <summary>
    /// Creates an exception indicating that videos are not allowed according to the specified <paramref name="config"/>.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <returns>An instance of <see cref="TwentyThreeConfigException"/>.</returns>
    public static TwentyThreeConfigException VideosNotAllowed(TwentyThreeConfiguration config) {
        return new TwentyThreeConfigException(config, "Videos not allowed.", "videosNotAllowed");
    }

    /// <summary>
    /// Creates an exception indicating that spots are not allowed according to the specified <paramref name="config"/>.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <returns>An instance of <see cref="TwentyThreeConfigException"/>.</returns>
    public static TwentyThreeConfigException SpotsNotAllowed(TwentyThreeConfiguration config) {
        return new TwentyThreeConfigException(config, "Spots not allowed.", "spotsNotAllowed");
    }

    #endregion

}