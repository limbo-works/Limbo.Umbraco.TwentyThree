using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Social.TwentyThree.Models.Spots;

namespace Limbo.Umbraco.TwentyThree.Models {

    /// <summary>
    /// Class representing the details about a TwentyThree spot.
    /// </summary>
    public class TwentyThreeSpotDetails : TwentyThreeDetails {

        #region Properties

        /// <summary>
        /// Gets a reference to the raw video spot as received from the TwentyThree API.
        /// </summary>
        [JsonIgnore]
        public TwentyThreeSpot Data { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="json"/> object.
        /// </summary>
        /// <param name="json">The JSON object representing the details.</param>
        /// <param name="thumbnails">An array of thumbnails.</param>
        public TwentyThreeSpotDetails(JObject json, IReadOnlyList<TwentyThreeThumbnail> thumbnails) {
            Data = json.GetString("_data", x => JsonUtils.ParseJsonObject(x, TwentyThreeSpot.Parse));
            Id = Data.SpotId;
            Title = Data.SpotName;
            Thumbnails = thumbnails;
        }

        #endregion

    }

}