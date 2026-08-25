using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Asp.Versioning;
using Limbo.Umbraco.TwentyThree.Api;
using Limbo.Umbraco.TwentyThree.Exceptions;
using Limbo.Umbraco.TwentyThree.Factories;
using Limbo.Umbraco.TwentyThree.Models.Api;
using Limbo.Umbraco.TwentyThree.Models.Api.Albums;
using Limbo.Umbraco.TwentyThree.Models.Settings;
using Limbo.Umbraco.TwentyThree.Options;
using Limbo.Umbraco.TwentyThree.PropertyEditors;
using Limbo.Umbraco.TwentyThree.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.AspNetCore.Json.Newtonsoft;
using Skybrud.Essentials.Security.Extensions;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Social.TwentyThree;
using Skybrud.Social.TwentyThree.Exceptions;
using Skybrud.Social.TwentyThree.Models.Photos;
using Skybrud.Social.TwentyThree.Models.Players;
using Skybrud.Social.TwentyThree.Models.Sites;
using Skybrud.Social.TwentyThree.Models.Spots;
using Skybrud.Social.TwentyThree.Options.Albums;
using Skybrud.Social.TwentyThree.Options.Photos;
using Skybrud.Social.TwentyThree.Options.Players;
using Skybrud.Social.TwentyThree.Options.Spots;
using Skybrud.Social.TwentyThree.Responses.Albums;
using Skybrud.Social.TwentyThree.Responses.Photos;
using Skybrud.Social.TwentyThree.Responses.Players;
using Skybrud.Social.TwentyThree.Responses.Spots;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Extensions;
using TwentyThreeThumbnail = Limbo.Umbraco.TwentyThree.Models.TwentyThreeThumbnail;

// ReSharper disable RedundantAssignment

#pragma warning disable 1591

namespace Limbo.Umbraco.TwentyThree.Controllers;

[ApiController]
[VersionedApiBackOfficeRoute(TwentyThreeApiConstants.Route)]
[Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
[MapToApi(TwentyThreeApiConstants.Alias)]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = TwentyThreeApiConstants.GroupName)]
public class TwentyThreeController : ManagementApiControllerBase {

    private readonly ILogger<TwentyThreeController> _logger;
    private readonly IOptions<GlobalSettings> _globalSettings;
    private readonly IDataTypeService _dataTypeService;
    private readonly ILocalizedTextService _localizedTextService;
    private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
    private readonly IOptions<TwentyThreeSettings> _options;
    private readonly TwentyThreeService _service;
    private readonly TwentyThreeModelFactory _modelFactory;

    public TwentyThreeController(ILogger<TwentyThreeController> logger, IOptions<GlobalSettings> globalSettings, IDataTypeService dataTypeService, ILocalizedTextService localizedTextService, IBackOfficeSecurityAccessor backOfficeSecurityAccessor, IOptions<TwentyThreeSettings> options, TwentyThreeService service, TwentyThreeModelFactory modelFactory) {
        _logger = logger;
        _globalSettings = globalSettings;
        _dataTypeService = dataTypeService;
        _localizedTextService = localizedTextService;
        _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        _options = options;
        _service = service;
        _modelFactory = modelFactory;
    }

    #region Public API methods

    [HttpGet]
    [Route("serverVariables")]
    public object GetServerVariables() {
        return new {
            version = TwentyThreePackage.InformationalVersion,
            cacheBuster = TwentyThreePackage.InformationalVersion.ToMd5Hash()
        };
    }

    /// <summary>
    /// Returns information about the video or spot with the specified <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The video source (URL or embed code).</param>
    /// <param name="dataTypeKey">The key of the underlying data type, if any.</param>
    /// <returns>Information about the video matching <paramref name="source"/>.</returns>
    [HttpGet("video")]
    public object GetVideo(string? source, Guid? dataTypeKey = null) {

        // Get the "source" parameter from either GET or POST
        source = HttpContext.Request.Query["source"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(source) && HttpContext.Request.HasFormContentType) {
            source = HttpContext.Request.Form["source"].FirstOrDefault();
        }

