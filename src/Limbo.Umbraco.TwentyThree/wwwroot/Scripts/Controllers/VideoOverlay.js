angular.module("umbraco").controller("Limbo.Umbraco.TwentyThree.VideoOverlay", function ($scope, $element, $http, $timeout, localizationService, notificationsService, twentyThreeService) {

    const umbracoPath = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath;

    const vm = this;

    vm.account = null;
    vm.videos = [];
    vm.loaded = false;

    vm.text = null;
    vm.limit = null;

    // Parse the configuration passed to the overlay. This will likely come from the data type
    vm.config = $scope.model.config ?? {};
    if (!vm.config.descriptionMaxLength) vm.config.descriptionMaxLength = 0;

    let wait = null;

    // Loads the previous page
    vm.prev = function () {
        if (vm.pagination.page > 1) vm.getVideos(vm.pagination.page - 1);
    };

    // Loads the next pages
    vm.next = function () {
        if (vm.pagination.page < vm.pagination.pages) vm.getVideos(vm.pagination.page + 1);
    };

    vm.getVideos = function (page) {

        if (!vm.limit) {
            const container = $element[0].querySelector(".umb-editor-container");
            if (!container) {
                vm.limit = 20;
            } else {
                const width = container.clientWidth;
                const height = container.clientHeight;
                const x = Math.floor(width / 290); // 290
                const y = Math.floor(height / 230);
                vm.limit = Math.floor(Math.min(x * y, 50) / x) * x;
            }
        }

        $scope.model.loading = true;

        const params = {
            accountId: vm.account.id,
            limit: Math.max(10, vm.limit)
        };

        if (page) params.page = page;
        if (vm.text) params.text = vm.text;

        $http.get(`${umbracoPath}/backoffice/Limbo/TwentyThree/GetVideos`, { params }).then(function (res) {

            vm.pagination = {
                page: res.data.page,
                pages: res.data.pages,
                total: res.data.pages,
                pagination: []
            };

            const from = Math.max(1, vm.pagination.page - 7);
            const to = Math.min(vm.pagination.pages, vm.pagination.page + 7);

            for (let i = from; i <= to; i++) {
                vm.pagination.pagination.push({
                    page: i,
                    active: vm.pagination.page === i
                });
            }

            vm.videos = res.data.videos.map(function (x) {

                const scheme = x.absolute_url.split(":")[0];
                const domain = x.absolute_url.split("/")[2];

                const item = {
                    credentials: vm.account,
                    parameters: {
                        videoId: x.photo_id,
                        token: x.token,
                        playerId: null,
                        autoplay: null,
                        endOn: null
                    },
                    url: scheme + "://" + domain + "/manage/video/" + x.photo_id,
                    site: res.data.site,
                    video: x,
                    videoId: x.photo_id,
                    title: x.title,
                    description: x.content_text,
                    duration: x.video_length,
                    thumbnails: twentyThreeService.getThumbnails(x),
                    player: vm.player
                };

                // If the description is the same as the title, we shouldn't show the description
                if (item.description == item.title) item.description = null;

                // If a maximum description length is configured, we truncate the description if it's too long
                if (vm.config.descriptionMaxLength > 3 && item.description && item.description.length > vm.config.descriptionMaxLength) {
                    item.description = item.description.substr(0, vm.config.descriptionMaxLength - 3) + "...";
                }

                // If the maximum description length is a negative number, we hide the escription entirely
                if (vm.config.descriptionMaxLength < 0) item.description = null;

                return item;

            });

            $scope.model.loading = false;
            vm.loaded = true;

        }, function (res) {

            $scope.model.loading = false;

            if (typeof res.data === "string") {
                notificationsService.error("TwentyThree", res.data);
            } else {
                notificationsService.error("TwentyThree", "Failed getting list of videos from the TwentyThree API.");
            }

        });

    };

    vm.selectAccount = function (account) {

        $scope.model.loading = true;

        $scope.model.title = "Select video";
        $scope.model.size = "large";

        vm.account = account;

        localizationService.localize("twentyThree_videoOverlayTitle").then(function (value) {
            $scope.model.title = value;
        });

        $http.get(`${umbracoPath}/backoffice/Limbo/TwentyThree/GetPlayers?credentialsId=${vm.account.id}`).then(function (res1) {

            vm.player = res1.data.find(x => x.default);

            vm.getVideos();

        }, function (res) {

            if (typeof res.data === "string") {
                notificationsService.error("TwentyThree", res.data);
            } else {
                notificationsService.error("TwentyThree", "Failed getting list of TwentyThree accounts.");
            }

        });

    };

    vm.selectVideo = function (item) {
        $scope.model.selectedItem = item;
        $scope.model.submit($scope.model);
    };

    vm.textChanged = function () {

        if (wait) $timeout.cancel(wait);

        // Add a small delay so we dont call the API on each keystroke
        wait = $timeout(function () {
            vm.getVideos(1);
        }, 300);

    };

});