using System.Collections.Generic;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

#pragma warning disable CS1591

namespace Limbo.Umbraco.TwentyThree.PropertyEditors {
    
    public class TwentyThreeConfigurationEditor : ConfigurationEditor<TwentyThreeConfiguration> {

        public TwentyThreeConfigurationEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser) : base(ioHelper, editorConfigurationParser) {

            foreach (var field in Fields) {

                if (field.View is not null) {

                    field.View = field.View
                        .Replace("{version}", TwentyThreePackage.InformationalVersion)
                        .Replace("{alias}", field.Key);

                }

            }

        }

        public override IDictionary<string, object> DefaultConfiguration => new Dictionary<string, object> {
            { "allowVideos", true },
            { "allowSpots", true }
        };

    }

}