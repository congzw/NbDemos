(function () {
    'use strict';
    var mainApp = zqnb.mainApp;

    var dicHelper = zqnb.createDicHelper();

    mainApp.factory('dicCatalogMockService', ['$http', function ($http) {

        var getDicCatalogsUrl = '/Content/scripts/GetDicCatalogs.js';
        var getDicCatalogs = function () {
            return $http.get(getDicCatalogsUrl);
        };

        var getDicOrgTypesUrl = '/Content/scripts/GetDicOrgTypes.js';
        var getDicOrgTypes = function () {
            return $http.get(getDicOrgTypesUrl);
        };

        var vm = {};

        var preInit = function () {
            vm.autoAppendEmpty = true;
            vm.orgs = [];
            vm.phases = [];
            vm.subjects = [];
            vm.grades = [];
            vm.grades = [];
            vm.initCodes = { OrgCode: '', PhaseCode: '', SubjectCode: '', GradeCode: '' };
        };
        var initDicSettings = function (dicSettings) {

            vm.dicSettings = dicSettings;
            //initCodes
            vm.initCodes = { OrgCode: 'whyz', PhaseCode: 'Phase2', SubjectCode: 'GS001', GradeCode: 'GO003' };

            var phases = [];
            var subjects = [];
            var grades = [];

            angular.forEach(dicSettings, function (phase) {
                if (!phase.InUse) {
                    return;
                }
                phases.push({ Code: phase.Code, Name: phase.Name, Hidden: false });
                //subjects
                angular.forEach(phase.Subjects, function (subject) {
                    if (!subject.InUse) {
                        return;
                    }
                    if (!dicHelper.containItem(subjects, subject)) {
                        subjects.push({ Code: subject.Code, Name: subject.Name, Hidden: false });
                    }
                });
                //grades
                angular.forEach(phase.Grades, function (grade) {
                    if (!grade.InUse) {
                        return;
                    }
                    if (!dicHelper.containItem(grades, grade)) {
                        grades.push({ Code: grade.Code, Name: grade.Name, Hidden: false });
                    }
                });
            });

            vm.phases = phases;
            vm.subjects = subjects;
            vm.grades = grades;

            return vm;
        };
        var initOrgTypes = function (returnOrgTypes) {
            var orgTypes = [];
            var orgTypePhases = [];
            for (var i = 0; i < returnOrgTypes.length; i++) {
                var orgType = returnOrgTypes[i];
                orgTypes.push({ Code: orgType.Code, Name: orgType.Name });
                for (var j = 0; j < orgType.Phases.length; j++) {
                    orgTypePhases.push({ OrgTypeCode: orgType.Code, PhaseCode: orgType.Phases[j].Code });
                }
            }
            vm.orgTypes = orgTypes;
            vm.orgTypePhases = orgTypePhases;
        };
        var initOrgs = function () {
            var orgs = [];
            //mock orgs
            orgs.push({ Id: "whhc", Name: "威海环翠", OrgTypeCode: "JiaoYuJu", ParentId: "" });
            orgs.push({ Id: "jyz", Name: "教研组", OrgTypeCode: "JiGou-KeShi", ParentId: "whhc" });
            orgs.push({ Id: "ywjyz", Name: "语文教研组", OrgTypeCode: "JiGou-KeShi", ParentId: "jyz" });
            orgs.push({ Id: "sxjyz", Name: "数学教研组", OrgTypeCode: "JiGou-KeShi", ParentId: "jyz" });
            orgs.push({ Id: "whsyxx", Name: "威海试验小学", OrgTypeCode: "XueXiao-211", ParentId: "whhc" });
            orgs.push({ Id: "whyz", Name: "威海一中（完中）", OrgTypeCode: "XueXiao-341", ParentId: "whhc" });
            orgs.push({ Id: "whez", Name: "威海二中（高中）", OrgTypeCode: "XueXiao-342", ParentId: "whhc" });
            orgs.push({ Id: "whsz", Name: "威海三中（高中）", OrgTypeCode: "XueXiao-342", ParentId: "whhc" });

            vm.orgs = orgs;
        };

        var initAsync = function (callback) {
            preInit();
            initOrgs();
            getDicCatalogs().success(function (dicSettings) {

                initDicSettings(dicSettings);

                getDicOrgTypes()
                .success(function (orgTypes) {
                    initOrgTypes(orgTypes);
                    callback(vm);
                });
            });
        };
        return { initAsync: initAsync };
    }
    ]);
}());