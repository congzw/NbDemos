(function () {
    'use strict';
    var mainApp = zqnb.mainApp;
    mainApp.directive('nbDicCatalog', function ($injector) {
        //console.log('nbDicCatalog call!!!!!!');

        var nbDicCatalogMetaKey = "nbDicCatalogMeta";
        //use customize meta, if find one!
        var categories = function() {
            //todo refactor name : dicItemMetas
            var items = [];
            items.push({ key: "orgType", name: "类型", itemsKey: 'orgTypes', code: "orgTypeCode", disabled: false });
            items.push({ key: "org", name: "组织", itemsKey: 'orgs', code: "orgCode", disabled: false });
            items.push({ key: "phase", name: "学段", itemsKey: 'phases', code: "phaseCode", disabled: false });
            items.push({ key: "subject", name: "学科", itemsKey: 'subjects', code: "subjectCode", disabled: false });
            items.push({ key: "grade", name: "年级", itemsKey: 'grades', code: "gradeCode", disabled: false });
            return items;
        }();
        var hiddenPropertyName = 'Hidden';
        if ($injector.has(nbDicCatalogMetaKey)) {
            var nbDicCatalogMeta = $injector.get(nbDicCatalogMetaKey);
            //console.log(nbDicCatalogMeta);
            if (nbDicCatalogMeta) {
                //console.log('----------nbDicCatalogMeta');
                //console.log(nbDicCatalogMeta);
                if (nbDicCatalogMeta.categories) {
                    categories = nbDicCatalogMeta.categories;
                    //console.log('use customize meta: ');
                    //console.log(categories);
                }
                if (nbDicCatalogMeta.hidePropertyName) {
                    hiddenPropertyName = nbDicCatalogMeta.hidePropertyName;
                }
            }
        }


        var createCategoryTemplate1 = function (category) {
            var categoryCode = category.code;
            var name = category.name;
            var itemsKey = category.itemsKey;
            //<div class="term-box">
            //    <span class="term">类型(<span class="selectedDicCatalogItem">{{vm.selectResult.orgType.Name}}</span>)</span>
            //    <ul class="nav nav-pills overflow-h">
            //        <li ng-repeat="item in vm.orgTypes" ng-class="{active: isCurrentItem(item, 'orgType'), hidden: item.Hidden}">
            //            <a href="javascript:void(0)" ng-click="selectItem(item, 'orgType')"> {{item.Name}} </a>
            //        </li>
            //    </ul>
            //</div>
            return '' +
                ' <div class="term-box" ng-if="!isEmptyItems(\'' + categoryCode + '\')">  ' +
                //'      <span class="term">' + name + '(<span class="selectedDicCatalogItem">{{vm.selectResult.' + key + '.Name}}</span>)</span>  ' +
                '      <span class="term">' + name + '</span>  ' +
                '      <ul class="nav nav-pills overflow-h">  ' +
                '          <li ng-repeat="item in vm.' + itemsKey + '" ng-class="{active: isCurrentItem(item, \'' + categoryCode + '\'), hidden: item.' + hiddenPropertyName + '}">  ' +
                '              <a href="javascript:void(0)" ng-click="selectItem(item, \'' + categoryCode + '\')">  ' +
                '                  {{item.Name}}  ' +
                '              </a>  ' +
                '          </li>  ' +
                '      </ul>  ' +
                '  </div>';
        };
        var createCategoryTemplate2 = function (category) {
            var categoryCode = category.code;
            var name = category.name;
            var itemsKey = category.itemsKey;
            return '' +
                '      <li class="dropdown" ng-if="!isEmptyItems(\'' + categoryCode + '\')">  ' +
                '          <a href="javascript:void(0);" class="dropdown-toggle" data-toggle="dropdown">  ' +
                '              ' + name + '(<span class="selectedDicCatalogItem">{{vm.selectResult.' + categoryCode + '.Name}}</span>)  ' +
                '          </a>  ' +
                '          <ul class="dropdown-menu">  ' +
                '               <li ng-repeat="item in vm.' + itemsKey + '" ng-class="{active: isCurrentItem(item, \'' + categoryCode + '\'), hidden: item.' + hiddenPropertyName + '}">  ' +
                '                  <a href="javascript:void(0)" ng-click="selectItem(item, \'' + categoryCode + '\')">  ' +
                '                  {{item.Name}}  ' +
                '                  </a>  ' +
                '              </li>  ' +
                '          </ul>  ' +
                '      </li>  ';
        };
        var template1 = function () {
            var templateValue = '';
            for (var i = 0; i < categories.length; i++) {
                var category = categories[i];
                var value = createCategoryTemplate1(category);
                templateValue += value;
            }
            return templateValue;
        }();
        var template2 = function () {
            var templateValue = ' <ul class="search-dropdown col margin-top-bottom">  ';
            for (var i = 0; i < categories.length; i++) {
                var category = categories[i];
                var value = createCategoryTemplate2(category);
                templateValue += value;
            }
            return templateValue + ' </ul>  ';
        }();
        var template3 = function () {
            return '' +
                '<h2>todo template3</h2>';
        }();
        var template4 = function () {
            return '' +
                '<h2>todo template4</h2>';
        }();
        var getTemplate = function (tElem, tAttrs) {
            var mode = tAttrs.viewMode;
            console.log('nbDicCatalog getTemplate: ' + mode);
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
                vm: '=',
                viewMode: '@'
            },
            //link: function (scope, element, attributes) {
            //    console.log('linking!');
            //},
            controller: function ($scope, $element, $attrs, $transclude) {
                var vm = $scope.vm;
                console.log('nbDicCatalog Ctrl');
                //console.log(vm);

                //是否是空的集合（或只有【全部】按钮），或者被禁用
                $scope.isEmptyItems = function (categoryCode) {
                    return vm.isEmptyItems(categoryCode);
                }

                $scope.isCurrentItem = function (item, categoryCode) {
                    var currentItem = vm.selectResult[categoryCode];
                    if (currentItem) {
                        return currentItem === item;
                    }
                    return false;
                };

                $scope.selectItem = function (item, categoryCode) {
                    var oldItem = vm.selectResult[categoryCode];
                    if (oldItem) {
                        if (oldItem === item) {
                            //no change
                            return;
                        }
                        vm.selectResult[categoryCode] = item;
                        if (vm.onSelectResultChanged) {
                            //console.log('before onSelectResultChanged event => ' + categoryCode + 'old: ' + oldItem.Code + ' -> new: ' + item.Code);
                            vm.onSelectResultChanged(categoryCode, item, oldItem);
                        }
                    }
                };

            },
            template: getTemplate
        };
    });
}());