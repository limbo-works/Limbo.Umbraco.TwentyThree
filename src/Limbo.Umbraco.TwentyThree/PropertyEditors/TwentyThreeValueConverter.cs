using System;
using Limbo.Umbraco.TwentyThree.Models;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

#pragma warning disable 1591

namespace Limbo.Umbraco.TwentyThree.PropertyEditors;

public class TwentyThreeValueConverter : PropertyValueConverterBase {

    public override bool IsConverter(IPublishedPropertyType propertyType) {
        return propertyType.EditorAlias == TwentyThreeEditor.EditorAlias;
    }

    public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview) {
        return source is string str && str.DetectIsJson() ? JsonUtils.ParseJsonObject(str) : null;
    }

    public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) {

        var config = propertyType.DataType.ConfigurationAs<TwentyThreeConfiguration>();

        if (inter is not JObject json) return null;

        if (json.Property("video") is not null) return TwentyThreeVideoValue.Create(json, config);
        if (json.Property("spot") is not null) return TwentyThreeSpotValue.Create(json);

        return null;

    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {

        var config = propertyType.DataType.ConfigurationAs<TwentyThreeConfiguration>();

        return config switch {
            { AllowVideos: true, AllowSpots: false } => typeof(TwentyThreeVideoValue),
            { AllowVideos: false, AllowSpots: true } => typeof(TwentyThreeSpotValue),
            _ => typeof(TwentyThreeValue)
        };

    }

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
        return PropertyCacheLevel.Element;
    }

}