using Limbo.Umbraco.Video.Models.Providers;

namespace Limbo.Umbraco.TwentyThree.Models {

    /// <summary>
    /// Class with limited information about a video provider.
    /// </summary>
    public class TwentyThreeProvider : VideoProvider {

        /// <summary>
        /// Gets a reference to a <see cref="TwentyThreeProvider"/> instance.
        /// </summary>
        public static readonly TwentyThreeProvider Default = new();

        private TwentyThreeProvider() : base("twentythree", "TwentyThree") { }

    }

}