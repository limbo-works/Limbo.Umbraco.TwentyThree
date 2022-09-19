using Limbo.Umbraco.TwentyThree.PropertyEditors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Limbo.Umbraco.TwentyThree.Models {

    /// <summary>
    /// Class representing the value returned by the <see cref="TwentyThreeEditor"/> property editor.
    /// </summary>
    public class TwentyThreeSpotValue : TwentyThreeValue {

        #region Properties

        /// <summary>
        /// Gets a reference to the spot details.
        /// </summary>
        [JsonProperty("details", NullValueHandling = NullValueHandling.Ignore)]
        public new TwentyThreeSpotDetails Details { get; }

        /// <summary>
        /// Gets a reference to the video embed information.
        /// </summary>
        [JsonProperty("embed")]
        public new TwentyThreeSpotEmbed Embed { get; }

        #endregion

        #region Constructors
        
        private TwentyThreeSpotValue(JObject json, TwentyThreeSpotDetails details, TwentyThreeSpotEmbed embed) : base(json, "spot", details, embed) {
            Details = details;
            Embed = embed;
        }

        #endregion

        #region Static methods

        internal static TwentyThreeSpotValue Create(JObject json) {
            var thumbnails = json.GetArrayItems("thumbnails", TwentyThreeThumbnail.Parse);
            var details = json.GetObject("spot", x => new TwentyThreeSpotDetails(x, thumbnails));
            var embed = new TwentyThreeSpotEmbed(details);
            return new TwentyThreeSpotValue(json, details, embed);
        }

        #endregion

    }

}