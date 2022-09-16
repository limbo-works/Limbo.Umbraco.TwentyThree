using System;
using Limbo.Umbraco.TwentyThree.Models;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

#pragma warning disable 1591

namespace Limbo.Umbraco.TwentyThree.PropertyEditors {

    public class TwentyThreeValueConverter : PropertyValueConverterBase {

        public override bool IsConverter(IPublishedPropertyType propertyType) {
            return propertyType.EditorAlias == TwentyThreeEditor.EditorAlias;
        }
        
        public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview) {
            return source is string str && str.DetectIsJson() ? JsonUtils.ParseJsonObject(str) : null;
        }
        
        public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) {
            var config = (TwentyThreeConfiguration) propertyType.DataType.Configuration!;
            return inter is JObject json ? new TwentyThreeValue(json, config) : null;
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {
            return typeof(TwentyThreeValue);
        }

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
            return PropertyCacheLevel.Element;
        }

    }

}