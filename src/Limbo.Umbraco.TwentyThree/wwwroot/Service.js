import { getToken } from "@limbo/twentythree/auth";

// API client for the TwentyThree backoffice controller.
// The controller is routed through the Umbraco management API (ManagementApiControllerBase +
// [VersionedApiBackOfficeRoute("twentythree")]), so it is served at <umbracoPath>/management/api/v1/twentythree.
// We derive <umbracoPath> from the current location so a customised Umbraco path keeps working, defaulting to
// "/umbraco".
//
// Authentication (Umbraco 17): the real token lives in an HTTP-only "__Host-umbAccessToken" cookie. Requests
// must send BOTH the redacted sentinel token as "Authorization: Bearer <sentinel>" (obtained from the auth
// context via EntryPoint.js) AND the cookie (`credentials: "include"`). Umbraco's HideBackOfficeTokensHandler
// then swaps the sentinel for the real token. Sending only the cookie yields OpenIddict "missing_token".
// [CHANGE: Umbraco 17 migration — replaces the AngularJS twentyThreeService]

const BASE = (() => {
    const segment = window.location.pathname.split("/").filter(Boolean)[0];
    const umbracoPath = segment ? `/${segment}` : "/umbraco";
    return `${umbracoPath}/management/api/v1/twentythree`;
})();

function queryString(params) {
    const search = new URLSearchParams();
    for (const [key, value] of Object.entries(params)) {
        if (value === undefined || value === null || value === "") continue;
        search.append(key, value);
    }
    const result = search.toString();
    return result ? `?${result}` : "";
}

async function request(path) {
    const headers = { Accept: "application/json" };

    const token = await getToken();
    if (token) headers.Authorization = `Bearer ${token}`;

    const response = await fetch(`${BASE}${path}`, {
        method: "GET",
        credentials: "include",
        headers
    });

    const text = await response.text();

    if (!response.ok) {
        let message = text;
        try {
            const parsed = JSON.parse(text);
            message = typeof parsed === "string" ? parsed : (parsed?.message ?? parsed?.detail ?? text);
        } catch {
            // Keep the raw text as the message
        }
        if (!message) message = `Request failed (HTTP ${response.status}).`;
        throw new Error(`${message} [${response.status}]`);
    }

    return text ? JSON.parse(text) : null;
}

export const TwentyThreeService = {

    /** Resolves a video or spot from a URL or embed code. */
    getVideo(source) {
        return request(`/video${queryString({ source })}`);
    },

    /** Returns the configured TwentyThree accounts. */
    getAccounts() {
        return request("/accounts");
    },

    /** Returns the albums (categories) of an account. */
    getAlbums(accountId) {
        return request(`/albums${queryString({ accountId })}`);
    },

    /** Returns a paginated list of videos for an account. */
    getVideos(accountId, { text, limit, page, albumId } = {}) {
        return request(`/videos${queryString({ accountId, text, limit, page, albumId })}`);
    },

    /** Returns a paginated list of spots for an account. */
    getSpots(accountId, { text, limit, page } = {}) {
        return request(`/spots${queryString({ accountId, text, limit, page })}`);
    },

    /** Returns the players available for an account. */
    getPlayers(credentialsId) {
        return request(`/players${queryString({ credentialsId })}`);
    },

    /** Builds the thumbnail set for a raw TwentyThree photo/video object. */
    getThumbnails(video) {
        if (!video || !video.absolute_url) return null;

        const scheme = video.absolute_url.split(":")[0];
        const domain = video.absolute_url.split("/")[2];

        const aliases = ["quad16", "quad50", "quad75", "quad100", "small", "medium", "portrait", "standard", "large", "original"];

        const thumbnails = [];

        for (const name of aliases) {
            const download = video[`${name}_download`];
            if (!download) continue;

            const thumbnail = {
                name,
                width: video[`${name}_width`],
                height: video[`${name}_height`],
                url: `${scheme}://${domain}${download}`
            };

            thumbnails.push(thumbnail);
            thumbnails[name] = thumbnail;
        }

        return thumbnails;
    },

    /** Builds the "manage in control panel" URL for a video. */
    getAppUrl(site, id) {
        return site?.secureDomain ? `https://${site.secureDomain}/manage/video/${id}` : null;
    }

};

export default TwentyThreeService;
