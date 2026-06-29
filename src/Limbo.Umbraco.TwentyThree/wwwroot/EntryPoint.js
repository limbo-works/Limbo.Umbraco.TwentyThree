import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";

import { TwentyThreeAuth } from "@limbo/twentythree/auth";
import { TwentyThreePackage } from "@limbo/twentythree/package";
import { TwentyThreeService } from "@limbo/twentythree/service";


const BUTTON_LIST = "Limbo.Umbraco.TwentyThree.ButtonList.PropertyEditorUi";

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
        "name": "English",
        "js": () => import("./Localization/en-US.js?v=" + TwentyThreePackage.cacheBuster),
        "meta": {
            "culture": "en"
        }
    });

    extensionRegistry.register({
        "type": "localization",
        "alias": "Limbo.Umbraco.TwentyThree.DaDk",
        "name": "Danish",
        "js": () => import("./Localization/da-DK.js?v=" + TwentyThreePackage.cacheBuster),
        "meta": {
            "culture": "da"
        }
    });

    extensionRegistry.register({
        type: "propertyEditorSchema",
        alias: "Limbo.Umbraco.TwentyThree.Video",
        name: "TwentyThree Video",
        meta: {
            label: "TwentyThree Video",
            icon: "limbo-twentythree",
            group: "Limbo",
            defaultPropertyEditorUiAlias: "Limbo.Umbraco.TwentyThree.Video.Ui",
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
        name: "Limbo TwentyThree Button List",
        js: () => import("./Elements/ButtonList.js?v=" + TwentyThreePackage.cacheBuster),
        elementName: "limbo-twentythree-button-list",
        meta: {
            label: "TwentyThree Button List",
            icon: "icon-list",
            group: "common"
        }
    });

    extensionRegistry.register({
        type: "propertyEditorUi",
        alias: "Limbo.Umbraco.TwentyThree.Video.Ui",
        name: "TwentyThree Video Property Editor UI",
        js: () => import("./Elements/Video.js?v=" + TwentyThreePackage.cacheBuster),
        elementName: "limbo-twentythree-video",
        meta: {
            label: "TwentyThree Video",
            propertyEditorSchemaAlias: "Limbo.Umbraco.TwentyThree.Video",
            icon: "limbo-twentythree",
            group: "Limbo",
            supportsReadOnly: true
        }
    });

    extensionRegistry.register({
        "type": "icons",
        "alias": "Limbo.Umbraco.TwentyThree.Icons",
        "name": "Limbo TwentyThree Icons",
        "js": "/App_Plugins/Limbo.Umbraco.TwentyThree/Icons.js?v=" + TwentyThreePackage.cacheBuster,
    });

    extensionRegistry.register({
        "type": "modal",
        "alias": "Limbo.Umbraco.TwentyThree.SelectVideoModal",
        "name": "Select Video Modal",
        "element": "/App_Plugins/Limbo.Umbraco.TwentyThree/Modals/SelectVideo.js?v=" + TwentyThreePackage.cacheBuster,
    });

    extensionRegistry.register({
        "type": "modal",
        "alias": "Limbo.Umbraco.TwentyThree.SelectSpotModal",
        "name": "Select Spot Modal",
        "element": "/App_Plugins/Limbo.Umbraco.TwentyThree/Modals/SelectSpot.js?v=" + TwentyThreePackage.cacheBuster,
    });

    extensionRegistry.register({
        "type": "modal",
        "alias": "Limbo.Umbraco.TwentyThree.SelectPlayerModal",
        "name": "Select Player Modal",
        "element": "/App_Plugins/Limbo.Umbraco.TwentyThree/Modals/SelectPlayer.js?v=" + TwentyThreePackage.cacheBuster,
    });

    extensionRegistry.register({
        "type": "modal",
        "alias": "Limbo.Umbraco.TwentyThree.UploadVideoModal",
        "name": "Upload Video Modal",
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