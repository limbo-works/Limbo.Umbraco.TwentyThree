using Newtonsoft.Json;
using Skybrud.Social.TwentyThree.Models.Albums;

namespace Limbo.Umbraco.TwentyThree.Models.Api.Albums;

internal class ApiAlbum {

    [JsonProperty("id")]
    public string Id { get; }

    [JsonProperty("title")]
    public string Title { get; }

    public ApiAlbum(TwentyThreeAlbum album) {
        Id = album.AlbumId;
        Title = album.Title;
    }

}