﻿(function () {
    'use strict';
    var mainApp = zqnb.mainApp;
    
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
                category: '@',
                currentChanged: '=',
                items: '=',
                current: '=',
                dicViewMode: '@'
            },
            controller: function ($scope, $element, $attrs, $transclude) {

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
            },
            template: getTemplate
        };
    });
    mainApp.directive('nbDicSearch', function () {

        var getTemplate = function () {
            return '' +
'   <div ng-if="dicCatalogVm.orgs">  ' +
'       <div nb-select-items category="学校" current-changed="orgChanged" items="dicCatalogVm.orgs" current="dicCatalogVm.org" view-mode="dicCatalogVm.viewMode"></div>  ' +
'   </div>  ' +
'   <div ng-if="dicCatalogVm.phases">  ' +
'       <div nb-select-items category="学段" current-changed="phaseChanged" items="dicCatalogVm.phases" current="dicCatalogVm.phase" view-mode="dicCatalogVm.viewMode"></div>  ' +
'   </div>  ' +
'   <div ng-if="dicCatalogVm.subjects">  ' +
'       <div nb-select-items category="学科" current-changed="subjectChanged" items="dicCatalogVm.subjects" current="dicCatalogVm.subject" view-mode="dicCatalogVm.viewMode"></div>  ' +
'   </div>  ' +
'   <div ng-if="dicCatalogVm.grades">  ' +
'       <div nb-select-items category="年级" current-changed="gradeChanged" items="dicCatalogVm.grades" current="dicCatalogVm.grade" view-mode="dicCatalogVm.viewMode"></div>  ' +
'  </div>  ';
        };

        return {
            require: '^nbSelectItems',
            scope: {
                resultChanged: '=',
                dicCatalogVm: '='
            },
            controller: function ($scope, $element, $attrs, $transclude) {

                var dicCatalogVm = $scope.dicCatalogVm;

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
                    notifyChanged({ ChangeBy: "Org", NewItem: newItem, OldItem :oldItem });
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
            },
            template: getTemplate()
        };
    });
}());