        // Check whether a "source" was specified
        if (string.IsNullOrWhiteSpace(source)) return BadRequest("No URL or embed code specified.");


        try {

            // Parse the source
            ITwentyThreeOptions options = _service.GetOptionsFromSource(source);

            // Do we have valid credentials for the TwentyThree site/domain?
            if (!_service.TryGetCredentials(options, out TwentyThreeCredentials? credentials)) {
                if (options is TwentyThreeVideoOptions video && !string.IsNullOrWhiteSpace(video.SiteKey)) {
                    throw TwentyThreeSiteNotFoundException.Create(video.SiteKey, video);
                }

                throw TwentyThreeDomainNotFoundException.Create(options.Domain, options);
            }

            // Get a reference to the data type (if specified)
            IDataType? dataType = dataTypeKey == null ? null : _dataTypeService.GetAsync(dataTypeKey.Value).GetAwaiter().GetResult();
            TwentyThreeConfiguration? config = dataType?.ConfigurationAs<TwentyThreeConfiguration>();

            // Handle the different options types
            return options switch {
                TwentyThreeVideoOptions vo => NewtonsoftJsonResult.Ok(GetVideo(credentials, vo, config)),
                TwentyThreeSpotOptions so => NewtonsoftJsonResult.Ok(GetSpot(credentials, so, config)),
                _ => BadRequest($"Unknown type {options.GetType()}.")
            };

        } catch (TwentyThreeUserException ex) {

            string?[] tokens = ex.UserMessageArgs.Select(x => x.ToString()).ToArray();

            _logger.LogError(ex, "Failed getting TwentyThree video information: {Source}", source);

            return InternalServerError(Localize(ex.UserMessageKey, tokens));

        } catch (Exception ex) {

            _logger.LogError(ex, "Failed getting TwentyThree video information: {Source}", source);

            return GenericError();

        }

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
    [HttpGet("accounts")]
    public object GetAccounts() {
        return Ok(_options.Value.Credentials.Select(ToApiModel));
    }

    [HttpGet("accounts/{accountId}/albums")]
    public object GetAlbums(Guid accountId) {

        TwentyThreeCredentials? credentials = _options.Value.Credentials.FirstOrDefault(x => x.Key == accountId);
        if (credentials == null) return NotFound("Account not found.");

        TwentyThreeHttpService http = _service.GetHttpService(credentials);

        try {

            TwentyThreeAlbumListResponse response = http.Albums.GetList(new TwentyThreeGetAlbumsOptions {
                Size = 1000
            });

            return Ok(new ApiAlbumList(response));

        } catch (TwentyThreeHttpException ex) when (ex.HasError) {

            _logger.LogError(ex, "Failed getting list of videos from the TwentyThree API: {ErrorCode} - {Message}", ex.Error.Code, ex.Error.Message);

            return InternalServerError("Failed getting list of videos from the TwentyThree API.");

        } catch (Exception ex) {

            _logger.LogError(ex, "Failed getting list of videos from the TwentyThree API.");

            return InternalServerError("Failed getting list of videos from the TwentyThree API.");

        }

    }

    /// <summary>
    /// Returns a list of videos of the account with the specified <paramref name="accountId"/>.
    /// </summary>
    /// <param name="accountId">The GUID ID of the account.</param>
    /// <param name="text">The text to search for.</param>
    /// <param name="limit">The maximum amount of videos to return for each page.</param>
    /// <param name="page">The page to be returned.</param>
    /// <param name="albumId">The ID of the album the returned videos should match. Default is <see langword="null"/>.</param>
    /// <returns>A list of vídeos.</returns>
    [HttpGet("accounts/{accountId}/videos")]
    public object GetVideos(Guid accountId, string? text = null, int limit = 0, int page = 1, string? albumId = null) {

        if (limit == 0) limit = 10;

        TwentyThreeCredentials? credentials = _options.Value.Credentials.FirstOrDefault(x => x.Key == accountId);
        if (credentials == null) return NotFound("Account not found.");

