using System;
using System.Collections.Generic;
using Limbo.Umbraco.Video.Models.Credentials;

namespace Limbo.Umbraco.TwentyThree.Models.Settings;

/// <summary>
/// Class with information about the credentials used for accessing the TwentyThree API.
/// </summary>
public class TwentyThreeCredentials : ICredentials {

    /// <summary>
    /// Gets the key of the credentials.
    /// </summary>
    public required Guid Key { get; set; }

    /// <summary>
    /// Gets the key (alias) of the associated TwentyThree site.
    /// </summary>
    public string? SiteKey { get; set; }

    /// <summary>
    /// Gets the friendly name of the credentials.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets the description of the credentials.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets the icon of the credentials.
    /// </summary>
    public string? Icon { get; internal set; }

    /// <summary>
    /// If configured, gets the domains of the associated TwentyThree site.
    /// </summary>
    public List<string> Domains { get; internal set; } = [];

    /// <summary>
    /// If configured, gets the TwentyThree consumer key.
    /// </summary>
    public required string ConsumerKey { get; set; }

    /// <summary>
    /// If configured, gets the TwentyThree consumer secret.
    /// </summary>
    public required string ConsumerSecret { get; set; }

    /// <summary>
    /// If configured, gets the TwentyThree access token.
    /// </summary>
    public required string? AccessToken { get; set; }

    /// <summary>
    /// If configured, gets the TwentyThree access token secret.
    /// </summary>
    public required string? AccessTokenSecret { get; set; }

    /// <summary>
    /// If configured, gets the URL of the upload page.
    /// </summary>
    public string? UploadUrl { get; set; }

    /// <summary>
    /// Initializes a new instance with default options.
    /// </summary>
    public TwentyThreeCredentials() { }

}