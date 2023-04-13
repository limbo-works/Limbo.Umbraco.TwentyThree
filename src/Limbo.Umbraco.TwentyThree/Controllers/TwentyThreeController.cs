using System;
using System.Collections.Generic;
using System.Linq;
using Limbo.Umbraco.TwentyThree.Models.Api;
using Limbo.Umbraco.TwentyThree.Models.Credentials;
using Limbo.Umbraco.TwentyThree.Models.Settings;
using Limbo.Umbraco.TwentyThree.Options;
using Limbo.Umbraco.TwentyThree.PropertyEditors;
using Limbo.Umbraco.TwentyThree.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Social.TwentyThree.Exceptions;
using Skybrud.Social.TwentyThree.Models.Photos;
using Skybrud.Social.TwentyThree.Models.Players;
using Skybrud.Social.TwentyThree.Models.Sites;
using Skybrud.Social.TwentyThree.Models.Spots;
using Skybrud.Social.TwentyThree.Options.Photos;
using Skybrud.Social.TwentyThree.Options.Players;
using Skybrud.Social.TwentyThree.Options.Spots;
using Skybrud.Social.TwentyThree.Responses.Photos;
using Skybrud.Social.TwentyThree.Responses.Players;
using Skybrud.Social.TwentyThree.Responses.Spots;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;
using TwentyThreeThumbnail = Limbo.Umbraco.TwentyThree.Models.TwentyThreeThumbnail;

// ReSharper disable RedundantAssignment

#pragma warning disable 1591

namespace Limbo.Umbraco.TwentyThree.Controllers {

    [PluginController("Limbo")]
    public class TwentyThreeController : UmbracoAuthorizedApiController {

        private readonly ILogger<TwentyThreeController> _logger;
        private readonly IDataTypeService _dataTypeService;
        private readonly IOptions<TwentyThreeSettings> _options;
        private readonly TwentyThreeService _service;

        public TwentyThreeController(ILogger<TwentyThreeController> logger, IDataTypeService dataTypeService, IOptions<TwentyThreeSettings> options, TwentyThreeService service) {
            _logger = logger;
            _dataTypeService = dataTypeService;
            _options = options;
            _service = service;
        }

        #region Public API methods

        /// <summary>
        /// Returns information about the video or spot with the specified <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The video source (URL or embed code).</param>
        /// <param name="dataTypeKey">The key of the underlying data type, if any.</param>
        /// <returns>Information about the video matching <paramref name="source"/>.</returns>
        public object GetVideo(string? source, Guid? dataTypeKey = null) {

            // Get the "source" parameter from either GET or POST
            source = HttpContext.Request.Query["source"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(source) && HttpContext.Request.HasFormContentType) {
                source = HttpContext.Request.Form["source"].FirstOrDefault();
            }

            // Check whether a "source" was specified
            if (string.IsNullOrWhiteSpace(source)) return BadRequest("No URL or embed code specified.");

            // Does "source" match a valid TwentyThree URL or embed code?
            if (!_service.IsMatch(source, out ITwentyThreeOptions? options)) return BadRequest("Invalid URL or embed code specified.");

            // Do we have valid credentials for the TwentyThree site/domain?
            if (!_service.TryGetCredentials(options!.Domain, out TwentyThreeCredentials? credentials)) return BadRequest($"No or invalid configuration found for the '{options.Domain}' domain.");

            // Get a reference to the data type (if specified)
            IDataType? dataType = dataTypeKey == null ? null : _dataTypeService.GetDataType(dataTypeKey.Value);
            TwentyThreeConfiguration? config = dataType?.Configuration as TwentyThreeConfiguration;

            // Handle the different options types
            return options switch {
                TwentyThreeVideoOptions vo => GetVideo(credentials!, vo, config),
                TwentyThreeSpotOptions so => GetSpot(credentials!, so, config),
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

            TwentyThreePhotoList list;

            try {

                TwentyThreePhotoListResponse response = http.Photos.GetList(new TwentyThreeGetPhotosOptions {
                    Search = text,
                    Size = limit,
                    Page = page
                });

                list = response.Body;

            } catch (Exception ex) {

                _logger.LogError(ex, "Failed getting list of videos from the TwentyThree API.");

                return InternalServerError("Failed getting list of videos from the TwentyThree API.");

            }

            return new {
                page = list.Page,
                limit = list.Size,
                total = list.TotalCount,
                pages = list.TotalCount == 0 ? 0 : Math.Ceiling((double) list.TotalCount / list.Size),
                site = new ApiSite(list.Site),
                videos = list.Photos.Select(ToApiModel)
            };

        }

        public object GetSpots(Guid accountId, string? text = null, int limit = 0, int page = 1) {

            var credentials = _options.Value.Credentials.FirstOrDefault(x => x.Key == accountId);
            if (credentials == null) return NotFound("Account not found.");

            var http = _service.GetHttpService(credentials);

            TwentyThreeSpotList list;

            try {

                var response = http.Spots.GetList(new TwentyThreeGetSpotsOptions {
                    Size  = limit,
                    Page = page
                });

                list = response.Body;

            } catch (Exception ex) {

                _logger.LogError(ex, "Failed getting list of spots from the TwentyThree API.");

                return InternalServerError("Failed getting list of spots from the TwentyThree API.");

            }

            List<string> photoIds = new();
            foreach (var spot in list.Spots) {
                if (string.IsNullOrWhiteSpace(spot.SpotSelection)) continue;
                foreach (string selection in spot.SpotSelection.Split(' ')) {
                    if (!selection.StartsWith("photo:")) continue;
                    photoIds.Add(selection.Split(':')[1]);
                    break;
                }
            }

            Dictionary<string, TwentyThreePhoto> hest = new();

