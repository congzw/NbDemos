requirejs(['angular', 'nbLog'], function (angular, nbLog) {

    nbLog.debug('bootstrap.js');

    requirejs(['mainApp'], function (mainApp) {

        nbLog.debug('bootstrap with mainApp(callback)');

        angular.element(document).ready(function () {
            angular.bootstrap(document, ['mainApp']);
            angular.element(document).find('html').addClass('ng-app');
        });
        return mainApp;
    });
});
