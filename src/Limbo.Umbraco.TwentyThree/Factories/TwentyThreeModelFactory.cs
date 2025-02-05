using Limbo.Umbraco.TwentyThree.Models;
using Limbo.Umbraco.TwentyThree.PropertyEditors;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Collections.Extensions;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Json.Newtonsoft;
using Skybrud.Essentials.Json.Newtonsoft.Extensions;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Social.TwentyThree.Models.Photos;
using TwentyThreeThumbnail = Limbo.Umbraco.TwentyThree.Models.TwentyThreeThumbnail;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.TwentyThree.Factories;

/// <summary>
/// Model factory used throughout the TwentyThree package.
/// </summary>
public class TwentyThreeModelFactory {

    /// <summary>
    /// Creates a new instance of <see cref="TwentyThreeVideoValue"/> based on the specified <paramref name="json"/> object.
    /// </summary>
    /// <param name="json">The JSON object representing the video value.</param>
    /// <param name="config">An instance of <see cref="TwentyThreeConfiguration"/> representing the data type configuration.</param>
    /// <returns>An instance of <see cref="TwentyThreeVideoValue"/> representing the video value.</returns>
    public TwentyThreeVideoValue CreateVideoValue(JObject json, TwentyThreeConfiguration? config) {

        var parameters = json.GetObject("parameters", x => new TwentyThreeParameters(x))!;
        var details = json.GetObject("video", CreateVideoDetails)!;
        var embed = json.GetObject("embed", x => CreateVideoEmbed(x, details, parameters, config))!;

        return new TwentyThreeVideoValue(json, parameters, details, embed);

    }

    /// <summary>
    /// Creates a new instance of <see cref="TwentyThreeVideoDetails"/> based on the specified <paramref name="json"/> object.
    /// </summary>
    /// <param name="json">The JSON object representing the video details.</param>
    /// <returns>An instance of <see cref="TwentyThreeVideoDetails"/> representing the video details.</returns>
    public virtual TwentyThreeVideoDetails CreateVideoDetails(JObject json) {

        // Parse the video data
        TwentyThreePhoto data = json
            .GetString("_data", x => JsonUtils.ParseJsonObject(x, TwentyThreePhoto.Parse))!;

        // Parse the thumbnails and video files
        var thumbnails = data.Thumbnails.SelectList(x => CreateThumbnail(x, data));
        var files = data.VideoFormats.SelectList(x => CreateVideoFile(x, data));

        // Initialize and return the details
        return new TwentyThreeVideoDetails(data, thumbnails, files);

    }

    /// <summary>
    /// Creates a new instance of <see cref="TwentyThreeVideoEmbed"/> based on the specified <paramref name="json"/> object.
    /// </summary>
    /// <param name="json">The JSON object representing the embed information.</param>
    /// <param name="details">The video details.</param>
    /// <param name="parameters">The video parameters.</param>
    /// <param name="config">The configuration of the <see cref="TwentyThreeEditor"/> data type.</param>
    /// <returns>An instance of <see cref="TwentyThreeVideoEmbed"/> representing the video embed information.</returns>
    public virtual TwentyThreeVideoEmbed CreateVideoEmbed(JObject json, TwentyThreeVideoDetails details, TwentyThreeParameters parameters, TwentyThreeConfiguration? config) {

        string token = details.Data.Token;
        string? playerId = parameters.PlayerId.NullIfWhiteSpace();
        bool? autoplay = ParseAutoplay(json, parameters, config);
        TwentyThreeEndOn? endOn = ParseEndOn(json, parameters, config);

        string domain = details.Data.AbsoluteUrl.Split('/')[2];

        string embedUrl = $"//{domain}/{playerId ?? "v"}.ihtml/player.html?token={details.Data.Token}&source=embed&photo%5fid={details.Data.PhotoId}";
        if (autoplay != null) embedUrl += $"&autoPlay={(autoplay.Value ? "1" : "0")}";
        if (endOn != null) embedUrl += $"&endOn={endOn.Value.ToLower()}";

        HtmlString html = new($"<div style=\"width:100%; height:0; position: relative; padding-bottom:33.333333333333336%\"><iframe src=\"{embedUrl}\" style=\"width:100%; height:100%; position: absolute; top: 0; left: 0;\" frameborder=\"0\" border=\"0\" scrolling=\"no\" mozallowfullscreen=\"1\" webkitallowfullscreen=\"1\" allowfullscreen=\"1\" allow=\"autoplay; fullscreen\"></iframe></div>");

        return new TwentyThreeVideoEmbed(token, playerId, autoplay, endOn, html);

    }

