namespace Limbo.Umbraco.TwentyThree.Models;

/// <summary>
/// Enum type indicating the type of source for a TwentyThree video.
/// </summary>
public enum TwentyThreeSourceType {

    /// <summary>
    /// Indicates that the source is an app URL (<c>app.twentythree.com</c>).
    /// </summary>
    AppUrl,

    /// <summary>
    /// Indicates that the source is an old app URL (<c>{domain}.twentythree.com</c>).
    /// </summary>
    OldAppUrl,

    /// <summary>
    /// Indicates that the source is an embed code.
    /// </summary>
    Embed,

    /// <summary>
    /// Indicates that the source is a &lt;script&gt; element (spot).
    /// </summary>
    Script

}