#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Limbo.Umbraco.TwentyThree.Api;

public class TwentyThreeSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions> {
    public void Configure(SwaggerGenOptions options) {
        options.SwaggerDoc(TwentyThreeApiConstants.Alias, new OpenApiInfo {
            Title = TwentyThreeApiConstants.Name,
            Version = "1.0"
        });
        options.OperationFilter<TwentyThreeSecurityFilter>();
    }
}