        TwentyThreeHttpService http = _service.GetHttpService(credentials);

        TwentyThreePhotoList list;

        try {

            // Initialize the options for the request
            TwentyThreeGetPhotosOptions options = new() { Search = text, Size = limit, Page = page };

            // Set the album ID if present in the query string
            if (!string.IsNullOrWhiteSpace(albumId)) options.AlbumId = albumId;

            // Make the request to the TwentyThree API
            TwentyThreePhotoListResponse response = http.Photos.GetList(options);

            list = response.Body;

        } catch (TwentyThreeHttpException ex) when(ex.HasError) {

            _logger.LogError(ex, "Failed getting list of videos from the TwentyThree API: {ErrorCode} - {Message}", ex.Error.Code, ex.Error.Message);

            return InternalServerError("Failed getting list of videos from the TwentyThree API.");

        } catch (Exception ex) {

            _logger.LogError(ex, "Failed getting list of videos from the TwentyThree API.");

            return InternalServerError("Failed getting list of videos from the TwentyThree API.");

        }

        var result = new {
            page = list.Page,
            limit = list.Size,
            total = list.TotalCount,
            pages = list.TotalCount == 0 ? 0 : Math.Ceiling((double) list.TotalCount / list.Size),
            site = new ApiSite(list.Site),
            videos = list.Photos.Select(ToApiModel)
        };

        return Ok(result);

    }


    [HttpGet("accounts/{accountId}/spots")]
    public object GetSpots(Guid accountId, string? text = null, int limit = 0, int page = 1) {

        var credentials = _options.Value.Credentials.FirstOrDefault(x => x.Key == accountId);
        if (credentials == null) return NotFound("Account not found.");

        var http = _service.GetHttpService(credentials);

        TwentyThreeSpotList list;

        try {

            // Initialize the options for the request
            TwentyThreeGetSpotsOptions options = new() { Size = limit, Page = page };

            // Make the request to the TwentyThree API
            TwentyThreeSpotListResponse response = http.Spots.GetList(options);

            list = response.Body;

        } catch (TwentyThreeHttpException ex) when (ex.HasError) {

            _logger.LogError(ex, "Failed getting list of spots from the TwentyThree API: {ErrorCode} - {Message}", ex.Error.Code, ex.Error.Message);

            return InternalServerError("Failed getting list of spots from the TwentyThree API.");

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

        var result = new {
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

        return Ok(result);

    }

    [HttpGet("accounts/{accountId}/players")]
    public object GetPlayers(Guid accountId) {

        // Find the credentials
        var credentials = _options.Value.Credentials.FirstOrDefault(x => x.Key == accountId);
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

        } catch (TwentyThreeHttpException ex) when(ex.HasError) {

            _logger.LogError(ex, "Failed getting list of players from the TwentyThree API: {ErrorCode} - {Message}", ex.Error.Code, ex.Error.Message);

            return InternalServerError("Failed getting list of players from the TwentyThree API.");

        } catch (Exception ex) {

            _logger.LogError(ex, "Failed getting list of players from the TwentyThree API.");

            return InternalServerError("Failed getting list of players from the TwentyThree API.");

        }

        // Return the players
        return Ok(players.Select(ToApiModel));

    }

    #endregion

    #region Private methods

    private static new NewtonsoftJsonResult Ok(object value) {
        return NewtonsoftJsonResult.Ok(value);
    }

    private string Localize(string alias, params string?[] args) {

        var culture = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser?
            .GetUserCulture(_localizedTextService, _globalSettings.Value) ?? CultureInfo.CurrentCulture;


        return _localizedTextService.Localize("twentyThree", alias, culture, args);

    }

    private bool TryGetTranslation(string alias, [NotNullWhen(true)] out string? result) {

        var culture = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser?
            .GetUserCulture(_localizedTextService, _globalSettings.Value) ?? CultureInfo.CurrentCulture;

        string temp = _localizedTextService.Localize("twentyThree", alias, culture);

        if (string.IsNullOrWhiteSpace(temp) || temp.StartsWith('[')) {
            result = null;
            return false;
        }

        result = temp;
        return true;

    }

