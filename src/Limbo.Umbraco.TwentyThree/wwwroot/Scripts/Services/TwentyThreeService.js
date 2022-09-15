angular.module("umbraco.services").factory("twentyThreeService", function ($http, editorService, limboVideoService) {

    // Get relevant settings from Umbraco's server variables
    const cacheBuster = Umbraco.Sys.ServerVariables.application.cacheBuster;
    const umbracoPath = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath;

    function getVideo(source) {

        const data = {
            source: source
        };

        return $http({
            method: "POST",
            url: `${umbracoPath}/backoffice/Limbo/TwentyThree/GetVideo`,
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            data: $.param(data)
        });

    }

    function openAddVideo(submit) {

        const options = {
            title: "Select account",
            view: `/App_Plugins/Limbo.Umbraco.TwentyThree/Views/VideoOverlay.html?v=${cacheBuster}`,
            size: "medium",
            loading: true,
            accounts: [],
            close: function () {
                editorService.close();
            },
            submit: submit ? submit : function () { editorService.close(); }
        };

        editorService.open(options);

        $http.get(`${umbracoPath}/backoffice/Limbo/TwentyThree/GetAccounts`).then(function (res) {
            options.accounts = res.data;
            if (options.selectAccount && res.data.length === 1) {
                options.selectAccount(res.data[0]);
            } else {
                options.loading = false;
            }
        });

    }

    function selectPlayer(options) {

        if (!options) options = {};

        if (!options.title) options.title = "Select player";
        if (!options.view) options.view = `/App_Plugins/Limbo.Umbraco.TwentyThree/Views/PlayerOverlay.html?v=${cacheBuster}`;
        if (!options.size) options.size = "medium";
        if (!options.close) options.close = function () { editorService.close(); };
        if (!options.submit) options.submit = function () { editorService.close(); };

        options.loading = true;

        editorService.open(options);

        $http.get(`${umbracoPath}/backoffice/Limbo/TwentyThree/GetPlayers?credentialsId=${options.credentials.id}`).then(function(res) {
            options.players = res.data;
            options.loading = false;
        });

    }

    function getDuration(value, callback) {
        if (!value) return;
        const seconds = parseInt(value.video_length);
        limboVideoService.getDuration(seconds, callback);
    }

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

    return {
        getVideo,
        openAddVideo,
        getDuration,
        selectPlayer,
        getThumbnails
    };

});