import { TwentyThreeAuth } from "@limbo/twentythree/auth";

async function hi(url, config) {

    if (!config) config = {};
    if (!config.method) config.method = "GET";
    if (!config.headers) config.headers = {};

    const token = await TwentyThreeAuth.TOKEN();
    config.headers.Authorization = `Bearer ${token}`;

    const res = await fetch(url, config);

    const contentType = res.headers.get("content-type") || "";

    if (contentType.includes("application/json")) {
        res.data = await res.json();
    } else if (contentType.startsWith("text/")) {
        res.textContent = await res.text();
    } else {
        throw new Error(`Unsupported content type: ${contentType}`);
    }

    if (!res.ok) {
        throw res;
    }

    return res;

}

async function get(url) {
    return await hi(url);
}

const baseUrl = "/umbraco/limbo/twentythree";

function getThumbnails(video) {

    if (!video) return null;

    const scheme = video.absolute_url.split(":")[0];
    const domain = video.absolute_url.split("/")[2];

    const aliases = ["quad16", "quad50", "quad75", "quad100", "small", "medium", "portrait", "standard", "large", "original"];

    const thumbnails = [];

    aliases.forEach(function (name) {

        const download = video[name + "_download"];
        if (!download) return;

        const width = video[name + "_width"];
        const height = video[name + "_height"];

        const thumbnail = {
            name: name,
            width: width,
            height: height,
            url: `${scheme}://${domain}${download}`
        };

        thumbnails.push(thumbnail);
        thumbnails[name] = thumbnail;

    });

    return thumbnails;

}

export class TwentyThreeService {

    static getServerVariables() {
        return get(`${baseUrl}/serverVariables`).then(function (res) {
            return res.data;
        });
    }

    static getVideo(source) {
        return get(`${baseUrl}/video?source=${encodeURIComponent(source)}`);
    }

    static async getAccounts(source) {
        const response = await get(`${baseUrl}/accounts`);
        return response.data;
    }

    static async getAlbums(account) {
        const response = await get(`${baseUrl}/accounts/${account.id}/albums`);
        return response.data;
    }

    static async getVideos(account, query) {
        if (!query) query = {};
        const response = await get(`${baseUrl}/accounts/${account.id}/videos?${new URLSearchParams(query).toString()}`);
        return response.data;
    }

    static async getSpots(account, query) {
        if (!query) query = {};
        const response = await get(`${baseUrl}/accounts/${account.id}/spots?${new URLSearchParams(query).toString()}`);
        return response.data;
    }

    static async getPlayers(account) {
        const response = await get(`${baseUrl}/accounts/${account.id}/players`);
        return response.data;
    }

    static getThumbnails(video) {
        return getThumbnails(video);
    }

};

export default TwentyThreeService;