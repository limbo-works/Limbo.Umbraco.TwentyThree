# Umbraco 17 Migration — Limbo.Umbraco.TwentyThree

This document describes the upgrade of **Limbo.Umbraco.TwentyThree** (the TwentyThree / 23Video
video‑picker property editor) from **Umbraco 13** to **Umbraco 17**, what changed, why, and the gotchas
discovered along the way.

---

## 1. Summary

Umbraco 14+ replaced the AngularJS backoffice with a Lit / Web‑Components stack, so this was a near‑total
rewrite of the client side plus targeted server‑side changes.

| | Before (v13) | After (v17) |
|---|---|---|
| Target framework | `net8.0` | `net10.0` |
| Umbraco packages | `[13.0.0,13.999)` (`Web.BackOffice`) | `[17.0.0,17.999)` (`Api.Management`, `Web.Common`, `Web.Website`, `Core`) |
| Backoffice UI | AngularJS controllers + HTML views | Lit web components (plain ESM JS, **no build step**) |
| Editor registration | `IManifestFilter` (C#) | `umbraco-package.json` (importmap + entry point + bundle) |
| Server API | `UmbracoAuthorizedApiController` | `ManagementApiControllerBase` (management API) |
| Package version | `13.0.3` | `17.0.0-alpha000` |

**Design decisions (agreed up front):**

* **Plain ESM JavaScript** for the frontend — Lit web components loaded directly from `wwwroot`, **no
  Node / Vite / TypeScript build**. Keeps the NuGet packaging simple.
* **Full feature parity** with the v13 editor: source paste/preview, refresh, embed options, and all browse
  overlays (video picker, spot picker, player picker, external upload).

**Hard constraint — byte compatibility:** the persisted property value JSON shape is unchanged so existing
content keeps resolving through `TwentyThreeValueConverter`. The client emits exactly:

```jsonc
{ "source": "...", "site": {…}, "credentials": {…}, "parameters": {…},
  "video": { "_data": "<json string>" }, "player": {…},
  "embed": { "autoplay": "inherit", "loop": "inherit", "endOn": "inherit" },
  "spot": { "_data": "<json string>" }, "thumbnails": [...], "type": "video|spot" }
```

---

## 2. Server‑side (C#)

### 2.1 Project file
`src/Limbo.Umbraco.TwentyThree/Limbo.Umbraco.TwentyThree.csproj`

* `TargetFramework` → `net10.0`, `VersionPrefix` → `17.0.0-alpha000`.
* Dependencies: `Umbraco.Cms.Api.Management`, `Umbraco.Cms.Core`, `Umbraco.Cms.Web.Common`,
  `Umbraco.Cms.Web.Website` (all `[17.0.0,17.999)`); `Limbo.Umbraco.Video 17.0.0-alpha001`;
  `Skybrud.Essentials 1.1.61`; `Skybrud.Social.TwentyThree 1.1.3`.
* Removed the dead `compilerconfig.json` / `Intellisense.js` content entries.

### 2.2 Property editor schema
`PropertyEditors/TwentyThreeEditor.cs`

The U17 `[DataEditor]` no longer takes a view — the UI lives client‑side. It is now simply:

```csharp
[DataEditor(EditorAlias, ValueType = ValueTypes.Json)]
public class TwentyThreeEditor : DataEditor { … }
```

`label` / `icon` / `group` moved to the JS manifest. `CreateConfigurationEditor()` still returns the typed
configuration editor (`ConfigurationEditor<T>` now requires `IIOHelper`).

### 2.3 Configuration
`PropertyEditors/TwentyThreeConfiguration.cs`, `TwentyThreeConfigurationEditor.cs`

* `[ConfigurationField]` is single‑argument in U17 (`[ConfigurationField("alias")]`) — the name / view /
  description metadata moved to the manifest `meta.settings.properties`.
* The POCO is still required so `dataType.ConfigurationAs<TwentyThreeConfiguration>()` works in the value
  converter; the field aliases are the contract between the stored config and the model.
* **Gotcha:** U17 binds data type configuration via **System.Text.Json**. The `autoplay` / `loop` / `endOn`
  enums only had Newtonsoft converters, so STJ `JsonStringEnumConverter` attributes were added (they coexist
  with the existing Newtonsoft ones).

### 2.4 API controller
`Controllers/TwentyThreeController.cs`

Now a **management API controller**:

```csharp
[VersionedApiBackOfficeRoute("twentythree")]
[ApiVersion("1.0")]
public class TwentyThreeController : ManagementApiControllerBase { … }
```

* Served at **`/umbraco/management/api/v1/twentythree/{action}`**.
* Actions (all `[HttpGet]`): `video`, `accounts`, `albums`, `videos`, `spots`, `players`. Business logic is
  unchanged from v13.
* **Serialization gotcha:** the API models use Newtonsoft `[JsonProperty]` attributes and some actions return
  raw `JObject`s. U17's management pipeline defaults to System.Text.Json, which would mangle the property
  names. All success responses are therefore serialized explicitly with Newtonsoft via a `ContentResult`
  helper (`JsonNet(...)`), preserving the exact JSON contract the client expects.
* The `dataTypeKey` round‑trip was dropped (`IDataTypeService` became async in U17 and the client doesn't send
  it); allow/deny of videos vs spots is enforced client‑side from the config.

### 2.5 Composition
`Composers/TwentyThreeComposer.cs`

Removed the `IManifestFilter` registration (the interface is gone in U17). Service / factory / options
registrations are unchanged. `Manifests/TwentyThreeManifestFilter.cs` was deleted — the static
`umbraco-package.json` replaces it.

### 2.6 Models
`Models/TwentyThreeDetails.cs`, `TwentyThreeVideoDetails.cs` were updated for the v17
`Limbo.Umbraco.Video` / `Skybrud.Essentials` API (converter rename, `IVideoThumbnail` / `IVideoFile`
interfaces). Everything else under `Models/**`, `Factories/`, `Services/`, `Extensions/`, `Exceptions/`,
`Options/`, `Json/` and `TwentyThreeValueConverter.cs` carries over unchanged.

---

## 3. Client‑side (plain ESM JavaScript)

All under `src/Limbo.Umbraco.TwentyThree/wwwroot/` (served at
`/App_Plugins/Limbo.Umbraco.TwentyThree/…`).

```
wwwroot/
├── umbraco-package.json          # importmap + backofficeEntryPoint + bundle
├── Auth.js                       # holds the access-token resolver
├── EntryPoint.js                 # captures the token resolver from UMB_AUTH_CONTEXT
├── Service.js                    # authed fetch client for the management API
├── Lang/ (en-US.xml, da-DK.xml)  # server-side error localization (still read in U17)
└── BackOffice/
    ├── Icons/ (svg)
    └── TwentyThree/
        ├── twentythree.bundle.js          # registers all extensions
        ├── property-editor-ui.element.js  # the main editor
        ├── option-select.element.js       # config button-list (autoplay/loop/endOn)
        └── modals/
            ├── tokens.js                   # UmbModalToken definitions
            ├── shared.js                   # shared render helpers + styles
            ├── video-overlay.element.js    # account → searchable/paged/album-filtered video grid
            ├── spot-overlay.element.js     # account → searchable/paged spot grid
            ├── player-overlay.element.js   # player list
            └── upload-overlay.element.js   # external upload accounts
```

### 3.1 Manifest
`umbraco-package.json` declares:
* an **importmap** exposing `@limbo/twentythree/auth` and `@limbo/twentythree/service`;
* a **`backofficeEntryPoint`** (`EntryPoint.js`);
* a **`bundle`** (`twentythree.bundle.js`) that registers the property editor UI, the option‑select config UI,
  and the four modal extensions.

### 3.2 Property editor & overlays
* `property-editor-ui.element.js` — `UmbLitElement` implementing the property‑editor contract: `value`
  get/set (manual accessors, so Lit doesn't replace them), `set config` reading the nine config aliases,
  dispatches `UmbChangeEvent`. Renders the source input / embed textarea, the video & spot previews, account
  info, and the embed block (player picker + autoplay/loop/endOn button‑lists honouring config
  inherit/override). Opens overlays via `UMB_MODAL_MANAGER_CONTEXT`.
* The four overlays are `UmbModalBaseElement`s; each loads accounts (auto‑selecting when there is exactly one),
  then drives its grid. The video overlay reproduces the responsive page‑size, ±7 pagination window, album
  filter and description truncation from the original.
* Overlays surface API errors (e.g. `… [401]`) instead of silently showing "No accounts configured".

---

## 4. Authentication & routing — the important gotcha

Getting a custom endpoint authenticated by the U17 backoffice took several iterations. The chain of failures
and what each taught us:

| Symptom | Cause | Fix |
|---|---|---|
| `No accounts configured` (silent) | A `[BackOfficeRoute]` controller + `Authorization: Bearer <token>`; in U17 the token is a redacted sentinel, so the bearer scheme rejected it. | Surface errors; stop sending a real bearer token. |
| **HTTP 404** | Wrong URL — `[umbracoBackOffice]` resolves to the **Umbraco path root** (`umbraco`), not `umbraco/backoffice`. | Corrected the path. |
| **HTTP 401** | A plain `[BackOfficeRoute]` controller is **not** covered by the backoffice cookie auth pipeline. | Route through the management API. |
| **HTTP 401** (still) | Umbraco's management API only discovers controllers that inherit **`ManagementApiControllerBase`**; a plain controller under `/management/api` falls through. | Inherit `ManagementApiControllerBase`. |
| OpenIddict `missing_token` (ID2000) | Controller now reached, but the request sent only the cookie. Umbraco's `HideBackOfficeTokensHandler` only swaps in the real cookie token when the request carries the **redacted sentinel** `Authorization: Bearer [redacted]`. | Send **both** the sentinel bearer (from the auth context) **and** `credentials: "include"`. |

**Final working recipe:**

* **Server:** inherit `ManagementApiControllerBase` + `[VersionedApiBackOfficeRoute("twentythree")]` +
  `[ApiVersion("1.0")]`. The base supplies `[ApiController]`, `[MapToApi("management")]` and the
  `BackOfficeAccess` authorization.
* **Client:** every request sends `Authorization: Bearer <sentinel>` (the sentinel is read from
  `UMB_AUTH_CONTEXT.getOpenApiConfiguration().token` in `EntryPoint.js` and stashed in `Auth.js`) **and**
  `credentials: "include"` (sends the `__Host-umbAccessToken` cookie). Umbraco swaps the sentinel for the
  real token server‑side.

This mirrors exactly what Umbraco's own generated backoffice client does.

---

## 5. Configuration (appsettings.json)

Credentials are read from the `Limbo:TwentyThree:Credentials` section (unchanged from v13). The properties use
internal setters and are bound with `BindNonPublicProperties`.

```jsonc
{
  "Limbo": {
    "TwentyThree": {
      "Credentials": [
        {
          "Key": "00000000-0000-0000-0000-000000000000",
          "Name": "My account",
          "Domains": [ "myaccount.23video.com" ],
          "ConsumerKey": "...",
          "ConsumerSecret": "...",
          "AccessToken": "...",
          "AccessTokenSecret": "..."
        }
      ]
    }
  }
}
```

---

## 6. Build, pack & install

```bash
# build
dotnet build src/Limbo.Umbraco.TwentyThree.sln -c Release

# pack (produces Limbo.Umbraco.TwentyThree.17.0.0-alpha000.nupkg)
dotnet pack src/Limbo.Umbraco.TwentyThree/Limbo.Umbraco.TwentyThree.csproj -c Release -o ./artifacts
```

Because the version is a prerelease, install with the prerelease flag:

```bash
dotnet add package Limbo.Umbraco.TwentyThree --version 17.0.0-alpha000
```

The `wwwroot` assets are shipped as static web assets (under `staticwebassets/` in the package) and served at
`/App_Plugins/Limbo.Umbraco.TwentyThree/…`.

> **Tip while developing:** the backoffice caches extension JS aggressively. After rebuilding, hard‑refresh
> (Cmd/Ctrl + Shift + R) so the updated bundle / importmap is loaded.

---

## 7. Verification status

Verified statically: Release build, `dotnet pack`, `node --check` on every JS module, manifest JSON validity,
and that all assets land in the `.nupkg`. Verified at runtime against a live U17 site: the management API
endpoint authenticates (account/video/spot/player overlays reachable).

**Possible follow‑ups / notes**

* The brand SVG icon registration was deferred; the property editor currently uses the built‑in
  `icon-movie-alt`. The original SVGs are still shipped under `wwwroot/BackOffice/Icons/`.
* Editor UI labels are inlined in English in the JS. A JS `localization` extension manifest could be added if
  multi‑language backoffice labels are needed (server‑side error messages still use `wwwroot/Lang/*.xml`).
* The controller maps into the core `management` OpenAPI group via `[MapToApi("management")]`. If a separate
  API group is preferred, register a custom Swagger document and change the map name.
