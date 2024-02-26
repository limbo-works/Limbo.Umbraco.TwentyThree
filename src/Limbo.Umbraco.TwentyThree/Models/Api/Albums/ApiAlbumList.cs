using System.Collections.Generic;
using Newtonsoft.Json;
using Skybrud.Essentials.Collections.Extensions;
using Skybrud.Social.TwentyThree.Models.Albums;
using Skybrud.Social.TwentyThree.Responses.Albums;

namespace Limbo.Umbraco.TwentyThree.Models.Api.Albums;

internal class ApiAlbumList {

    [JsonProperty("total")]
    public int Total { get; }

    [JsonProperty("albums")]
    public IReadOnlyList<ApiAlbum> Albums { get; }

    public ApiAlbumList(TwentyThreeAlbumListResponse response) : this(response.Body) { }

    private ApiAlbumList(TwentyThreeAlbumList albumList) {
        Total = albumList.TotalCount;
        Albums = albumList.Albums.SelectList(x => new ApiAlbum(x));
    }

}