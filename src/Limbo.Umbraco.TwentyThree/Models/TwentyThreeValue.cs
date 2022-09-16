using Limbo.Umbraco.TwentyThree.PropertyEditors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Limbo.Umbraco.TwentyThree.Models {

    /// <summary>
    /// Class representing the value returned by the <see cref="TwentyThreeEditor"/> property editor.
    /// </summary>
    public class TwentyThreeValue {

        /// <summary>
        /// Gets a reference to the underlying JSON the instance was parsed from.
        /// </summary>
        [JsonIgnore]
        public JObject Json { get; }

        /// <summary>
        /// Gets a reference to the video parameters.
        /// </summary>
        [JsonIgnore]
        public TwentyThreeParameters Parameters { get; }

        /// <summary>
        /// Gets a reference to the video details.
        /// </summary>
        [JsonProperty("video")]
        public TwentyThreeVideoDetails Video { get; }

        /// <summary>
        /// Gets a reference to the video embed information.
        /// </summary>
        [JsonProperty("embed")]
        public TwentyThreeEmbed Embed { get; }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="json"/> object.
        /// </summary>
        /// <param name="json">The JSON object representing the value.</param>
        /// <param name="config">The configuration of the <see cref="TwentyThreeEditor"/> data type.</param>
        public TwentyThreeValue(JObject json, TwentyThreeConfiguration? config) {
            Json = json;
            Parameters = json.GetObject("parameters", x => new TwentyThreeParameters(x));
            Video = json.GetObject("video", x => new TwentyThreeVideoDetails(x));
            Embed = json.GetObject("embed", x => new TwentyThreeEmbed(x, Video, Parameters, config));
        }

    }

}