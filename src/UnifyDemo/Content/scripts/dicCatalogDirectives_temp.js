(function () {
    'use strict';
    var mainApp = zqnb.mainApp;
    mainApp.directive('optionsClass', function ($parse) {

        //console.log('optionsClass');

        return {
            require: 'select',
            link: function (scope, elem, attrs, ngSelect) {
                // get the source for the items array that populates the select.
                var optionsSourceStr = attrs.ngOptions.split(' ').pop(),
                // use $parse to get a function from the options-class attribute
                // that you can use to evaluate later.
                    getOptionsClassFunc = $parse(attrs.optionsClass);

                ////vm.subjects
                //console.log(optionsSourceStr);
                //{ hidden: item.Hidden }
                console.log(attrs.optionsClass);

                scope.$watch(optionsSourceStr, function (items) {
                    // when the options source changes loop through its items.
                    angular.forEach(items, function (item, index) {
                        // evaluate against the item to get a mapping object for
                        // for your classes.
                        var classes = getOptionsClassFunc(item),
                        // also get the option you're going to need. This can be found
                        // by looking for the option with the appropriate index in the
                        // value attribute.
                            option = elem.find('option[value=' + index + ']');

                        ////{hidden: false}
                        console.log(item);
                        console.log(classes);

                        // now loop through the key/value pairs in the mapping object
                        // and apply the classes that evaluated to be truthy.
                        angular.forEach(classes, function (value, key) {
                            console.log(key + ':' + value);
                            if (value) {
                                angular.element(option).addClass(key);
                            }
                        });
                    });
                });
            }
        };
    });

    mainApp.directive('nbDicCatalog', function () {

        //级联关系的逻辑
        //学段（空）：展示所有学科、所有年级
        //学段（非空）：筛选学段的学科、学段的年级
        //学段+ 年级（同时非空）：筛选学段的学科、学段的年级、学段的年级的学科
        //hacks:
        //https://stackoverflow.com/questions/21177582/directive-is-being-rendered-before-promise-is-resolved

        var equalIgnoreCase = function (str, str2) {
            if (str === null || str2 === null) {
                return true;
            }
            if (typeof str !== 'string' || typeof str2 !== 'string') {
                return false;
            }
            if (str === '' && str2 === '') {
                return true;
            }
            return (str2.toUpperCase() === str.toUpperCase());
        };
        var sameCodeItem = function (item1, item2) {
            return item1 === item2 || equalIgnoreCase(item1.Code, item2.Code);
        };
        var containItem = function (items, itemToCheck) {
            if (!items || !itemToCheck) {
                return false;
            }
            for (var i = 0; i < items.length; i++) {
                if (sameCodeItem(items[i], itemToCheck)) {
                    return true;
                }
            }
            return false;
        };
        var createEmptyItem = function () {
            return { Code: "", Name: "全部", Selected: true, Hidden: false };
        };
        var emptyPhase = createEmptyItem();
        var emptySubject = createEmptyItem();
        var emptyGrade = createEmptyItem();
        var createPhaseCodeItem = function (phase) {
            var phaseCodeItem = { Code: phase.Code };
            return phaseCodeItem;
        };
        var createPhaseSubjectCodeItem = function (phase, subject) {
            var phaseSubjectCodeItem = { Code: phase.Code + ',' + subject.Code };
            return phaseSubjectCodeItem;
        };
        var createPhaseGradeCodeItem = function (phase, grade) {
            var phaseGradeCodeItem = { Code: phase.Code + ',' + grade.Code };
            return phaseGradeCodeItem;
        };
        var createPhaseSubjectGradeCodeItem = function (phase, subject, grade) {
            var phaseSubjectGradeCodeItem = { Code: phase.Code + ',' + subject.Code + ',' + grade.Code };
            return phaseSubjectGradeCodeItem;
        };
        var setupDicCatalogVm = function (theVm, dicSettings) {

            //dicSettings[0].InUse = true;
            //console.log(theVm);

            //try convert to all dicItems from dicSettingVm

            //all dic items
            var phases = [];
            var subjects = [];
            var grades = [];
            phases.push(emptyPhase);
            subjects.push(emptySubject);
            grades.push(emptyGrade);
            angular.forEach(dicSettings, function (phase, key) {
                if (!phase.InUse) {
                    return;
                }
                phases.push({ Code: phase.Code, Name: phase.Name, Hidden: false });
                //subjects
                angular.forEach(phase.Subjects, function (subject, key) {
                    if (!subject.InUse) {
                        return;
                    }
                    if (!containItem(subjects, subject)) {
                        subjects.push({ Code: subject.Code, Name: subject.Name, Hidden: false });
                    }
                });
                //grades
                angular.forEach(phase.Grades, function (grade, key) {
                    if (!grade.InUse) {
                        return;
                    }
                    if (!containItem(grades, grade)) {
                        grades.push({ Code: grade.Code, Name: grade.Name, Hidden: false });
                    }
                });
            });
            theVm.phases = phases;
            theVm.subjects = subjects;
            theVm.grades = grades;
            //console.log(phases);
            //console.log(subjects);
            //console.log(grades);

            //dic setting
            var visiablePhases = [];
            var visiablePhaseSubjects = [];
            var visiablePhaseGrades = [];
            var visiablePhaseSubjectGrades = [];
            angular.forEach(dicSettings, function (phase, key) {
                if (!phase.InUse) {
                    return;
                }
                var phaseCodeItem = createPhaseCodeItem(phase);
                if (!containItem(visiablePhases, phaseCodeItem)) {
                    visiablePhases.push(phaseCodeItem);
                }

                angular.forEach(phase.Grades, function (grade, key) {
                    if (!grade.InUse) {
                        return;
                    }
                    var phaseGradeCodeItem = createPhaseGradeCodeItem(phase, grade);
                    if (!containItem(visiablePhaseGrades, phaseGradeCodeItem)) {
                        visiablePhaseGrades.push(phaseGradeCodeItem);
                    }
                });

                angular.forEach(phase.Subjects, function (subject, key) {
                    if (!subject.InUse) {
                        return;
                    }
                    var subjectCodeItem = createPhaseSubjectCodeItem(phase, subject);
                    if (!containItem(visiablePhaseSubjects, subjectCodeItem)) {
                        visiablePhaseSubjects.push(subjectCodeItem);
                    }

                    angular.forEach(subject.Grades, function (grade, key) {
                        if (!grade.InUse) {
                            return;
                        }
                        var phaseSubjectGradeCodeItem = createPhaseSubjectGradeCodeItem(phase, subject, grade);
                        if (!containItem(visiablePhaseSubjectGrades, phaseSubjectGradeCodeItem)) {
                            visiablePhaseSubjectGrades.push(phaseSubjectGradeCodeItem);
                        }
                    });
                });
            });
            theVm.visiablePhases = visiablePhases;
            theVm.visiablePhaseSubjects = visiablePhaseSubjects;
            theVm.visiablePhaseGrades = visiablePhaseGrades;
            theVm.visiablePhaseSubjectGrades = visiablePhaseSubjectGrades;
            //console.log(dicSettings);
            //console.log(dicCatalogSearch);

            return theVm;
        };
        var tryFixNameWithCode = function (items, itemToFix) {
            angular.forEach(items, function (item, key) {
                if (sameCodeItem(item, itemToFix)) {
                    itemToFix.Name = item.Name;
                    return;
                }
            });
        }
        var tryFindItemWithCode = function (items, code) {
            var theItem = null;
            angular.forEach(items, function (item, key) {
                if (equalIgnoreCase(item.Code, code)) {
                    theItem = item;
                    return;
                }
            });
            return theItem;
        }
        var prepareSelectResult = function (theVm, selectResult) {
            if (!selectResult) {
                selectResult = {};
            }
            if (!selectResult.Phase) {
                selectResult.Phase = { Code: emptyPhase.Code, Name: emptyPhase.Name };
            }
            if (!selectResult.Subject) {
                selectResult.Subject = { Code: emptySubject.Code, Name: emptySubject.Name };
            }
            if (!selectResult.Grade) {
                selectResult.Grade = { Code: emptyGrade.Code, Name: emptyGrade.Name };
            }

            //fix init code without name!
            if (!selectResult.Phase.Name) {
                tryFixNameWithCode(theVm.phases, selectResult.Phase);
            }

            if (!selectResult.Subject.Name) {
                tryFixNameWithCode(theVm.subjects, selectResult.Subject);
            }

            if (!selectResult.Grade.Name) {
                tryFixNameWithCode(theVm.grades, selectResult.Grade);
            }
        };

        var template1 = function () {
            return '<div class="term-box">  ' +
                '                               <span class="term">学段(<span class="selectedDicCatalogItem">{{vm.selectResult.Phase.Name}}</span>)</span>  ' +
                '                               <ul class="nav nav-pills overflow-h">  ' +
                '                                   <li ng-repeat="item in vm.phases" ng-class="{active: vm.isCurrentPhase(item), hidden: item.Hidden}">  ' +
                '                                       <a href="javascript:void(0)" ng-click="vm.selectPhase(item)">  ' +
                '                                           {{item.Name}}  ' +
                '                                       </a>  ' +
                '                                   </li>  ' +
                '                               </ul>  ' +
                '                           </div>  ' +
                '                           <div class="term-box">  ' +
                '                               <span class="term">学科(<span class="selectedDicCatalogItem">{{vm.selectResult.Subject.Name}}</span>)</span>  ' +
                '                               <ul class="nav nav-pills overflow-h">  ' +
                '                                   <li ng-repeat="item in vm.subjects" ng-class="{active: vm.isCurrentSubject(item), hidden: item.Hidden}"><a href="javascript:void(0)" ng-click="vm.selectSubject(item)">{{item.Name}}</a></li>  ' +
                '                               </ul>  ' +
                '                           </div>  ' +
                '                           <div class="term-box">  ' +
                '                               <span class="term">年级(<span class="selectedDicCatalogItem">{{vm.selectResult.Grade.Name}}</span>)</span>  ' +
                '                               <ul class="nav nav-pills overflow-h">  ' +
                '                                   <li ng-repeat="item in vm.grades" ng-class="{active: vm.isCurrentGrade(item), hidden: item.Hidden}"><a href="javascript:void(0)" ng-click="vm.selectGrade(item)">{{item.Name}}</a></a></li>  ' +
                '                               </ul>  ' +
                '                          </div>';
        }();

        var template2 = function () {
            return '<ul class="search-dropdown col margin-top-bottom">  ' +
 '                                       <li class="dropdown">  ' +
 '                                           <a href="javascript:void(0);" class="dropdown-toggle" data-toggle="dropdown">  ' +
 '                                               学段(<span class="selectedDicCatalogItem">{{vm.selectResult.Phase.Name}}</span>)  ' +
 '                                           </a>  ' +
 '                                           <ul class="dropdown-menu">  ' +
 '                                               <li ng-repeat="item in vm.phases" ng-class="{active: vm.isCurrentPhase(item), hidden: item.Hidden}">  ' +
 '                                                   <a href="javascript:void(0)" ng-click="vm.selectPhase(item)">{{item.Name}}</a>  ' +
 '                                               </li>  ' +
 '                                           </ul>  ' +
 '                                       </li>  ' +
 '                                       <li class="dropdown">  ' +
 '                                           <a href="javascript:void(0);" class="dropdown-toggle" data-toggle="dropdown">  ' +
 '                                               学科(<span class="selectedDicCatalogItem">{{vm.selectResult.Subject.Name}}</span>)  ' +
 '                                           </a>  ' +
 '                                           <ul class="dropdown-menu style-width">  ' +
 '                                               <li ng-repeat="item in vm.subjects" ng-class="{active: vm.isCurrentSubject(item), hidden: item.Hidden}">  ' +
 '                                                   <a href="javascript:void(0)" ng-click="vm.selectSubject(item)">{{item.Name}}</a>  ' +
 '                                               </li>  ' +
 '                                           </ul>  ' +
 '                                       </li>  ' +
 '                                       <li class="dropdown">  ' +
 '                                           <a href="javascript:void(0);" class="dropdown-toggle" data-toggle="dropdown">  ' +
 '                                               年级(<span class="selectedDicCatalogItem">{{vm.selectResult.Grade.Name}}</span>)  ' +
 '                                           </a>  ' +
 '                                           <ul class="dropdown-menu style-width">  ' +
 '                                               <li ng-repeat="item in vm.grades" ng-class="{active: vm.isCurrentGrade(item), hidden: item.Hidden}">  ' +
 '                                                   <a href="javascript:void(0)" ng-click="vm.selectGrade(item)">{{item.Name}}</a>  ' +
 '                                               </li>  ' +
 '                                           </ul>  ' +
 '                                       </li>  ' +
 '                                  </ul>  ';
        }();

        var template3 = function () {
            return '   <section class="col col-2" title="学段">  ' +
 '       <label class="select">  ' +
 '           <select ng-change="vm.hackSelectOption(vm, vm.selectResult.Phase.Code, 1)" ng-model="vm.selectResult.Phase.Code" ng-options="item.Code as item.Name for item in vm.phases" options-class="{ hidden : Hidden }"></select>  ' +
 '           <i></i>  ' +
 '       </label>  ' +
 '   </section>  ' +
 '   <section class="col col-2" title="学科">  ' +
 '       <label class="select">  ' +
 '           <select ng-change="vm.hackSelectOption(vm, vm.selectResult.Subject.Code, 2)" ng-model="vm.selectResult.Subject.Code" ng-options="item.Code as item.Name for item in vm.subjects" options-class="{ hidden : Hidden }"></select>  ' +
 '           <i></i>  ' +
 '       </label>  ' +
 '   </section>  ' +
 '   <section class="col col-2" title="年级">  ' +
 '       <label class="select">  ' +
 '           <select ng-change="vm.hackSelectOption(vm, vm.selectResult.Grade.Code, 3)" ng-model="vm.selectResult.Grade.Code" ng-options="item.Code as item.Name for item in vm.grades" options-class="{ hidden : Hidden }"></select>  ' +
 '           <i></i>  ' +
 '       </label>  ' +
 '  </section>  ';
        }();

 //       var template3 = function() {
 //           return '   <section class="col col-2" title="学段">  ' +
 //'       <label class="select">  ' +
 //'           <select ng-change="hackSelectOption(vm, hackSelectOptionCodes.Phase, 1)" ng-model="hackSelectOptionCodes.Phase"><option ng-repeat="item in vm.phases" ng-class="{ hidden: item.Hidden}" value="{{item.Code}}">{{item.Name}}</option></select>  ' +
 //'           <i></i>  ' +
 //'       </label>  ' +
 //'   </section>  ' +
 //'   <section class="col col-2" title="学科">  ' +
 //'       <label class="select">  ' +
 //'           <select ng-change="hackSelectOption(vm, hackSelectOptionCodes.Subject, 2)" ng-model="hackSelectOptionCodes.Subject"><option ng-repeat="item in vm.subjects" ng-class="{ hidden: item.Hidden}" value="{{item.Code}}">{{item.Name}}</option></select>  ' +
 //'           <i></i>  ' +
 //'       </label>  ' +
 //'   </section>  ' +
 //'   <section class="col col-2" title="年级">  ' +
 //'       <label class="select">  ' +
 //'           <select ng-change="hackSelectOption(vm, hackSelectOptionCodes.Grade, 3)" ng-model="hackSelectOptionCodes.Grade"><option ng-repeat="item in vm.grades" ng-class="{ hidden: item.Hidden}" value="{{item.Code}}">{{item.Name}}</option></select>  ' +
 //'           <i></i>  ' +
 //'       </label>  ' +
 //'  </section>  ';
 //       }();

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
                
                vm.hackSelectOption = function (theVm, hackItemCode, dicTypeValue) {
                    var theItem = null;
                    //todo, why ex? angular.js:9413 TypeError: Cannot read property 'prop' of undefined
                    if (1 === dicTypeValue) {
                        var thePhase = tryFindItemWithCode(theVm.phases, hackItemCode);
                        if (thePhase !== null) {
                            theVm.selectPhase(thePhase);
                            theItem = thePhase;
                        }
                    } else if (2 === dicTypeValue) {
                        var theSubject = tryFindItemWithCode(theVm.subjects, hackItemCode);
                        if (theSubject !== null) {
                            theVm.selectSubject(theSubject);
                            theItem = theSubject;
                        }
                    } else if (3 === dicTypeValue) {
                        var theGrade = tryFindItemWithCode(theVm.grades, hackItemCode);
                        if (theGrade !== null) {
                            theVm.selectGrade(theGrade);
                            theItem = theGrade;
                        }
                    }

                    if (theItem) {
                        console.log('hack for view mode 3 ng-options: ' + hackItemCode + ' -> '+ theItem.Name);
                    }
                };

                ////hack for view mode 3 ng-options
                //$scope.hackSelectOptionCodes = { Phase: "", Subject: "", Grade: "" };
                //$scope.$watch('hackSelectOptionCodes', function (newValue, oldValue, scope) {
                //    console.log(newValue.Phase + ',' + newValue.Subject + ',' + newValue.Grade);
                //    console.log(oldValue.Phase + ',' + oldValue.Subject + ',' + oldValue.Grade);
                //});

                //var updatingUI = false;
                //var theItem = null;
              
                
                var dicSettings = $scope.dicSettings;
                setupDicCatalogVm(vm, dicSettings);
                var selectResult = $scope.selectResult;
                prepareSelectResult(vm, selectResult);
                vm.selectResult = selectResult;

                var shouldShowThisPhase = function (visiablePhases, phase) {
                    //全部永远显示
                    if(sameCodeItem(phase, emptyPhase)) {
                        return true;
                    }
                    var phaseCodeItem = { Code: phase.Code };
                    var shouldShow = containItem(visiablePhases, phaseCodeItem);
                    return shouldShow;
                };
                var shouldShowCurrentPhaseSubject = function (theVm, subject) {

                    //全部永远显示
                    if(sameCodeItem(subject, emptySubject)) {
                        return true;
                    }
                    var phaseSubjectCodeItem = { Code: theVm.selectResult.Phase.Code + ',' + subject.Code };
                    var shouldShow = containItem(theVm.visiablePhaseSubjects, phaseSubjectCodeItem);
                    if (shouldShow) {
                        //console.log("refresh phase subjects: " + theVm.selectResult.Phase.Name + ',' + subject.Name + ' ' + shouldShow);
                    }
                    return shouldShow;
                };
                var shouldShowCurrentPhaseGrade = function (theVm, grade) {

                    //全部永远显示
                    if(sameCodeItem(grade, emptyGrade)) {
                        return true;
                    }
                    var phaseGradeCodeItem = { Code: theVm.selectResult.Phase.Code + ',' + grade.Code };
                    var shouldShow = containItem(theVm.visiablePhaseGrades, phaseGradeCodeItem);
                    if (shouldShow) {
                        //console.log("refresh phase grades: " + theVm.selectResult.Phase.Name + ',' + grade.Name + ' ' + shouldShow);
                    }
                    return shouldShow;
                };
                var shouldShowCurrentPhaseSubjectGrade = function (theVm, grade) {

                    //全部永远显示
                    if(sameCodeItem(grade, emptyGrade)) {
                        return true;
                    }
                    var phaseSubjectGradeCodeItem = { Code: theVm.selectResult.Phase.Code + ',' + theVm.selectResult.Subject.Code + ',' + grade.Code };
                    var shouldShow = containItem(theVm.visiablePhaseSubjectGrades, phaseSubjectGradeCodeItem);
                    if (shouldShow) {
                        //console.log("refresh phase subject grades: " + theVm.selectResult.Phase.Name + ','+ theVm.selectResult.Subject.Name + ',' + grade.Name + ' ' + shouldShow);
                    }
                    return shouldShow;
                };
                var changeSelect = function (items, theItem) {
                    angular.forEach(items, function (item) {
                        item.Selected = false;
                        if (sameCodeItem(item, theItem)) {
                            item.Selected = true;
                            console.log('change to selected: ' + item.Name);
                        }
                    });
                };
                var copyCodeAndName = function (copyTo, copyFrom) {
                    copyTo.Code = copyFrom.Code;
                    copyTo.Name = copyFrom.Name;
                };
                var hideAll = function (items) {
                    angular.forEach(items, function (item) {
                        if(item.Code === "") {
                            //console.log("Empty Hide: " + item.Name);
                            item.Hidden = false;
                            return;
                        }
                        item.Hidden = true;
                    });
                };
                var changeDisplay = function (theVm) {
                    hideAll(theVm.phases);
                    hideAll(theVm.subjects);
                    hideAll(theVm.grades);

                    //refresh phases
                    //console.log(vm.visiablePhases);
                    angular.forEach(theVm.phases, function (phase) {
                        var shouldShow = shouldShowThisPhase(theVm.visiablePhases, phase);
                        if (shouldShow) {
                            phase.Hidden = false;
                        }
                    });

                    //refresh phase subjects
                    //console.log(vm.visiablePhaseSubjects);
                    var needChangeToEmptySubject = true;
                    angular.forEach(theVm.subjects, function (subject) {
                        if (theVm.isEmptyPhase()) {
                            //全部（学科）
                            subject.Hidden = false;
                            needChangeToEmptySubject = false;
                            return;
                        }

                        var shouldShow = shouldShowCurrentPhaseSubject(theVm, subject);
                        if (shouldShow) {
                            //console.log("refresh phase subjects: " + theVm.selectResult.Phase.Name + ',' + subject.Name + ' ' + shouldShow);
                            subject.Hidden = false;
                            if (sameCodeItem(subject, theVm.selectResult.Subject)) {
                                //console.log("当前的选中学科在其中: " + subject.Code + ',' + theVm.selectResult.Subject.Code);
                                needChangeToEmptySubject = false;
                            }
                        }
                    });
                    //如果当前的选中学科不在其中，则使用全部
                    if (needChangeToEmptySubject) {
                        copyCodeAndName(theVm.selectResult.Subject, emptySubject);
                    }

                    //refresh phase grades
                    //console.log(vm.visiablePhaseGrades);
                    var needChangeToEmptyGrade = true;
                    angular.forEach(theVm.grades, function (grade) {
                        if (theVm.isEmptyPhase()) {
                            //全部（年级）
                            grade.Hidden = false;
                            needChangeToEmptyGrade = false;
                            return;
                        }
                        var shouldShow = shouldShowCurrentPhaseGrade(theVm, grade);
                        if (shouldShow) {
                            //console.log("refresh phase grades: " + vm.selectResult.Phase.Name + ',' + grade.Name + ' ' + shouldShow);
                            grade.Hidden = false;
                            if (sameCodeItem(grade, theVm.selectResult.Grade)) {
                                //console.log("当前的选中的年级在其中: " + grade.Code + ',' + theVm.selectResult.Grade.Code);
                                needChangeToEmptyGrade = false;
                            }
                        }
                    });

                    //refresh phase subject grades
                    angular.forEach(theVm.grades, function (grade) {
                        //如果学段和学科同时不为空，则需要二次筛选
                        if (!theVm.isEmptyPhase() && !theVm.isEmptySubject()) {
                            var shouldShow = shouldShowCurrentPhaseSubjectGrade(theVm, grade);
                            if (!shouldShow) {
                                grade.Hidden = true;
                            } else {
                                if (sameCodeItem(grade, theVm.selectResult.Grade)) {
                                    //console.log("当前的选中的年级在隐藏的按钮中: " + grade.Code + ',' + theVm.selectResult.Grade.Code);
                                    needChangeToEmptyGrade = false;
                                }
                            }
                        }
                    });

                    //如果当前的选中年级不在其中，则使用全部
                    if (needChangeToEmptyGrade) {
                        copyCodeAndName(theVm.selectResult.Grade, emptyGrade);
                    }
                };

                vm.isEmptyPhase = function () {
                    return vm.selectResult.Phase.Code === "";
                };
                vm.isEmptySubject = function () {
                    return vm.selectResult.Subject.Code === "";
                };
                vm.isEmptyGrade = function () {
                    return vm.selectResult.Grade.Code === "";
                };

                vm.selectPhase = function (item) {
                    changeSelect(vm.phases, item);
                    copyCodeAndName(vm.selectResult.Phase, item);
                    changeDisplay(vm);
                    console.log('selectPhase: ' + item.Name);
                }
                vm.selectSubject = function (item) {
                    changeSelect(vm.subjects, item);
                    copyCodeAndName(vm.selectResult.Subject, item);
                    changeDisplay(vm);
                }
                vm.selectGrade = function (item) {
                    changeSelect(vm.grades, item);
                    copyCodeAndName(vm.selectResult.Grade, item);
                    changeDisplay(vm);
                }
                vm.isCurrentPhase = function (phase) {
                    return sameCodeItem(phase, vm.selectResult.Phase);
                };
                vm.isCurrentSubject = function (subject) {
                    return sameCodeItem(subject, vm.selectResult.Subject);
                };
                vm.isCurrentGrade = function (grade) {
                    return sameCodeItem(grade, vm.selectResult.Grade);
                };
                //console.log(dicSettings);
                
                changeDisplay(vm);
            },
            controllerAs: 'vm',
            template: getTemplate
        };
    });
}());