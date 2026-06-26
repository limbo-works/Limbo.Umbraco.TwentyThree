// Holds the backoffice access-token resolver, populated by EntryPoint.js from UMB_AUTH_CONTEXT.
//
// In Umbraco 17 the real access token lives in an HTTP-only "__Host-umbAccessToken" cookie. JavaScript can
// only obtain a redacted sentinel ("[redacted]") via the auth context. Requests must send that sentinel as a
// "Authorization: Bearer" header AND include the cookie (credentials: "include"); Umbraco's
// HideBackOfficeTokensHandler then swaps the sentinel for the real token server-side.
// [CHANGE: Umbraco 17 migration — auth bridge for the management-API Service.js client] Related: EntryPoint.js, Service.js

let tokenResolver;

/**
 * Stores the resolver used to obtain the (sentinel) access token.
 * @param {string | (() => string | Promise<string>)} resolver
 */
export function setTokenResolver(resolver) {
    tokenResolver = resolver;
}

/**
 * Resolves the (sentinel) access token, or undefined if the auth context isn't ready yet.
 * @returns {Promise<string | undefined>}
 */
export async function getToken() {
    if (tokenResolver === undefined || tokenResolver === null) return undefined;
    try {
        return typeof tokenResolver === "function" ? await tokenResolver() : await tokenResolver;
    } catch {
        return undefined;
    }
}
