using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Skybrud.Social.TwentyThree.Exceptions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.TwentyThree.Exceptions;

/// <summary>
/// Exception type representing user-friendly errors in the TwentyThree package.
/// </summary>
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