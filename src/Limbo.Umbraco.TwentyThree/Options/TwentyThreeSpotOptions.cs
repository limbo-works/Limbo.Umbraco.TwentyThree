#pragma warning disable CS1591

namespace Limbo.Umbraco.TwentyThree.Options;

public class TwentyThreeSpotOptions : ITwentyThreeOptions {

    /// <summary>
    /// Gets a reference to the original source the options were parsed from.
    /// </summary>
    public string? Source { get; }

    public string Scheme { get; }

    public string Domain { get; }

    public string SpotId { get; }

    public string Token { get; }

    public TwentyThreeSpotOptions(string source, string scheme, string domain, string spotId, string token) {
        Source = source;
        Scheme = scheme;
        Domain = domain;
        SpotId = spotId;
        Token = token;
    }

}