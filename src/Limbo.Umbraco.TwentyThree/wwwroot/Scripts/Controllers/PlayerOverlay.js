angular.module("umbraco").controller("Limbo.Umbraco.TwentyThree.PlayerOverlay", function ($scope) {

    const vm = this;

    vm.select = function (player) {
        $scope.model.selectedPlayer = player;
        $scope.model.submit($scope.model);
    };

});