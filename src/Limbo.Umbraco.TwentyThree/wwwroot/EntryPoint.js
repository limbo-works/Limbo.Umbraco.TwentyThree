import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";

import { TwentyThreeAuth } from "@limbo/twentythree/auth";
import { TwentyThreePackage } from "@limbo/twentythree/package";
import { TwentyThreeService } from "@limbo/twentythree/service";

console.log("hello from enty point");

export const onInit = (_host, extensionRegistry) => {

    _host.consumeContext(UMB_AUTH_CONTEXT, (authContext) => {

        const config = authContext.getOpenApiConfiguration();
        TwentyThreeAuth.TOKEN = config.token;

    });

};