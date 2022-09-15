using Microsoft.AspNetCore.Html;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Essentials.Strings.Extensions;

namespace Limbo.Umbraco.TwentyThree.Models {

    /// <summary>
    /// Class with embed information about a TwentyThree video.
    /// </summary>
    public class TwentyThreeEmbed {

        #region Properties

        /// <summary>
        /// Gets the embed HTML for the video.
        /// </summary>
        public IHtmlContent Html { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance from the specified <paramref name="json"/> object, <paramref name="video"/> and <paramref name="parameters"/>.
        /// </summary>
        /// <param name="json">The JSON object representing the embed information.</param>
        /// <param name="video">The video.</param>
        /// <param name="parameters">The video parameters.</param>
        public TwentyThreeEmbed(JObject json, TwentyThreeVideoDetails video, TwentyThreeParameters parameters) {
            
            bool? autoplay = ParseAutoplay(json, parameters);
            TwentyThreeEndOn? endOn = ParseEndOn(json, parameters);
            
            string domain = video.Data.AbsoluteUrl.Split('/')[2];

            string playerId = string.IsNullOrWhiteSpace(parameters.PlayerId) ? "v" : parameters.PlayerId;

            string embedUrl = $"//{domain}/{playerId}.ihtml/player.html?token={video.Data.Token}&source=embed&photo%5fid={video.Data.PhotoId}";
            if (autoplay != null) embedUrl += $"&autoPlay={(autoplay.Value ? "1" : "0")}";
            if (endOn != null) embedUrl += $"&endOn={endOn.Value.ToLower()}";

            Html = new HtmlString($"<div style=\"width:100%; height:0; position: relative; padding-bottom:33.333333333333336%\"><iframe src=\"{embedUrl}\" style=\"width:100%; height:100%; position: absolute; top: 0; left: 0;\" frameborder=\"0\" border=\"0\" scrolling=\"no\" mozallowfullscreen=\"1\" webkitallowfullscreen=\"1\" allowfullscreen=\"1\" allow=\"autoplay; fullscreen\"></iframe></div>");

        }

        #endregion

        #region Static methods 

        private static bool? ParseAutoplay(JObject json, TwentyThreeParameters parameters) {
            string value = json.GetString("autoplay");
            return value switch {
                "enabled" => true,
                "disabled" => false,
                _ => parameters.Autoplay
            };
        }

        private static TwentyThreeEndOn? ParseEndOn(JObject json, TwentyThreeParameters parameters) {
            JToken? value = json.GetValue("endOn");
            return value?.Type switch {
                JTokenType.String => EnumUtils.TryParseEnum(value.ToObject<string>(), out TwentyThreeEndOn result) ? result : parameters.EndOn,
                _ => null
            };
        }

        #endregion

    }
}