angular.module("umbraco").controller("Limbo.Umbraco.TwentyThree.VideoOverlay", function ($scope, $http, $timeout, twentyThreeService) {

    const umbracoPath = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath;

    const vm = this;

    vm.account = null;
    vm.videos = [];
    vm.loaded = false;

    vm.text = null;

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

        $scope.model.loading = true;

        const params = {
            accountId: vm.account.id,
            limit: 20
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
                    duration: x.video_length,
                    thumbnails: twentyThreeService.getThumbnails(x),
                    player: vm.player
                };

                return item;

            });

            $scope.model.loading = false;
            vm.loaded = true;

        });

    };

    vm.selectAccount = function (account) {

        $scope.model.loading = true;

        $scope.model.title = "Select video";
        $scope.model.size = "normal";

        vm.account = account;

        $http.get(`${umbracoPath}/backoffice/Limbo/TwentyThree/GetPlayers?credentialsId=${vm.account.id}`).then(function (res1) {

            vm.player = res1.data.find(x => x.default);

            vm.getVideos();

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