            foreach (var group in photoIds.Distinct().InGroupsOf(50)) {

                try {

                    TwentyThreePhotoListResponse response2 = http.Photos.GetList(new TwentyThreeGetPhotosOptions {
                        Search = string.Join(" OR ", group), Size = 50
                    });

                    foreach (var photo in response2.Body.Photos) {
                        hest[photo.PhotoId] = photo;
                    }

                } catch {

                    // Ignore

                }

            }

            return new {
                page = list.Page,
                limit = list.Size,
                total = list.TotalCount,
                pages = list.TotalCount == 0 ? 0 : Math.Ceiling((double) list.TotalCount / list.Size),
                site = new ApiSite(list.Site),
                spots = list.Spots.Select(x => {
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

            IReadOnlyList<TwentyThreePlayer> players;

            try {

                // Request the first 200 players from the ID (ideally there shouldn't be more)
                var response = http.Players.GetList(new TwentyThreeGetPlayersOptions {
                    Size = 200
                });

                // Get the players from the response body
                players = response.Body.Players;

            } catch (Exception ex) {

                _logger.LogError(ex, "Failed getting list of players from the TwentyThree API.");

                return InternalServerError("Failed getting list of players from the TwentyThree API.");

            }

            // Return the players
            return players.Select(ToApiModel);

        }

        #endregion

        #region Private methods

        private object GetVideo(TwentyThreeCredentials credentials, TwentyThreeVideoOptions options, TwentyThreeConfiguration? config) {

            if (config is { AllowVideos: false }) return BadRequest("Videos are not allowed.");

            var http = _service.GetHttpService(credentials);

            TwentyThreePhoto? video = null;
            TwentyThreeSite? site = null;
            TwentyThreePlayer? player = null;

            try {

                // Get information about the video from the TwentyThree API
                TwentyThreePhotoListResponse response = http.Photos.GetList(new TwentyThreeGetPhotosOptions {
                    PhotoId = options.VideoId
                });

                // Get the first video of the response (if any)
                video = response.Body.Photos.FirstOrDefault();
                if (video == null) return NotFound("Video not found.");

                // Get a reference to the current site
                site = response.Body.Site;

            } catch (TwentyThreeHttpException ex) {

                if (ex.Error.Code == "photo_not_found") return NotFound("Video not found.");

                _logger.LogError(ex, "Failed getting video information from the TwentyThree API for video with {VideoId}.", options.VideoId);

                return InternalServerError("Failed getting video information from the TwentyThree API.");


            } catch (Exception ex) {

                _logger.LogError(ex, "Failed getting video information from the TwentyThree API for video with {VideoId}.", options.VideoId);

                return InternalServerError("Failed getting video information from the TwentyThree API.");

            }

            try {

                // Get a list of the first 200 players from the TwentyThree API
                TwentyThreePlayerListResponse? response = http.Players.GetList(new TwentyThreeGetPlayersOptions {
                    Size = 200
                });

                // Get the selected player from the response
                player = response.Body.Players.FirstOrDefault(x => options.PlayerId is null ? x.IsDefault : x.PlayerId == options.PlayerId);
                if (player == null) return NotFound("Player not found.");

            } catch (Exception ex) {

                _logger.LogError(ex, "Failed getting player information from the TwentyThree API for player with {PlayerId}.", options.PlayerId);

                return InternalServerError("Failed getting player information from the TwentyThree API.");

            }

            return new ApiVideoDetails(options, credentials, video, player, site);

        }

        private object GetSpot(TwentyThreeCredentials credentials, TwentyThreeSpotOptions options, TwentyThreeConfiguration? config) {

            if (config is { AllowSpots: false }) return BadRequest("Spots are not allowed.");

            var http = _service.GetHttpService(credentials);

            TwentyThreeSpot? spot = null;
            TwentyThreeSite? site = null;

            try {

                // Get information about the spot
                TwentyThreeSpotListResponse response = http.Spots.GetList(new TwentyThreeGetSpotsOptions {
                    SpotId = options.SpotId,
                    Token = options.Token
                });

                // Get the first spot (if any)
                spot = response.Body.Spots.FirstOrDefault();
                if (spot == null) return NotFound("Spot not found.");

                // Get a reference to the current site
                site = response.Body.Site;

            } catch {

                return InternalServerError("Failed getting spot information from the TwentyThree API.");

            }

            // The spot is made up of one or more videos (aka photos), so we can get information about the first video
            // to find some thumbnails (seems to be the best approach for now)
            string? firstPhotoId = spot.SpotSelection
                .Split(' ')
                .Select(x => x.Split(':')[1])
                .FirstOrDefault();

            TwentyThreeThumbnail[] thumbnails;
            if (firstPhotoId.HasValue()) {
                try {

                    TwentyThreePhotoListResponse response = http.Photos.GetList(new TwentyThreeGetPhotosOptions {
                        PhotoId = firstPhotoId
                    });

                    // Get the thumbnails from the first video/photo (currently asuming that one video is returned)
                    thumbnails = response.Body.Photos[0].Thumbnails.Select(x => new TwentyThreeThumbnail(options, x)).ToArray();

                } catch {
                    thumbnails = Array.Empty<TwentyThreeThumbnail>();
                }
            } else {
                thumbnails = Array.Empty<TwentyThreeThumbnail>();
            }

            return new ApiSpotDetails(options, credentials, spot, thumbnails, site);

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
            if (photo != null) spot.JObject!.Add("__thumbnails", JArray.FromObject(photo.Thumbnails.Select(x => new TwentyThreeThumbnail(photo, x))));
            return spot.JObject;
        }

        private static object InternalServerError(object value) {
            return new ObjectResult(value) {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        #endregion

    }

}