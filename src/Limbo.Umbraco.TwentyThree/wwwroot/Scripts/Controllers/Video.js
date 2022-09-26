angular.module("umbraco").controller("Limbo.Umbraco.TwentyThree.Video", function ($scope, $element, $timeout, notificationsService, twentyThreeService) {

    const vm = this;

    vm.config = $scope.model.config ?? {};

    if (!vm.config.autoplay) vm.config.autoplay = "inherit";
    if (!vm.config.endOn) vm.config.endOn = "inherit";

    vm.config.hideSite = vm.config.hideSite === true;
    vm.config.hidePlayer = vm.config.hidePlayer === true;
    vm.config.hideEmbed = vm.config.hideEmbed === true;

    vm.config.allowVideos = vm.config.allowVideos !== false;
    vm.config.allowSpots = vm.config.allowSpots !== false;

    vm.config.dataTypeKey = $scope.model.dataTypeKey;

    vm.showSite = vm.config.hideSite !== true;
    vm.showPlayer = vm.config.hidePlayer !== true;
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
        addSpot: "Select a spot",
        refreshVideo: "Refresh current video",
        refreshSpot: "Refresh current spot",
        overridden: "Overridden by data type.",
        video: "video",
        videos: "videos"
    };

    vm.setVideo = function (item, source, refresh) {

        if (!item) {
            vm.spot = null;
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

        if (item.video) {

            delete vm.spot;

            delete $scope.model.value.spot;

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

        } else if (item.spot) {

            delete vm.video;

            delete $scope.model.value.parameters;
            delete $scope.model.value.video;
            delete $scope.model.value.player;
            delete $scope.model.value.embed;

            $scope.model.value.spot = { _data: angular.toJson(item.spot) };
            $scope.model.value.thumbnails = item.thumbnails;

            // Keep the raw data around for later
            vm.spot = item.spot;
            vm.thumbnails = item.thumbnails;

        }

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

        twentyThreeService.getVideo(source, vm.config).then(function(res) {
            vm.setVideo(res.data, null, refresh);
            vm.loading = false;
            vm.update();
        }, function(res) {
            vm.loading = false;
            if (typeof res.data === "string") {
                notificationsService.error("TwentyThree", res.data);
            } else {
                notificationsService.error("TwentyThree", "An unknown error occured.");
            }
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

        if (vm.video) {

            vm.id = vm.video.photo_id;
            vm.title = vm.video.title;
            vm.duration = vm.video.video_length;
            vm.thumbnails = twentyThreeService.getThumbnails(vm.video);
            vm.thumbnail = vm.thumbnails.medium;

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

        } else if (vm.spot) {

            vm.id = vm.spot.spot_id;
            vm.title = vm.spot.spot_name;
            vm.videoCount = vm.spot.video_count;
            vm.duration = null;
            vm.thumbnail = vm.thumbnails?.find(x => x.alias === "medium");

        } else {

            vm.id = null;
            vm.title = null;
            vm.duration = null;
            vm.thumbnail = null;
            vm.thumbnails = null;

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

    // Opens a new overlay where the editor can browse and pick spots
    vm.addSpot = function () {
        twentyThreeService.openAddSpot(function (model) {
            if (!$scope.model.value) $scope.model.value = {};
            vm.setVideo(model.selectedItem, model.selectedItem.source);
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

        if ($scope.model.value.video) {

            // Get the video data from the "_data" property (necessary due to Umbraco/JSON.net issue)
            vm.video = angular.fromJson($scope.model.value.video._data);
            vm.duration = vm.video.video_length;

            if (!$scope.model.value.embed) $scope.model.value.embed = { autoplay: "inherit", endOn: "inherit" };

            vm.update();

        } else if ($scope.model.value.spot) {

            // Get the spot data from the "_data" property (necessary due to Umbraco/JSON.net issue)
            vm.spot = angular.fromJson($scope.model.value.spot._data);
            vm.thumbnails = $scope.model.value.thumbnails;

        }

        vm.update();

    }

    init();

});