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
    addCodeItemIfNotExist = function (codeItems, codeItem) {
        if (!containItem(codeItems, codeItem)) {
            codeItems.push(codeItem);
        }
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
    },
    categoriesDic = function () {
        return {
            orgType: {
                sort: 0,
                key: "orgType",
                name: "组织类型"
            },
            org: {
                sort: 1,
                key: "org",
                name: "组织"
            },
            phase: {
                sort: 2,
                key: "phase",
                name: "学段"
            },
            subject: {
                sort: 3,
                key: "subject",
                name: "学科"
            },
            grade: {
                sort: 4,
                key: "grade",
                name: "年级"
            }
        };
    }(),
    categories = function () {
        return [categoriesDic.orgType, categoriesDic.org, categoriesDic.phase, categoriesDic.subject, categoriesDic.grade];
    }(),
    getCategoryKey = function (category) {
        if (category === undefined) {
            console.log('getCategoryKey typeof');
            console.log(typeof category);
            throw { name: 'bad' };
        }
        if (typeof category === "string") {
            return category;
        }
        return category.key;
    },
    getCategoryItemsKey = function (category) {
        var key = getCategoryKey(category);
        return key + 's';

    },
    getCategoryEmptyItemKey = function (category) {
        var key = getCategoryKey(category);
        return key + 'Empty';
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
            createInitItems: createInitItems,
            categories: categories
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
        };
        var isEmptyItem = function (item) {
            //console.log(item);
            return item.Code === '';
        };
        var hiddenByRelation = function (theVm, items, showShowFunc) {
            //refresh hidden
            angular.forEach(items, function (item) {
                item.Hidden = true;
                var shouldShow = showShowFunc(theVm, item);
                if (shouldShow) {
                    item.Hidden = false;
                }
            });
        };
        var shouldShowOrgTypeOrg = function (theVm, org) {
            var currentOrgType = theVm.selectResult.orgType;
            //当前上级类型为【全部】，或未知组织类型，所有【组织】永远显示
            if (isEmptyItem(currentOrgType) || !org.OrgTypeCode) {
                return true;
            }
            //【全部】按钮永远显示
            if (isEmptyItem(org)) {
                return true;
            }
            //按关系查找
            var shouldShow = containItem(theVm.visiableOrgTypeOrgs, createCodeItem(currentOrgType.Code, org.Code));
            //console.log('shouldShowThisOrg: ' + createCodeItem(currentOrgType.Code, org.Code).Code + "+" + org.OrgTypeCode + " = " + shouldShow);
            return shouldShow;
        };
        var shouldShowOrgTypePhase = function (theVm, phase) {
            var currentOrgType = theVm.selectResult.orgType;
            //当前上级类型为【全部】，所有【学段】永远显示
            if (isEmptyItem(currentOrgType) || currentOrgType.Code === "JiGou-KeShi") {
                return true;
            }
            //【全部】按钮永远显示
            if (isEmptyItem(phase)) {
                return true;
            }
            //按关系查找
            var shouldShow = containItem(theVm.visiableOrgTypePhases, createCodeItem(currentOrgType.Code, phase.Code));
            return shouldShow;
        };
        var shouldShowPhaseSubject = function (theVm, subject) {
            var currentPhase = theVm.selectResult.phase;
            //当前上级类型为【全部】，所有【学段】永远显示
            if (isEmptyItem(currentPhase) || !currentPhase.Code) {
                return true;
            }
            //【学科（全部）】按钮永远显示
            if (isEmptyItem(subject)) {
                return true;
            }
            //按关系查找
            var codeItem = createCodeItem(currentPhase.Code, subject.Code);
            var shouldShow = containItem(theVm.visiablePhaseSubjects, codeItem);
            if (shouldShow) {
                //console.log("refresh phase subjects: " + currentPhase.Name + ',' + subject.Name + ' ' + shouldShow);
            }
            return shouldShow;
        };
        var shouldShowPhaseGrade = function (theVm, grade) {
            var currentPhase = theVm.selectResult.phase;
            //当前全部学段，或未知学段类型，所有【年级】永远显示
            if (isEmptyItem(currentPhase) || !currentPhase.Code) {
                return true;
            }
            //【年级（全部）】按钮永远显示
            if (isEmptyItem(grade)) {
                return true;
            }

            //按关系查找
            var codeItem = createCodeItem(currentPhase.Code, grade.Code);
            var shouldShow = containItem(theVm.visiablePhaseGrades, codeItem);
            if (shouldShow) {
                //console.log("refresh phase grades: " + currentPhase.Name + ',' + grade.Name + ' ' + shouldShow);
            }
            return shouldShow;
        };
        var shouldShowPhaseSubjectGrade = function (theVm, grade) {
            var currentPhase = theVm.selectResult.phase;
            var currentSubject = theVm.selectResult.subject;
            //当前全部学段、学科，或未知学段、学科类型，所有【年级】永远显示
            if (isEmptyItem(currentPhase) || !currentPhase.Code || isEmptyItem(currentSubject) || !currentSubject.Code) {
                return true;
            }
            //【年级（全部）】按钮永远显示
            if (isEmptyItem(grade)) {
                return true;
            }

            //按关系查找
            var codeItem = createCodeItem(currentPhase.Code, currentSubject.Code, grade.Code);
            var shouldShow = containItem(theVm.visiablePhaseSubjectGrades, codeItem);
            if (shouldShow) {
                //console.log("refresh phase subject grades: " + currentPhase.Name + ',' + currentSubject.Name + ',' + grade.Name + ' ' + shouldShow);
            }
            return shouldShow;
        };

        //return model
        var vm = {
            //选择结果
            selectResult: {},
            //是否自动补齐全部
            autoAppendEmpty: true
        };

        //setup properties for categories
        for (var i = 0; i < categories.length; i++) {
            var emptyItem = createEmptyItem();
            var category = categories[i];

            var categoryKey = getCategoryKey(category);
            var categoryItemsKey = getCategoryItemsKey(category);
            var categoryEmptyItemKey = getCategoryEmptyItemKey(category);

            //items: null
            //emptyItem: createEmptyItem()
            vm[categoryItemsKey] = null;
            vm[categoryEmptyItemKey] = emptyItem;
            //selectResult.item = emptyItem;
            vm.selectResult[categoryKey] = emptyItem;
        }

        //[this.orgType.Name, this.org.Name, this.phase.Name, this.subject.Name, this.grade.Name];
        vm.selectResult.display = function () {
            //console.log('display');
            //console.log(dicCatalogVm.selectResult);
            //return [this.orgType.Name, this.org.Name, this.phase.Name, this.subject.Name, this.grade.Name];
            var items = [];
            for (var i = 0; i < categories.length; i++) {
                var category = categories[i];
                var categoryKey = getCategoryKey(category);
                items.push(vm.selectResult[categoryKey].Name);
            }
            return items;
        }

        //初始化字典项
        vm.initItems = function (config) {
            if (!config) {
                return;
            }
            for (var i = 0; i < categories.length; i++) {
                var category = categories[i];
                var categoryItemsKey = getCategoryItemsKey(category);
                var categoryEmptyItemKey = getCategoryEmptyItemKey(category);
                var items = config[categoryItemsKey];
                if (items) {
                    //hack for orgs
                    if (categoryItemsKey === "orgs") {
                        items = fixOrgModels(items);
                    }
                    var appendEmptyItem = vm.autoAppendEmpty ? vm[categoryEmptyItemKey] : null;
                    vm[categoryItemsKey] = createInitItems(items, appendEmptyItem);
                }
            }
        };

        //初始化字典项的关系
        vm.initRelations = function (config) {

            //dic relations

            var visiableOrgTypeOrgs = [];
            var orgTypes = config.orgTypes; //org.OrgTypeCode
            var orgs = config.orgs; //org.OrgTypeCode
            angular.forEach(orgTypes, function (orgType) {
                angular.forEach(orgs, function (org) {
                    if (org.OrgTypeCode === "" || equalIgnoreCase(org.OrgTypeCode, orgType.Code)) {
                        //组织类型空，或者二者匹配
                        addCodeItemIfNotExist(visiableOrgTypeOrgs, createCodeItem(orgType.Code, org.Id));
                    }
                });
            });

            var visiableOrgTypePhases = [];
            var orgTypePhases = config.orgTypePhases;
            angular.forEach(orgTypePhases, function (orgTypePhase) {
                //console.log(orgTypePhase);
                addCodeItemIfNotExist(visiableOrgTypePhases, createCodeItem(orgTypePhase.OrgTypeCode, orgTypePhase.PhaseCode));
            });

            var visiablePhaseSubjects = [];
            var visiablePhaseGrades = [];
            var visiablePhaseSubjectGrades = [];
            angular.forEach(config.dicSettings, function (phase) {
                if (!phase.InUse) {
                    return;
                }

                angular.forEach(phase.Grades, function (grade) {
                    if (!grade.InUse) {
                        return;
                    }
                    addCodeItemIfNotExist(visiablePhaseGrades, createCodeItem(phase.Code, grade.Code));
                });

                angular.forEach(phase.Subjects, function (subject) {
                    if (!subject.InUse) {
                        return;
                    }
                    addCodeItemIfNotExist(visiablePhaseSubjects, createCodeItem(phase.Code, subject.Code));

                    angular.forEach(subject.Grades, function (grade) {
                        if (!grade.InUse) {
                            return;
                        }
                        addCodeItemIfNotExist(visiablePhaseSubjectGrades, createCodeItem(phase.Code, subject.Code, grade.Code));
                    });
                });
            });

            vm.visiableOrgTypeOrgs = visiableOrgTypeOrgs;
            vm.visiableOrgTypePhases = visiableOrgTypePhases;
            vm.visiablePhaseSubjects = visiablePhaseSubjects;
            vm.visiablePhaseGrades = visiablePhaseGrades;
            vm.visiablePhaseSubjectGrades = visiablePhaseSubjectGrades;

            vm.dicSettings = config.dicSettings;

            //console.log('initRelation');
            //console.log(dicVm);
        };

        //是否是空的集合（或只有【全部】按钮）
        vm.isEmptyItems = function (currentCategory) {
            //theVm.phases, theVm.orgs, ...
            var categoryItemsKey = getCategoryItemsKey(currentCategory);
            var currentItems = vm[categoryItemsKey];
            if (!currentItems) {
                return false;
            }
            if (currentItems.length === 0 || (currentItems.length === 1 && currentItems[0].Code === '')) {
                return false;
            }
            return true;
        };


        vm.updateView = function () {

            console.log('updateView');
            //console.log(getSelectCodes());
            //console.log(vm.org);

            hiddenByRelation(vm, vm.orgs, shouldShowOrgTypeOrg);
            //console.log('shouldShowOrgTypeOrg');
            hiddenByRelation(vm, vm.phases, shouldShowOrgTypePhase);
            //console.log('shouldShowOrgTypePhase');
            hiddenByRelation(vm, vm.subjects, shouldShowPhaseSubject);
            //console.log('shouldShowPhaseSubject');
            hiddenByRelation(vm, vm.grades, shouldShowPhaseGrade);
            //console.log('shouldShowPhaseGrade');
            //二次筛选
            hiddenByRelation(vm, vm.grades, shouldShowPhaseSubjectGrade);
            //console.log('shouldShowPhaseSubjectGrade');
        };

        vm.resultChanged = function (category, newItem, oldItem) {
            console.log('resultChanged notify => ' + category + ': ' + oldItem.Code + ' -> ' + newItem.Code);
            if (!category) {
                return;
            }

            var changedCategory = categoriesDic[category];
            for (var item in categoriesDic) {
                if (categoriesDic.hasOwnProperty(item)) {
                    var currentItem = categoriesDic[item];
                    //console.log(currentItem);
                    if (currentItem.sort > changedCategory.sort) {
                        //vm.selectResult.item = emptyItem;
                        vm.selectResult[currentItem.key] = vm[getCategoryEmptyItemKey(currentItem.key)];
                    }
                }
            }
            vm.updateView();
        };

        //个人空间多选
        var shouldShowPhase = function(theVm, orgTypeCode, phase) {
            if (isEmptyItem(orgTypeCode) || orgTypeCode.Code === "JiGou-KeShi" || !orgTypeCode) {
                return true;
            }
            if (isEmptyItem(phase)) {
                return true;
            }
            //按关系查找
            var shouldShow = containItem(theVm.visiableOrgTypePhases, createCodeItem(orgTypeCode, phase.Code));
            return shouldShow;
        };
        //为多选场景准备的方法
        vm.createCurrentOrgTypeCodePhases = function (orgTypeCode) {
            var phases = vm.dicSettings;
            var phasesCopy = [];
            angular.forEach(phases, function (phase) {
                var shouldShow = shouldShowPhase(vm, orgTypeCode, phase);
                if (!shouldShow) {
                    return;
                }
                var phaseCopy = { Code: phase.Code, Name: phase.Name, Hidden: !shouldShow, Subjects:[] };
                phasesCopy.push(phaseCopy);
                angular.forEach(phase.Subjects, function (subject) {
                    phaseCopy.Subjects.push({ Code: subject.Code, Name: subject.Name });
                });
            });
            return phasesCopy;
        };

        return vm;
    };
    _.createDicCatalogVm = function () {
        return createDicCatalogVm();
    };
})(zqnb || {});