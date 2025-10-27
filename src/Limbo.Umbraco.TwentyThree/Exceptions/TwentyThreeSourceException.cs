using System.Diagnostics.CodeAnalysis;

namespace Limbo.Umbraco.TwentyThree.Exceptions;

/// <summary>
/// Class representing exceptions related to invalid or missing sources in TwentyThree.
/// </summary>
public class TwentyThreeSourceException : TwentyThreeUserException {

    /// <summary>
    /// The source related to the exception, if any.
    /// </summary>
    public new string? Source { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TwentyThreeSourceException"/> class with a specified source, error message and user message key.
    /// </summary>
    /// <param name="source">The source (URL or embed code).</param>
    /// <param name="message">The developer-focused error message.</param>
    /// <param name="userMessageKey">The key of the user-focused error message.</param>
    [SetsRequiredMembers]
    public TwentyThreeSourceException(string? source, string message, string userMessageKey) : base(message, userMessageKey) {
        Source = source;
    }

    /// <summary>
    /// Creates an exception indicating that no source was specified.
    /// </summary>
    /// <returns>An instance of <see cref="TwentyThreeSourceException"/>.</returns>
    public static TwentyThreeSourceException NoSource() {
        return new TwentyThreeSourceException(null, "No source specified.", "noSource");
    }

    /// <summary>
    /// Creates an exception indicating that an invalid source was specified.
    /// </summary>
    /// <param name="source"></param>
    /// <returns>An instance of <see cref="TwentyThreeSourceException"/>.</returns>
    public static TwentyThreeSourceException InvalidSource(string source) {
        return new TwentyThreeSourceException(source, "Invalid source specified.", "invalidSource");
    }

}