using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Limbo.Umbraco.TwentyThree.PropertyEditors;
using Skybrud.Social.TwentyThree.Exceptions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.TwentyThree.Exceptions;

public class TwentyThreeUserException : TwentyThreeException {

    #region Properties

    /// <summary>
    /// A user-friendly message that can be displayed to the user.
    /// </summary>
    public required string UserMessageKey { get; init; }

    /// <summary>
    /// Arguments for the user-friendly message, if any.
    /// </summary>
    public IReadOnlyList<object> UserMessageArgs { get; init; } = [];

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TwentyThreeUserException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public TwentyThreeUserException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TwentyThreeUserException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public TwentyThreeUserException(string message, Exception? innerException) : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TwentyThreeUserException"/> class with a developer message and a user message key.
    /// </summary>
    /// <param name="developerMessage">The message intended for developers.</param>
    /// <param name="userMessageKey">The key for the user-friendly message.</param>
    [SetsRequiredMembers]
    public TwentyThreeUserException(string developerMessage, string userMessageKey) : base(developerMessage) {
        UserMessageKey = userMessageKey;
        UserMessageArgs = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TwentyThreeUserException"/> class with a developer message, a user message key, and an inner exception.
    /// </summary>
    /// <param name="developerMessage">The message intended for developers.</param>
    /// <param name="userMessageKey">The key for the user-friendly message.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    [SetsRequiredMembers]
    public TwentyThreeUserException(string developerMessage, string userMessageKey, Exception? innerException) : base(developerMessage, innerException) {
        UserMessageKey = userMessageKey;
        UserMessageArgs = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TwentyThreeUserException"/> class with a developer message, a user message key, and user message arguments.
    /// </summary>
    /// <param name="developerMessage">The message intended for developers.</param>
    /// <param name="userMessageKey">The key for the user-friendly message.</param>
    /// <param name="userMessageArgs">Arguments for the user-friendly message.</param>
    [SetsRequiredMembers]
    public TwentyThreeUserException(string developerMessage, string userMessageKey, object[]? userMessageArgs) : base(developerMessage) {
        UserMessageKey = userMessageKey;
        UserMessageArgs = userMessageArgs ?? [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TwentyThreeUserException"/> class with a developer message, a user message key, user message arguments, and an inner exception.
    /// </summary>
    /// <param name="developerMessage">The message intended for developers.</param>
    /// <param name="userMessageKey">The key for the user-friendly message.</param>
    /// <param name="userMessageArgs">Arguments for the user-friendly message.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    [SetsRequiredMembers]
    public TwentyThreeUserException(string developerMessage, string userMessageKey, object[]? userMessageArgs, Exception? innerException) : base(developerMessage, innerException) {
        UserMessageKey = userMessageKey;
        UserMessageArgs = userMessageArgs ?? [];
    }

    #endregion

}

public static class HejException {

    public static TwentyThreeVideoNotFoundException VideoNotFound(string videoId) {
        return new TwentyThreeVideoNotFoundException(videoId);
    }

    public static TwentyThreeVideoNotFoundException VideoNotFound(string videoId, Exception? innerException) {
        return new TwentyThreeVideoNotFoundException(videoId, innerException);
    }

    public static TwentyThreeSpotNotFoundException SpotNotFound(string spotId) {
        return new TwentyThreeSpotNotFoundException(spotId);
    }

    public static TwentyThreeSpotNotFoundException SpotNotFound(string spotId, Exception? innerException) {
        return new TwentyThreeSpotNotFoundException(spotId, innerException);
    }

}

public class TwentyThreeVideoNotFoundException : TwentyThreeUserException {

    [SetsRequiredMembers]
    public TwentyThreeVideoNotFoundException(string videoId) : base($"Video with ID '{videoId}' not found.", "videoNotFound", [videoId]) { }

    [SetsRequiredMembers]
    public TwentyThreeVideoNotFoundException(string videoId, Exception? innerException) : base($"Video with ID '{videoId}' not found.", "videoNotFound", [videoId], innerException) { }

}

public class TwentyThreeSpotNotFoundException : TwentyThreeUserException {

    [SetsRequiredMembers]
    public TwentyThreeSpotNotFoundException(string spotId) : base($"Spot with ID '{spotId}' not found.", "spotNotFound", [spotId]) { }

    [SetsRequiredMembers]
    public TwentyThreeSpotNotFoundException(string spotId, Exception? innerException) : base($"Spot with ID '{spotId}' not found.", "spotNotFound", [spotId], innerException) { }

}

public class TwentyThreePlayerNotFoundException : TwentyThreeUserException {

    public string? PlayerId { get; }

    [SetsRequiredMembers]
    private TwentyThreePlayerNotFoundException(string? playerId, string message, string userMessageKey, object[]? userMessageArgs) : base(message, userMessageKey, userMessageArgs) {
        PlayerId = playerId;
    }

    public static TwentyThreePlayerNotFoundException Create(string? playerId) {
        return string.IsNullOrWhiteSpace(playerId)
            ? new TwentyThreePlayerNotFoundException(null, "Player not found.", "playerNotFound1", null)
            : new TwentyThreePlayerNotFoundException(playerId, $"Player with ID '{playerId}' not found.", "playerNotFound2", [playerId]);
    }

}

public class TwentyThreeConfigException : TwentyThreeUserException {

    public TwentyThreeConfiguration Config { get; }

    [SetsRequiredMembers]
    public TwentyThreeConfigException(TwentyThreeConfiguration config, string message, string userMessageKey) : base(message, userMessageKey) {
        Config = config;
    }

    public static TwentyThreeConfigException VideosNotAllowed(TwentyThreeConfiguration config) {
        return new TwentyThreeConfigException(config, "Videos not allowed.", "videosNotAllowed");
    }

    public static TwentyThreeConfigException SpotsNotAllowed(TwentyThreeConfiguration config) {
        return new TwentyThreeConfigException(config, "Spots not allowed.", "spotsNotAllowed");
    }

}

public class TwentyThreeSourceException : TwentyThreeUserException {

    public new string? Source { get; }

    [SetsRequiredMembers]
    public TwentyThreeSourceException(string? source, string message, string userMessageKey) : base(message, userMessageKey) {
        Source = source;
    }

    public static TwentyThreeSourceException NoSource() {
        return new TwentyThreeSourceException(null, "No source specified.", "noSource");
    }

    public static TwentyThreeSourceException InvalidSource(string source) {
        return new TwentyThreeSourceException(source, "Invalid source specified.", "invalidSource");
    }

}