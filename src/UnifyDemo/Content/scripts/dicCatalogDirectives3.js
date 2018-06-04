(function () {
    'use strict';
    var mainApp = zqnb.mainApp;

    var createEmptyItem = function () {
        return { Code: "", Name: "全部" };
    };

    mainApp.directive('nbSelectItems', function () {
        
        var template1 = function () {
            return '' +
                '<div class="term-box">  ' +
                '    <span class="term">{{vm.Title}}(<span class="selectedDicCatalogItem">{{vm.currentItem.Name}}</span>)</span>  ' +
                '    <ul class="nav nav-pills overflow-h">  ' +
                '        <li ng-repeat="item in vm.items" ng-class="{active: vm.isCurrentItem(item), hidden: item.Hidden}">  ' +
                '            <a href="javascript:void(0)" ng-click="vm.selectItem(item)">  ' +
                '                {{item.Name}}  ' +
                '            </a>  ' +
                '        </li>  ' +
                '    </ul>  ' +
                '</div>  ';
        }();
        var template2 = '';
        var template3 = '';
        var template4 = '';
        var getTemplate = function (tElem, tAttrs) {
            var mode = tAttrs.dicViewMode;
            if (!mode) {
                return template1;
            }

            if (mode === "1") {
                return template1;
            }
            if (mode === "2") {
                return template2;
            }
            if (mode === "3") {
                return template3;
            }
            if (mode === "4") {
                return template4;
            }

            return template1;
        }

        return {
            scope: {
                selectResult: '=',
                dicSettings: '=',
                dicViewMode: '@'
            },
            controller: function ($scope, $element, $attrs, $transclude) {
                var vm = this;

                var emptyItem = createEmptyItem();

                if ($scope.currentItem) {
                    vm.currentItem = $scope.currentItem;
                } else {
                    vm.currentItem = emptyItem;
                }
                vm.items = $scope.items;
                vm.isCurrentItem = function (item) {
                    return vm.currentItem === item;
                };
                vm.selectItem = function (item) {
                    vm.currentItem = item;
                };
            },
            controllerAs: 'vm',
            template: getTemplate
        };
    });
}());