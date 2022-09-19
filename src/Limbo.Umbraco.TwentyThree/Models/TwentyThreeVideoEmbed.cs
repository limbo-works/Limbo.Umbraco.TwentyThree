using Limbo.Umbraco.TwentyThree.PropertyEditors;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Essentials.Strings.Extensions;

namespace Limbo.Umbraco.TwentyThree.Models {

    /// <summary>
    /// Class with embed information about a TwentyThree video.
    /// </summary>
    public class TwentyThreeVideoEmbed : TwentyThreeEmbed {

        #region Properties

        /// <summary>
        /// Gets the token of the video.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; }

        /// <summary>
        /// Gets the ID of the player, if specified.
        /// </summary>
        [JsonProperty("playerId")]
        public string? PlayerId { get; }

        /// <summary>
        /// Gets whether embedded videos should automatically start playing.
        /// </summary>
        [JsonProperty("autoplay")]
        public bool? Autoplay { get; set; }

        /// <summary>
        /// Gets what should happen when a video ends.
        /// </summary>
        [JsonProperty("endOn")]
        public TwentyThreeEndOn? EndOn { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance from the specified <paramref name="json"/> object, <paramref name="video"/> and <paramref name="parameters"/>.
        /// </summary>
        /// <param name="json">The JSON object representing the embed information.</param>
        /// <param name="video">The video.</param>
        /// <param name="parameters">The video parameters.</param>
        /// <param name="config">The configuration of the <see cref="TwentyThreeEditor"/> data type.</param>
        public TwentyThreeVideoEmbed(JObject json, TwentyThreeVideoDetails video, TwentyThreeParameters parameters, TwentyThreeConfiguration? config) {

            Token = video.Data.Token;
            PlayerId = parameters.PlayerId;
            Autoplay = ParseAutoplay(json, parameters, config);
            EndOn = ParseEndOn(json, parameters, config);
            
            string domain = video.Data.AbsoluteUrl.Split('/')[2];

            string playerId = string.IsNullOrWhiteSpace(parameters.PlayerId) ? "v" : parameters.PlayerId;

            string embedUrl = $"//{domain}/{playerId}.ihtml/player.html?token={video.Data.Token}&source=embed&photo%5fid={video.Data.PhotoId}";
            if (Autoplay != null) embedUrl += $"&autoPlay={(Autoplay.Value ? "1" : "0")}";
            if (EndOn != null) embedUrl += $"&endOn={EndOn.Value.ToLower()}";

            Html = new HtmlString($"<div style=\"width:100%; height:0; position: relative; padding-bottom:33.333333333333336%\"><iframe src=\"{embedUrl}\" style=\"width:100%; height:100%; position: absolute; top: 0; left: 0;\" frameborder=\"0\" border=\"0\" scrolling=\"no\" mozallowfullscreen=\"1\" webkitallowfullscreen=\"1\" allowfullscreen=\"1\" allow=\"autoplay; fullscreen\"></iframe></div>");

        }

        #endregion

        #region Static methods 

        private static bool? ParseAutoplay(JObject json, TwentyThreeParameters parameters, TwentyThreeConfiguration? config) {

            if (config != null && config.Autoplay != TwentyThreeAutoplay.Inherit) {
                return config.Autoplay == TwentyThreeAutoplay.Enabled;
            }
            
            string value = json.GetString("autoplay");
            return value switch {
                "enabled" => true,
                "disabled" => false,
                _ => parameters.Autoplay
            };

        }

        private static TwentyThreeEndOn? ParseEndOn(JObject json, TwentyThreeParameters parameters, TwentyThreeConfiguration? config) {
            
            if (config != null && config.EndOn != TwentyThreeEndOn.Inherit) {
                return config.EndOn;
            }

            JToken? value = json.GetValue("endOn");
            return value?.Type switch {
                JTokenType.String => EnumUtils.TryParseEnum(value.ToObject<string>(), out TwentyThreeEndOn result) && result != TwentyThreeEndOn.Inherit ? result : parameters.EndOn,
                _ => parameters.EndOn
            };

        }

        #endregion

    }

}