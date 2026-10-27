import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";

import { TwentyThreeAuth } from "@limbo/twentythree/auth";
import { TwentyThreePackage } from "@limbo/twentythree/package";
import { TwentyThreeService } from "@limbo/twentythree/service";

const ALIAS = "Limbo.Umbraco.TwentyThree";

const NAME = "Limbo TwentyThree";

const VIDEO_SCHEMA = "Limbo.Umbraco.TwentyThree.Video";

const VIDEO_UI = "Limbo.Umbraco.TwentyThree.PropertyEditorUi.Video";

const BUTTON_LIST = "Limbo.Umbraco.TwentyThree.PropertyEditorUi.ButtonList";

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

function onPackageLoaded(extensionRegistry) {

    extensionRegistry.register({
        "type": "localization",
        "alias": "Limbo.Umbraco.TwentyThree.EnUs",
        "name": `${NAME}: English (en-US)`,
        "js": () => import("./Localization/en-US.js?v=" + TwentyThreePackage.cacheBuster),
        "meta": {
            "culture": "en"
        }
    });

    extensionRegistry.register({
        "type": "localization",
        "alias": "Limbo.Umbraco.TwentyThree.DaDk",
        "name": `${NAME}: Danish (da-DK)`,
        "js": () => import("./Localization/da-DK.js?v=" + TwentyThreePackage.cacheBuster),
        "meta": {
            "culture": "da"
        }
    });

    extensionRegistry.register({
        type: "propertyEditorSchema",
        alias: VIDEO_SCHEMA,
        name: `${NAME}: Video Property Editor Schema`,
        meta: {
            label: "Limbo TwentyThree Video",
            icon: "limbo-twentythree-alt",
            group: "Limbo",
            defaultPropertyEditorUiAlias: VIDEO_UI,
            settings: {
                properties: [
                    {
                        alias: "autoplay",
                        label: "Autoplay?",
                        description: "Allow autoplay?",
                        propertyEditorUiAlias: BUTTON_LIST,
                        config: [
                            { alias: "items", value: ON_OFF_INHERIT }
                        ]
                    },
                    {
                        alias: "loop",
                        label: "Loop?",
                        description: "Allow looping?",
                        propertyEditorUiAlias: BUTTON_LIST,
                        config: [
                            { alias: "items", value: ON_OFF_INHERIT }
                        ]
                    },
                    {
                        alias: "endOn",
                        label: "End On?",
                        description: "Specify when to end?",
                        propertyEditorUiAlias: BUTTON_LIST,
                        config: [
                            { alias: "items", value: END_ON }
                        ]
                    },
                    {
                        alias: "hideSite",
                        label: "Hide site?",
                        description: "Specify whether site information should be hidden in the property editor.",
                        propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
                    },
                    {
                        alias: "hideEmbed",
                        label: "Hide embed?",
                        description: "Specify whether embed information should be hidden in the property editor.",
                        propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
                    },
                    {
                        alias: "hidePlayer",
                        label: "Hide player?",
                        description: "Specify whether player information should be hidden in the property editor.",
                        propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
                    },
                    {
                        alias: "allowVideos",
                        label: "Allow videos?",
                        description: "Specify whether videos should be allowed.",
                        propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
                    },
                    {
                        alias: "allowSpots",
                        label: "Allow spots?",
                        description: "Specify whether spots should be allowed.",
                        propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
                    },
                    {
                        alias: "showUploadLink",
                        label: "Show upload link?",
                        description: "Specify whether the upload link should be shown.",
                        propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
                    },
                    {
                        alias: "descriptionMaxLength",
                        label: "Description max length?",
                        description: "Specify the maximum length for the description.",
                        propertyEditorUiAlias: "Umb.PropertyEditorUi.Integer"
                    }
                ],
                defaultData: [
                    { alias: "autoplay", value: "inherit" },
                    { alias: "loop", value: "inherit" },
                    { alias: "endOn", value: "inherit" },
                    { alias: "allowVideos", value: true },
                    { alias: "allowSpots", value: true },
                    { alias: "showUploadLink", value: false }
                ]
            }
        }
    });

    extensionRegistry.register({
        type: "propertyEditorUi",
        alias: BUTTON_LIST,
        name: `${NAME}: Button List Property Editor UI`,
        js: () => import("./Elements/ButtonList.js?v=" + TwentyThreePackage.cacheBuster),
        elementName: "limbo-twentythree-button-list",
        meta: {
            label: "Limbo TwentyThree Button List",
            icon: "icon-list",
            group: "Limbo"
        }
    });

    extensionRegistry.register({
        type: "propertyEditorUi",
        alias: VIDEO_UI,
        name: `${NAME}: Video Property Editor UI`,
        js: () => import("./Elements/Video.js?v=" + TwentyThreePackage.cacheBuster),
        elementName: "limbo-twentythree-video",
        meta: {
            label: "Limbo TwentyThree Video",
            propertyEditorSchemaAlias: VIDEO_SCHEMA,
            icon: "limbo-twentythree-alt",
            group: "Limbo",
            supportsReadOnly: true
        }
    });

    extensionRegistry.register({
        "type": "icons",
        "alias": "Limbo.Umbraco.TwentyThree.Icons",
        "name": `${NAME}: Icons`,
        "js": "/App_Plugins/Limbo.Umbraco.TwentyThree/Icons.js?v=" + TwentyThreePackage.cacheBuster,
    });

    extensionRegistry.register({
        "type": "modal",
        "alias": "Limbo.Umbraco.TwentyThree.SelectVideoModal",
        "name": `${NAME}: Select Video Modal`,
        "element": "/App_Plugins/Limbo.Umbraco.TwentyThree/Modals/SelectVideo.js?v=" + TwentyThreePackage.cacheBuster,
    });

    extensionRegistry.register({
        "type": "modal",
        "alias": "Limbo.Umbraco.TwentyThree.SelectSpotModal",
        "name": `${NAME}: Select Spot Modal`,
        "element": "/App_Plugins/Limbo.Umbraco.TwentyThree/Modals/SelectSpot.js?v=" + TwentyThreePackage.cacheBuster,
    });

    extensionRegistry.register({
        "type": "modal",
        "alias": "Limbo.Umbraco.TwentyThree.SelectPlayerModal",
        "name": `${NAME}: Select Player Modal`,
        "element": "/App_Plugins/Limbo.Umbraco.TwentyThree/Modals/SelectPlayer.js?v=" + TwentyThreePackage.cacheBuster,
    });

    extensionRegistry.register({
        "type": "modal",
        "alias": "Limbo.Umbraco.TwentyThree.UploadVideoModal",
        "name": `${NAME}: Upload Video Modal`,
        "element": "/App_Plugins/Limbo.Umbraco.TwentyThree/Modals/UploadVideo.js?v=" + TwentyThreePackage.cacheBuster,
    });

}

export const onInit = (_host, extensionRegistry) => {

    _host.consumeContext(UMB_AUTH_CONTEXT, (authContext) => {

        const config = authContext.getOpenApiConfiguration();
        TwentyThreeAuth.TOKEN = config.token;

        TwentyThreeService.getServerVariables().then(function (serverVariables) {
            TwentyThreePackage.serverVariables = serverVariables;
            onPackageLoaded(extensionRegistry);
        });

    });

};