    private IActionResult GenericError() {
        if (!TryGetTranslation("errorGeneric", out string? message)) message = "An error occured on the server.";
        return InternalServerError(message);
    }

    private object GetVideo(TwentyThreeCredentials credentials, TwentyThreeVideoOptions options, TwentyThreeConfiguration? config) {

        if (config is { AllowVideos: false }) return BadRequest("Videos are not allowed.");

        var http = _service.GetHttpService(credentials);

        TwentyThreePhoto? video = null;
        TwentyThreeSite? site = null;

        try {

            // Get information about the video from the TwentyThree API
            TwentyThreePhotoListResponse response = http.Photos.GetList(new TwentyThreeGetPhotosOptions {
                PhotoId = options.VideoId,
                Token = options.Token
            });

            // Get the first video of the response (if any)
            video = response.Body.Photos.FirstOrDefault();
            if (video == null) return NotFound("Video not found.");

            // Get a reference to the current site
            site = response.Body.Site;

        } catch (TwentyThreeHttpException ex) when (ex.HasError) {

            if (ex.Error.Code == "photo_not_found") return NotFound("Video not found.");

            _logger.LogError(ex, "Failed getting video information from the TwentyThree API for video with ID {VideoId}: {ErrorCode} - {Message}", options.VideoId, ex.Error.Code, ex.Error.Message);

            return InternalServerError("Failed getting video information from the TwentyThree API.");

        } catch (Exception ex) {

            _logger.LogError(ex, "Failed getting video information from the TwentyThree API for video with ID {VideoId}.", options.VideoId);

            return InternalServerError("Failed getting video information from the TwentyThree API.");

        }

        try {

            // Get a list of the first 200 players from the TwentyThree API
            TwentyThreePlayerListResponse response = http.Players.GetList(new TwentyThreeGetPlayersOptions {
                Size = 200
            });

            // Get the selected player from the response
            TwentyThreePlayer? player = response.Body.Players.FirstOrDefault(x => options.PlayerId is null ? x.IsDefault : x.PlayerId == options.PlayerId);
            if (player == null) return NotFound("Player not found.");

            return new ApiVideoDetails(options, credentials, video, player, site);

        } catch (TwentyThreeHttpException ex) when (ex.HasError) {

            _logger.LogError(ex, "Failed getting video information from the TwentyThree API for video with ID {VideoId}: {ErrorCode} - {Message}", options.PlayerId, ex.Error.Code, ex.Error.Message);

        } catch (Exception ex) {

            _logger.LogError(ex, "Failed getting player information from the TwentyThree API for player with {PlayerId}.", options.PlayerId);

        }

        return InternalServerError("Failed getting player information from the TwentyThree API.");

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

        } catch (TwentyThreeHttpException ex) when (ex.HasError) {

            _logger.LogError(ex, "Failed getting spot information from the TwentyThree API: {ErrorCode} - {Message}", ex.Error.Code, ex.Error.Message);

            return InternalServerError("Failed getting spot information from the TwentyThree API.");

        } catch (Exception ex) {

            _logger.LogError(ex, "Failed getting spot information from the TwentyThree API.");

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

                // Get the thumbnails from the first video/photo (currently assuming that one video is returned)
                thumbnails = response.Body.Photos[0].Thumbnails.Select(x => new TwentyThreeThumbnail(options, x)).ToArray();

            } catch {
                thumbnails = [];
            }
        } else {
            thumbnails = [];
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

    private object? ToApiModel(TwentyThreeSpot? spot, TwentyThreePhoto? photo) {
        if (spot == null) return null;
        if (photo != null) spot.JObject.Add("__thumbnails", JArray.FromObject(photo.Thumbnails.Select(x => _modelFactory.CreateThumbnail(x, photo))));
        return spot.JObject;
    }

    private static IActionResult InternalServerError(object value) {
        return new ObjectResult(value) {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }

    #endregion

}
