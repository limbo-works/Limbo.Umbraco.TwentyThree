using Limbo.Umbraco.TwentyThree.Api;
using Limbo.Umbraco.TwentyThree.Extensions;
using Limbo.Umbraco.TwentyThree.Factories;
using Limbo.Umbraco.TwentyThree.Manifests;
using Limbo.Umbraco.TwentyThree.Models.Settings;
using Limbo.Umbraco.TwentyThree.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Infrastructure.Manifest;

#pragma warning disable 1591

namespace Limbo.Umbraco.TwentyThree.Composers;

public class TwentyThreeComposer : IComposer {

    public void Compose(IUmbracoBuilder builder) {

        builder.Services.AddSingleton<TwentyThreeService>();

        builder.Services.AddSingleton<TwentyThreeModelFactory>();

        builder.AddUmbracoOptions<TwentyThreeSettings>();

        builder.Services.AddSingleton<IPackageManifestReader, TwentyThreePackageManifestReader>();

        builder.Services.ConfigureOptions<TwentyThreeSwaggerGenOptions>();

    }

}