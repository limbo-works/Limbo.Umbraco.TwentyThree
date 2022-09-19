using System;
using System.Collections.Generic;
using Limbo.Umbraco.Video.Models.Videos;
using Newtonsoft.Json;
using Skybrud.Essentials.Json.Converters.Time;

namespace Limbo.Umbraco.TwentyThree.Models {

    /// <summary>
    /// Class representing the details about a TwentyThree video or spot.
    /// </summary>
    public abstract class TwentyThreeDetails : IVideoDetails {

        /// <summary>
        /// Gets the ID of the video.
        /// </summary>
        [JsonProperty("id", Order = -99)]
        public string Id { get; protected set; } = null!;

        /// <summary>
        /// Gets the title of the video.
        /// </summary>
        [JsonProperty("title", Order = -98)]
        public string Title { get; protected set; } = null!;

        /// <summary>
        /// Gets the duration of the video.
        /// </summary>
        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(TimeSpanSecondsConverter))]
        public TimeSpan? Duration { get; protected set; }

        /// <summary>
        /// Returns a list of <see cref="TwentyThreeThumbnail"/> representing the thumbnails of the video.
        /// </summary>
        [JsonProperty("thumbnails", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<TwentyThreeThumbnail> Thumbnails { get; protected set; } = null!;

        /// <summary>
        /// Returns a list of <see cref="TwentyThreeThumbnail"/> representing the video formats of the video.
        /// </summary>
        [JsonProperty("files", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<TwentyThreeVideoFile>? Files { get; protected set; }

        IEnumerable<IVideoThumbnail> IVideoDetails.Thumbnails => Thumbnails;

        IEnumerable<IVideoFile>? IVideoDetails.Files => Files;

    }

}