(function () {
    'use strict';
    var mainApp = zqnb.mainApp;

    //'&' and '=' function binding
    //https://stackoverflow.com/questions/25808193/angularjs-isolate-scope-vs

    mainApp.directive('nbSelectItems', function () {

        var template1 = function () {
            return '' +
                '<div class="term-box">  ' +
                '    <span class="term">{{category}}(<span class="selectedDicCatalogItem">{{current.Name}}</span>)</span>  ' +
                '    <ul class="nav nav-pills overflow-h">  ' +
                '        <li ng-repeat="item in items" ng-class="{active: isCurrentItem(item), hidden: item.Hidden}">  ' +
                '            <a href="javascript:void(0)" ng-click="selectItem(item)">  ' +
                '                {{item.Name}}  ' +
                '            </a>  ' +
                '        </li>  ' +
                '    </ul>  ' +
                '</div>  ';
        }();
        var template2 = '' +
                '<li class="dropdown">  ' +
                '    <a href="javascript:void(0);" class="dropdown-toggle" data-toggle="dropdown">  ' +
                '        {{category}}(<span class="selectedDicCatalogItem">{{current.Name}}</span>)  ' +
                '    </a>  ' +
                '    <ul class="dropdown-menu">  ' +
                '        <li ng-repeat="item in items" ng-class="{active: isCurrentPhase(item), hidden: item.Hidden}">  ' +
                '            <a href="javascript:void(0)" ng-click="vm.selectPhase(item)">{{item.Name}}</a>  ' +
                '        </li>  ' +
                '    </ul>  ' +
                '</li>  ';
        var template3 = '';
        var template4 = '';
        var getTemplate = function (tElem, tAttrs) {
            var mode = "1";
            if (tAttrs.viewMode) {
                mode = tAttrs.viewMode;
            }
            //console.log('nbSelectItems getTemplate');
            ////console.log(tAttrs);
            //console.log(mode);
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
                category: '@',
                currentChanged: '=',
                items: '=',
                current: '=',
                itemViewMode: '@'
            },
            template: getTemplate,
            controller: function ($scope, $element, $attrs, $transclude) {

                //console.log('nbSelectItems ctrl');
                //console.log($scope.viewMode);

                $scope.isCurrentItem = function (item) {
                    return $scope.current === item;
                };

                $scope.selectItem = function (item) {
                    if ($scope.current === item) {
                        return;
                    }
                    var old = $scope.current;
                    $scope.current = item;
                    //console.log('selected:');
                    //console.log(item);
                    if ($scope.currentChanged) {
                        $scope.currentChanged(item, old);
                    }
                };
            }
        };
    });
    mainApp.directive('nbDicSearch', function () {

        var getTemplate = function (tElem, tAttrs) {
            //console.log('nbDicSearch getTemplate');
            var mode = "1";
            if (tAttrs.viewMode) {
                mode = tAttrs.viewMode;
            }
            //console.log(mode);
            return '' +
'   <div ng-if="dicCatalogVm.orgs">  ' +
'       <div nb-select-items category="学校" current-changed="orgChanged" items="dicCatalogVm.orgs" current="dicCatalogVm.org" view-mode="'+ mode +'"></div>  ' +
'   </div>  ' +
'   <div ng-if="dicCatalogVm.phases">  ' +
'       <div nb-select-items category="学段" current-changed="phaseChanged" items="dicCatalogVm.phases" current="dicCatalogVm.phase" view-mode="' + mode + '"></div>  ' +
'   </div>  ' +
'   <div ng-if="dicCatalogVm.subjects">  ' +
'       <div nb-select-items category="学科" current-changed="subjectChanged" items="dicCatalogVm.subjects" current="dicCatalogVm.subject" view-mode="' + mode + '"></div>  ' +
'   </div>  ' +
'   <div ng-if="dicCatalogVm.grades">  ' +
'       <div nb-select-items category="年级" current-changed="gradeChanged" items="dicCatalogVm.grades" current="dicCatalogVm.grade" view-mode="' + mode + '"></div>  ' +
'  </div>  ';
        };

        return {
            require: '^nbSelectItems',
            scope: {
                resultChanged: '=',
                dicCatalogVm: '=',
                viewMode: '@'
            },
            template: getTemplate,
            controller: function ($scope, $element, $attrs, $transclude) {

                var dicCatalogVm = $scope.dicCatalogVm;
                //console.log('nbDicSearch ctrl');
                ////console.log(dicCatalogVm.viewMode);
                //console.log($scope.viewMode);

                var notifyChanged = function (event) {
                    if ($scope.resultChanged) {
                        $scope.resultChanged(event);
                    }
                };

                $scope.orgChanged = function (newItem, oldItem) {
                    //console.log('orgChanged: ' + oldItem.Code + ' -> ' + newItem.Code);
                    //fix angular auto watch slow problem!
                    dicCatalogVm.org = newItem;
                    //console.log(dicCatalogVm.org);
                    notifyChanged({ ChangeBy: "Org", NewItem: newItem, OldItem: oldItem });
                };

                $scope.phaseChanged = function (newItem, oldItem) {
                    //console.log('phaseChanged: ' + oldItem.Code + ' -> ' + newItem.Code);
                    dicCatalogVm.phase = newItem;
                    notifyChanged({ ChangeBy: "Phase", NewItem: newItem, OldItem: oldItem });
                };

                $scope.subjectChanged = function (newItem, oldItem) {
                    //console.log('subjectChanged: ' + oldItem.Code + ' -> ' + newItem.Code);
                    dicCatalogVm.subject = newItem;
                    notifyChanged({ ChangeBy: "Subject", NewItem: newItem, OldItem: oldItem });
                };

                $scope.gradeChanged = function (newItem, oldItem) {
                    //console.log('gradeChanged: ' + oldItem.Code + ' -> ' + newItem.Code);
                    dicCatalogVm.grade = newItem;
                    notifyChanged({ ChangeBy: "Grade", NewItem: newItem, OldItem: oldItem });
                };
            }
        };
    });
}());