    /// <summary>
    /// Creates and returns a new <see cref="TwentyThreeThumbnail"/> instanced based on the specified <paramref name="thumbnail"/> and <paramref name="video"/>.
    /// </summary>
    /// <param name="thumbnail">An instance of <see cref="Skybrud.Social.TwentyThree.Models.Photos.TwentyThreeThumbnail"/> as received from the TwentyThree integration package.</param>
    /// <param name="video">An instance of <see cref="TwentyThreePhoto"/> as received from the TwentyThree integration package.</param>
    /// <returns>An instance of <see cref="TwentyThreeThumbnail"/> reprenting the created thumbnail.</returns>
    public virtual TwentyThreeThumbnail CreateThumbnail(Skybrud.Social.TwentyThree.Models.Photos.TwentyThreeThumbnail thumbnail, TwentyThreePhoto video) {

        string scheme = video.AbsoluteUrl.Split(':')[0];
        string domain = video.AbsoluteUrl.Split('/')[2];
        string url = $"{scheme}://{domain}{thumbnail.Url}";

        return new TwentyThreeThumbnail(thumbnail.Alias, thumbnail.Width, thumbnail.Height, url);

    }

    /// <summary>
    /// Creates and returns a new <see cref="TwentyThreeVideoFile"/> instanced based on the specified <paramref name="format"/> and <paramref name="video"/>.
    /// </summary>
    /// <param name="format">An instance of <see cref="TwentyThreeVideoFormat"/> as received from the TwentyThree integration package.</param>
    /// <param name="video">An instance of <see cref="TwentyThreePhoto"/> as received from the TwentyThree integration package.</param>
    /// <returns>An instance of <see cref="TwentyThreeVideoFile"/> reprenting the created video file.</returns>
    public virtual TwentyThreeVideoFile CreateVideoFile(TwentyThreeVideoFormat format, TwentyThreePhoto video) {

        string scheme = video.AbsoluteUrl.Split(':')[0];
        string domain = video.AbsoluteUrl.Split('/')[2];
        string url = $"{scheme}://{domain}{format.Url}";

        return new TwentyThreeVideoFile(format.Alias, format.Width, format.Height, url, format.Size);

    }

    protected virtual bool? ParseAutoplay(JObject json, TwentyThreeParameters parameters, TwentyThreeConfiguration? config) {

        if (config != null && config.Autoplay != TwentyThreeAutoplay.Inherit) {
            return config.Autoplay == TwentyThreeAutoplay.Enabled;
        }

        string? value = json.GetString("autoplay");
        return value switch {
            "enabled" => true,
            "disabled" => false,
            _ => parameters.Autoplay
        };

    }

    protected virtual TwentyThreeEndOn? ParseEndOn(JObject json, TwentyThreeParameters parameters, TwentyThreeConfiguration? config) {

        if (config != null && config.EndOn != TwentyThreeEndOn.Inherit) {
            return config.EndOn;
        }

        JToken? value = json.GetValue("endOn");
        return value?.Type switch {
            JTokenType.String => EnumUtils.TryParseEnum(value.ToObject<string>(), out TwentyThreeEndOn result) && result != TwentyThreeEndOn.Inherit ? result : parameters.EndOn,
            _ => parameters.EndOn
        };

    }

}