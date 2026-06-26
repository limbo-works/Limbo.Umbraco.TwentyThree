// Bundle manifest for the Limbo TwentyThree backoffice extensions (Umbraco 17).
// Registers the property editor UI, its configuration option-select UI, and the four picker overlays.
// [CHANGE: Umbraco 17 migration — replaces the AngularJS package.manifest] Related: umbraco-package.json

const BASE = "/App_Plugins/Limbo.Umbraco.TwentyThree/BackOffice/TwentyThree";

const TOGGLE = "Umb.PropertyEditorUi.Toggle";
const INTEGER = "Umb.PropertyEditorUi.Integer";
const OPTION_SELECT = "Limbo.PropertyEditorUi.TwentyThree.OptionSelect";

const ON_OFF_INHERIT = [
    { alias: "inherit", label: "Inherit" },
    { alias: "enabled", label: "Enabled" },
    { alias: "disabled", label: "Disabled" }
];

const END_ON = [
    { alias: "inherit", label: "Inherit", title: "Inherit from player or embed code" },
    { alias: "share", label: "Share" },
    { alias: "browse", label: "Browse" },
    { alias: "loop", label: "Loop" },
    { alias: "thumbnail", label: "Thumbnail" }
];

export const manifests = [
    {
        type: "propertyEditorUi",
        alias: "Limbo.PropertyEditorUi.TwentyThree",
        name: "Limbo TwentyThree Property Editor UI",
        element: `${BASE}/property-editor-ui.element.js`,
        elementName: "twentythree-property-editor-ui",
        meta: {
            label: "Limbo TwentyThree Video",
            icon: "icon-movie-alt",
            group: "media",
            propertyEditorSchemaAlias: "Limbo.Umbraco.TwentyThree",
            supportsReadOnly: true,
            settings: {
                properties: [
                    { alias: "autoplay", label: "Autoplay", description: "Select whether videos should autoplay when embedded.", propertyEditorUiAlias: OPTION_SELECT, config: [{ alias: "items", value: ON_OFF_INHERIT }] },
                    { alias: "loop", label: "Loop", description: "Select whether videos should loop.", propertyEditorUiAlias: OPTION_SELECT, config: [{ alias: "items", value: ON_OFF_INHERIT }] },
                    { alias: "endOn", label: "End on", description: "Select what happens when a video ends.", propertyEditorUiAlias: OPTION_SELECT, config: [{ alias: "items", value: END_ON }] },
                    { alias: "hideSite", label: "Hide account information", description: "Select whether the account information should be hidden in the property editor.", propertyEditorUiAlias: TOGGLE },
                    { alias: "hideEmbed", label: "Hide embed options", description: "Select whether embed options should be hidden in the property editor.", propertyEditorUiAlias: TOGGLE },
                    { alias: "hidePlayer", label: "Hide player", description: "Select whether the player option should be hidden in the property editor.", propertyEditorUiAlias: TOGGLE },
                    { alias: "allowVideos", label: "Allow videos", description: "Select whether videos should be allowed in the property editor.", propertyEditorUiAlias: TOGGLE },
                    { alias: "allowSpots", label: "Allow spots", description: "Select whether spots should be allowed in the property editor.", propertyEditorUiAlias: TOGGLE },
                    { alias: "showUploadLink", label: "Show upload link", description: "Select whether the property editor should show a link for an external upload page.", propertyEditorUiAlias: TOGGLE },
                    { alias: "descriptionMaxLength", label: "Max description length", description: "The maximum description length shown in overlays. Descriptions exceeding this limit are truncated.", propertyEditorUiAlias: INTEGER }
                ],
                defaultData: [
                    { alias: "autoplay", value: "inherit" },
                    { alias: "loop", value: "inherit" },
                    { alias: "endOn", value: "inherit" },
                    { alias: "allowVideos", value: true },
                    { alias: "allowSpots", value: true },
                    { alias: "showUploadLink", value: true }
                ]
            }
        }
    },
    {
        type: "propertyEditorUi",
        alias: OPTION_SELECT,
        name: "Limbo TwentyThree Option Select",
        element: `${BASE}/option-select.element.js`,
        elementName: "twentythree-option-select",
        meta: {
            label: "TwentyThree Option Select",
            icon: "icon-list",
            group: "common"
        }
    },
    {
        type: "modal",
        alias: "Limbo.Umbraco.TwentyThree.Modal.VideoPicker",
        name: "Limbo TwentyThree Video Picker Modal",
        element: `${BASE}/modals/video-overlay.element.js`,
        elementName: "twentythree-video-overlay"
    },
    {
        type: "modal",
        alias: "Limbo.Umbraco.TwentyThree.Modal.SpotPicker",
        name: "Limbo TwentyThree Spot Picker Modal",
        element: `${BASE}/modals/spot-overlay.element.js`,
        elementName: "twentythree-spot-overlay"
    },
    {
        type: "modal",
        alias: "Limbo.Umbraco.TwentyThree.Modal.PlayerPicker",
        name: "Limbo TwentyThree Player Picker Modal",
        element: `${BASE}/modals/player-overlay.element.js`,
        elementName: "twentythree-player-overlay"
    },
    {
        type: "modal",
        alias: "Limbo.Umbraco.TwentyThree.Modal.Upload",
        name: "Limbo TwentyThree Upload Modal",
        element: `${BASE}/modals/upload-overlay.element.js`,
        elementName: "twentythree-upload-overlay"
    }
];

export default manifests;
