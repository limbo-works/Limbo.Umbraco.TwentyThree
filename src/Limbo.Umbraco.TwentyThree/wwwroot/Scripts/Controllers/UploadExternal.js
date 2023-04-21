angular.module("umbraco").controller("Limbo.Umbraco.TwentyThree.UploadExternal", function ($scope) {

    const vm = this;

    vm.selectAccount = function (account) {
        window.open(account.uploadUrl, "_blank");
        $scope.model.close();
    };

});