using System;
using System.Collections.Generic;
using System.Linq;
using Limbo.Umbraco.TwentyThree.Options;
using Limbo.Umbraco.Video.Models.Videos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Social.TwentyThree.Models.Photos;
using Skybrud.Social.TwentyThree.Options.Spots;

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
        /// Gets the ID of the video.
        /// </summary>
        [JsonProperty("id")]
        public string Id => Data.PhotoId;

        /// <summary>
        /// Gets the title of the video.
        /// </summary>
        [JsonProperty("title")]
        public string Title => Data.Title;

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
        public TimeSpan Duration => Data.VideoLength;

        /// <summary>
        /// Returns a list of <see cref="TwentyThreeThumbnail"/> representing the thumbnails of the video.
        /// </summary>
        [JsonProperty("thumbnails")]
        public IEnumerable<TwentyThreeThumbnail> Thumbnails { get; }

        /// <summary>
        /// Returns a list of <see cref="TwentyThreeThumbnail"/> representing the video formats of the video.
        /// </summary>
        [JsonProperty("files")]
        public IEnumerable<TwentyThreeVideoFile> Files { get; }

        IEnumerable<IVideoThumbnail> IVideoDetails.Thumbnails => Thumbnails;

        IEnumerable<IVideoFile> IVideoDetails.Files => Files;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="json"/> object.
        /// </summary>
        /// <param name="json">The JSON object representing the details.</param>
        public TwentyThreeVideoDetails(JObject json) {
            Data = json.GetString("_data", x => JsonUtils.ParseJsonObject(x, TwentyThreePhoto.Parse));
            Thumbnails = Data.Thumbnails.Select(x => new TwentyThreeThumbnail(Data, x));
            Files = Data.VideoFormats.Select(x => new TwentyThreeVideoFile(Data, x));
        }

        #endregion

    }

    public class TwentyThreeThumbnail : IVideoThumbnail {

        [JsonProperty("alias")]
        public string Alias { get; }

        [JsonProperty("width")]
        public int Width { get; }

        [JsonProperty("height")]
        public int Height { get; }

        [JsonProperty("url")]
        public string Url { get; }

        public TwentyThreeThumbnail(TwentyThreeSpotOptions options, Skybrud.Social.TwentyThree.Models.Photos.TwentyThreeThumbnail thumbnail) {
            Alias = thumbnail.Alias;
            Width = thumbnail.Width;
            Height = thumbnail.Height;
            Url = $"{options.Scheme}://{options.Domain}{thumbnail.Url}";
        }

        public TwentyThreeThumbnail(TwentyThreePhoto video, Skybrud.Social.TwentyThree.Models.Photos.TwentyThreeThumbnail thumbnail) {
            Alias = thumbnail.Alias;
            Width = thumbnail.Width;
            Height = thumbnail.Height;
            Url = $"{video.AbsoluteUrl.Split('/').Take(2).Join("/")}{thumbnail.Url}";
        }

    }

    public class TwentyThreeVideoFile : IVideoFile {

        [JsonProperty("alias")]
        public string Alias { get; }

        [JsonProperty("width")]
        public int Width { get; }

        [JsonProperty("height")]
        public int Height { get; }

        [JsonProperty("url")]
        public string Url { get; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string? Type { get; }

        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public long? Size { get; }

        public TwentyThreeVideoFile(TwentyThreePhoto video, TwentyThreeVideoFormat format) {
            Alias = format.Alias;
            Width = format.Width;
            Height = format.Height;
            Url = $"{video.AbsoluteUrl.Split('/').Take(2).Join("/")}{format.Url}";
            Size = format.Size;
        }

    }

}