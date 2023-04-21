using System;
using System.Collections.Generic;
using Limbo.Umbraco.TwentyThree.Models.Credentials;
using Newtonsoft.Json;
using Skybrud.Essentials.Strings.Extensions;

#pragma warning disable CS1591

namespace Limbo.Umbraco.TwentyThree.Models.Api {

    public class ApiCredentials {

        [JsonProperty("id")]
        public Guid Key { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string? Description { get; }

        [JsonProperty("domains")]
        public IReadOnlyList<string> Domains { get; }

        [JsonProperty("uploadUrl")]
        public string? UploadUrl { get; }

        public ApiCredentials(TwentyThreeCredentials credentials) {

            Key = credentials.Key;
            Name = credentials.Name;
            Description = credentials.Description;
            Domains = credentials.Domains;
            UploadUrl = credentials.UploadUrl.NullIfWhiteSpace();

            if (UploadUrl is not null && UploadUrl.StartsWith("/")) {
                UploadUrl = Domains.Count > 0 ? $"https://{Domains[0]}{UploadUrl}" : null;
            }

        }

    }

}