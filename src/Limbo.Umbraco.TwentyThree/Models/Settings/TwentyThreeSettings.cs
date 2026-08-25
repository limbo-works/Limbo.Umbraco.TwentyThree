using System.Collections.Generic;
using Umbraco.Cms.Core.Configuration.Models;

namespace Limbo.Umbraco.TwentyThree.Models.Settings;

/// <summary>
/// Class representing the settings for this package.
/// </summary>
[UmbracoOptions("Limbo:TwentyThree", BindNonPublicProperties = true)]
public class TwentyThreeSettings {

    /// <summary>
    /// Gets a collection of the credentials configured for the TwentyThree API.
    /// </summary>
    public List<TwentyThreeCredentials> Credentials { get; set; } = [];

}