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
        knownCategoryCodes = function () {
            var codes = {
                orgType: 'orgType',
                org: 'org',
                phase: 'phase',
                subject: 'subject',
                grade: 'grade'
            };
            return codes;
        }(),
        createCategories = function () {
            var items = [];
            //code as registry, should never changed!
            items.push({ code: knownCategoryCodes.orgType, itemKey: "orgType", name: "类型", itemsKey: 'orgTypes', emptyItemKey: "orgTypeEmpty", disabled: false });
            items.push({ code: knownCategoryCodes.org, itemKey: "org", name: "组织", itemsKey: 'orgs', emptyItemKey: "orgEmpty", disabled: false });
            items.push({ code: knownCategoryCodes.phase, itemKey: "phase", name: "学段", itemsKey: 'phases', emptyItemKey: "phaseEmpty", disabled: false });
            items.push({ code: knownCategoryCodes.subject, itemKey: "subject", name: "学科", itemsKey: 'subjects', emptyItemKey: "subjectEmpty", disabled: false });
            items.push({ code: knownCategoryCodes.grade, itemKey: "grade", name: "年级", itemsKey: 'grades', emptyItemKey: "gradeEmpty", disabled: false });
            return items;
        },
        createCatalogMeta = function () {
            var categories = createCategories();
            return {
                hidePropertyName: "Hidden",
                autoAppendEmpty: true, //是否自动补齐全部
                knownCategoryCodes: knownCategoryCodes,
                categories: categories,
                getCategory: function (categoryCode) {
                    if (categoryCode === undefined || categoryCode === null) {
                        console.log('getCategoryKey typeof ' + typeof categoryCode);
                        throw { name: 'bad category code: ' };
                    }
                    for (var i = 0; i < categories.length; i++) {
                        var category = categories[i];
                        if (category && category.code === categoryCode) {
                            return category;
                        }
                    }
                    console.log('getCategoryKey typeof ' + typeof categoryCode);
                    throw { name: 'bad category code: ' + categoryCode };
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
            var hidePropertyName = dicCatalogMeta.hidePropertyName;
            
            //private methods
            var getDicCatalogItems = function (dicCatalog, categoryCode) {
                var category = getCategory(categoryCode);
                var items = dicCatalog[category.itemsKey];
                return items;
            };
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
            var shouldShowSubjectFromPhase = function (theVm, currentPhase, currentSubject) {

                //【学科（全部）】按钮永远显示
                if (isEmptyItem(currentSubject)) {
                    return true;
                }
                //按关系查找
                var codeItem = createCodeItem(currentPhase.Code, currentSubject.Code);
                var shouldShow = containItem(theVm.visiablePhaseSubjects, codeItem);
                //if (shouldShow) {
                //    console.log("refresh phase subjects: " + currentPhase.Name + ',' + subject.Name + ' ' + shouldShow);
                //}
                return shouldShow;
            }
            var shouldShowGradeFromPhase = function (theVm, currentPhase, currentGrade) {

                //【年级（全部）】按钮永远显示
                if (isEmptyItem(currentGrade)) {
                    return true;
                }
                //按关系查找
                var codeItem = createCodeItem(currentPhase.Code, currentGrade.Code);
                var shouldShow = containItem(theVm.visiablePhaseGrades, codeItem);
                //if (shouldShow) {
                //    console.log("refresh phase grades: " + currentPhase.Name + ',' + grade.Name + ' ' + shouldShow);
                //}
                return shouldShow;
            }
            var getCurrentShowPhases = function (theVm) {
                var phases = getDicCatalogItems(theVm, knownCategoryCodes.phase);
                var results = [];
                phases.forEach(function (phase) {
                    if (!phase[hidePropertyName]) {
                        results.push(phase);
                    }
                });
                return results;
            };
            var hiddenByRelation = function (theVm, categoryCode, showShowFunc) {
                var category = getCategory(categoryCode);
                var items = theVm[category.itemsKey];

                //refresh hidden
                items.forEach(function (item) {
                    item[hidePropertyName] = true;
                    var shouldShow = showShowFunc(theVm, item);
                    if (shouldShow) {
                        item[hidePropertyName] = false;
                    }
                    //if (categoryCode === 'grade') {
                    //    console.log('hiddenByRelation: ' + item.Name + '=> shouldShow:' + shouldShow);
                    //}
                });

                //如果当前选中（非全部选项）被隐藏，则重置为全部
                var currentItem = theVm.selectResult[categoryCode];
                if (currentItem[hidePropertyName]) {
                    //console.log('当前选中（非全部选项）被隐藏 => 重置为全部： ' + categoryCode);
                    var emptyItem = theVm[category.emptyItemKey];
                    theVm.selectResult[categoryCode] = emptyItem;
                }
            };
            var hiddenGradeByRelationCustomize = function (theVm, showShowFunc) {
                //refresh hidden              
                var category = getCategory(knownCategoryCodes.grade);
                var grades = theVm[category.itemsKey];
                grades.forEach(function (grade) {
                    if (grade[hidePropertyName]) {
                        //本来就是隐藏的，忽略
                        //console.log('return ! ' + grade.Name + ' => Hidden:' + grade[hidePropertyName]);
                        return;
                    }
                    if (theVm.selectResult.phase.Code === "") {
                        //全部学段，忽略
                        //console.log('return ! ' + grade.Name + ' => Hidden:' + grade[hidePropertyName]);
                        return;
                    }
                    var shouldShow = showShowFunc(theVm, grade);
                    if (shouldShow) {
                        //console.log('check with customize: ' + grade.Name + ' => ' + shouldShow);
                        grades[hidePropertyName] = false;
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
                    var visiablePhases = getCurrentShowPhases(theVm);
                    var phases = getDicCatalogItems(theVm, knownCategoryCodes.phase);
                    if (phases.length === visiablePhases.length) {
                        return true;
                    }
                    for (var i = 0; i < visiablePhases.length; i++) {
                        var showThis = shouldShowSubjectFromPhase(theVm, visiablePhases[i], subject);
                        if (showThis) {
                            return true;
                        }
                    }
                }

                return shouldShowSubjectFromPhase(theVm, currentPhase, subject);
            };
            var shouldShowPhaseGrade = function (theVm, grade) {
                var currentPhase = theVm.selectResult.phase;
                //当前全部学段，或未知学段类型，所有【年级】永远显示
                if (isEmptyItem(currentPhase) || !currentPhase.Code) {
                    var visiablePhases = getCurrentShowPhases(theVm);
                    var phases = getDicCatalogItems(theVm, knownCategoryCodes.phase);
                    if (phases.length === visiablePhases.length) {
                        return true;
                    }
                    for (var i = 0; i < visiablePhases.length; i++) {
                        var showThis = shouldShowGradeFromPhase(theVm, visiablePhases[i], grade);
                        if (showThis) {
                            return true;
                        }
                    }
                }

                //按关系查找
                return shouldShowGradeFromPhase(theVm, currentPhase, grade);
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
                            items.push(vm.selectResult[category.code].Name);
                        }
                        return items;
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
                    vm.selectResult[category.code] = emptyItem;
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
                        if (equalIgnoreCase(categoryItemsKey, "orgs")) {
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
                var orgTypes = getDicCatalogItems(dicCatalog, knownCategoryCodes.orgType);
                var orgs = getDicCatalogItems(dicCatalog, knownCategoryCodes.org);
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

                //vm.dicSettings = dicCatalog.dicSettings;

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
                //console.log('----- updateView start ');
                //console.log('override this to updateView by customize logic');

                hiddenByRelation(vm, knownCategoryCodes.org, shouldShowOrgTypeOrg);
                //console.log('shouldShowOrgTypeOrg');
                hiddenByRelation(vm, knownCategoryCodes.phase, shouldShowOrgTypePhase);
                //console.log('shouldShowOrgTypePhase');
                hiddenByRelation(vm, knownCategoryCodes.subject, shouldShowPhaseSubject);
                //console.log('shouldShowPhaseSubject');
                hiddenByRelation(vm, knownCategoryCodes.grade, shouldShowPhaseGrade);
                //console.log('shouldShowPhaseGrade');
                hiddenGradeByRelationCustomize(vm, shouldShowPhaseSubjectGrade); //二次筛选
                //console.log('shouldShowPhaseSubjectGrade');
                //console.log('----- updateView end ');
            };

            vm.onSelectResultChanged = function (category, newItem, oldItem) {
                //console.log('use onSelectResultChanged event to notify ui, if needed => ' + category + 'old : ' + oldItem.Code + ' -> new: ' + newItem.Code);
                if (!category) {
                    return;
                }
                vm.updateView();
            };
            
            var setSelectResultByQueryCodes = function (queryCodes) {
                var needRefresh = false;
                for (var prop in queryCodes) {
                    if (queryCodes.hasOwnProperty(prop)) {
                        var codeValue = queryCodes[prop];
                        var category = getCategory(prop);
                        var categoryCode = category.code;
                        var categoryKey = category.itemKey;
                        var categoryItemsKey = category.itemsKey;
                        var items = vm[categoryItemsKey];
                        var theItem = findItem(items, codeValue);
                        var categoryEmptyItemKey = category.emptyItemKey;
                        var theEmptyItem = vm[categoryEmptyItemKey];
                        if (theItem !== null) {
                            vm.selectResult[categoryCode] = theItem;
                        } else {
                            vm.selectResult[categoryCode] = theEmptyItem;
                        }
                        //console.log('set query result: ' + categoryKey + vm.selectResult[categoryCode].Code + ',' + vm.selectResult[categoryCode].Name);
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