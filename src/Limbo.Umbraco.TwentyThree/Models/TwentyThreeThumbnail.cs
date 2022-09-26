using Limbo.Umbraco.TwentyThree.Options;
using Limbo.Umbraco.Video.Models.Videos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Social.TwentyThree.Models.Photos;

namespace Limbo.Umbraco.TwentyThree.Models {

    /// <summary>
    /// Class representing a thumbnail for a TwentyThree video.
    /// </summary>
    public class TwentyThreeThumbnail : IVideoThumbnail {

        #region Properties

        /// <summary>
        /// Gets the alias of the thumbnail.
        /// </summary>
        [JsonProperty("alias")]
        public string Alias { get; }

        /// <summary>
        /// Gets the width of the thumbnail.
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; }

        /// <summary>
        /// Gets the height of the thumbnail.
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; }

        /// <summary>
        /// Gets the URL of the thumbnail.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new thumbnail based on the specified <paramref name="thumbnail"/>.
        /// </summary>
        /// <param name="thumbnail">The thumbnail as received from the TwentyThree API.</param>
        public TwentyThreeThumbnail(Skybrud.Social.TwentyThree.Models.Photos.TwentyThreeThumbnail thumbnail) {
            Alias = thumbnail.Alias;
            Width = thumbnail.Width;
            Height = thumbnail.Height;
            Url = thumbnail.Url;
        }

        /// <summary>
        /// Initializes a new thumbnail based on the specified <paramref name="thumbnail"/>.
        /// </summary>
        /// <param name="options">The options about the spot.</param>
        /// <param name="thumbnail">The thumbnail as received from the TwentyThree API.</param>
        public TwentyThreeThumbnail(TwentyThreeSpotOptions options, Skybrud.Social.TwentyThree.Models.Photos.TwentyThreeThumbnail thumbnail) {
            Alias = thumbnail.Alias;
            Width = thumbnail.Width;
            Height = thumbnail.Height;
            Url = $"{options.Scheme}://{options.Domain}{thumbnail.Url}";
        }

        /// <summary>
        /// Initializes a new thumbnail based on the specified <paramref name="thumbnail"/>.
        /// </summary>
        /// <param name="video">The video.</param>
        /// <param name="thumbnail">The thumbnail as received from the TwentyThree API.</param>
        public TwentyThreeThumbnail(TwentyThreePhoto video, Skybrud.Social.TwentyThree.Models.Photos.TwentyThreeThumbnail thumbnail) {

            string scheme = video.AbsoluteUrl.Split(':')[0];
            string domain = video.AbsoluteUrl.Split('/')[2];

            Alias = thumbnail.Alias;
            Width = thumbnail.Width;
            Height = thumbnail.Height;
            Url = $"{scheme}://{domain}{thumbnail.Url}";
        }

        private TwentyThreeThumbnail(string alias, int width, int height, string url) {
            Alias = alias;
            Width = width;
            Height = height;
            Url = url;
        }

        #endregion

        #region Static methods

        internal static TwentyThreeThumbnail Parse(JObject json) {
            string alias = json.GetString("alias");
            int width = json.GetInt32("width");
            int height = json.GetInt32("height");
            string url = json.GetString("url");
            return new TwentyThreeThumbnail(alias, width, height, url);
        }

        #endregion

    }

}