using System.Diagnostics.CodeAnalysis;

namespace Limbo.Umbraco.TwentyThree.Exceptions;

/// <summary>
/// Class representing an exception thrown when a TwentyThree player could not be found.
/// </summary>
public class TwentyThreePlayerNotFoundException : TwentyThreeUserException {

    /// <summary>
    /// The ID of the player.
    /// </summary>
    public string? PlayerId { get; }

    [SetsRequiredMembers]
    private TwentyThreePlayerNotFoundException(string? playerId, string message, string userMessageKey, object[]? userMessageArgs) : base(message, userMessageKey, userMessageArgs) {
        PlayerId = playerId;
    }

    /// <summary>
    /// Creates a new instance of <see cref="TwentyThreePlayerNotFoundException"/> for the specified <paramref name="playerId"/>.
    /// </summary>
    /// <param name="playerId">The ID of the player.</param>
    /// <returns>An instance of <see cref="TwentyThreePlayerNotFoundException"/>.</returns>
    public static TwentyThreePlayerNotFoundException Create(string? playerId) {
        return string.IsNullOrWhiteSpace(playerId)
            ? new TwentyThreePlayerNotFoundException(null, "Player not found.", "playerNotFound1", null)
            : new TwentyThreePlayerNotFoundException(playerId, $"Player with ID '{playerId}' not found.", "playerNotFound2", [playerId]);
    }

}