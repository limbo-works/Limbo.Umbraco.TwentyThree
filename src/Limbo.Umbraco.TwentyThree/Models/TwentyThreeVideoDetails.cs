using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Social.TwentyThree.Models.Photos;

namespace Limbo.Umbraco.TwentyThree.Models {

    /// <summary>
    /// Class representing the details about a TwentyThree video.
    /// </summary>
    public class TwentyThreeVideoDetails : TwentyThreeDetails {

        #region Properties

        /// <summary>
        /// Gets a reference to the raw video data as received from the TwentyThree API.
        /// </summary>
        [JsonIgnore]
        public TwentyThreePhoto Data { get; }

        /// <summary>
        /// Gets the width of the video.
        /// </summary>
        [JsonProperty("width")]
        public int Width => Data.JObject.GetInt32("video_original_width");

        /// <summary>
        /// Gets the height of the video.
        /// </summary>
        [JsonProperty("height")]
        public int Height => Data.JObject.GetInt32("video_original_height");

        /// <summary>
        /// Gets the duration of the video.
        /// </summary>
        [JsonProperty("duration")]
        public new TimeSpan Duration {
            get => base.Duration!.Value;
            set => base.Duration = value;
        }

        /// <summary>
        /// Returns a list of <see cref="TwentyThreeThumbnail"/> representing the video formats of the video.
        /// </summary>
        [JsonProperty("files")]
        public new IReadOnlyList<TwentyThreeVideoFile> Files {
            get => base.Files!;
            set => base.Files = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="json"/> object.
        /// </summary>
        /// <param name="json">The JSON object representing the details.</param>
        public TwentyThreeVideoDetails(JObject json) {
            Data = json.GetString("_data", x => JsonUtils.ParseJsonObject(x, TwentyThreePhoto.Parse));
            Id = Data.PhotoId;
            Title = Data.Title;
            Duration = Data.VideoLength;
            Thumbnails = Data.Thumbnails.Select(x => new TwentyThreeThumbnail(Data, x)).ToArray();
            Files = Data.VideoFormats.Select(x => new TwentyThreeVideoFile(Data, x)).ToArray();
        }

        #endregion

    }
}