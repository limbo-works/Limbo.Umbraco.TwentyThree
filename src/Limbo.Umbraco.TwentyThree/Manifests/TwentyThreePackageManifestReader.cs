using System.Collections.Generic;
using System.Threading.Tasks;
using Skybrud.Essentials.Security.Extensions;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.TwentyThree.Manifests;

public class TwentyThreePackageManifestReader : IPackageManifestReader {

    public async Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync() {

        const string alias = TwentyThreePackage.Alias;
        string cacheBuster = TwentyThreePackage.InformationalVersion.ToMd5Hash();

        List<PackageManifest> temp = [
            new () {
                Id = TwentyThreePackage.Alias,
                Name = TwentyThreePackage.Name,
                AllowTelemetry = true,
                Version = TwentyThreePackage.InformationalVersion,
                Extensions = [
                    new {
                        name = $"{alias}.EntryPoint",
                        alias = $"{alias}.EntryPoint",
                        type = "backofficeEntryPoint",
                        js = $"/App_Plugins/{alias}/EntryPoint.js?v={cacheBuster}"
                    }
                ],
                Importmap = new PackageManifestImportmap {
                    Imports = new Dictionary<string, string> {
                        {"@limbo/twentythree/auth", $"/App_Plugins/{alias}/Auth.js?v={cacheBuster}"},
                        {"@limbo/twentythree/package", $"/App_Plugins/{alias}/Package.js?{cacheBuster}"},
                        {"@limbo/twentythree/service", $"/App_Plugins/{alias}/Service.js?v={cacheBuster}"},
                        {"@limbo/twentythree/elements/pagination", $"/App_Plugins/{alias}/Elements/Pagination.js?v={cacheBuster}"},
                        {"@limbo/twentythree/modals/tokens", $"/App_Plugins/{alias}/Modals/Tokens.js?v={cacheBuster}"},
                        {"@limbo/twentythree/modals/select-video", $"/App_Plugins/{alias}/Modals/SelectVideo.js?v={cacheBuster}"},
                        {"@limbo/twentythree/modals/select-spot", $"/App_Plugins/{alias}/Modals/SelectSpot.js?v={cacheBuster}"},
                        {"@limbo/twentythree/modals/upload-video", $"/App_Plugins/{alias}/Modals/UploadVideo.js?v={cacheBuster}"}
                    }
                }
            }

        ];

        return await Task.FromResult(temp);

    }

}