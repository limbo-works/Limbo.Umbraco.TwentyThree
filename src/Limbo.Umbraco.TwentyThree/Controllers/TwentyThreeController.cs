using System;
using System.Linq;
using Limbo.Umbraco.TwentyThree.Models.Api;
using Limbo.Umbraco.TwentyThree.Models.Credentials;
using Limbo.Umbraco.TwentyThree.Models.Settings;
using Limbo.Umbraco.TwentyThree.Options;
using Limbo.Umbraco.TwentyThree.Services;
using Microsoft.Extensions.Options;
using Skybrud.Social.TwentyThree.Models.Photos;
using Skybrud.Social.TwentyThree.Models.Players;
using Skybrud.Social.TwentyThree.Options.Photos;
using Skybrud.Social.TwentyThree.Options.Players;
using Skybrud.Social.TwentyThree.Responses.Photos;
using Skybrud.Social.TwentyThree.Responses.Players;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

// ReSharper disable RedundantAssignment

#pragma warning disable 1591

namespace Limbo.Umbraco.TwentyThree.Controllers {

    [PluginController("Limbo")]
    public class TwentyThreeController : UmbracoAuthorizedApiController {

        private readonly IOptions<TwentyThreeSettings> _options;
        private readonly TwentyThreeService _service;

        public TwentyThreeController(IOptions<TwentyThreeSettings> options, TwentyThreeService service) {
            _options = options;
            _service = service;
        }

        #region Public API methods

        /// <summary>
        /// Returns information about the video with the specified <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The video source (URL or embed code).</param>
        /// <returns>Information about the video matching <paramref name="source"/>.</returns>
        public object GetVideo(string? source) {

            // Get the "source" parameter from either GET or POST
            source = HttpContext.Request.Query["source"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(source) && HttpContext.Request.HasFormContentType) {
                source = HttpContext.Request.Form["source"].FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(source)) return BadRequest("No URL or embed code specified.");

            if (!_service.IsMatch(source, out ITwentyThreeOptions? options)) return BadRequest("Invalid URL or embed code specified.");

            if (!_service.TryGetCredentials(options!.Domain, out TwentyThreeCredentials? credentials)) return BadRequest($"No or invalid configuration found for the '{options.Domain}' domain.");

            return options switch {
                TwentyThreeVideoOptions vo => GetVideo(credentials!, vo),
                _ => BadRequest($"Unknown type {options.GetType()}.")
            };

        }

        ///// <summary>
        ///// Returns information about the video with the specified <paramref name="source"/>.
        ///// </summary>
        ///// <param name="source">The video source (URL or embed code).</param>
        ///// <returns>Information about the video matching <paramref name="source"/>.</returns>
        //public object PostVideo([FromBody] string? source) {
        //    return GetVideo(source);
        //}

        /// <summary>
        /// Returns a list of all configured TwentyThree accounts (credentials).
        /// </summary>
        /// <returns>A list of accounts.</returns>
        public object GetAccounts() {
            return _options.Value.Credentials.Select(ToApiModel);
        }

        /// <summary>
        /// Returns a list of videos of the account with the specified <paramref name="accountId"/>.
        /// </summary>
        /// <param name="accountId">The GUID ID of the account.</param>
        /// <param name="text">The text to search for.</param>
        /// <param name="limit">The maximum amount of videos to return for each page.</param>
        /// <param name="page">The page to be returned.</param>
        /// <returns>A list of vídeos.</returns>
        public object GetVideos(Guid accountId, string? text = null, int limit = 0, int page = 1) {

            var credentials = _options.Value.Credentials.FirstOrDefault(x => x.Key == accountId);
            if (credentials == null) return NotFound("Account not found.");

            var http = _service.GetHttpService(credentials);

            TwentyThreePhotoListResponse response = http.Photos.GetList(new TwentyThreeGetPhotosOptions {
                Search = text,
                Size = limit,
                Page = page
            });

            return new {
                page = response.Body.Page,
                limit = response.Body.Size,
                total = response.Body.TotalCount,
                pages = response.Body.TotalCount == 0 ? 0 : Math.Ceiling((double) response.Body.TotalCount / response.Body.Size),
                site = new ApiSite(response.Body.Site),
                videos = response.Body.Photos.Select(ToApiModel)
            };

        }

        public object GetPlayers(Guid credentialsId) {

            // Find the credentials
            var credentials = _options.Value.Credentials.FirstOrDefault(x => x.Key == credentialsId);
            if (credentials == null) return NotFound("Account not found.");

            // Get a reference to the HTTP service
            var http = _service.GetHttpService(credentials);

            // Request the first 200 players from the ID (ideally there shouldn't be more)
            var response = http.Players.GetList(new TwentyThreeGetPlayersOptions {
                Size = 200
            });

            // Return the players
            return response.Body.Players.Select(ToApiModel);

        }

        #endregion

        #region Private methods

        private object GetVideo(TwentyThreeCredentials credentials, TwentyThreeVideoOptions options) {

            var http = _service.GetHttpService(credentials);

            // Get information about the video from the TwentyThree API
            TwentyThreePhotoListResponse response1 = http.Photos.GetList(new TwentyThreeGetPhotosOptions {
                PhotoId = options.VideoId
            });

            // Get the first video of the response (if any)
            TwentyThreePhoto? video = response1.Body.Photos.FirstOrDefault();
            if (video == null) return NotFound("Video not found.");

            // Get a list of the first 200 players from the TwentyThree API
            TwentyThreePlayerListResponse? response2 = http.Players.GetList(new TwentyThreeGetPlayersOptions {
                Size = 200
            });

            // Get the selected player from the response
            TwentyThreePlayer? player = response2.Body.Players
                .First(x => options.PlayerId is null ? x.IsDefault : x.PlayerId == options.PlayerId);

            return new ApiVideoDetails(options, credentials, video, player, response1.Body.Site);

        }

        private static ApiCredentials ToApiModel(TwentyThreeCredentials credentials) {
            return new ApiCredentials(credentials);
        }

        private static ApiPlayer? ToApiModel(TwentyThreePlayer? player) {
            return player == null ? null : new ApiPlayer(player);
        }

        private static object? ToApiModel(TwentyThreePhoto? photo) {
            return photo?.JObject;
        }

        #endregion

    }

}