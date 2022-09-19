using Limbo.Umbraco.Video.Models.Videos;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Skybrud.Essentials.Json.Converters;

namespace Limbo.Umbraco.TwentyThree.Models {

    /// <summary>
    /// Class with embed information about a TwentyThree video or spot.
    /// </summary>
    public abstract class TwentyThreeEmbed : IVideoEmbed {

        /// <summary>
        /// Gets the embed HTML for the video.
        /// </summary>
        [JsonProperty("html", Order = 100)]
        [JsonConverter(typeof(StringJsonConverter))]
        public IHtmlContent Html { get; init; } = null!;

    }

}