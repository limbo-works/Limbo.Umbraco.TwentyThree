using System.Runtime.Serialization;
using Limbo.Umbraco.TwentyThree.Models;
using Newtonsoft.Json;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.TwentyThree.PropertyEditors {

    /// <summary>
    /// Class represrnting the configuration for the <see cref="TwentyThreeEditor"/>.
    /// </summary>
    public class TwentyThreeConfiguration {

        /// <summary>
        /// Gets or sets whether embedded videos should automatically start playing.
        /// </summary>
        [ConfigurationField("autoplay",
            "Autoplay",
            $"/App_Plugins/{TwentyThreePackage.Alias}/Views/ButtonList.html?type={{alias}}",
            Description = "Select whether videos should autoplay when embedded.")]
        public TwentyThreeAutoplay Autoplay { get; set; }

        /// <summary>
        /// Gets or sets what should happen when a video ends.
        /// </summary>
        [ConfigurationField("endOn",
            "End on",
            $"/App_Plugins/{TwentyThreePackage.Alias}/Views/ButtonList.html?type={{alias}}",
            Description = "Select what happens when a video ends.")]
        public TwentyThreeEndOn EndOn { get; set; }

        /// <summary>
        /// Gets or sets whether the <strong>Site</strong> block should be hidden in the property editor.
        /// </summary>
        [ConfigurationField("hideSite",
            "Hide site information",
            "boolean",
            Description = "Select whether the site information should be hidden in the property editor.")]
        public bool HideSite { get; set; }

        /// <summary>
        /// Gets or sets whether the <strong>Embed</strong> block should be hidden in the property editor.
        /// </summary>
        [ConfigurationField("hideEmbed",
            "Hide embed options",
            "boolean",
            Description = "Select whether embed options should be hidden in the property editor.")]
        public bool HideEmbed { get; set; }

        [ConfigurationField("allowVideos",
            "Allow videos",
            "boolean",
            Description = "Select whether videos should be allowed in the property editor.")]
        public bool AllowVideos { get; } = true;

        [ConfigurationField("allowSpots",
            "Allow spots",
            "boolean",
            Description = "Select whether spots should be allowed in the property editor.")]
        [JsonProperty("allowVideos")]
        public bool AllowSpots { get; } = false;

    }
}