(function (_) {
    'use strict';

    //common
    var getProperty = function (model, propName) {
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
        createArrayCode = function () {
            var arr = Array.from(arguments);
            if (arr.length === 0) {
                throw {
                    message: "invalid arguments: ",
                    data: arguments
                }
            }
            var code = arr.join(',');
            return code;
        },
        throwException = function (message, data) {
            throw { exception: message, data: data };
        };

    //dic
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
        createCategories = function () {
            var items = [];
            //code as registry, should never changed!
            items.push({ code: "orgType", itemKey: "orgType", name: "类型", itemsKey: 'orgTypes', emptyItemKey: "orgTypeEmpty", disabled: false, sort: 0 });
            items.push({ code: "org", itemKey: "org", name: "组织", itemsKey: 'orgs', emptyItemKey: "orgEmpty", disabled: false, sort: 1 });
            items.push({ code: "phase", itemKey: "phase", name: "学段", itemsKey: 'phases', emptyItemKey: "phaseEmpty", disabled: false, sort: 2 });
            items.push({ code: "subject", itemKey: "subject", name: "学科", itemsKey: 'subjects', emptyItemKey: "subjectEmpty", disabled: false, sort: 3 });
            items.push({ code: "grade", itemKey: "grade", name: "年级", itemsKey: 'grades', emptyItemKey: "gradeEmpty", disabled: false, sort: 4 });
            return items;
        },
        createEmptyItem = function () {
            return { Code: "", Name: "全部" };
        },
        isEmptyItem = function (item) {
            return item.Code === '';
        },
        isItemCodeEquals = function (item, code) {
            if (!item || !code) {
                return false;
            }
            return equalIgnoreCase(item.Code, code);
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
        createHashtable = function (getItemKeyFunc, items, setWithItem) {
            var table = {};
            items.forEach(function (item) {
                var key = getItemKeyFunc(item);
                if (setWithItem === true) {
                    table[key] = item;
                } else {
                    table[key] = true;
                }
            });
            return table;
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
                throwException("invalid config!", config);
            }
            if (!config.dicCatalogMeta) {
                throwException("config.dicCatalogMeta should not null!", config);
            }

            //private vars & alias 
            var dicCatalog = config.dicCatalog,
                initQueryCodes = config.initQueryCodes,
                dicCatalogMeta = config.dicCatalogMeta,
                categories = dicCatalogMeta.categories,
                autoAppendEmpty = dicCatalogMeta.autoAppendEmpty,
                getCategory = dicCatalogMeta.getCategory,
                hidePropertyName = dicCatalogMeta.hidePropertyName,
                selectResult = function () {
                    var result = {};
                    result.display = function () {
                        var items = [];
                        for (var i = 0; i < categories.length; i++) {
                            var category = categories[i];
                            items.push(result[category.code].Name);
                        }
                        return items;
                    };
                    return result;
                }();

            //private method
            var setupEmptyCategories = function (theVm) {
                //setup empty default properties with categories
                categories.forEach(function (category) {
                    var emptyItem = createEmptyItem();
                    theVm[category.itemsKey] = null;
                    theVm[category.emptyItemKey] = emptyItem;
                    theVm.selectResult[category.code] = emptyItem;
                });
            },
                initItems = function (theDicCatalog, theVm) {
                    if (!theDicCatalog) {
                        throwException("invalid DicCatalog!", theDicCatalog);
                    }
                    if (!theVm) {
                        throwException("invalid Vm!", theVm);
                    }

                    var fixOrgModels = function (orgs) {
                        var fixOrgs = [];
                        for (var i = 0; i < orgs.length; i++) {
                            var current = orgs[i];
                            fixOrgs.push({ Code: current.Id, Name: current.Name, OrgTypeCode: current.OrgTypeCode, ParentCode: current.ParentId });
                        }
                        return fixOrgs;
                    };
                    var createInitItems = function (items, emptyItem) {
                        if (!items || items.length === 0) {
                            return null;
                        }

                        //fix orders
                        if (items[0].SortNum) {
                            items = items.sort(function (a, b) { return a.SortNum - b.SortNum });
                        }

                        var initItems = [];
                        if (emptyItem) {
                            initItems.push(emptyItem);
                        }

                        items.forEach(function (item) {
                            var copy = { Code: item.Code, Name: item.Name };
                            //hack for org
                            if (item.ParentCode) {
                                copy.ParentCode = item.ParentCode;
                            }
                            if (item.OrgTypeCode) {
                                copy.OrgTypeCode = item.OrgTypeCode;
                            }

                            //filter disabled 
                            if (item.InUse === undefined || item.InUse) {
                                initItems.push(copy);
                            }
                        });
                        for (var i = 0; i < items.length; i++) {
                        }

                        return initItems;
                    };

                    categories.forEach(function (category) {
                        var categoryItemsKey = category.itemsKey;
                        var categoryEmptyItemKey = category.emptyItemKey;
                        var items = getProperty(theDicCatalog, categoryItemsKey);
                        if (items) {
                            //hack for orgs
                            if (equalIgnoreCase(category.code, knownCategoryCodes.org)) {
                                items = fixOrgModels(items);
                            }
                            var appendEmptyItem = autoAppendEmpty ? theVm[categoryEmptyItemKey] : null;
                            theVm[categoryItemsKey] = createInitItems(items, appendEmptyItem);
                        }
                    });
                },
                getDicCatalogItems = function (theDicCatalog, categoryCode) {
                    var category = getCategory(categoryCode);
                    var items = getProperty(theDicCatalog, category.itemsKey);
                    return items;
                },
                hasCode = function (code, codes) {
                    for (var i = 0; i < codes.length; i++) {
                        if (codes[i] === code) {
                            return true;
                        }
                    }
                    return false;
                },
                setCustomziePhaseSubjects = function (phase, subjects, customizePhaseSubjects) {
                    //set customizes if necessary
                    if (customizePhaseSubjects == null || customizePhaseSubjects.length === 0) {
                        return;
                    }

                    var getCustomizePhaseSubjectKey = function (item) {
                        return item.PhaseCode + ',' + item.SubjectCode;
                    };
                    //for better preformance, convert to hashtable
                    var customizePhaseSubjectTable = createHashtable(getCustomizePhaseSubjectKey, customizePhaseSubjects, true);

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
                },
                createPhaseTrees = function (phases, subjects, grades, phaseSubjects, phaseGrades, customizePhaseSubjects) {
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
                    return thePhases;
                },
                initRelationsAndPhaseTrees = function (theDicCatalog, theVm) {
                    if (!theDicCatalog) {
                        throwException("invalid DicCatalog!", theDicCatalog);
                    }
                    if (!theVm) {
                        throwException("invalid Vm!", theVm);
                    }
                    var createLeftRightKey = function (relationItem) {
                        return createArrayCode(relationItem.LeftDicItemCode, relationItem.RightDicItemCode);
                    };
                    //1 orgType, org
                    var orgTypeOrgs = function () {
                        var orgTypes = getDicCatalogItems(theDicCatalog, knownCategoryCodes.orgType);
                        var orgs = getDicCatalogItems(theDicCatalog, knownCategoryCodes.org);
                        var relations = [];
                        orgTypes.forEach(function (orgType) {
                            orgs.forEach(function (org) {
                                if (org.OrgTypeCode === "" || equalIgnoreCase(org.OrgTypeCode, orgType.Code)) {
                                    //组织类型空，或者二者匹配
                                    relations.push({ LeftDicItemCode: orgType.Code, RightDicItemCode: org.Id });
                                }
                            });
                        });
                        return relations;
                    }();
                    var relationOrgTypeOrgs = createHashtable(createLeftRightKey, orgTypeOrgs);

                    //2 orgType, phase
                    var orgTypePhases = getProperty(theDicCatalog, "orgTypePhases");
                    var relationOrgTypePhases = createHashtable(createLeftRightKey, orgTypePhases);

                    //3 phase, subject
                    //4 phase, grade
                    //5 phase, subject, grade(phase, grade, subject)
                    //6 customize!
                    var phases = getDicCatalogItems(theDicCatalog, knownCategoryCodes.phase);
                    var subjects = getDicCatalogItems(theDicCatalog, knownCategoryCodes.subject);
                    var grades = getDicCatalogItems(theDicCatalog, knownCategoryCodes.grade);
                    var phaseSubjects = getProperty(theDicCatalog, "phaseSubjects");
                    var phaseGrades = getProperty(theDicCatalog, "phaseGrades");
                    var customizePhaseSubjects = getProperty(dicCatalog, "customizePhaseSubjects");

                    var relationPhaseSubjects = {};
                    var relationPhaseGrades = {};
                    var relationPhaseSubjectGrades = {};

                    var phaseTrees = createPhaseTrees(phases, subjects, grades, phaseSubjects, phaseGrades, customizePhaseSubjects);
                    var initTreeRelations = function (thePhaseTrees) {
                        //console.log('--initVisiableDics-----');
                        //console.log(phases);
                        var phases = thePhaseTrees;
                        phases.forEach(function (phase) {
                            phase.grades.forEach(function (grade) {
                                if (!grade.InUse) {
                                    return;
                                }
                                relationPhaseGrades[createArrayCode(phase.Code, grade.Code)] = true;
                            });
                            phase.subjects.forEach(function (subject) {
                                if (!subject.InUse) {
                                    return;
                                }
                                relationPhaseSubjects[createArrayCode(phase.Code, subject.Code)] = true;

                                subject.grades.forEach(function (grade) {
                                    if (!grade.InUse) {
                                        return;
                                    }
                                    relationPhaseSubjectGrades[createArrayCode(phase.Code, subject.Code, grade.Code)] = true;
                                });
                            });
                        });
                    };
                    initTreeRelations(phaseTrees);

                    var relations = {
                        relationOrgTypeOrgs: relationOrgTypeOrgs,
                        relationOrgTypePhases: relationOrgTypePhases,
                        relationPhaseSubjects: relationPhaseSubjects,
                        relationPhaseGrades: relationPhaseGrades,
                        relationPhaseSubjectGrades: relationPhaseSubjectGrades
                    };

                    theVm.relations = relations;
                    theVm.phaseTrees = phaseTrees;
                },
                setSelectResultByQueryCodes = function (queryCodes, theVm) {
                var needRefresh = false;
                for (var prop in queryCodes) {
                    if (queryCodes.hasOwnProperty(prop)) {
                        var codeValue = queryCodes[prop];
                        var category = getCategory(prop);
                        var categoryCode = category.code;
                        var categoryItemsKey = category.itemsKey;
                        var items = getProperty(theVm, categoryItemsKey);
                        var theItem = findItemByCode(items, codeValue);
                        var categoryEmptyItemKey = category.emptyItemKey;
                        var theEmptyItem = theVm[categoryEmptyItemKey];
                        if (theItem !== null) {
                            theVm.selectResult[categoryCode] = theItem;
                        } else {
                            theVm.selectResult[categoryCode] = theEmptyItem;
                        }
                        //console.log('set query result: ' + categoryKey + theVm.selectResult[categoryCode].Code + ',' + theVm.selectResult[categoryCode].Name);
                        needRefresh = true;
                    }
                }
                if (needRefresh) {
                    theVm.updateView();
                }
            };

            //private methods
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
                //var shouldShow = containItem(theVm.visiableOrgTypeOrgs, createCodeItem(currentOrgType.Code, org.Code));
                var shouldShow = theVm.relations.relationOrgTypeOrgs[createArrayCode(currentOrgType.Code, org.Code)] === true;
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
                //var shouldShow = containItem(theVm.visiableOrgTypePhases, createCodeItem(theOrgTypeCode, phase.Code));
                var shouldShow = theVm.relations.relationOrgTypePhases[createArrayCode(theOrgTypeCode, phase.Code)] === true;
                return shouldShow;
            };
            var shouldShowSubjectFromPhase = function (theVm, currentPhase, currentSubject) {

                //【学科（全部）】按钮永远显示
                if (isEmptyItem(currentSubject)) {
                    return true;
                }
                //按关系查找
                //var codeItem = createCodeItem(currentPhase.Code, currentSubject.Code);
                //var shouldShow = containItem(theVm.visiablePhaseSubjects, codeItem);
                var shouldShow = theVm.relations.relationPhaseSubjects[createArrayCode(currentPhase.Code, currentSubject.Code)] === true;
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
                //var codeItem = createCodeItem(currentPhase.Code, currentGrade.Code);
                //var shouldShow = containItem(theVm.visiablePhaseGrades, codeItem);
                var shouldShow = theVm.relations.relationPhaseGrades[createArrayCode(currentPhase.Code, currentGrade.Code)] === true;
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
                    return true;
                }

                var currentPhase = theVm.selectResult.phase;
                var currentSubject = theVm.selectResult.subject;
                //当前全部学段、学科，或未知学段、学科类型，所有【年级】永远显示
                if (isEmptyItem(currentPhase) || isEmptyItem(currentSubject)) {
                    return true;
                }

                //按关系查找
                //var codeItem = createCodeItem(currentPhase.Code, currentSubject.Code, grade.Code);
                //var shouldShow = containItem(theVm.visiablePhaseSubjectGrades, codeItem);
                var shouldShow = theVm.relations.relationPhaseSubjectGrades[createArrayCode(currentPhase.Code, currentSubject.Code, grade.Code)] === true;
                //console.log('>>>>>> shouldShowPhaseSubjectGrade: ' + codeItem.Code + " => " + shouldShow);
                //console.log("refresh phase subject grades: " + currentPhase.Name + ',' + currentSubject.Name + ',' + grade.Name + ' ' + shouldShow);
                return shouldShow;
            };
            var shouldShowPhaseGradeSubject = function (theVm, subject) {

                //【学科（全部）】按钮永远显示
                if (isEmptyItem(subject)) {
                    return true;
                }

                var currentPhase = theVm.selectResult.phase;
                var currentGrade = theVm.selectResult.grade;
                if (isEmptyItem(currentPhase) || isEmptyItem(currentGrade)) {
                    //console.log('>>>>>> shouldShowPhaseGradeSubject: 1');
                    return true;
                }

                //按关系查找
                //var codeItem = createCodeItem(currentPhase.Code, subject.Code, currentGrade.Code);
                //var shouldShow = containItem(theVm.visiablePhaseSubjectGrades, codeItem);
                var shouldShow = theVm.relations.relationPhaseSubjectGrades[createArrayCode(currentPhase.Code, subject.Code, currentGrade.Code)] === true;
                //console.log('>>>>>> shouldShowPhaseGradeSubject: ' + codeItem.Code + " => " + shouldShow);
                return shouldShow;
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
            var hiddenGradeByRelationCustomize = function (theVm) {
                //refresh hidden
                var subjectCategory = getCategory(knownCategoryCodes.subject);
                var gradeCategory = getCategory(knownCategoryCodes.grade);
                if (subjectCategory.sort <= gradeCategory.sort) {
                    //console.log(">>>>>> subject grade");
                    var grades = theVm[gradeCategory.itemsKey];
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
                        var shouldShow = shouldShowPhaseSubjectGrade(theVm, grade);
                        grade[hidePropertyName] = !shouldShow;
                        //console.log('check with customize: ' + grade.Name + ' => ' + shouldShow);
                    });
                } else {
                    //console.log(">>>>>> grade subject");
                    var subjects = theVm[subjectCategory.itemsKey];
                    subjects.forEach(function (subject) {
                        if (subject[hidePropertyName]) {
                            //本来就是隐藏的，忽略
                            //console.log('return ! ' + subject.Name + ' => Hidden:' + subject[hidePropertyName]);
                            return;
                        }
                        if (theVm.selectResult.phase.Code === "") {
                            //全部，忽略
                            //console.log('return ! ' + subject.Name + ' => Hidden:' + subject[hidePropertyName]);
                            return;
                        }
                        var shouldShow = shouldShowPhaseGradeSubject(theVm, subject);
                        subject[hidePropertyName] = !shouldShow;
                        //console.log('check with customize: ' + subject.Name + ' => ' + shouldShow);
                    });
                }
            };

            var onUpdating_OrgTypeOrgs = function (theVm) {
                hiddenByRelation(theVm, knownCategoryCodes.org, shouldShowOrgTypeOrg);
            };
            var onUpdating_OrgTypePhases = function (theVm) {
                hiddenByRelation(theVm, knownCategoryCodes.phase, shouldShowOrgTypePhase);
            };
            var onUpdating_PhaseSubjects = function (theVm) {
                hiddenByRelation(theVm, knownCategoryCodes.subject, shouldShowPhaseSubject);
            };
            var onUpdating_PhaseGrades = function (theVm) {
                hiddenByRelation(theVm, knownCategoryCodes.grade, shouldShowPhaseGrade);
            };
            var onUpdating_PhaseSubjectGrades = function (theVm) {
                hiddenGradeByRelationCustomize(theVm, shouldShowPhaseSubjectGrade);
            };

            var vm = {
                //元信息
                _metas: dicCatalogMeta,
                //选择的结果
                selectResult: selectResult,
                //是否是空的集合（或只有【全部】按钮），或者被配置为禁用
                isEmptyItems: function (category) {
                    var categoryItem = getCategory(category);
                    if (categoryItem) {
                        return categoryItem.disabled;
                    }
                    //theVm.phases, theVm.orgs, ...
                    var currentItems = this[categoryItem.itemsKey];
                    if (!currentItems) {
                        return false;
                    }
                    if (currentItems.length === 0 || (currentItems.length === 1 && currentItems[0].Code === '')) {
                        return false;
                    }
                    return true;
                },
                //刷新视图模型的状态（根据内置的关系）
                updateView: function () {
                    //console.log('----- updateView start ');
                    //console.log('override this to updateView by customize logic');

                    //console.log('onUpdating_OrgTypeOrgs');
                    onUpdating_OrgTypeOrgs(this);
                    //console.log('onUpdating_OrgTypePhases');
                    onUpdating_OrgTypePhases(this);
                    //console.log('onUpdating_PhaseSubjects');
                    onUpdating_PhaseSubjects(this);
                    //console.log('onUpdating_PhaseGrades');
                    onUpdating_PhaseGrades(this);
                    //console.log('onUpdating_PhaseSubjectGrades');
                    onUpdating_PhaseSubjectGrades(this);
                    //console.log('----- updateView end ');
                },
                //选择项改变后，通知刷新视图模型的事件
                onSelectResultChanged: function (category, newItem, oldItem) {
                    //console.log('use onSelectResultChanged event to notify ui, if needed => ' + category + 'old : ' + oldItem.Code + ' -> new: ' + newItem.Code);
                    if (!category) {
                        return;
                    }
                    this.updateView();
                }
            };

            //初始化结果项、空项、集合项
            setupEmptyCategories(vm);
            //初始化字典项
            initItems(dicCatalog, vm);
            //初始化字典项的关系
            initRelationsAndPhaseTrees(dicCatalog, vm);
            //初始化选中参数
            setSelectResultByQueryCodes(initQueryCodes, vm);
            return vm;
        };

    var dicHelper = function () {
        //public
        return {
            createCatalogMeta: createCatalogMeta,
            createDicCatalogVm: createDicCatalogVm
        };
    }();
    _.createDicHelper = function () {
        return dicHelper;
    };
})(zqnb || {});