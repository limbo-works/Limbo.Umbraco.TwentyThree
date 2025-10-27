using System;
using System.Diagnostics.CodeAnalysis;

namespace Limbo.Umbraco.TwentyThree.Exceptions;

/// <summary>
/// Class representing an exception thrown when a TwentyThree video could not be found.
/// </summary>
public class TwentyThreeVideoNotFoundException : TwentyThreeUserException {

    /// <summary>
    /// Creates a new instance of <see cref="TwentyThreeVideoNotFoundException"/> for the specified <paramref name="videoId"/>.
    /// </summary>
    /// <param name="videoId">The ID of the video.</param>
    [SetsRequiredMembers]
    public TwentyThreeVideoNotFoundException(string videoId) : base($"Video with ID '{videoId}' not found.", "videoNotFound", [videoId]) { }

    /// <summary>
    /// Creates a new instance of <see cref="TwentyThreeVideoNotFoundException"/> for the specified <paramref name="videoId"/>.
    /// </summary>
    /// <param name="videoId">The ID of the video.</param>
    /// <param name="innerException">An inner exception, if any.</param>
    [SetsRequiredMembers]
    public TwentyThreeVideoNotFoundException(string videoId, Exception? innerException) : base($"Video with ID '{videoId}' not found.", "videoNotFound", [videoId], innerException) { }

}