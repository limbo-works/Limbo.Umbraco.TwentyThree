using System.Collections.Generic;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591

namespace Limbo.Umbraco.TwentyThree.PropertyEditors;

public class TwentyThreeConfigurationEditor : ConfigurationEditor<TwentyThreeConfiguration> {

    public TwentyThreeConfigurationEditor(IIOHelper ioHelper) : base(ioHelper) {

    }

    public override IDictionary<string, object> DefaultConfiguration => new Dictionary<string, object> {
        { "allowVideos", true },
        { "allowSpots", true }
    };

}
