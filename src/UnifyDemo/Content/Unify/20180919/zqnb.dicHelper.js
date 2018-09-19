(function (_) {
    'use strict';

    //private
    var knownCategoryCodes = function () {
            var codes = {
                orgType: 'orgType',
                org: 'org',
                phase: 'phase',
                subject: 'subject',
                grade: 'grade'
            };
            return codes;
        }(),
        createEmptyItem = function () { return { Code: "", Name: "全部" }; },
        getProperty = function (model, propName) {
            //Access property case-insensitively
            var lowerPropName = (propName + "").toLowerCase();
            for (var prop in model) {
                //console.log('find prop' + propName + ':' + prop);
                if (model.hasOwnProperty(prop) && lowerPropName === (prop + "").toLowerCase()) {
                    return model[prop];
                }
            }
            return undefined;
        },
        copyData = function (data) {

            //// Shallow copy
            //var newObject = jQuery.extend({}, oldObject);
            //// Deep copy
            //var newObject = jQuery.extend(true, {}, oldObject);

            if (Array.isArray(data)) {
                var newArr = [];
                for (var i = 0; i < data.length; i++) {
                    newArr.push(copyData(data[i]));
                }

                return newArr;
            }
            var newData = jQuery.extend(true, {}, data);
            return newData;
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
        isItemCodeEquals = function (item, code) {
            if (!item || !code) {
                return false;
            }
            return equalIgnoreCase(item.Code, code);
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
        findItemByCode = function (items, code) {
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                if (isItemCodeEquals(item, code)) {
                    return item;
                }
            }
            return null;
        },
        findRightItemsForLeft = function (relations, rightItems, leftCode) {
            var result = [];
            for (var i = 0; i < relations.length; i++) {
                var relation = relations[i];
                if (relation.LeftDicItemCode !== leftCode) {
                    continue;
                }
                var theRightItem = findItemByCode(rightItems, relation.RightDicItemCode);
                if (theRightItem != null) {
                    result.push(theRightItem);
                }
            }
            return result;
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

            //fix orders
            if (items[0].SortNum) {
                items = items.sort(function (a, b) { return a.SortNum - b.SortNum });
            }

            //var copyItems = copyData(items);
            //if (emptyItem) {
            //    //arrName.splice(1, 0, 'newName1');
            //    ////1: index number, 0: number of element to remove, newName1: new element
            //    copyItems.splice(0, 0, emptyItem);
            //}
            //console.log(copyItems);

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

                //filter disabled 
                if (current.InUse === undefined || current.InUse) {
                    initItems.push(copy);
                }
            }

            return initItems;
        },
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
                //var items = dicCatalog[category.itemsKey];
                var items = getProperty(dicCatalog, category.itemsKey);
                if (items !== undefined) {
                    return items;
                }
                //auto fix
                var fixResult = getProperty(dicCatalog, category.itemsKey);
                //console.log('>>>>>>> auto fix category.itemsKey: ' + category.itemsKey);
                //console.log(fixResult);
                return fixResult;
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
            var getOrgTypeCodeForFilterPhases = function (theVm) {
                var currentOrg = theVm.selectResult.org;
                if (!isEmptyItem(currentOrg)) {
                    //console.log('>>>>smart change OrgTypeCode: ' + currentOrg.OrgTypeCode);
                    return currentOrg.OrgTypeCode;
                }

                return theVm.selectResult.orgType.Code;
            };
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

                //【全部】按钮永远显示
                if (isEmptyItem(phase)) {
                    return true;
                }
                var theOrgTypeCode = getOrgTypeCodeForFilterPhases(theVm);
                //当前上级类型为【全部】，所有【学段】永远显示
                if (theOrgTypeCode === "" || theOrgTypeCode === "JiaoYuJu" || theOrgTypeCode === "JiGou-KeShi" || theOrgTypeCode === "LogicOrg") {
                    return true;
                }
                //按关系查找
                var shouldShow = containItem(theVm.visiableOrgTypePhases, createCodeItem(theOrgTypeCode, phase.Code));
                return shouldShow;
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
                        //console.log('>>>>>> shouldShowGradeFromPhase => ', +showThis);
                        //console.log(visiablePhases[i].Name, grade.Name);
                        if (showThis) {
                            return true;
                        }
                    }
                }

                //按关系查找
                return shouldShowGradeFromPhase(theVm, currentPhase, grade);
            };
            var shouldShowPhaseSubjectGrade = function (theVm, grade) {

                //【年级（全部）】按钮永远显示
                if (isEmptyItem(grade)) {
                    console.log('>>>>>>shouldShowPhaseSubjectGrade 1 (empty grade)');
                    return true;
                }

                var currentPhase = theVm.selectResult.phase;
                var currentSubject = theVm.selectResult.subject;
                //当前全部学段、学科，或未知学段、学科类型，所有【年级】永远显示
                if (isEmptyItem(currentPhase) || isEmptyItem(currentSubject)) {
                    console.log('>>>>>>shouldShowPhaseSubjectGrade 1');
                    return true;
                }

                //按关系查找
                var codeItem = createCodeItem(currentPhase.Code, currentSubject.Code, grade.Code);
                var shouldShow = containItem(theVm.visiablePhaseSubjectGrades, codeItem);
                console.log('>>>>>>shouldShowPhaseSubjectGrade 2');
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

            //初始化字典项
            var initItems = function (dicCatalog) {
                if (!dicCatalog) {
                    return;
                }
                for (var i = 0; i < categories.length; i++) {
                    var category = categories[i];
                    var categoryItemsKey = category.itemsKey;
                    var categoryEmptyItemKey = category.emptyItemKey;
                    var items = getProperty(dicCatalog, categoryItemsKey);
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
                var orgTypePhases = getProperty(dicCatalog, "orgTypePhases");
                //console.log('>>>>>> orgTypePhases');
                //console.log(orgTypePhases);
                orgTypePhases.forEach(function (orgTypePhase) {
                    //console.log(orgTypePhase);
                    addCodeItemIfNotExist(visiableOrgTypePhases, createCodeItem(orgTypePhase.LeftDicItemCode, orgTypePhase.RightDicItemCode));
                });

                var visiablePhaseSubjects = [];
                var visiablePhaseGrades = [];
                var visiablePhaseSubjectGrades = [];

                var initVisiableDics = function (phases) {
                    //console.log('--initVisiableDics-----');
                    //console.log(phases);
                    phases.forEach(function (phase) {
                        phase.grades.forEach(function (grade) {
                            if (!grade.InUse) {
                                return;
                            }
                            addCodeItemIfNotExist(visiablePhaseGrades, createCodeItem(phase.Code, grade.Code));
                        });

                        phase.subjects.forEach(function (subject) {
                            if (!subject.InUse) {
                                return;
                            }
                            addCodeItemIfNotExist(visiablePhaseSubjects, createCodeItem(phase.Code, subject.Code));

                            subject.grades.forEach(function (grade) {
                                if (!grade.InUse) {
                                    return;
                                }
                                addCodeItemIfNotExist(visiablePhaseSubjectGrades, createCodeItem(phase.Code, subject.Code, grade.Code));
                            });
                        });
                    });
                };

                var createHashTable = function (getItemKeyFunc, items) {
                    var table = {};
                    for (var i = 0; i < items.length; i++) {
                        var item = items[i];
                        var key = getItemKeyFunc(item);
                        table[key] = item;
                    }
                    return table;
                };
                var hasCode = function (code, codes) {
                    for (var i = 0; i < codes.length; i++) {
                        if (codes[i] === code) {
                            return true;
                        }
                    }
                    return false;
                };
                var setCustomziePhaseSubjects = function (phase, subjects, customizePhaseSubjects) {
                    //set customizes if necessary
                    if (customizePhaseSubjects == null || customizePhaseSubjects.length === 0) {
                        return;
                    }

                    var getCustomizePhaseSubjectKey = function (item) {
                        return item.PhaseCode + ',' + item.SubjectCode;
                    };
                    //for better preformance, convert to hashtable
                    var customizePhaseSubjectTable = createHashTable(getCustomizePhaseSubjectKey, customizePhaseSubjects);

                    for (var i = 0; i < subjects.length; i++) {
                        var subject = subjects[i];
                        var customizePhaseSubjectKey = getCustomizePhaseSubjectKey({ PhaseCode: phase.Code, SubjectCode: subject.Code });
                        var theOne = customizePhaseSubjectTable[customizePhaseSubjectKey];
                        if (theOne) {
                            subject.Name = theOne.CustomizeName;
                            subject.SortNum = theOne.CustomizeSortNum;
                            subject.InUse = theOne.CustomizeInUse;
                            var hideGradeCodeArray = [];
                            if (theOne.HideGradeCodes.trim()) {
                                hideGradeCodeArray = theOne.HideGradeCodes.split(',');
                            }
                            for (var j = 0; j < subject.grades.length; j++) {
                                var grade = subject.grades[j];
                                grade.InUse = !hasCode(grade.Code, hideGradeCodeArray);
                            }
                        }
                    }
                    //fix orders
                    subjects.sort(function (a, b) { return a.SortNum - b.SortNum });
                };
                var resetPhaseSubjectGrade = function (dicCatalog) {
                    //console.log('--------resetPhaseSubjectGrade----------');
                    //console.log(dicCatalog);
                    //set children: Phase.Subjects, Phase.Subject.Grades
                    var phases = getDicCatalogItems(dicCatalog, knownCategoryCodes.phase);
                    var subjects = getDicCatalogItems(dicCatalog, knownCategoryCodes.subject);
                    var grades = getDicCatalogItems(dicCatalog, knownCategoryCodes.grade);
                    var phaseSubjects = getProperty(dicCatalog, "phaseSubjects");
                    var phaseGrades = getProperty(dicCatalog, "phaseGrades");
                    var customizePhaseSubjects = getProperty(dicCatalog, "customizePhaseSubjects");
                    var thePhases = copyData(phases);
                    thePhases.forEach(function (thePhase) {
                        var theSubjectsForPhase = findRightItemsForLeft(phaseSubjects, subjects, thePhase.Code);
                        thePhase.subjects = copyData(theSubjectsForPhase);

                        var theGradesForPhase = findRightItemsForLeft(phaseGrades, grades, thePhase.Code);
                        thePhase.grades = copyData(theGradesForPhase);

                        //set grades for subject
                        thePhase.subjects.forEach(function (theSubject) {
                            theSubject.grades = copyData(theGradesForPhase);
                        });
                        setCustomziePhaseSubjects(thePhase, thePhase.subjects, customizePhaseSubjects);
                    });

                    initVisiableDics(thePhases);
                };
                resetPhaseSubjectGrade(dicCatalog);

                vm.visiableOrgTypeOrgs = visiableOrgTypeOrgs;
                vm.visiableOrgTypePhases = visiableOrgTypePhases;
                vm.visiablePhaseSubjects = visiablePhaseSubjects;
                vm.visiablePhaseGrades = visiablePhaseGrades;
                vm.visiablePhaseSubjectGrades = visiablePhaseSubjectGrades;

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
                        var categoryItemsKey = category.itemsKey;
                        var items = getProperty(vm, categoryItemsKey);
                        var theItem = findItemByCode(items, codeValue);
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
        //public
        return {
            equalIgnoreCase: equalIgnoreCase,
            sameCodeItem: sameCodeItem,
            containItem: containItem,
            findItemByCode: findItemByCode,
            createArrayCode: createArrayCode,
            createCodeItem: createCodeItem,
            createInitItems: createInitItems,
            createCatalogMeta: createCatalogMeta,
            createDicCatalogVm: createDicCatalogVm
        };
    }();
    _.createDicHelper = function () {
        return dicHelper;
    };
})(zqnb || {});