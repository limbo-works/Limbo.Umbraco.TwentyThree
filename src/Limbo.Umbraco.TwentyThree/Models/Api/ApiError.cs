using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

#pragma warning disable CS1591

namespace Limbo.Umbraco.TwentyThree.Models.Api;

public class ApiError : ContentResult {

    [JsonProperty("title")]
    public string Title { get; }

    [JsonProperty("message")]
    public string Message { get; }

    public ApiError(string title, string message) {
        Title = title;
        Message = message;
    }

}