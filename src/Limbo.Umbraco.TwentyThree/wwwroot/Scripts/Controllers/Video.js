angular.module("umbraco").controller("Limbo.Umbraco.TwentyThree.Video", function ($scope, $element, $timeout, twentyThreeService) {

    const vm = this;

    vm.config = $scope.model.config ?? {};

    vm.showSite = vm.config.hideSite !== true;
    vm.showPlayer = true;
    vm.showEmbed = vm.config.hideEmbed !== true;
    
    vm.autoplay = [
        { alias: "inherit", label: "Inherit" },
        { alias: "enabled", label: "Enabled" },
        { alias: "disabled", label: "Disabled" }
    ];

    vm.endOn = [
        { alias: "inherit", label: "Inherit" },
        { alias: "share", label: "Share" },
        { alias: "browse", label: "Browse" },
        { alias: "loop", label: "Loop" },
        { alias: "thumbnail", label: "Thumbnail" }
    ];

    vm.labels = {
        urlOrEmbedCode: "URL or embed code",
        addVideo: "Select a video",
        refreshVideo: "Refresh current video",
        overridden: "Overridden by data type."
    };

    vm.setVideo = function (item, source, refresh) {

        if (!item) {
            vm.video = null;
            if ($scope.model.value) {
                if ($scope.model.value.source) {
                    delete $scope.model.value.credentials;
                    delete $scope.model.value.site;
                    delete $scope.model.value.parameters;
                    delete $scope.model.value.video;
                    delete $scope.model.value.player;
                } else {
                    $scope.model.value = null;
                }
            }
            vm.update();
            return;
        }

        if (!$scope.model.value) $scope.model.value = {};

        if (source) $scope.model.value.source = source;

        $scope.model.value.site = item.site;
        $scope.model.value.credentials = item.credentials;
        $scope.model.value.parameters = item.parameters;
        $scope.model.value.video = { _data: angular.toJson(item.video) };

        if (!$scope.model.value.player) {
            $scope.model.value.player = item.player;
        } else if (!refresh && item.parameters.playerId) {
            $scope.model.value.player = item.player;
        }

        if (!$scope.model.value.embed) $scope.model.value.embed = { autoplay: "inherit", endOn: "inherit" };

        // Keep the raw video data around for later
        vm.video = item.video;

        vm.loading = false;
        vm.update();

    };

    // Gets information about the video of the entered URL
    vm.getVideo = function (refresh) {

        const source = $scope.model.value && $scope.model.value.source ? $scope.model.value.source.trim() : null;

        if (!source) {
            vm.setVideo(null);
            return;
        }

        vm.loading = true;
        vm.update();

        twentyThreeService.getVideo(source).then(function(res) {
            vm.setVideo(res.data, null, refresh);
            vm.loading = false;
            vm.update();
        });

    };

    // Triggered by the UI when the user changes the URL
    vm.updated = function () {
        vm.getVideo();
    };

    // Triggered by the user when they click the refresh button
    vm.refreshVideo = function () {
        vm.getVideo(true);
    };

    vm.update = function () {

        const embed = $scope.model.value && $scope.model.value.source && $scope.model.value.source.indexOf("<") >= 0;

        if (embed !== vm.embed) {
            vm.embed = embed;
            const el = $element[0].querySelector(embed ? "textarea" : "input");
            if (el) {
                // Add a bit delay so the element is visible before we try to focus it
                $timeout(function () {
                    el.focus();
                }, 20);
            }
        }

        if (!vm.video) {
            vm.videoId = null;
            vm.title = null;
            vm.duration = null;
            vm.thumbnail = null;
            vm.thumbnails = null;
            return;
        }

        vm.videoId = vm.video.photo_id;
        vm.title = vm.video.title;
        vm.thumbnail = twentyThreeService.getThumbnails(vm.video).medium;
        vm.duration = vm.video.video_length;

        if (vm.config.autoplay !== "inherit") {
            vm.currentAutoplay = vm.autoplay.find(x => x.alias === vm.config.autoplay);
        } else {
            vm.currentAutoplay = vm.autoplay.find(x => x.alias === $scope.model.value.embed.autoplay);
        }

        if (vm.config.endOn !== "inherit") {
            vm.currentEndOn = vm.endOn.find(x => x.alias === vm.config.endOn);
        } else {
            vm.currentEndOn = vm.endOn.find(x => x.alias === $scope.model.value.embed.endOn);
        }

    };

    // Opens a new overlay where the editor can search and pick videos
    vm.addVideo = function () {
        twentyThreeService.openAddVideo(function (model) {
            if (!$scope.model.value) $scope.model.value = {};
            vm.setVideo(model.selectedItem, model.selectedItem.url);
            model.close();
        });
    };

    vm.selectPlayer = function () {
        twentyThreeService.selectPlayer({
            credentials: $scope.model.value.credentials,
            player: $scope.model.value.player,
            submit: function(model) {
                $scope.model.value.player = model.selectedPlayer;
                model.close();
            }
        });
    };

    vm.setAutoplay = function(o) {
        if (!$scope.model.embed) $scope.model.embed = {};
        switch (o.alias) {
            case "enabled":
                $scope.model.embed.autoplay = true;
                break;
            case "disabled":
                $scope.model.embed.autoplay = false;
                break;
            default:
                $scope.model.embed.autoplay = null;
                break;
        }
    };

    vm.setAutoplay = function (o) {
        if (!$scope.model.embed) $scope.model.embed = {};
        $scope.model.value.embed.autoplay = o.alias;
    };

    vm.setEndOn = function (o) {
        if (!$scope.model.embed) $scope.model.embed = {};
        $scope.model.value.embed.endOn = o.alias;
    };

    function init() {

        if (!$scope.model.value) {
            $scope.model.value = null;
            return;
        }

        if (!$scope.model.value.video) return;
        if (!$scope.model.value.video._data) return;

        // Get the video data from the "_data" property (necessary due to Umbraco/JSON.net issue)
        vm.video = angular.fromJson($scope.model.value.video._data);

        if (!$scope.model.value.embed) $scope.model.value.embed = { autoplay: "inherit", endOn: "inherit" };

        vm.update();

    }

    init();

});