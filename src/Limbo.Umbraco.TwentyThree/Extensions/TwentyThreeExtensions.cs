using System;
using Limbo.Umbraco.TwentyThree.Models.Intermediary;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Skybrud.Essentials.Reflection.Extensions;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.TwentyThree.Extensions;

public static class TwentyThreeExtensions {

    internal static IUmbracoBuilder AddUmbracoOptions<TOptions>(this IUmbracoBuilder builder, Action<OptionsBuilder<TOptions>>? configure = null) where TOptions : class {

        var umbracoOptionsAttribute = typeof(TOptions).GetCustomAttribute<UmbracoOptionsAttribute>();
        if (umbracoOptionsAttribute is null) {
            throw new ArgumentException($"{typeof(TOptions)} do not have the UmbracoOptionsAttribute.");
        }

        var optionsBuilder = builder.Services.AddOptions<TOptions>()
            .Bind(
                builder.Config.GetSection(umbracoOptionsAttribute.ConfigurationKey),
                o => o.BindNonPublicProperties = umbracoOptionsAttribute.BindNonPublicProperties
            )
            .ValidateDataAnnotations();

        configure?.Invoke(optionsBuilder);

        return builder;

    }

    public static void SetValue(this IContentBase content, string propertyAlias, TwentyThreeIntermediaryValue? details) {
        content.SetValue(propertyAlias, details is null ? null : JsonConvert.SerializeObject(details, Formatting.None));
    }

}