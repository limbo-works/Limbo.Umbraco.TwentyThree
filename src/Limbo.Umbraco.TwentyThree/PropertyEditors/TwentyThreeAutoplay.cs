using Newtonsoft.Json;
using Skybrud.Essentials.Json.Newtonsoft.Converters.Enums;

#pragma warning disable CS1591

namespace Limbo.Umbraco.TwentyThree.PropertyEditors {

    [JsonConverter(typeof(EnumCamelCaseConverter))]
    public enum TwentyThreeAutoplay {

        Inherit,

        Enabled,

        Disabled

    }

}