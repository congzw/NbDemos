﻿
(function (_) {
    'use strict';
    var oldConsoleLog = console.log;
    //array.forEach(function(currentValue, index, arr), thisValue)
    var copyData = function (data) {

        //// Shallow copy
        //var newObject = jQuery.extend({}, oldObject);
        //// Deep copy
        //var newObject = jQuery.extend(true, {}, oldObject);

        if (Array.isArray(data)) {
            var newArr = [];

            for (var i = 0; i < data.length; i++) {
                newArr.push(copyData(data[i]));
            }

            //sort by SortNum
            newArr.sort(function (a, b) { return a.SortNum - b.SortNum });
            return newArr;
        }
        var newData = jQuery.extend(true, {}, data);
        return newData;
    },
    resetLog = function (enabled) {
        if (enabled === true) {
            console.log = oldConsoleLog;
            return;
        }
        console.log = function (message) {
            //oldConsoleLog('disabled!');
            //oldConsoleLog('[Dic]=> ' + message);
        };
    },
        equalIgnoreCase = function (str, str2) {
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
        createCategories = function () {
            var items = [];
            items.push({ key: "orgType", name: "类型", itemsKey: 'orgTypes', emptyItemKey: "orgTypeEmpty", code: "orgTypeCode", disabled: false });
            items.push({ key: "org", name: "组织", itemsKey: 'orgs', emptyItemKey: "orgEmpty", code: "orgCode", disabled: false });
            items.push({ key: "phase", name: "学段", itemsKey: 'phases', emptyItemKey: "phaseEmpty", code: "phaseCode", disabled: false });
            items.push({ key: "subject", name: "学科", itemsKey: 'subjects', emptyItemKey: "subjectEmpty", code: "subjectCode", disabled: false });
            items.push({ key: "grade", name: "年级", itemsKey: 'grades', emptyItemKey: "gradeEmpty", code: "gradeCode", disabled: false });
            return items;
        },
        createCatalogMeta = function () {
            var categories = createCategories();
            return {
                hidePropertyName: "Hidden",
                autoAppendEmpty: true, //是否自动补齐全部
                categories: categories,
                getCategory: function (name) {
                    if (name === undefined || name === null) {
                        console.log('getCategoryKey typeof' + typeof name);
                        throw { name: 'bad category name' };
                    }
                    for (var i = 0; i < categories.length; i++) {
                        var category = categories[i];
                        if (category && category.key === name) {
                            return category;
                        }
                    }
                    throw { name: 'bad category name' + name };
                }
            }
        },
        createDicCatalogVm = function (config) {
            if (!config) {
                return null;
            }

            //private helper functions
            var dicCatalog = config.dicCatalog;
            var initQueryCodes = config.initQueryCodes;
            var dicCatalogMeta = config.dicCatalogMeta;
            if (!dicCatalogMeta) {
                dicCatalogMeta = createCatalogMeta();
            }
            var categories = dicCatalogMeta.categories;
            var autoAppendEmpty = dicCatalogMeta.autoAppendEmpty;
            var getCategory = dicCatalogMeta.getCategory;

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
                items.forEach(function (item) {
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
                if (isEmptyItem(currentOrgType) || currentOrgType.Code === "JiGou-KeShi" || currentOrgType.Code === "LogicOrg") {
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

            //vm[categoryItemsKey] => item: X5;
            //vm[categoryEmptyItemKey] => emptyItem: X5;
            //vm.selectResult[categoryKey] => item: X5 (default => emptyItem);
            //vm._metas => 
            //hidePropertyName: "Hidden",
            //autoAppendEmpty: true, //是否自动补齐全部
            //categories: categories,
            //getCategory: function(name){...}

            var vm = {
                _metas: dicCatalogMeta,
                //选择结果
                selectResult: {
                    display: function () {
                        //console.log('display');
                        //console.log(dicCatalogVm.selectResult);
                        //return [this.orgType.Name, this.org.Name, this.phase.Name, this.subject.Name, this.grade.Name];
                        var items = [];
                        for (var i = 0; i < categories.length; i++) {
                            var category = categories[i];
                            items.push(vm.selectResult[category.key].Name);
                        }
                        return items;
                    },
                    changeSelectItem: function (categoryName, currentItem) {
                        var category = getCategory(categoryName);
                        this[category.key] = currentItem;
                    },
                    getSelectItem : function(categoryName) {
                        var category = getCategory(categoryName);
                        return this[category.key];
                    }
                }
            };

            var setupCategories = function () {
                //setup properties with categories
                for (var i = 0; i < categories.length; i++) {
                    var emptyItem = createEmptyItem();
                    var category = categories[i];
                    vm[category.itemsKey] = null;
                    vm[category.emptyItemKey] = emptyItem;
                    vm.selectResult[category.key] = emptyItem;
                }
            };
            setupCategories();

            //[this.orgType.Name, this.org.Name, this.phase.Name, this.subject.Name, this.grade.Name];

            //初始化字典项
            var initItems = function (dicCatalog) {
                if (!dicCatalog) {
                    return;
                }
                for (var i = 0; i < categories.length; i++) {
                    var category = categories[i];
                    var categoryItemsKey = category.itemsKey;
                    var categoryEmptyItemKey = category.emptyItemKey;
                    var items = dicCatalog[categoryItemsKey];
                    if (items) {
                        //hack for orgs
                        if (categoryItemsKey === "orgs") {
                            items = fixOrgModels(items);
                        }
                        var appendEmptyItem = autoAppendEmpty ? vm[categoryEmptyItemKey] : null;
                        vm[categoryItemsKey] = createInitItems(items, appendEmptyItem);
                    }
                }
            };
            initItems(dicCatalog);

            //初始化字典项的关系
            var initRelations = function (dicCatalog) {

                //dic relations

                var visiableOrgTypeOrgs = [];
                var orgTypes = dicCatalog.orgTypes;
                var orgs = dicCatalog.orgs;
                orgTypes.forEach(function (orgType) {
                    orgs.forEach(function (org) {
                        if (org.OrgTypeCode === "" || equalIgnoreCase(org.OrgTypeCode, orgType.Code)) {
                            //组织类型空，或者二者匹配
                            addCodeItemIfNotExist(visiableOrgTypeOrgs, createCodeItem(orgType.Code, org.Id));
                        }
                    });
                });

                var visiableOrgTypePhases = [];
                var orgTypePhases = dicCatalog.orgTypePhases;
                orgTypePhases.forEach(function (orgTypePhase) {
                    //console.log(orgTypePhase);
                    addCodeItemIfNotExist(visiableOrgTypePhases, createCodeItem(orgTypePhase.OrgTypeCode, orgTypePhase.PhaseCode));
                });

                var visiablePhaseSubjects = [];
                var visiablePhaseGrades = [];
                var visiablePhaseSubjectGrades = [];
                dicCatalog.dicSettings.forEach(function (phase) {
                    if (!phase.InUse) {
                        return;
                    }

                    phase.Grades.forEach(function (grade) {
                        if (!grade.InUse) {
                            return;
                        }
                        addCodeItemIfNotExist(visiablePhaseGrades, createCodeItem(phase.Code, grade.Code));
                    });

                    phase.Subjects.forEach(function (subject) {
                        if (!subject.InUse) {
                            return;
                        }
                        addCodeItemIfNotExist(visiablePhaseSubjects, createCodeItem(phase.Code, subject.Code));

                        subject.Grades.forEach(function (grade) {
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

                vm.dicSettings = dicCatalog.dicSettings;

                //console.log('initRelation');
                //console.log(dicVm);
            };
            initRelations(dicCatalog);

            //是否是空的集合（或只有【全部】按钮），或者被配置为禁用
            vm.isEmptyItems = function (category) {
                var categoryItem = getCategory(category);
                if (categoryItem) {
                    return categoryItem.disabled;
                }
                //theVm.phases, theVm.orgs, ...
                var currentItems = vm[categoryItem.itemsKey];
                if (!currentItems) {
                    return false;
                }
                if (currentItems.length === 0 || (currentItems.length === 1 && currentItems[0].Code === '')) {
                    return false;
                }
                return true;
            };

            vm.updateView = function () {

                //console.log('override this to updateView by customize logic');

                hiddenByRelation(vm, vm.orgs, shouldShowOrgTypeOrg);
                //console.log('shouldShowOrgTypeOrg');
                hiddenByRelation(vm, vm.phases, shouldShowOrgTypePhase);
                //console.log('shouldShowOrgTypePhase');
                hiddenByRelation(vm, vm.subjects, shouldShowPhaseSubject);
                //console.log('shouldShowPhaseSubject');
                hiddenByRelation(vm, vm.grades, shouldShowPhaseGrade);
                //console.log('shouldShowPhaseGrade');
                hiddenByRelation(vm, vm.grades, shouldShowPhaseSubjectGrade); //二次筛选
                //console.log('shouldShowPhaseSubjectGrade');
            };

            vm.resultChanged = function (category, newItem, oldItem) {
                console.log('resultChanged notify => ' + category + ': ' + oldItem.Code + ' -> ' + newItem.Code);
                if (!category) {
                    return;
                }
                vm.updateView();
            };

            //个人空间多选
            var shouldShowPhase = function (theVm, orgTypeCode, phase) {
                if (isEmptyItem(orgTypeCode) || orgTypeCode.Code === "JiGou-KeShi" || orgTypeCode.Code === "LogicOrg" || !orgTypeCode) {
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
                phases.forEach(function (phase) {
                    var shouldShow = shouldShowPhase(vm, orgTypeCode, phase);
                    if (!shouldShow) {
                        return;
                    }
                    var phaseCopy = { Code: phase.Code, Name: phase.Name, Hidden: !shouldShow, Subjects: [] };
                    phasesCopy.push(phaseCopy);
                    phase.Subjects.forEach(function (subject) {
                        phaseCopy.Subjects.push({ Code: subject.Code, Name: subject.Name });
                    });
                });
                return phasesCopy;
            };

            var setSelectResultByQueryCodes = function (queryCodes) {
                var needRefresh = false;
                for (var prop in queryCodes) {
                    if (queryCodes.hasOwnProperty(prop)) {
                        var codeValue = queryCodes[prop];
                        var category = getCategory(prop);
                        var categoryKey = category.key;
                        var categoryItemsKey = category.itemsKey;
                        var items = vm[categoryItemsKey];
                        var theItem = findItem(items, codeValue);
                        var categoryEmptyItemKey = category.emptyItemKey;
                        var theEmptyItem = vm[categoryEmptyItemKey];
                        if (theItem !== null) {
                            vm.selectResult[categoryKey] = theItem;
                        } else {
                            vm.selectResult[categoryKey] = theEmptyItem;
                        }
                        //console.log('set query result: ' + categoryKey + vm.selectResult[categoryKey].Code + ',' + vm.selectResult[categoryKey].Name);
                        needRefresh = true;
                    }
                }
                if (needRefresh) {
                    vm.updateView();
                }
            };
            setSelectResultByQueryCodes(initQueryCodes);
            return vm;
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
            resetLog: resetLog,
            createCatalogMeta: createCatalogMeta,
            createDicCatalogVm: createDicCatalogVm
        };
    }();
    _.createDicHelper = function () {
        return dicHelper;
    };
})(zqnb || {});