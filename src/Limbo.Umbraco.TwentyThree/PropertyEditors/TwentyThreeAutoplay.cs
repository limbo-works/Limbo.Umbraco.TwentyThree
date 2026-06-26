using Newtonsoft.Json;
using Skybrud.Essentials.Json.Newtonsoft.Converters.Enums;

#pragma warning disable CS1591

namespace Limbo.Umbraco.TwentyThree.PropertyEditors;

// [CHANGE: Umbraco 17 binds data type config via System.Text.Json] Related: TwentyThreeConfiguration.cs, TwentyThreeLoop.cs, Models/TwentyThreeEndOn.cs
[JsonConverter(typeof(EnumCamelCaseConverter))]
[System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
public enum TwentyThreeAutoplay {

    Inherit,

    Enabled,

    Disabled

}