import { TwentyThreeAuth } from "@limbo/twentythree/auth";


function hi(url, config) {

    if (!config) config = {};
    if (!config.method) config.method = "GET";
    if (!config.headers) config.headers = {};

    return new Promise((resolve, reject) => {

        TwentyThreeAuth.TOKEN().then(function (token) {

            config.headers.Authorization = "Bearer " + token;

            //console.log(config.method + " " + url);

            const response = fetch(url, config);

            response.then(function (res) {

                res.json().then(function (json) {
                    res.data = json;
                    if (res.status < 400) {
                        resolve(res);
                    } else {
                        reject(res);
                    }
                });

            }, function (res) {

                // sending the request failed (before actually calling the URL)

                console.log("failed", arguments);

            });

        });

    });

}

function get(url) {
    return hi(url);
}

export class TwentyThreeService {

    static getVideo(source) {
        return get("/umbraco/limbo/twentythree/video?source=" + encodeURIComponent(source));
    }

};

export default TwentyThreeService;