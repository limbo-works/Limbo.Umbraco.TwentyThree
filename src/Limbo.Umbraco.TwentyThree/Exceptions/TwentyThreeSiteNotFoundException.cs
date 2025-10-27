using System.Diagnostics.CodeAnalysis;
using Limbo.Umbraco.TwentyThree.Options;

namespace Limbo.Umbraco.TwentyThree.Exceptions;

/// <summary>
/// Class representing an exception for when a site key (from an app URL) isn't found in the package configuration.
/// </summary>
public class TwentyThreeSiteNotFoundException : TwentyThreeUserException {

    /// <summary>
    /// Gets the key of the site.
    /// </summary>
    public string SiteKey { get; }

    /// <summary>
    /// Gets the parsed video source.
    /// </summary>
    public new TwentyThreeVideoOptions Source { get; }

    [SetsRequiredMembers]
    private TwentyThreeSiteNotFoundException(string siteKey, TwentyThreeVideoOptions source, string message, string userMessageKey, object[]? userMessageArgs) : base(message, userMessageKey, userMessageArgs) {
        SiteKey = siteKey;
        Source = source;
    }

    /// <summary>
    /// Creates a new instance of <see cref="TwentyThreeSiteNotFoundException"/> for the specified <paramref name="siteKey"/> and <paramref name="source"/>.
    /// </summary>
    /// <param name="siteKey">The site of the key.</param>
    /// <param name="source">The parsed video source.</param>
    /// <returns>An instance of <see cref="TwentyThreeSiteNotFoundException"/>.</returns>
    public static TwentyThreeSiteNotFoundException Create(string siteKey, TwentyThreeVideoOptions source) {
        return new TwentyThreeSiteNotFoundException(siteKey, source, $"No or invalid configuration found for site with key '{siteKey}' was not found.", "siteNotFound", [siteKey]);
    }

}