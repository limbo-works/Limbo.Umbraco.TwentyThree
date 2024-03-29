﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Reflection.Extensions;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.DependencyInjection;

namespace Limbo.Umbraco.TwentyThree.Extensions;

internal static class TwentyThreeExtensions {

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

}