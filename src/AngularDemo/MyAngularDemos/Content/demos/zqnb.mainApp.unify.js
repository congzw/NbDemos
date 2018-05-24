(function () {
    "use strict";
    var mainApp = zqnb.mainApp;
    mainApp.directive('nbTabs', function () {
        return {
            restrict: 'EA',
            transclude: true,
            scope: {},
            controller: function ($scope, $element) {
                var nbTabs = $scope.nbTabs = [];

                $scope.select = function (nbTab) {
                    angular.forEach(nbTabs, function (nbTab) {
                        nbTab.selected = false;
                    });
                    nbTab.selected = true;
                }

                this.addPane = function (nbTab) {
                    if (nbTabs.length === 0) $scope.select(nbTab);
                    nbTabs.push(nbTab);
                }
            },
            template:
                '<div class="tabbable">' +
                    '<ul class="nav nav-tabs">' +
                    '<li ng-repeat="nbTab in nbTabs" ng-class="{active:nbTab.selected}">' +
                    '<a href="" ng-click="select(nbTab)">{{nbTab.title}}</a>' +
                    '</li>' +
                    '</ul>' +
                    '<div class="tab-content" ng-transclude></div>' +
                    '</div>',
            replace: true
        };
    })
        .directive('nbTab', function () {
            return {
                require: '^nbTabs',
                restrict: 'EA',
                transclude: true,
                scope: { title: '@' },
                link: function (scope, element, attrs, tabsController) {
                    tabsController.addPane(scope);
                },
                template:
                    '<div class="tab-pane" ng-class="{active: selected}" ng-transclude>' +
                        '</div>',
                replace: true
            };
        });

}());