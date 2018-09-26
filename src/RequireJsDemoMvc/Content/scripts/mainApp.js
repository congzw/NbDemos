define(["angular", 'nbLog'], function (angular, nbLog) {

    nbLog.debug("mainApp.js");
    var mainApp = angular.module("mainApp", []);

    //服务演示代码
    mainApp.factory("demoMockService", function ($log) {

        var mockDatas = [1, 2, 3];

        var getSomeData = function () {
            $log.log('get is invoke: ' + mockDatas);
            return mockDatas;
        }

        return {
            getSomeData: getSomeData
        };
    });

    return mainApp;
});