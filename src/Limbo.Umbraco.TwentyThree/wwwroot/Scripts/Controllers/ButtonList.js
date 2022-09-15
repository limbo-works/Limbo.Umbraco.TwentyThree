angular.module("umbraco").controller("Limbo.Umbraco.TwentyThree.ButtonList", function ($scope) {

    const vm = this;

    // Get the query string of the view URL
    const urlParts = $scope.model.view.split("?");
    const urlQuery = new URLSearchParams(urlParts.length === 1 ? "" : urlParts[1]);
    const type = urlQuery.get("type");

    vm.options = [];

    vm.select = function(option) {
        $scope.model.value = option.alias;
    };

    if (type === "autoplay") {

        vm.options = [
            { alias: "inherit", label: "Inherit" },
            { alias: "enabled", label: "Enabled" },
            { alias: "disabled", label: "Disabled" }
        ];

        if (!$scope.model.value) $scope.model.value = vm.options[0].alias;

    } else if (type === "endOn") {

        vm.options = [
            { alias: "inherit", label: "Inherit" },
            { alias: "share", label: "Share" },
            { alias: "browse", label: "Browse" },
            { alias: "loop", label: "Loop" },
            { alias: "thumbnail", label: "Thumbnail" }
        ];

        if (!$scope.model.value) $scope.model.value = vm.options[0].alias;

    }

});