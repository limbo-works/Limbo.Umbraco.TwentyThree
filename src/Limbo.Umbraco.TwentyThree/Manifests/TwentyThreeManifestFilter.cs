using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;

namespace Limbo.Umbraco.TwentyThree.Manifests {

    /// <inheritdoc />
    public class TwentyThreeManifestFilter : IManifestFilter {

        /// <inheritdoc />
        public void Filter(List<PackageManifest> manifests) {
            manifests.Add(new PackageManifest {
                AllowPackageTelemetry = true,
                PackageName = TwentyThreePackage.Name,
                Version = TwentyThreePackage.InformationalVersion,
                BundleOptions = BundleOptions.Independent,
                Scripts = new[] {
                    $"/App_Plugins/{TwentyThreePackage.Alias}/Scripts/Services/TwentyThreeService.js",
                    $"/App_Plugins/{TwentyThreePackage.Alias}/Scripts/Controllers/ButtonList.js",
                    $"/App_Plugins/{TwentyThreePackage.Alias}/Scripts/Controllers/SpotOverlay.js",
                    $"/App_Plugins/{TwentyThreePackage.Alias}/Scripts/Controllers/UploadExternal.js",
                    $"/App_Plugins/{TwentyThreePackage.Alias}/Scripts/Controllers/Video.js",
                    $"/App_Plugins/{TwentyThreePackage.Alias}/Scripts/Controllers/VideoOverlay.js",
                    $"/App_Plugins/{TwentyThreePackage.Alias}/Scripts/Controllers/VideoOverlay.js",
                    $"/App_Plugins/{TwentyThreePackage.Alias}/Scripts/Controllers/PlayerOverlay.js"
                },
                Stylesheets = new[] {
                    $"/App_Plugins/{TwentyThreePackage.Alias}/Styles/Default.css"
                }
            });
        }

    }

}