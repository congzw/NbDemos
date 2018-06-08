(function (_) {
    'use strict';

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
    },
    sameCodeItem = function (item1, item2) {
        return item1 === item2 || equalIgnoreCase(item1.Code, item2.Code);
    },
    containItem = function (items, itemToCheck) {
        if (!items || !itemToCheck) {
            return false;
        }
        for (var i = 0; i < items.length; i++) {
            if (sameCodeItem(items[i], itemToCheck)) {
                return true;
            }
        }
        return false;
    },
    findItem = function (items, code) {
        for (var i = 0; i < items.length; i++) {
            var current = items[i];
            var sameCode = equalIgnoreCase(current.Code, code);
            if (sameCode) {
                return current;
            }
        }
        return null;
    },
    createEmptyItem = function () {
        return { Code: "", Name: "全部" };
    },
    createArrayCode = function (arr) {
        if (arr.length === 0) {
            return '';
        }
        var code = arr.join(',');
        return code;
    },
    createCodeItem = function () {
        var arr = Array.from(arguments);
        var code = createArrayCode(arr);
        var codeItem = { Code: code };
        return codeItem;
    },
    createInitItems = function (items, emptyItem) {
        if (!items || items.length === 0) {
            return null;
        }
        var initItems = [];
        if (emptyItem) {
            initItems.push(emptyItem);
        }
        for (var i = 0; i < items.length; i++) {
            var current = items[i];
            var copy = { Code: current.Code, Name: current.Name };
    
            //hack for org
            if (current.ParentCode) {
                copy.ParentCode = current.ParentCode;
            }
            if (current.OrgTypeCode) {
                copy.OrgTypeCode = current.OrgTypeCode;
            }
            initItems.push(copy);
        }
        return initItems;
    };

    var dicHelper = function () {
        return {
            equalIgnoreCase: equalIgnoreCase,
            sameCodeItem: sameCodeItem,
            containItem: containItem,
            findItem: findItem,
            createEmptyItem: createEmptyItem,
            createArrayCode: createArrayCode,
            createCodeItem: createCodeItem,
            createInitItems: createInitItems
        };
    }();
    _.createDicHelper = function () {
        return dicHelper;
    };

    var createDicCatalogVm = function () {

        //private methods
        var fixOrgModels = function (orgs) {
            var fixOrgs = [];
            for (var i = 0; i < orgs.length; i++) {
                var current = orgs[i];
                fixOrgs.push({ Code: current.Id, Name: current.Name, OrgTypeCode: current.OrgTypeCode, ParentCode: current.ParentId });
            }
            return fixOrgs;
        },
        initOrgTypes = function (theVm, orgTypes) {
            var appendEmptyItem = theVm.autoAppendEmpty ? theVm.emptyOrgType : null;
            theVm.orgTypes = createInitItems(orgTypes, appendEmptyItem);
        },
        initOrgs = function (theVm, orgs) {
            //fix orgs
            theVm.emptyOrg.OrgTypeCode = "";
            var items = fixOrgModels(orgs);
            var appendEmptyItem = theVm.autoAppendEmpty ? theVm.emptyOrg : null;
            theVm.orgs = createInitItems(items, appendEmptyItem);
        },
        initPhases = function (theVm, phases) {
            var appendEmptyItem = theVm.autoAppendEmpty ? theVm.emptyPhase : null;
            theVm.phases = createInitItems(phases, appendEmptyItem);
        },
        initSubjects = function (theVm, subjects) {
            var appendEmptyItem = theVm.autoAppendEmpty ? theVm.emptySubject : null;
            theVm.subjects = createInitItems(subjects, appendEmptyItem);
        },
        initGrades = function (theVm, grades) {
            var appendEmptyItem = theVm.autoAppendEmpty ? theVm.emptyGrade : null;
            theVm.grades = createInitItems(grades, appendEmptyItem);
        },
        initItems = function (theVm, config) {
            if (!config) {
                return;
            }

            if (config.orgTypes) {
                initOrgTypes(theVm, config.orgTypes);
            }

            if (config.orgs) {
                initOrgs(theVm, config.orgs);
            }

            if (config.phases) {
                initPhases(theVm, config.phases);
            }

            if (config.subjects) {
                initSubjects(theVm, config.subjects);
            }

            if (config.grades) {
                initGrades(theVm, config.grades);
            }
        },
        dicVm = function () {
            return {
                //是否自动补齐【全部】按钮
                autoAppendEmpty: true,

                //组织类型
                orgTypes: null,
                emptyOrgType: createEmptyItem(),
                //组织
                orgs: null,
                emptyOrg: createEmptyItem(),
                //学段
                phases: null,
                emptyPhase: createEmptyItem(),
                //学科
                subjects: null,
                emptySubject: createEmptyItem(),
                //年级
                grades: null,
                emptyGrade: createEmptyItem(),
                orgTypePhases: null,
                visiableOrgTypePhases: null
            }
        }();

        //-------------字典项-------------
        dicVm.initItems = function (config) {
            return initItems(dicVm, config);
        };
        //-------------字典关系-------------
        //dicVm.shouldShowThisPhase = function (theVm, phase) {
        //};

        //-------------视图状态-------------
        dicVm.selectResult = {
            orgType: dicVm.emptyOrgType,
            org: dicVm.emptyOrg,
            phase: dicVm.emptyPhase,
            subject: dicVm.emptySubject,
            grade: dicVm.emptyGrade,
            display: function () {
                return [this.orgType.Name, this.org.Name, this.phase.Name, this.subject.Name, this.grade.Name];
            }
        };


        //var shouldShowThisPhase = function (theVm, phase) {

        //    var currentOrg = theVm.org;
        //    //当前全部组织，或未知组织类型，所有【学科】永远显示
        //    if (isEmptyItem(currentOrg) || !currentOrg.OrgTypeCode) {
        //        return true;
        //    }

        //    //【学科（全部）】按钮永远显示
        //    if (isEmptyItem(phase)) {
        //        return true;
        //    }

        //    //按关系查找
        //    var orgTypePhaseCodeItem = createCodeItem(currentOrg.OrgTypeCode, phase.Code);
        //    var shouldShow = containItem(theVm.visiableOrgTypePhases, orgTypePhaseCodeItem);
        //    return shouldShow;
        //};

        return dicVm;
    };
    _.createDicCatalogVm = function () {
        return createDicCatalogVm();
    };
})(zqnb || {});