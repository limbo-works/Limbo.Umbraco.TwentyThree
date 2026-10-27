using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

using static Limbo.Umbraco.TwentyThree.TwentyThreePackage;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.TwentyThree.Manifests;

public class TwentyThreePackageManifestReader : IPackageManifestReader {

    public async Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync() {

        List<PackageManifest> temp = [
            new () {
                Id = Alias,
                Name = Name,
                AllowTelemetry = true,
                Version = InformationalVersion,
                Extensions = [
                    new {
                        name = $"{Alias}.EntryPoint",
                        alias = $"{Alias}.EntryPoint",
                        type = "backofficeEntryPoint",
                        js = $"/App_Plugins/{Alias}/EntryPoint.js"
                    }
                ],
                Importmap = new PackageManifestImportmap {
                    Imports = new Dictionary<string, string> {
                        {"@limbo/twentythree/auth", $"/App_Plugins/{Alias}/Auth.js"},
                        {"@limbo/twentythree/package", $"/App_Plugins/{Alias}/Package.js"},
                        {"@limbo/twentythree/service", $"/App_Plugins/{Alias}/Service.js"},
                        {"@limbo/twentythree/elements/pagination", $"/App_Plugins/{Alias}/Elements/Pagination.js"},
                        {"@limbo/twentythree/modals/tokens", $"/App_Plugins/{Alias}/Modals/Tokens.js"},
                        {"@limbo/twentythree/modals/select-video", $"/App_Plugins/{Alias}/Modals/SelectVideo.js"},
                        {"@limbo/twentythree/modals/select-spot", $"/App_Plugins/{Alias}/Modals/SelectSpot.js"},
                        {"@limbo/twentythree/modals/upload-video", $"/App_Plugins/{Alias}/Modals/UploadVideo.js"}
                    }
                }
            }

        ];

        return await Task.FromResult(temp);

    }

}