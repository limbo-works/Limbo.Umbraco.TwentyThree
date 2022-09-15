#pragma warning disable CS1591

namespace Limbo.Umbraco.TwentyThree.Options;

public class TwentyThreeSpotOptions : ITwentyThreeOptions {

    public string Scheme { get; }

    public string Domain { get; }

    public string SpotId { get; }

    public string Token { get; }

    public TwentyThreeSpotOptions(string scheme, string domain, string spotId, string token) {
        Scheme = scheme;
        Domain = domain;
        SpotId = spotId;
        Token = token;
    }

}