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

    //check string equals
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
    var containItem = function (items, itemToCheck) {
        if (!items || !itemToCheck) {
            return false;
        }
        for (var i = 0; i < items.length; i++) {
            if (equalIgnoreCase(items[i].Code, itemToCheck.Code)) {
                return true;
            }
        }
        return false;
    };

    mainApp.controller('dicCatalogSearchCtrl', function ($scope, dicCatalogAppService) {

        var vm = this;
        var createEmptyItem = function () {
            return { Code: "", Name: "全部", Selected: true, Hidden: false };
        };
        var dicCatalogContext = function () {

            var ctx = {};

            var emptyPhase = createEmptyItem();
            var emptySubject = createEmptyItem();
            var emptyGrade = createEmptyItem();

            ctx.currentPhase = emptyPhase;
            ctx.currentSubject = emptySubject;
            ctx.currentGrade = emptyGrade;
            
            //todo
            ctx.phases = [];
            ctx.subjects = [];
            ctx.grades = [];

            var visiablePhases = [];
            var visiablePhaseSubjects = [];
            var visiablePhaseGrades = [];
            var visiablePhaseSubjectGrades = [];

            ctx.dicCatalogs = {};

            ctx.getPhases = function () {
                var phases = ctx.phases;
                var dicCatalogs = ctx.dicCatalogs;

                //refresh phases
                angular.forEach(phases, function (item) {
                    var phaseCodeItem = { Code: item.Code };
                    if (containItem(vm.visiablePhases, phaseCodeItem)) {
                        item.Hidden = false;
                    }
                });
            };

            ctx.getPhaseSubjects = function () {
                var currentPhase = ctx.currentPhase;
                if (currentPhase === emptyPhase) {
                    //全部（学科）
                }
            };

            ctx.getPhaseSubjectGrades = function (currentPhase) {

            };


            return ctx;
        }();

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
        var changeContext = function (vm) {

            hideAll(vm.phases);
            hideAll(vm.subjects);
            hideAll(vm.grades);

            //show phases
            angular.forEach(vm.phases, function (item) {
                var phaseCodeItem = { Code: item.Code };
                if (containItem(vm.visiablePhases, phaseCodeItem)) {
                    item.Hidden = false;
                }
            });

            //show phase subjects
            angular.forEach(vm.subjects, function (item) {
                if (vm.currentPhase.Code === "") {
                    //全部（学科）
                    item.Hidden = false;
                    return;
                }

                var phaseSubjectCodeItem = { Code: vm.currentPhase.Code + ',' + vm.currentSubject.Code };
                if (containItem(vm.visiablePhaseSubjects, phaseSubjectCodeItem)) {
                    console.log();
                    item.Hidden = false;
                }
            });

            //show phase grades
            angular.forEach(vm.grades, function (item) {
                if (vm.currentPhase.Code === "") {
                    //全部（学科）
                    item.Hidden = false;
                    return;
                }

                var phaseGradeCodeItem = { Code: vm.currentPhase.Code + ',' + item.Code };
                if (containItem(vm.visiablePhaseGrades, phaseGradeCodeItem)) {
                    item.Hidden = false;
                }
            });

            //show phase subject grades
            angular.forEach(vm.grades, function (item) {
                if (vm.currentPhase.Code !== "" && vm.currentSubject.Code !== "") {
                    var phaseSubjectGradeCodeItem = { Code: vm.currentPhase.Code + ',' + vm.currentSubject.Code + ',' + item.Code };
                    if (!containItem(vm.visiablePhaseSubjectGrades, phaseSubjectGradeCodeItem)) {
                        item.Hidden = false;
                    }
                }
            });
        }


        vm.currentPhase = emptyPhase;
        vm.currentSubject = emptySubject;
        vm.currentGrade = emptyGrade;

        vm.selectPhase = function (item) {
            changeSelect(vm.phases, item);
            vm.currentPhase = item;
            changeContext(vm);
        }
        vm.selectSubject = function (item) {
            changeSelect(vm.subjects, item);
            vm.currentSubject = item;
            changeContext(vm);
        }
        vm.selectGrade = function (item) {
            changeSelect(vm.grades, item);
            vm.currentGrade = item;
            changeContext(vm);
        }
        vm.searchCodes = function () {
            return [vm.currentPhase.Code, vm.currentSubject.Code, vm.currentGrade.Code];
        }


        dicCatalogAppService.getDicCatalogs().success(function (data) {
            vm.dicCatalogs = data;
            //console.log(data);

            //todo
            var phases = [];
            phases.push(createEmptyItem());
            angular.forEach(data, function (value, key) {
                phases.push({ Code: value.Code, Name: value.Name, Hidden: !value.InUse });
            });
            //console.log(phases);
            vm.phases = phases;

            //todo
            var subjects = [];
            subjects.push(createEmptyItem());
            angular.forEach(data, function (value, key) {

                angular.forEach(value.Subjects, function (value, key) {

                    if (!containItem(subjects, value)) {
                        subjects.push({ Code: value.Code, Name: value.Name, Hidden: !value.InUse });
                    }
                });
            });
            //console.log(subjects);
            vm.subjects = subjects;

            //todo
            var grades = [];
            grades.push(createEmptyItem());
            angular.forEach(data, function (value, key) {

                angular.forEach(value.Grades, function (value, key) {

                    if (!containItem(grades, value)) {
                        grades.push({ Code: value.Code, Name: value.Name, Hidden: !value.InUse });
                    }
                });
            });
            //console.log(grades);
            vm.grades = grades;

            //todo
            var visiablePhases = [];
            var visiablePhaseSubjects = [];
            var visiablePhaseGrades = [];
            var visiablePhaseSubjectGrades = [];

            angular.forEach(data, function (phase, key) {
                if (!phase.InUse) {
                    return;
                }
                var phaseCodeItem = { Code: phase.Code };
                if (!containItem(visiablePhases, phaseCodeItem)) {
                    visiablePhases.push(phaseCodeItem);
                }

                angular.forEach(phase.Grades, function (grade, key) {

                    if (!grade.InUse) {
                        return;
                    }
                    var phaseGradeCodeItem = { Code: phase.Code + ',' + grade.Code };
                    if (!containItem(visiablePhaseGrades, phaseGradeCodeItem)) {
                        visiablePhaseGrades.push(phaseGradeCodeItem);
                    }
                });

                angular.forEach(phase.Subjects, function (subject, key) {

                    if (!subject.InUse) {
                        return;
                    }
                    var subjectCodeItem = { Code: phase.Code + ',' + subject.Code };
                    if (!containItem(visiablePhaseSubjects, subjectCodeItem)) {
                        visiablePhaseSubjects.push(subjectCodeItem);
                    }

                    angular.forEach(subject.Grades, function (grade, key) {
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
            vm.visiablePhases = visiablePhases;
            vm.visiablePhaseSubjects = visiablePhaseSubjects;
            vm.visiablePhaseSubjectGrades = visiablePhaseSubjectGrades;


        });
    });

}());