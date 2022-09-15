using Limbo.Umbraco.TwentyThree.PropertyEditors;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Essentials.Strings;

namespace Limbo.Umbraco.TwentyThree.Models {

    /// <summary>
    /// Class representing the parameters parsed from the source field of the <see cref="TwentyThreeEditor"/> property editor.
    /// </summary>
    public class TwentyThreeParameters {

        #region Properties

        /// <summary>
        /// Gets the ID of the video.
        /// </summary>
        public string VideoId { get; }

        /// <summary>
        /// Gets the token of the video.
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Gets the ID of the player, if specified.
        /// </summary>
        public string? PlayerId { get; }

        /// <summary>
        /// Gets whether embedded videos should automatically start playing.
        /// </summary>
        public bool? Autoplay { get; }

        /// <summary>
        /// Gets what should happen when a video ends.
        /// </summary>
        public TwentyThreeEndOn? EndOn { get; }

        #endregion

        #region Properties

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="json"/> object.
        /// </summary>
        /// <param name="json">The JSON object representing the parameters.</param>
        public TwentyThreeParameters(JObject json) {
            VideoId = json.GetString("videoId");
            Token = json.GetString("token");
            PlayerId = json.GetString("playerId");
            Autoplay = json.GetString("autoplay", x => x == null ? default(bool?) : StringUtils.ParseBoolean(x));
            EndOn = json.GetString("endOn", x => x == null ? default(TwentyThreeEndOn?) : EnumUtils.ParseEnum<TwentyThreeEndOn>(x));
        }

        #endregion

    }

}