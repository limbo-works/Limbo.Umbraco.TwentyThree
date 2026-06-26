using Newtonsoft.Json;
using Skybrud.Essentials.Json.Newtonsoft.Converters.Enums;

#pragma warning disable CS1591

namespace Limbo.Umbraco.TwentyThree.PropertyEditors;

[JsonConverter(typeof(EnumCamelCaseConverter))]
[System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
public enum TwentyThreeLoop {

    Inherit,

    Enabled,

    Disabled

}