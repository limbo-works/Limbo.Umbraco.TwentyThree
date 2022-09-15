using Limbo.Umbraco.TwentyThree.Extensions;
using Limbo.Umbraco.TwentyThree.Manifests;
using Limbo.Umbraco.TwentyThree.Models.Settings;
using Limbo.Umbraco.TwentyThree.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

#pragma warning disable 1591

namespace Limbo.Umbraco.TwentyThree.Composers {

    public class TwentyThreeComposer : IComposer {

        public void Compose(IUmbracoBuilder builder) {

            builder.Services.AddSingleton<TwentyThreeService>();

            builder.AddUmbracoOptions<TwentyThreeSettings>();

            builder.ManifestFilters().Append<TwentyThreeManifestFilter>();

        }

    }

}