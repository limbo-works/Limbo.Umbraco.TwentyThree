using Limbo.Umbraco.TwentyThree.Options;
using Limbo.Umbraco.Video.Models.Videos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Social.TwentyThree.Models.Photos;

namespace Limbo.Umbraco.TwentyThree.Models;

public class TwentyThreeThumbnail : IVideoThumbnail {

    [JsonProperty("alias")]
    public string Alias { get; }

    [JsonProperty("width")]
    public int Width { get; }

    [JsonProperty("height")]
    public int Height { get; }

    [JsonProperty("url")]
    public string Url { get; }

    public TwentyThreeThumbnail(Skybrud.Social.TwentyThree.Models.Photos.TwentyThreeThumbnail thumbnail) {
        Alias = thumbnail.Alias;
        Width = thumbnail.Width;
        Height = thumbnail.Height;
        Url = thumbnail.Url;
    }

    public TwentyThreeThumbnail(TwentyThreeSpotOptions options, Skybrud.Social.TwentyThree.Models.Photos.TwentyThreeThumbnail thumbnail) {
        Alias = thumbnail.Alias;
        Width = thumbnail.Width;
        Height = thumbnail.Height;
        Url = $"{options.Scheme}://{options.Domain}{thumbnail.Url}";
    }

    public TwentyThreeThumbnail(TwentyThreePhoto video, Skybrud.Social.TwentyThree.Models.Photos.TwentyThreeThumbnail thumbnail) {

        string scheme = video.AbsoluteUrl.Split(':')[0];
        string domain = video.AbsoluteUrl.Split('/')[2];

        Alias = thumbnail.Alias;
        Width = thumbnail.Width;
        Height = thumbnail.Height;
        Url = $"{scheme}://{domain}{thumbnail.Url}";
    }

    private TwentyThreeThumbnail(string alias, int width, int height, string url) {
        Alias = alias;
        Width = width;
        Height = height;
        Url = url;
    }

    internal static TwentyThreeThumbnail Parse(JObject json) {
        string alias = json.GetString("alias");
        int width = json.GetInt32("width");
        int height = json.GetInt32("height");
        string url = json.GetString("url");
        return new TwentyThreeThumbnail(alias, width, height, url);
    }

}