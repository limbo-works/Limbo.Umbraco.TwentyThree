using Microsoft.AspNetCore.Html;

namespace Limbo.Umbraco.TwentyThree.Models {

    /// <summary>
    /// Class with embed information about a TwentyThree spot.
    /// </summary>
    public class TwentyThreeSpotEmbed : TwentyThreeEmbed {

        /// <summary>
        /// Initializes a new instance from the specified <paramref name="details"/>.
        /// </summary>
        /// <param name="details">The details about the spot.</param>
        public TwentyThreeSpotEmbed(TwentyThreeSpotDetails details) {
            Html = new HtmlString(details.Data.IncludeHtml);
        }

    }

}