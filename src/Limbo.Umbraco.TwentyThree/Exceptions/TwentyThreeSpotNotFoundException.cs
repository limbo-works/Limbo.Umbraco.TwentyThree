using System;
using System.Diagnostics.CodeAnalysis;

namespace Limbo.Umbraco.TwentyThree.Exceptions;

/// <summary>
/// Class representing an exception thrown when a TwentyThree spot could not be found.
/// </summary>
public class TwentyThreeSpotNotFoundException : TwentyThreeUserException {

    /// <summary>
    /// Creates a new instance of <see cref="TwentyThreeSpotNotFoundException"/> for the specified <paramref name="spotId"/>.
    /// </summary>
    /// <param name="spotId">The ID of the spot.</param>
    [SetsRequiredMembers]
    public TwentyThreeSpotNotFoundException(string spotId) : base($"Spot with ID '{spotId}' not found.", "spotNotFound", [spotId]) { }

    /// <summary>
    /// Creates a new instance of <see cref="TwentyThreeSpotNotFoundException"/> for the specified <paramref name="spotId"/>.
    /// </summary>
    /// <param name="spotId">The ID of the spot.</param>
    /// <param name="innerException">An inner exception, if any.</param>
    [SetsRequiredMembers]
    public TwentyThreeSpotNotFoundException(string spotId, Exception? innerException) : base($"Spot with ID '{spotId}' not found.", "spotNotFound", [spotId], innerException) { }

}