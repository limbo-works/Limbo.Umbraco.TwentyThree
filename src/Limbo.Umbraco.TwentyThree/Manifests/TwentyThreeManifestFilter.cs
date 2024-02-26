using System.Collections.Generic;
using System.Reflection;
using Umbraco.Cms.Core.Manifest;

namespace Limbo.Umbraco.TwentyThree.Manifests;

/// <inheritdoc />
public class TwentyThreeManifestFilter : IManifestFilter {

    /// <inheritdoc />
    public void Filter(List<PackageManifest> manifests) {

        // Initialize a new manifest filter for this package
        PackageManifest manifest = new() {
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
        };

        // The "PackageId" property isn't available prior to Umbraco 12, and since the package is build against
        // Umbraco 10, we need to use reflection for setting the property value for Umbraco 12+. Ideally this
        // shouldn't fail, but we might at least add a try/catch to be sure
        try {
            PropertyInfo? property = manifest.GetType().GetProperty("PackageId");
            property?.SetValue(manifest, TwentyThreePackage.Alias);
        } catch {
            // We don't really care about the exception
        }

        // Append the manifest
        manifests.Add(manifest);

    }

}