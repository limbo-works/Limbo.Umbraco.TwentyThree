#pragma warning disable CS1591

using Limbo.Umbraco.TwentyThree.Models;

namespace Limbo.Umbraco.TwentyThree.Options;

public class TwentyThreeSpotOptions : ITwentyThreeOptions {

    /// <summary>
    /// Gets a reference to the original source the options were parsed from.
    /// </summary>
    public string? Source { get; }

    /// <summary>
    /// Gets the type of the embed code.
    /// </summary>
    public TwentyThreeSourceType Type { get; }

    public string Scheme { get; }

    public string Domain { get; }

    public string SpotId { get; }

    public string Token { get; }

    public TwentyThreeSpotOptions(string source, TwentyThreeSourceType type, string scheme, string domain, string spotId, string token) {
        Source = source;
        Type = type;
        Scheme = scheme;
        Domain = domain;
        SpotId = spotId;
        Token = token;
    }

}