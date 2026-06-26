using Limbo.Umbraco.TwentyThree.Models;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.TwentyThree.PropertyEditors;

/// <summary>
/// Class representing the configuration for the <see cref="TwentyThreeEditor"/>.
/// </summary>
/// <remarks>
/// In Umbraco 17 the editing UI for each field is declared in <c>wwwroot/umbraco-package.json</c>
/// (<c>meta.settings.properties</c>); the <see cref="ConfigurationFieldAttribute"/> keys below are the
/// contract between the stored configuration and this strongly typed model (used by
/// <see cref="TwentyThreeValueConverter"/>). Values are bound via System.Text.Json (camelCase).
/// </remarks>
public class TwentyThreeConfiguration {

    /// <summary>
    /// Gets or sets whether embedded videos should automatically start playing.
    /// </summary>
    [ConfigurationField("autoplay")]
    public TwentyThreeAutoplay Autoplay { get; set; }

    /// <summary>
    /// Gets or sets whether embedded videos should loop.
    /// </summary>
    [ConfigurationField("loop")]
    public TwentyThreeLoop Loop { get; set; }

    /// <summary>
    /// Gets or sets what should happen when a video ends.
    /// </summary>
    [ConfigurationField("endOn")]
    public TwentyThreeEndOn EndOn { get; set; }

    /// <summary>
    /// Gets or sets whether the <strong>Account</strong> block should be hidden in the property editor.
    /// </summary>
    [ConfigurationField("hideSite")]
    public bool HideSite { get; set; }

    /// <summary>
    /// Gets or sets whether the <strong>Embed</strong> block should be hidden in the property editor.
    /// </summary>
    [ConfigurationField("hideEmbed")]
    public bool HideEmbed { get; set; }

    /// <summary>
    /// Gets or sets whether the <strong>Player</strong> option should be hidden in the property editor.
    /// </summary>
    [ConfigurationField("hidePlayer")]
    public bool HidePlayer { get; set; }

    /// <summary>
    /// Gets or sets whether the property editor should allow regular videos.
    /// </summary>
    [ConfigurationField("allowVideos")]
    public bool AllowVideos { get; set; }

    /// <summary>
    /// Gets or sets whether the property editor should allow spots.
    /// </summary>
    [ConfigurationField("allowSpots")]
    public bool AllowSpots { get; set; }

    /// <summary>
    /// Gets or sets whether the property editor should show a link for an external upload page.
    /// </summary>
    [ConfigurationField("showUploadLink")]
    public bool ShowUploadLink { get; set; }

    /// <summary>
    /// Gets or sets the maximum description length that will be shown in overlays.
    /// </summary>
    [ConfigurationField("descriptionMaxLength")]
    public int DescriptionMaxLength { get; set; }

}
