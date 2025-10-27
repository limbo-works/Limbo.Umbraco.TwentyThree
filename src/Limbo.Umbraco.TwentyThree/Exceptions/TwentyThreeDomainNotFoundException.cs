using System.Diagnostics.CodeAnalysis;
using Limbo.Umbraco.TwentyThree.Options;

namespace Limbo.Umbraco.TwentyThree.Exceptions;

/// <summary>
/// Class representing an exception for when a domain isn't found in the package configuration.
/// </summary>
public class TwentyThreeDomainNotFoundException : TwentyThreeUserException {

    /// <summary>
    /// Gets the domain.
    /// </summary>
    public string Domain { get; }

    /// <summary>
    /// Gets the parsed video source.
    /// </summary>
    public new ITwentyThreeOptions Source { get; }

    [SetsRequiredMembers]
    private TwentyThreeDomainNotFoundException(string domain, ITwentyThreeOptions source, string message, string userMessageKey, object[]? userMessageArgs) : base(message, userMessageKey, userMessageArgs) {
        Domain = domain;
        Source = source;
    }

    /// <summary>
    /// Creates a new instance of <see cref="TwentyThreeDomainNotFoundException"/> for the specified <paramref name="domain"/> and <paramref name="source"/>.
    /// </summary>
    /// <param name="domain">The domain.</param>
    /// <param name="source">The parsed video source.</param>
    /// <returns>An instance of <see cref="TwentyThreeDomainNotFoundException"/>.</returns>
    public static TwentyThreeDomainNotFoundException Create(string domain, ITwentyThreeOptions source) {
        return new TwentyThreeDomainNotFoundException(domain, source, $"No or invalid configuration found for the '{domain}' domain.", "domainNotFound", [domain]);
    }

}