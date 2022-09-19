angular.module("umbraco").controller("Limbo.Umbraco.TwentyThree.SpotOverlay", function ($scope, $http, $timeout) {

    const umbracoPath = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath;

    const vm = this;

    vm.labels = {
        video: "video",
        videos: "videos"
    };

    vm.account = null;
    vm.spots = [];
    vm.loaded = false;

    let wait = null;

    // Loads the previous page
    vm.prev = function () {
        if (vm.pagination.page > 1) vm.getSpots(vm.pagination.page - 1);
    };

    // Loads the next pages
    vm.next = function () {
        if (vm.pagination.page < vm.pagination.pages) vm.getSpots(vm.pagination.page + 1);
    };

    vm.getSpots = function (page) {

        $scope.model.loading = true;

        const params = {
            accountId: vm.account.id,
            limit: 20
        };

        if (page) params.page = page;
        if (vm.text) params.text = vm.text;

        $http.get(`${umbracoPath}/backoffice/Limbo/TwentyThree/GetSpots`, { params }).then(function (res) {

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

            vm.spots = res.data.spots.map(function (x) {
                const item = {
                    source: x.include_html,
                    credentials: vm.account,
                    site: res.data.site,
                    spot: x,
                    title: x.spot_name,
                    description: `${x.video_count} ${x.video_count === "1" ? vm.labels.video : vm.labels.videos}`,
                    thumbnails: x.__thumbnails
                };
                item.thumbnails.forEach(function(y) {
                    item.thumbnails[y.alias] = y;
                });
                delete x.__thumbnails;
                return item;
            });

            $scope.model.loading = false;
            vm.loaded = true;

        });

    };

    vm.selectAccount = function (account) {

        $scope.model.loading = true;

        $scope.model.title = "Select spot";
        $scope.model.size = "large";

        vm.account = account;

        vm.getSpots();

    };

    vm.selectSpot = function (item) {
        $scope.model.selectedItem = item;
        $scope.model.submit($scope.model);
    };

    vm.textChanged = function () {

        if (wait) $timeout.cancel(wait);

        // Add a small delay so we dont call the API on each keystroke
        wait = $timeout(function () {
            vm.getSpots(1);
        }, 300);

    };

});