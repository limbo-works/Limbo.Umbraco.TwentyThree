import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";

import { setTokenResolver } from "@limbo/twentythree/auth";

// Backoffice entry point: captures the auth-context token resolver so Service.js can attach the
// "Authorization: Bearer <sentinel>" header that Umbraco needs to extract the real token from the
// __Host-umbAccessToken cookie.
// [CHANGE: Umbraco 17 migration] Related: Auth.js, Service.js, umbraco-package.json

export const onInit = (host) => {
    host.consumeContext(UMB_AUTH_CONTEXT, (authContext) => {
        if (!authContext) return;
        const config = authContext.getOpenApiConfiguration();
        if (config?.token !== undefined) setTokenResolver(config.token);
    });
};
