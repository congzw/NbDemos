(function () {
    'use strict';
    var mainApp = zqnb.mainApp;

    mainApp.factory('dicCatalogAppService', [
        '$http', function ($http) {
            var serviceUrl = '/Content/scripts/GetDicCatalogs.js';
            return {
                getDicCatalogs: function () {
                    return $http.get(serviceUrl);
                }
            }
        }
    ]);
    
    //级联关系的逻辑
    //学段（空）：展示所有学科、所有年级
    //学段（非空）：筛选学段的学科、学段的年级
    //学段+ 年级（同时非空）：筛选学段的学科、学段的年级、学段的年级的学科
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
    var createDicCatalogSearch = function (dicCatalogSearch, data) {

        dicCatalogSearch.dicCatalogs = data;
        //data[0].InUse = true;
        //console.log(dicSettings);

        //todo
        var phases = [];
        phases.push(emptyPhase);
        angular.forEach(data, function(value, key) {
            phases.push({ Code: value.Code, Name: value.Name, Hidden: !value.InUse });
        });
        //console.log(phases);
        dicCatalogSearch.phases = phases;

        //todo
        var subjects = [];
        subjects.push(emptySubject);
        angular.forEach(data, function(value, key) {

            angular.forEach(value.Subjects, function(value, key) {

                if (!containItem(subjects, value)) {
                    subjects.push({ Code: value.Code, Name: value.Name, Hidden: !value.InUse });
                }
            });
        });
        //console.log(subjects);
        dicCatalogSearch.subjects = subjects;

        //todo
        var grades = [];
        grades.push(emptyGrade);
        angular.forEach(data, function(value, key) {
            angular.forEach(value.Grades, function(value, key) {
                if (!containItem(grades, value)) {
                    grades.push({ Code: value.Code, Name: value.Name, Hidden: !value.InUse });
                }
            });
        });
        //console.log(grades);
        dicCatalogSearch.grades = grades;

        //todo
        var visiablePhases = [];
        var visiablePhaseSubjects = [];
        var visiablePhaseGrades = [];
        var visiablePhaseSubjectGrades = [];

        angular.forEach(data, function(phase, key) {
            if (!phase.InUse) {
                return;
            }
            var phaseCodeItem = { Code: phase.Code };
            if (!containItem(visiablePhases, phaseCodeItem)) {
                visiablePhases.push(phaseCodeItem);
            }

            angular.forEach(phase.Grades, function(grade, key) {

                if (!grade.InUse) {
                    return;
                }
                var phaseGradeCodeItem = { Code: phase.Code + ',' + grade.Code };
                if (!containItem(visiablePhaseGrades, phaseGradeCodeItem)) {
                    visiablePhaseGrades.push(phaseGradeCodeItem);
                }
            });

            angular.forEach(phase.Subjects, function(subject, key) {

                if (!subject.InUse) {
                    return;
                }
                var subjectCodeItem = { Code: phase.Code + ',' + subject.Code };
                if (!containItem(visiablePhaseSubjects, subjectCodeItem)) {
                    visiablePhaseSubjects.push(subjectCodeItem);
                }

                angular.forEach(subject.Grades, function(grade, key) {
                    if (!grade.InUse) {
                        //console.log(settingCode + ': Hidden!');
                        return;
                    }
                    var phaseSubjectGradeCodeItem = { Code: phase.Code + ',' + subject.Code + ',' + grade.Code };
                    if (!containItem(visiablePhaseSubjectGrades, phaseSubjectGradeCodeItem)) {
                        visiablePhaseSubjectGrades.push(phaseSubjectGradeCodeItem);
                    }
                });
            });
        });
        //console.log(dicSettings);
        dicCatalogSearch.visiablePhases = visiablePhases;
        dicCatalogSearch.visiablePhaseSubjects = visiablePhaseSubjects;
        dicCatalogSearch.visiablePhaseGrades = visiablePhaseGrades;
        dicCatalogSearch.visiablePhaseSubjectGrades = visiablePhaseSubjectGrades;

        return dicCatalogSearch;
    };
    
    mainApp.controller('demoCtrl', function ($scope, dicCatalogAppService) {

        var shouldShowThisPhase = function (visiablePhases, phase) {
            //全部永远显示
            if (sameCodeItem(phase, emptyPhase)) {
                return true;
            }
            var phaseCodeItem = { Code: phase.Code };
            var shouldShow = containItem(visiablePhases, phaseCodeItem);
            return shouldShow;
        };
        var shouldShowCurrentPhaseSubject = function (theVm, subject) {

            //全部永远显示
            if (sameCodeItem(subject, emptySubject)) {
                return true;
            }
            var phaseSubjectCodeItem = { Code: theVm.currentPhase.Code + ',' + subject.Code };
            var shouldShow = containItem(theVm.visiablePhaseSubjects, phaseSubjectCodeItem);
            if (shouldShow) {
                //console.log("refresh phase subjects: " + theVm.currentPhase.Name + ',' + subject.Name + ' ' + shouldShow);
            }
            return shouldShow;
        };
        var shouldShowCurrentPhaseGrade = function (theVm, grade) {

            //全部永远显示
            if (sameCodeItem(grade, emptyGrade)) {
                return true;
            }
            var phaseGradeCodeItem = { Code: theVm.currentPhase.Code + ',' + grade.Code };
            var shouldShow = containItem(theVm.visiablePhaseGrades, phaseGradeCodeItem);
            if (shouldShow) {
                //console.log("refresh phase grades: " + theVm.currentPhase.Name + ',' + grade.Name + ' ' + shouldShow);
            }
            return shouldShow;
        };
        var shouldShowCurrentPhaseSubjectGrade = function (theVm, grade) {

            //全部永远显示
            if (sameCodeItem(grade, emptyGrade)) {
                return true;
            }
            var phaseSubjectGradeCodeItem = { Code: theVm.currentPhase.Code + ',' + theVm.currentSubject.Code + ',' + grade.Code };
            var shouldShow = containItem(theVm.visiablePhaseSubjectGrades, phaseSubjectGradeCodeItem);
            if (shouldShow) {
                //console.log("refresh phase subject grades: " + theVm.currentPhase.Name + ','+ theVm.currentSubject.Name + ',' + grade.Name + ' ' + shouldShow);
            }
            return shouldShow;
        };

        var changeSelect = function (items, item) {
            angular.forEach(items, function (item) {
                item.Selected = false;
            });
            item.Selected = true;
        }
        var hideAll = function (items) {
            angular.forEach(items, function (item) {
                if (item.Code === "") {
                    //console.log("Empty Hide: " + item.Name);
                    item.Hidden = false;
                    return;
                }
                item.Hidden = true;
            });
        }
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
                    //console.log("refresh phase subjects: " + theVm.currentPhase.Name + ',' + subject.Name + ' ' + shouldShow);
                    subject.Hidden = false;
                    if (sameCodeItem(subject, theVm.currentSubject)) {
                        //console.log("当前的选中学科在其中: " + subject.Code + ',' + theVm.currentSubject.Code);
                        needChangeToEmptySubject = false;
                    }
                }
            });
            //如果当前的选中学科不在其中，则使用全部
            if (needChangeToEmptySubject) {
                theVm.currentSubject = emptySubject;
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
                    //console.log("refresh phase grades: " + vm.currentPhase.Name + ',' + grade.Name + ' ' + shouldShow);
                    grade.Hidden = false;
                    if (sameCodeItem(grade, theVm.currentGrade)) {
                        //console.log("当前的选中的年级在其中: " + grade.Code + ',' + theVm.currentGrade.Code);
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
                    }
                    else {
                        if (sameCodeItem(grade, theVm.currentGrade)) {
                            console.log("当前的选中的年级在隐藏的按钮中: " + grade.Code + ',' + theVm.currentGrade.Code);
                            needChangeToEmptyGrade = false;
                        }
                    }
                }
            });

            //如果当前的选中年级不在其中，则使用全部
            if (needChangeToEmptyGrade) {
                theVm.currentGrade = emptyGrade;
            }
        }

        var vm = this;

        dicCatalogAppService.getDicCatalogs().success(function (data) {
            createDicCatalogSearch(vm, data);
        });

        vm.currentPhase = emptyPhase;
        vm.currentSubject = emptySubject;
        vm.currentGrade = emptyGrade;

        vm.isEmptyPhase = function() {
            return vm.currentPhase.Code === "";
        };
        vm.isEmptySubject = function() {
            return vm.currentSubject.Code === "";
        };
        vm.isEmptyGrade = function() {
            return vm.currentGrade.Code === "";
        };
        
        vm.selectPhase = function (item) {
            changeSelect(vm.phases, item);
            vm.currentPhase = item;
            changeDisplay(vm);
        }
        vm.selectSubject = function (item) {
            changeSelect(vm.subjects, item);
            vm.currentSubject = item;
            changeDisplay(vm);
        }
        vm.selectGrade = function (item) {
            changeSelect(vm.grades, item);
            vm.currentGrade = item;
            changeDisplay(vm);
        }
        vm.searchCodes = function () {
            return [vm.currentPhase.Code, vm.currentSubject.Code, vm.currentGrade.Code];
        }
    });

}());