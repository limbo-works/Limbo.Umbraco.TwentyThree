using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Limbo.Umbraco.TwentyThree.Exceptions;
using Limbo.Umbraco.TwentyThree.Models;
using Limbo.Umbraco.TwentyThree.Models.Credentials;
using Limbo.Umbraco.TwentyThree.Models.Intermediary;
using Limbo.Umbraco.TwentyThree.Models.Settings;
using Limbo.Umbraco.TwentyThree.Options;
using Limbo.Umbraco.TwentyThree.PropertyEditors;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Strings;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Social.TwentyThree;
using Skybrud.Social.TwentyThree.Exceptions;
using Skybrud.Social.TwentyThree.Models.Photos;
using Skybrud.Social.TwentyThree.Models.Players;
using Skybrud.Social.TwentyThree.Models.Sites;
using Skybrud.Social.TwentyThree.Models.Spots;
using Skybrud.Social.TwentyThree.OAuth;
using Skybrud.Social.TwentyThree.Options.Photos;
using Skybrud.Social.TwentyThree.Options.Players;
using Skybrud.Social.TwentyThree.Options.Spots;
using Skybrud.Social.TwentyThree.Responses.Photos;
using Skybrud.Social.TwentyThree.Responses.Players;
using Skybrud.Social.TwentyThree.Responses.Spots;
using Umbraco.Cms.Core.Models;
using TwentyThreeThumbnail = Limbo.Umbraco.TwentyThree.Models.TwentyThreeThumbnail;

