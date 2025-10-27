using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.TwentyThree.Json.Converters;

public class TwentyThreeJsonConverter : JsonConverter {

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {

        if (value is JObject json) {
            string data = json.ToString(Formatting.None);
            JObject details = new() { { "_data", data } };
            details.WriteTo(writer);
            return;
        }

        throw new NotImplementedException();

    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType) {
        return false;
    }

}