(function (_) {
    'use strict';

    var logEnabled = false;
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
        log = function (msg) {
            if (!logEnabled) {
                return;
            }
            console.log(msg);
        },
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
        setProperty = function (model, propName, val) {
            //Set property case-insensitively
            var fixPropName = (propName + "").toLowerCase();
            for (var prop in model) {
                if (model.hasOwnProperty(prop) && fixPropName === (prop + "").toLowerCase()) {
                    model[prop] = val;
                    return;
                }
            }

            //not defined yet, set it
            model[propName] = val;
        },
        resetLog = function (enabled) {
            if (enabled === true) {
                logEnabled = true;
                return;
            }
            logEnabled = false;
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
        dicCategory = function () {
            var categories = function () {
                //todo refactor name : dicItemMetas
                var items = [];
                items.push({ sort: 0, key: "orgType", name: "类型", ItemsKey: 'orgTypes' });
                items.push({ sort: 1, key: "org", name: "组织", ItemsKey: 'orgs' });
                items.push({ sort: 2, key: "phase", name: "学段", ItemsKey: 'phases' });
                items.push({ sort: 3, key: "subject", name: "学科", ItemsKey: 'subjects' });
                items.push({ sort: 4, key: "grade", name: "年级", ItemsKey: 'grades' });
                return items;
            }(),
            getCategoryKey = function (category) {
                if (!category) {
                    console.log('getCategoryKey typeof' + typeof category);
                    throw { name: 'bad category key' };
                }
                if (typeof category === "string") {
                    return category;
                }
                return category.key;
            },
            getCategoryItemsKey = function (category) {
                if (!category) {
                    console.log('getCategoryItemsKey typeof' + typeof category);
                    throw { name: 'bad category key' };
                }
                if (typeof category === "string") {
                    return category + 's';
                }
                return category.ItemsKey;

            },
            getCategoryEmptyItemKey = function (category) {
                var key = getCategoryKey(category);
                return key + 'Empty';
            };

            return {
                hidePropertyName: "Hidden",
                EmptyItemTemplate: { Code: "", Name: "全部" },
                categories: categories,
                getCategoryKey: getCategoryKey,
                getCategoryItemsKey: getCategoryItemsKey,
                getCategoryEmptyItemKey: getCategoryEmptyItemKey
            };
        }(),
        createEmptyItem = function () {
            var newItem = copyData(dicCategory.EmptyItemTemplate);
            return newItem;
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
        createDicRelationHashtable = function () {
            var split = ',',
            createRelationKey = function () {
                if (arguments.length === 0) {
                    throw { name: 'bad createRelationKey args' };
                }
                var arr = Array.from(arguments);
                var code = arr.join(',');
                return code;
            },
            relations = {};
            relations.createRelationKey = createRelationKey;
            relations.addRelation = function () {
                console.log(arguments);
                //var relationKey = createRelationKey(arguments);
                var relationKey = createRelationKey.apply(null, arguments);
                console.log(relationKey);
                relations[relationKey] = true;
            };
            relations.hasRelation = function () {
                //var relationKey = createRelationKey(arguments);
                var relationKey = createRelationKey.apply(null, arguments);
                if (relations[relationKey]) {
                    return relations[relationKey];
                }
                return false;
            };

            return relations;
        },
        createDicCatalogVm = function () {

            //private helper methods
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
            var containItem = function(relations, key) {

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
                var shouldShow = theVm.relations.hasRelation(currentOrgType.Code, org.Code);
                log('shouldShowThisOrg: ' + theVm.relations.createRelationKey(currentOrgType.Code, org.Code) + ' => '+ shouldShow);
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
                var shouldShow = theVm.relations.hasRelation(currentOrgType.Code, phase.Code);
                log('shouldShowOrgTypePhase: ' + theVm.relations.createRelationKey(currentOrgType.Code, phase.Code) + ' => ' + shouldShow);
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
                var shouldShow = theVm.relations.hasRelation(currentPhase.Code, subject.Code);
                log('shouldShowOrgTypePhase: ' + theVm.relations.createRelationKey(currentPhase.Code, subject.Code) + ' => ' + shouldShow);
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
                var shouldShow = theVm.relations.hasRelation(currentPhase.Code, grade.Code);
                log('shouldShowOrgTypePhase: ' + theVm.relations.createRelationKey(currentPhase.Code, grade.Code) + ' => ' + shouldShow);
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

                //按关系查找
                var shouldShow = theVm.relations.hasRelation(currentPhase.Code, currentSubject.Code, grade.Code);
                log('shouldShowOrgTypePhase: ' + theVm.relations.createRelationKey(currentPhase.Code, currentSubject.Code, grade.Code) + ' => ' + shouldShow);
                return shouldShow;
            };
            var categories = dicCategory.categories;
            var getCategoryKey = dicCategory.getCategoryKey;
            var getCategoryItemsKey = dicCategory.getCategoryItemsKey;
            var getCategoryEmptyItemKey = dicCategory.getCategoryEmptyItemKey;

            //return model
            var initVm = function () {

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

                return vm;
            };

            var vm = initVm();

            //初始化字典项
            vm.initItems = function (dicCatalog) {
                if (!dicCatalog) {
                    return;
                }
                for (var i = 0; i < categories.length; i++) {
                    var category = categories[i];
                    var categoryItemsKey = getCategoryItemsKey(category);
                    var categoryEmptyItemKey = getCategoryEmptyItemKey(category);
                    var items = getProperty(dicCatalog, categoryItemsKey);
                    //console.log('initItems: ' + categoryItemsKey);
                    //console.log(items);
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
            vm.initRelations = function (dicCatalog) {

                //dic relations
                //OrgType   ->  Org
                //OrgType   ->  Phase
                //Phase     ->  Subject
                //Phase     ->  Grade
                //Phase     ->  Subject     ->  Grade

                var relationTable = createDicRelationHashtable();

                var orgTypes = dicCatalog.OrgTypes; //org.OrgTypeCode
                var orgs = dicCatalog.Orgs; //org.OrgTypeCode

                //[].forEach(function (value, index, array) {
                //    //code something
                //});
                
                orgTypes.forEach(function (orgType) {
                    orgs.forEach(function (org) {
                        if (org.OrgTypeCode === "" || equalIgnoreCase(org.OrgTypeCode, orgType.Code)) {
                            //组织类型空，或者二者匹配
                            //console.log('add ' + orgType.Code + ', '+ org.Id);
                            relationTable.addRelation(orgType.Code, org.Id);
                        }
                    });
                });

                var orgTypePhases = dicCatalog.OrgTypePhases;
                orgTypePhases.forEach(function (orgTypePhase) {
                    console.log(orgTypePhase);
                    relationTable.addRelation(orgTypePhase.LeftDicItemCode, orgTypePhase.RightDicItemCode);
                });

                var phaseSubjects = dicCatalog.PhaseSubjects;
                phaseSubjects.forEach(function (phaseSubject) {
                    relationTable.addRelation(phaseSubject.LeftDicItemCode, phaseSubject.RightDicItemCode);
                });

                var phaseGrades = dicCatalog.PhaseGrades;
                phaseGrades.forEach(function (phaseGrade) {
                    relationTable.addRelation(phaseGrade.LeftDicItemCode, phaseGrade.RightDicItemCode);
                });

                //三元关系
                var customizePhaseSubjectGrades = dicCatalog.CustomizePhaseSubjectGrades;
                var phases = dicCatalog.Phases;
                var subjects = dicCatalog.Subjects;
                var grades = dicCatalog.Grades;
                var hasPhaseSubjectGradeRelation = function(customizePhaseSubjectGrades, phase, subject, grade) {
                    for (var i = 0; i < customizePhaseSubjectGrades.length; i++) {
                        var customizePhaseSubjectGrade = customizePhaseSubjectGrades[i];

                        if (customizePhaseSubjectGrade.InUse 
                            && customizePhaseSubjectGrade.PhaseCode === phase.Code 
                            && customizePhaseSubjectGrade.SubjectCode === subject.Code 
                            && customizePhaseSubjectGrade.GradeCode === grade.Code ) {
                            return true;
                        }
                    }
                    return false;
                };
                phases.forEach(function (phase) {
                    subjects.forEach(function (subject) {
                        var hasPhaseSubject = relationTable.hasRelation(phase.Code, subject.Code);
                        if (hasPhaseSubject) {
                            grades.forEach(function (grade) {
                                var hasPhaseGrade = relationTable.hasRelation(phase.Code, grade.Code);
                                if (hasPhaseGrade) {
                                    relationTable[relationTable.createRelationKey(phase.Code, subject.Code, grade.Code)] = hasPhaseSubjectGradeRelation(customizePhaseSubjectGrades, phase, subject, grade);
                                }
                            });
                        }
                    });

                });

                vm.relations = relationTable;

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

                var changedCategory = categories[category];
                for (var item in categories) {
                    if (categories.hasOwnProperty(item)) {
                        var currentItem = categories[item];
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
            var shouldShowPhase = function (theVm, orgTypeCode, phase) {
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
                    var phaseCopy = { Code: phase.Code, Name: phase.Name, Hidden: !shouldShow, Subjects: [] };
                    phasesCopy.push(phaseCopy);
                    angular.forEach(phase.Subjects, function (subject) {
                        phaseCopy.Subjects.push({ Code: subject.Code, Name: subject.Name });
                    });
                });
                return phasesCopy;
            };

            return vm;
        };

    var dicHelper = function () {
        return {
            copyData: copyData,
            resetLog: resetLog,
            log: log,
            createDicCatalogVm: createDicCatalogVm
        };
    }();
    _.createDicHelper = function () {
        return dicHelper;
    };
})(zqnb || {});