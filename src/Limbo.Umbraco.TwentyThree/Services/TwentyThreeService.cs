using System.Linq;
using System.Text.RegularExpressions;
using Limbo.Umbraco.TwentyThree.Models;
using Limbo.Umbraco.TwentyThree.Models.Credentials;
using Limbo.Umbraco.TwentyThree.Models.Settings;
using Limbo.Umbraco.TwentyThree.Options;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Strings;
using Skybrud.Social.TwentyThree;
using Skybrud.Social.TwentyThree.OAuth;

namespace Limbo.Umbraco.TwentyThree.Services {

    /// <summary>
    /// Service class used throughout the implementation of this package.
    /// </summary>
    public class TwentyThreeService {

        private readonly TwentyThreeSettings _settings;

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="options"/>.
        /// </summary>
        /// <param name="options">Ther options.</param>
        public TwentyThreeService(IOptions<TwentyThreeSettings> options) {
            _settings = options.Value;
        }

        /// <summary>
        /// Attempts to get the ccredentials matching the specified <paramref name="input"/> (ID or domain).
        /// </summary>
        /// <param name="input">The input (ID or domain).</param>
        /// <param name="result">When this method returns, holds the <see cref="TwentyThreeCredentials"/> if successful; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
        public bool TryGetCredentials(string input, out TwentyThreeCredentials? result) {

            foreach (var cred in _settings.Credentials) {

                if (cred.Key.ToString() == input || cred.Domains.Contains(input)) {
                    result = cred;
                    return true;
                }

            }

            result = null;
            return false;

        }

        /// <summary>
        /// Returns whether the specified <paramref name="source"/> matches a TwentyThree video URL or embed code.
        /// </summary>
        /// <param name="source">A source with either a video URL or embed code.</param>
        /// <param name="options">When this method returns, holds the <see cref="ITwentyThreeOptions"/> if successful; otherwise, <c>null</c>.</param>
        /// <returns></returns>
        public bool IsMatch(string source, out ITwentyThreeOptions? options) {

            options = null;

            Match m1 = Regex.Match(source, "^(http|https)://([a-zA-Z0-9-\\.]+)/manage/video/([0-9]+)$", RegexOptions.IgnoreCase);
            Match m2 = Regex.Match(source, "(http:|https:|)//(.+?)/(v|[0-9]+)\\.ihtml/player\\.html\\?token=([a-z0-9]+)&source=embed&photo%5fid=([0-9]+)");
            Match m3 = Regex.Match(source, "<script src=\"(http|https)://(.+?)/spot/([0-9]+)/([a-z0-9]+)/include\\.js");
            
            // From manage URL
            if (m1.Success) {

                string scheme = m1.Groups[1].Value;
                string domain = m1.Groups[2].Value;
                string videoId = m1.Groups[3].Value;

                options = new TwentyThreeVideoOptions(source, scheme, domain, videoId, null, null);

                return true;

            }

            // From <iframe>
            if (m2.Success) {

                string scheme = m2.Groups[1].Value.Trim(':');
                string domain = m2.Groups[2].Value;
                string? playerId = m2.Groups[3].Value;
                string token = m2.Groups[4].Value;
                string videoId = m2.Groups[5].Value;

                if (playerId == "v") playerId = null;

                bool? autoplay = null;
                TwentyThreeEndOn? endOn = null;

                if (RegexUtils.IsMatch(source, "autoPlay=(1|0)", out string result)) {
                    autoplay = result == "1";
                }

                if (RegexUtils.IsMatch(source, "endOn=([a-z]+)", out result) && EnumUtils.TryParseEnum(result, out TwentyThreeEndOn result2)) {
                    endOn = result2;
                }

                if (string.IsNullOrWhiteSpace(scheme)) scheme = "https";

                options = new TwentyThreeVideoOptions(source, scheme, domain, videoId, token, playerId, autoplay, endOn);

                return true;

            }

            // From spot <script>
            if (m3.Success) {

                string scheme = m3.Groups[1].Value;
                string domain = m3.Groups[2].Value;
                string spotId = m3.Groups[3].Value;
                string token = m2.Groups[4].Value;

                options = new TwentyThreeSpotOptions(scheme, domain, spotId, token);

                return true;

            }

            return false;

        }

        /// <summary>
        /// Returns a new <see cref="TwentyThreeHttpService"/> instance for the specified <paramref name="credentials"/>.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns>An instance of <see cref="TwentyThreeHttpService"/>.</returns>
        public TwentyThreeHttpService GetHttpService(TwentyThreeCredentials credentials) {
            
            TwentyThreeOAuthClient client = new() {
                Domain = credentials.Domains[0],
                ConsumerKey = credentials.ConsumerKey,
                ConsumerSecret = credentials.ConsumerSecret,
                Token = credentials.AccessToken,
                TokenSecret = credentials.AccessTokenSecret
            };

            return TwentyThreeHttpService.CreateFromOAuthClient(client);

        }

    }

}