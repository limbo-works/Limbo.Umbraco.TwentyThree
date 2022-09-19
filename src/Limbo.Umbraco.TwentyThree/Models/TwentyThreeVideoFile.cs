using System.Linq;
using Limbo.Umbraco.Video.Models.Videos;
using Newtonsoft.Json;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Social.TwentyThree.Models.Photos;

namespace Limbo.Umbraco.TwentyThree.Models;

public class TwentyThreeVideoFile : IVideoFile {

    [JsonProperty("alias")]
    public string Alias { get; }

    [JsonProperty("width")]
    public int Width { get; }

    [JsonProperty("height")]
    public int Height { get; }

    [JsonProperty("url")]
    public string Url { get; }

    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string? Type { get; }

    [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
    public long? Size { get; }

    public TwentyThreeVideoFile(TwentyThreePhoto video, TwentyThreeVideoFormat format) {
        Alias = format.Alias;
        Width = format.Width;
        Height = format.Height;
        Url = $"{video.AbsoluteUrl.Split('/').Take(2).Join("/")}{format.Url}";
        Size = format.Size;
    }

}