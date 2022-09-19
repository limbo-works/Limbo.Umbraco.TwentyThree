using System;
using System.Linq;
using J2N.Collections.Generic;
using Limbo.Umbraco.TwentyThree.Models.Api;
using Limbo.Umbraco.TwentyThree.Models.Credentials;
using Limbo.Umbraco.TwentyThree.Models.Settings;
using Limbo.Umbraco.TwentyThree.Options;
using Limbo.Umbraco.TwentyThree.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Social.TwentyThree.Models.Photos;
using Skybrud.Social.TwentyThree.Models.Players;
using Skybrud.Social.TwentyThree.Models.Spots;
using Skybrud.Social.TwentyThree.Options.Photos;
using Skybrud.Social.TwentyThree.Options.Players;
using Skybrud.Social.TwentyThree.Options.Spots;
using Skybrud.Social.TwentyThree.Responses.Photos;
using Skybrud.Social.TwentyThree.Responses.Players;
using Skybrud.Social.TwentyThree.Responses.Spots;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;
using TwentyThreeThumbnail = Limbo.Umbraco.TwentyThree.Models.TwentyThreeThumbnail;

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
                TwentyThreeSpotOptions so => GetSpot(credentials!, so),
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

        public object GetSpots(Guid accountId, string? text = null, int limit = 0, int page = 1) {

            var credentials = _options.Value.Credentials.FirstOrDefault(x => x.Key == accountId);
            if (credentials == null) return NotFound("Account not found.");

            var http = _service.GetHttpService(credentials);

            TwentyThreeSpotListResponse response1 = http.Spots.GetList(new TwentyThreeGetSpotsOptions {
                Size = limit,
                Page = page
            });

            List<string> photoIds = new();
            foreach (var spot in response1.Body.Spots) {
                if (string.IsNullOrWhiteSpace(spot.SpotSelection)) continue;
                foreach (string selection in spot.SpotSelection.Split(' ')) {
                    if (!selection.StartsWith("photo:")) continue;
                    photoIds.Add(selection.Split(':')[1]);
                    break;
                }
            }

            Dictionary<string, TwentyThreePhoto> hest = new();

            foreach (var group in photoIds.Distinct().InGroupsOf(50)) {
                
                TwentyThreePhotoListResponse response2 = http.Photos.GetList(new TwentyThreeGetPhotosOptions {
                    Search = string.Join(" OR ", group),
                    Size = 50
                });

                foreach (var photo in response2.Body.Photos) {
                    hest[photo.PhotoId] = photo;
                }

            }

            return new {
                page = response1.Body.Page,
                limit = response1.Body.Size,
                total = response1.Body.TotalCount,
                pages = response1.Body.TotalCount == 0 ? 0 : Math.Ceiling((double) response1.Body.TotalCount / response1.Body.Size),
                site = new ApiSite(response1.Body.Site),
                spots = response1.Body.Spots.Select(x => {
                    TwentyThreePhoto? photo = null;
                    if (!string.IsNullOrWhiteSpace(x.SpotSelection)) {
                        foreach (string selection in x.SpotSelection.Split(' ')) {
                            if (!selection.StartsWith("photo:")) continue;
                            if (hest.TryGetValue(selection.Split(':')[1], out photo)) break;
                        }
                    }
                    return ToApiModel(x, photo);
                })
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

        private object GetSpot(TwentyThreeCredentials credentials, TwentyThreeSpotOptions options) {

            var http = _service.GetHttpService(credentials);

            // Get information about the spot
            TwentyThreeSpotListResponse response1 = http.Spots.GetList(new TwentyThreeGetSpotsOptions {
                SpotId = options.SpotId,
                Token = options.Token
            });

            // Get the first spot (if any)
            TwentyThreeSpot? spot = response1.Body.Spots.FirstOrDefault();
            if (spot == null) return NotFound("Spot not found.");

            // The spot is made up of one or more videos (aka photos), so we can get information about the first video
            // to find some thumbnails (seems to be the best approach for now)
            string? firstPhotoId = spot.SpotSelection
                .Split(' ')
                .Select(x => x.Split(':')[1])
                .FirstOrDefault();

            TwentyThreeThumbnail[] thumbnails;
            if (firstPhotoId.HasValue()) {
                try {

                    TwentyThreePhotoListResponse response2 = http.Photos.GetList(new TwentyThreeGetPhotosOptions {
                        PhotoId = firstPhotoId
                    });

                    // Get the thumbnails from the first video/photo (currently asuming that one video is returned)
                    thumbnails = response2.Body.Photos[0].Thumbnails.Select(x => new TwentyThreeThumbnail(options, x)).ToArray();

                } catch {
                    thumbnails = Array.Empty<TwentyThreeThumbnail>();
                }
            } else {
                thumbnails = Array.Empty<TwentyThreeThumbnail>();
            }

            return new ApiSpotDetails(options, credentials, spot, thumbnails, response1.Body.Site);

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

        private static object? ToApiModel(TwentyThreeSpot? spot, TwentyThreePhoto? photo) {
            if (spot == null) return null;
            if (photo != null) spot.JObject.Add("__thumbnails", JArray.FromObject(photo.Thumbnails.Select(x => new TwentyThreeThumbnail(photo, x))));
            return spot.JObject;
        }

        #endregion

    }

}