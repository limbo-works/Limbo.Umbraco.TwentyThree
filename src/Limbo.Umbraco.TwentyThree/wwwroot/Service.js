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

export class TwentyThreeService {

    static getServerVariables() {
        return get(`${baseUrl}/serverVariables`).then(function (res) {
            return res.data;
        });
    }

    static getVideo(source) {
        return get(`${baseUrl}/video?source=${encodeURIComponent(source)}`);
    }

};

export default TwentyThreeService;