namespace Limbo.Umbraco.TwentyThree.Services;

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
    /// Attempts to get the credentials matching the specified <paramref name="input"/> (ID or domain).
    /// </summary>
    /// <param name="input">The input (ID or domain).</param>
    /// <param name="result">When this method returns, holds the <see cref="TwentyThreeCredentials"/> if successful; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
    public bool TryGetCredentials(string input, [NotNullWhen(true)] out TwentyThreeCredentials? result) {

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
    public bool IsMatch(string source, [NotNullWhen(true)] out ITwentyThreeOptions? options) {

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

            options = new TwentyThreeSpotOptions(source, scheme, domain, spotId, token);

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

    /// <summary>
    /// Parses the specified <paramref name="source"/> and returns an instance of <see cref="ITwentyThreeOptions"/>.
    /// </summary>
    /// <param name="source">The URL or embed code.</param>
    /// <returns>An instance of <see cref="ITwentyThreeOptions"/>.</returns>
    protected virtual ITwentyThreeOptions GetOptionsFromSource(string source) {

        // Check whether a "source" was specified
        if (string.IsNullOrWhiteSpace(source)) throw TwentyThreeSourceException.NoSource();

        source = source.Trim();

        // Does "source" match a valid TwentyThree URL or embed code?
        if (!IsMatch(source, out ITwentyThreeOptions? options)) throw TwentyThreeSourceException.InvalidSource(source);

        return options;

    }

    /// <summary>
    /// Attempts to look up the video or spot identified by the specified <paramref name="source"/>, and returns an instance of <see cref="TwentyThreeIntermediaryValue"/> if successful. When serialized to JSON, the value equals the property value saved in the database for properties using the TwentyThree video data type.
    /// </summary>
    /// <param name="source">The source (URL or embed code) as entered by the user.</param>
    /// <returns>An instance of <see cref="TwentyThreeIntermediaryValue"/> if successful; otherwise, <see langword="null"/>.</returns>
    public virtual TwentyThreeIntermediaryValue GetIntermediaryValue(string source) {
        return GetIntermediaryValue(source, null);
    }

    /// <summary>
    /// Attempts to look up the video or spot identified by the specified <paramref name="source"/>, and returns an instance of <see cref="TwentyThreeIntermediaryValue"/> if successful. When serialized to JSON, the value equals the property value saved in the database for properties using the TwentyThree video data type.
    /// </summary>
    /// <param name="source">The source (URL or embed code) as entered by the user.</param>
    /// <param name="config">An optional data type configuration. Used for additional validation if specified.</param>
    /// <returns>An instance of <see cref="TwentyThreeIntermediaryValue"/> if successful; otherwise, <see langword="null"/>.</returns>
    public virtual TwentyThreeIntermediaryValue GetIntermediaryValue(string source, TwentyThreeConfiguration? config) {

        ITwentyThreeOptions options = GetOptionsFromSource(source);

        // Do we have valid credentials for the TwentyThree site/domain?
        if (!TryGetCredentials(options.Domain, out TwentyThreeCredentials? credentials)) throw new Exception($"No or invalid configuration found for the '{options.Domain}' domain.");

        // Handle the different options types
        return options switch {
            TwentyThreeVideoOptions vo => GetIntermediaryVideoValue(credentials, vo, config),
            TwentyThreeSpotOptions so => GetSpot(credentials, so, config),
            _ => throw new Exception($"Unknown type {options.GetType()}.")
        };

    }

    /// <summary>
    /// Fetches information about the video identified by the specified <paramref name="options"/> and returns an instance of <see cref="TwentyThreeIntermediaryVideoValue"/>..
    /// </summary>
    /// <param name="credentials">The credentials to be used for accessing the TwentyThree API.</param>
    /// <param name="options">The options identifying the video.</param>
    /// <param name="config">An optional data type configuration. Used for additional validation if specified.</param>
    /// <returns>Returns an instance of <see cref="TwentyThreeIntermediaryVideoValue"/>.</returns>
    protected virtual TwentyThreeIntermediaryVideoValue GetIntermediaryVideoValue(TwentyThreeCredentials credentials, TwentyThreeVideoOptions options, TwentyThreeConfiguration? config) {

        if (config is { AllowVideos: false }) throw TwentyThreeConfigException.VideosNotAllowed(config);

        TwentyThreeHttpService http = GetHttpService(credentials);

        TwentyThreePhoto? video;
        TwentyThreeSite? site;

        try {

            // Get information about the video from the TwentyThree API
            TwentyThreePhotoListResponse response = http.Photos.GetList(new TwentyThreeGetPhotosOptions {
                PhotoId = options.VideoId,
                Token = options.Token
            });

            // Get the first video of the response (if any)
            video = response.Body.Photos.FirstOrDefault();
            if (video == null) throw new TwentyThreeVideoNotFoundException(options.VideoId);

            // Get a reference to the current site
            site = response.Body.Site;

        } catch (TwentyThreeHttpException ex) when (ex.HasError) {

            if (ex.Error.Code == "photo_not_found") throw new TwentyThreeVideoNotFoundException(options.VideoId, ex);

            //_logger.LogError(ex, "Failed getting video information from the TwentyThree API for video with ID {VideoId}: {ErrorCode} - {Message}", options.VideoId, ex.Error.Code, ex.Error.Message);

            throw new Exception("Failed getting video information from the TwentyThree API.", ex);

        } catch (Exception ex) {

            //_logger.LogError(ex, "Failed getting video information from the TwentyThree API for video with ID {VideoId}.", options.VideoId);

            throw new Exception("Failed getting video information from the TwentyThree API.", ex);

        }

        try {

            // Get a list of the first 200 players from the TwentyThree API
            TwentyThreePlayerListResponse response = http.Players.GetList(new TwentyThreeGetPlayersOptions {
                Size = 200
            });

            // Get the selected player from the response
            TwentyThreePlayer? player = response.Body.Players.FirstOrDefault(x => options.PlayerId is null ? x.IsDefault : x.PlayerId == options.PlayerId);
            if (player == null) throw TwentyThreePlayerNotFoundException.Create(options.PlayerId);

            return new TwentyThreeIntermediaryVideoValue(options, credentials, video, player, site);

        } catch (TwentyThreeHttpException ex) when (ex.HasError) {

            // _logger.LogError(ex, "Failed getting video information from the TwentyThree API for video with ID {VideoId}: {ErrorCode} - {Message}", options.PlayerId, ex.Error.Code, ex.Error.Message);

            throw new Exception("Failed getting player information from the TwentyThree API.", ex);

        } catch (Exception ex) {

            //_logger.LogError(ex, "Failed getting player information from the TwentyThree API for player with {PlayerId}.", options.PlayerId);

            throw new Exception("Failed getting player information from the TwentyThree API.", ex);

        }

    }

    /// <summary>
    /// Fetches information about the spot identified by the specified <paramref name="options"/> and returns an instance of <see cref="TwentyThreeIntermediarySpotValue"/>..
    /// </summary>
    /// <param name="credentials">The credentials to be used for accessing the TwentyThree API.</param>
    /// <param name="options">The options identifying the spot.</param>
    /// <param name="config">An optional data type configuration. Used for additional validation if specified.</param>
    /// <returns>Returns an instance of <see cref="TwentyThreeIntermediarySpotValue"/>.</returns>
    public virtual TwentyThreeIntermediarySpotValue GetSpot(TwentyThreeCredentials credentials, TwentyThreeSpotOptions options, TwentyThreeConfiguration? config) {

        if (config is { AllowSpots: false }) throw TwentyThreeConfigException.SpotsNotAllowed(config);

        TwentyThreeHttpService http = GetHttpService(credentials);

        TwentyThreeSpot? spot;
        TwentyThreeSite? site;

        try {

            // Get information about the spot
            TwentyThreeSpotListResponse response = http.Spots.GetList(new TwentyThreeGetSpotsOptions {
                SpotId = options.SpotId,
                Token = options.Token
            });

            // Get the first spot (if any)
            spot = response.Body.Spots.FirstOrDefault();
            if (spot == null) throw new TwentyThreeSpotNotFoundException(options.SpotId);

            // Get a reference to the current site
            site = response.Body.Site;

        } catch (TwentyThreeHttpException ex) when (ex.HasError) {

            //_logger.LogError(ex, "Failed getting spot information from the TwentyThree API: {ErrorCode} - {Message}", ex.Error.Code, ex.Error.Message);

            throw new Exception("Failed getting spot information from the TwentyThree API.", ex);

        } catch (Exception ex) {

            //_logger.LogError(ex, "Failed getting spot information from the TwentyThree API.");

            throw new Exception("Failed getting spot information from the TwentyThree API.", ex);

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

        return new TwentyThreeIntermediarySpotValue(options, credentials, spot, thumbnails, site);

    }

    /// <summary>
    /// Sets the value of the specified <paramref name="propertyAlias"/> on the given <paramref name="content"/> to the JSON representation of the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="content">The content item.</param>
    /// <param name="propertyAlias">The alias of the property.</param>
    /// <param name="value">The value to be saved.</param>
    /// <param name="culture">The culture, if any.</param>
    /// <param name="segment">The segment, if any.</param>
    public void SetValue(IContentBase content, string propertyAlias, TwentyThreeIntermediaryValue? value, string? culture = null, string? segment = null) {
        content.SetValue(propertyAlias, value is null ? null : JsonConvert.SerializeObject(value, Formatting.None), culture, segment);
    }

}