using Newtonsoft.Json;
using Skybrud.Essentials.Json.Newtonsoft.Converters.Enums;

namespace Limbo.Umbraco.TwentyThree.Models {

    /// <summary>
    /// Enum class indicating what should happen when a video ends.
    /// </summary>
    [JsonConverter(typeof(EnumCamelCaseConverter))]
    public enum TwentyThreeEndOn {

        /// <summary>
        /// Indicates that the value should be inherit - eg. from player or embed parameters.
        /// </summary>
        Inherit,

        /// <summary>
        /// Show sharing.
        /// </summary>
        Share,

        /// <summary>
        /// Show recommended videos.
        /// </summary>
        Browse,

        /// <summary>
        /// Loop through recommendations.
        /// </summary>
        Loop,

        /// <summary>
        /// Show thumbnail.
        /// </summary>
        Thumbnail

    }

}