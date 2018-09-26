require(['mainApp', 'nbLog'], function (mainApp, nbLog) {
    nbLog.debug("home.index.js");
    mainApp.controller('demoCtrl', function ($scope) {
        $scope.message = "from angular controller!";
    });
});