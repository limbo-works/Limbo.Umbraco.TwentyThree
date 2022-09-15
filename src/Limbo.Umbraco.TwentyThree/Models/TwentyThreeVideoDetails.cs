using System;
using System.Collections.Generic;
using Limbo.Umbraco.Video.Models.Videos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Social.TwentyThree.Models.Photos;

namespace Limbo.Umbraco.TwentyThree.Models {

    /// <summary>
    /// Class representing the details about a TwentyThree video.
    /// </summary>
    public class TwentyThreeVideoDetails : IVideoDetails {

        #region Properties

        /// <summary>
        /// Gets a reference to the raw video data as received from the TwentyThree API.
        /// </summary>
        [JsonIgnore]
        public TwentyThreePhoto Data { get; }

        /// <summary>
        /// Gets the title of the video.
        /// </summary>
        public string Title => Data.Title;

        /// <summary>
        /// Gets the duration of the video.
        /// </summary>
        public TimeSpan Duration => Data.VideoLength;

        /// <inheritdoc />
        public IEnumerable<IVideoThumbnail> Thumbnails { get; }

        /// <inheritdoc />
        public IEnumerable<IVideoFile> Files { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="json"/> object.
        /// </summary>
        /// <param name="json">The JSON object representing the details.</param>
        public TwentyThreeVideoDetails(JObject json) {
            Data = json.GetString("_data", x => JsonUtils.ParseJsonObject(x, TwentyThreePhoto.Parse));
            Thumbnails = Array.Empty<IVideoThumbnail>();
            Files = Array.Empty<IVideoFile>();
        }

        #endregion

    }

}