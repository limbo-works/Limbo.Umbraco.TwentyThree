import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";

import { TwentyThreeAuth } from "@limbo/twentythree/auth";
import { TwentyThreePackage } from "@limbo/twentythree/package";
import { TwentyThreeService } from "@limbo/twentythree/service";

function onPackageLoaded(extensionRegistry) {

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
                        propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
                    },
                    {
                        alias: "loop",
                        label: "Loop?",
                        description: "Allow looping?",
                        propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
                    },
                    {
                        alias: "endOn",
                        label: "End On?",
                        description: "Specify when to end?",
                        propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
                    },
                    {
                        alias: "hideSite",
                        label: "Hide Site?",
                        description: "Specify whether site information should be hidden in the property editor.",
                        propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
                    },
                    {
                        alias: "hideEmbed",
                        label: "Hide Embed?",
                        description: "Specify whether embed information should be hidden in the property editor.",
                        propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
                    },
                    {
                        alias: "hidePlayer",
                        label: "Hide Player?",
                        description: "Specify whether player information should be hidden in the property editor.",
                        propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
                    },
                    {
                        alias: "allowVideos",
                        label: "Allow Videos?",
                        description: "Specify whether videos should be allowed.",
                        propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
                    },
                    {
                        alias: "allowSpots",
                        label: "Allow Spots?",
                        description: "Specify whether spots should be allowed.",
                        propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
                    },
                    {
                        alias: "showUploadLink",
                        label: "Show Upload Link?",
                        description: "Specify whether the upload link should be shown.",
                        propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
                    },
                    {
                        alias: "descriptionMaxLength",
                        label: "Description Max Length?",
                        description: "Specify the maximum length for the description.",
                        propertyEditorUiAlias: "Umb.PropertyEditorUi.Integer"
                    }
                ]
            }
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