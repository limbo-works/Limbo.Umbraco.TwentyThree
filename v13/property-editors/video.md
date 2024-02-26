# Video

The **Limbo TwentyThree Video** property editor allows selecting a TwentyThree video by either entering the URL or embed code manually or by selecting the video via the property editor's video picker option.


Valid properties using this property editor will return an instance of <code type="Limbo.Umbraco.TwentyThree.Models.TwentyThreeValue, Limbo.Umbraco.TwentyThree">TwentyThreeValue</code> - or more specifically <code type="Limbo.Umbraco.TwentyThree.Models.TwentyThreeVideoValue, Limbo.Umbraco.TwentyThree">TwentyThreeVideoValue</code> or <code type="Limbo.Umbraco.TwentyThree.Models.TwentyThreeSpotValue, Limbo.Umbraco.TwentyThree">TwentyThreeSpotValue</code> depending on the type of the video that has been inserted.

```cshtml
@using Limbo.Umbraco.TwentyThree.Models
@inherits UmbracoViewPage

@{

    TwentyThreeValue? twentyThree = Model.Value<TwentyThreeValue>("video");

    if (twentyThree is TwentyThreeVideoValue video) {

        <h2>@video.Details.Title</h2>

        @video.Embed.Html

    } else if (twentyThree is TwentyThreeSpotValue spot) {

        <h2>@spot.Details.Title</h2>

        @spot.Embed.Html

    }

}
```