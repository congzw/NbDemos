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
            initItems.push({ Code: current.Code, Name: current.Name });
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

        var fixOrgModels = function (orgs) {
            var fixOrgs = [];
            for (var i = 0; i < orgs.length; i++) {
                var current = orgs[i];
                fixOrgs.push({ Code: current.Id, Name: current.Name, OrgTypeCode: current.OrgTypeCode });
            }
            return fixOrgs;
        }

        var dicVm = {};

        //是否自动补齐【全部】按钮
        dicVm.autoAppendEmpty = true;

        //-------------字典项-------------
        //组织类型
        dicVm.orgTypes = null;
        dicVm.emptyOrgType = createEmptyItem();
        dicVm.initOrgTypes = function (orgTypes) {
            var appendEmptyItem = dicVm.autoAppendEmpty ? dicVm.emptyOrgType : null;
            dicVm.orgTypes = createInitItems(orgTypes, appendEmptyItem);
        };

        //组织
        dicVm.orgs = null;
        dicVm.emptyOrg = createEmptyItem();
        dicVm.initOrgs = function (orgs) {
            //fix orgs
            dicVm.emptyOrg.OrgTypeCode = "";
            var items = fixOrgModels(orgs);
            var appendEmptyItem = dicVm.autoAppendEmpty ? dicVm.emptyOrg : null;
            dicVm.orgs = createInitItems(items, appendEmptyItem);
        };

        //学段
        dicVm.phases = null;
        dicVm.emptyPhase = createEmptyItem();
        dicVm.initPhases = function (phases) {
            var appendEmptyItem = dicVm.autoAppendEmpty ? dicVm.emptyPhase : null;
            dicVm.phases = createInitItems(phases, appendEmptyItem);
        };

        //学科
        dicVm.subjects = null;
        dicVm.emptySubject = createEmptyItem();
        dicVm.initSubjects = function (subjects) {
            var appendEmptyItem = dicVm.autoAppendEmpty ? dicVm.emptySubject : null;
            dicVm.subjects = createInitItems(subjects, appendEmptyItem);
        };

        //年级
        dicVm.grades = null;
        dicVm.emptyGrade = createEmptyItem();
        dicVm.initGrades = function (grades) {
            var appendEmptyItem = dicVm.autoAppendEmpty ? dicVm.emptyGrade : null;
            dicVm.grades = createInitItems(grades, appendEmptyItem);
        };

        //-------------字典关系-------------

        //-------------视图状态-------------
        dicVm.result = {
            orgType: dicVm.emptyOrgType,
            org: dicVm.emptyOrg,
            phase: dicVm.emptyPhase,
            subject: dicVm.emptySubject,
            grade: dicVm.emptyGrade,
            display: function () {
                return [this.orgType.Name, this.org.Name, this.phase.Name, this.subject.Name, this.grade.Name];
            }
        };

        var createOrgTypePhaseCodeItem = function (org, phase) {
            var orgTypePhaseCodeItem = { Code : org.OrgTypeCode + ',' + phase.Code };
            return orgTypePhaseCodeItem;
        },
            createDicCatalogVm = function () {
                var vm = { };

                var emptyOrg = createEmptyItem(),
                    emptyPhase = createEmptyItem(),
                    emptySubject = createEmptyItem(),
                    emptyGrade = createEmptyItem(),
                    initCodes = { OrgCode : '', PhaseCode: '', SubjectCode: '', GradeCode: '' },
                    getSelectCodes = function () {
                        return [vm.org.Code, vm.phase.Code, vm.subject.Code, vm.grade.Code];
                    },
                    initItems = function (initData) {
                        var orgs = [];
                        var phases = [];
                        var subjects = [];
                        var grades = [];

                        if (initData.autoAppendEmpty) {
                            orgs.push(emptyOrg);
                            phases.push(emptyPhase);
                            subjects.push(emptySubject);
                            grades.push(emptyGrade);
                        }

                        //setup orgs
                        angular.forEach(initData.orgs, function (item, key) {
                            var orgTypeCode = '';
                            if (item.OrgTypeCode) {
                                orgTypeCode = item.OrgTypeCode;
                            }
                            orgs.push({ Code: item.Code, Name: item.Name, OrgTypeCode: orgTypeCode });
                        });
                        vm.orgs = orgs;

                        //setup phases
                        angular.forEach(initData.phases, function (item, key) {
                            phases.push({ Code: item.Code, Name: item.Name });
                        });
                        vm.phases = phases;

                        //setup subjects
                        angular.forEach(initData.subjects, function (item, key) {
                            subjects.push({ Code: item.Code, Name: item.Name });
                        });
                        vm.subjects = subjects;

                        //setup grades
                        angular.forEach(initData.grades, function (item, key) {
                            grades.push({ Code: item.Code, Name: item.Name });
                        });
                        vm.grades = grades;

                        //console.log(vm);
                    },
                    initCurrent = function (initData) {
                        var initCodes = initData.initCodes;
                        if (initCodes) {
                            if (initCodes.OrgCode) {
                                vm.initCodes.OrgCode = initCodes.OrgCode;
                            }
                            if (initCodes.PhaseCode) {
                                vm.initCodes.PhaseCode = initCodes.PhaseCode;
                            }
                            if (initCodes.SubjectCode) {
                                vm.initCodes.SubjectCode = initCodes.SubjectCode;
                            }
                            if (initCodes.GradeCode) {
                                vm.initCodes.GradeCode = initCodes.GradeCode;
                            }
                        }
                        var theOrg = findItem(vm.orgs, vm.initCodes.OrgCode);
                        if (theOrg) {
                            vm.org = theOrg;
                        }

                        var thePhase = findItem(vm.phases, vm.initCodes.PhaseCode);
                        if (thePhase) {
                            vm.phase = thePhase;
                        }

                        var theSubject = findItem(vm.subjects, vm.initCodes.SubjectCode);
                        if (theSubject) {
                            vm.subject = theSubject;
                        }

                        var theGrade = findItem(vm.grades, vm.initCodes.GradeCode);
                        if (theGrade) {
                            vm.grade = theGrade;
                        }
                    },
                    initRelation = function (initData) {

                        //dic relations

                        var orgTypePhases = initData.orgTypePhases;
                        vm.orgTypePhases = orgTypePhases;

                        var visiableOrgTypePhases = [];
                        var visiablePhaseSubjects = [];
                        var visiablePhaseGrades = [];
                        var visiablePhaseSubjectGrades = [];

                        angular.forEach(orgTypePhases, function (orgTypePhase) {
                            var orgTypePhaseCodeItem = createCodeItem(orgTypePhase.OrgTypeCode, orgTypePhase.PhaseCode);
                            if (!containItem(visiableOrgTypePhases, orgTypePhaseCodeItem)) {
                                visiableOrgTypePhases.push(orgTypePhaseCodeItem);
                            }
                        });

                        angular.forEach(initData.dicSettings, function (phase) {

                            if(!phase.InUse) {
                                return;
                            }

                            angular.forEach(phase.Grades, function (grade) {
                                if(!grade.InUse) {
                                    return;
                                }
                                var phaseGradeCodeItem = createCodeItem(phase.Code, grade.Code);
                                if (!containItem(visiablePhaseGrades, phaseGradeCodeItem)) {
                                    visiablePhaseGrades.push(phaseGradeCodeItem);
                                }
                            });

                            angular.forEach(phase.Subjects, function (subject) {
                                if(!subject.InUse) {
                                    return;
                                }
                                var subjectCodeItem = createCodeItem(phase.Code, subject.Code);
                                if (!containItem(visiablePhaseSubjects, subjectCodeItem)) {
                                    visiablePhaseSubjects.push(subjectCodeItem);
                                }

                                angular.forEach(subject.Grades, function (grade) {
                                    if(!grade.InUse) {
                                        return;
                                    }
                                    var phaseSubjectGradeCodeItem = createCodeItem(phase.Code, subject.Code, grade.Code);
                                    if (!containItem(visiablePhaseSubjectGrades, phaseSubjectGradeCodeItem)) {
                                        visiablePhaseSubjectGrades.push(phaseSubjectGradeCodeItem);
                                    }
                                });
                            });
                        });

                        vm.visiableOrgTypePhases = visiableOrgTypePhases;
                        vm.visiablePhaseSubjects = visiablePhaseSubjects;
                        vm.visiablePhaseGrades = visiablePhaseGrades;
                        vm.visiablePhaseSubjectGrades = visiablePhaseSubjectGrades;

                        //console.log('initRelation');
                        //console.log(vm);

                    };

                //private method
                var isEmptyItem = function (item) {
                    //console.log(item);
                    return item.Code === '';
                }
                var shouldShowThisPhase = function (theVm, phase) {

                    var currentOrg = theVm.org;
                    //当前全部组织，或未知组织类型，所有【学科】永远显示
                    if (isEmptyItem(currentOrg) || !currentOrg.OrgTypeCode) {
                        return true;
                    }

                    //【学科（全部）】按钮永远显示
                    if (isEmptyItem(phase)) {
                        return true;
                    }

                    //按关系查找
                    var orgTypePhaseCodeItem = createCodeItem(currentOrg.OrgTypeCode, phase.Code);
                    var shouldShow = containItem(theVm.visiableOrgTypePhases, orgTypePhaseCodeItem);
                    return shouldShow;
                };
                var shouldShowThisPhaseSubject = function (theVm, subject) {
                    var currentPhase = theVm.phase;
                    //当前全部学段，或未知学段类型，所有【学科】永远显示
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
                var shouldShowThisPhaseGrade = function (theVm, grade) {
                    var currentPhase = theVm.phase;
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
                var shouldShowThisPhaseSubjectGrade = function (theVm, grade) {
                    var currentPhase = theVm.phase;
                    var currentSubject = theVm.subject;
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

                var updateView = function () {

                    //console.log('updateView');
                    //console.log(getSelectCodes());
                    //console.log(vm.org);

                    //refresh phases by current Org
                    angular.forEach(vm.phases, function (phase) {
                        phase.Hidden = true;
                        var shouldShow = shouldShowThisPhase(vm, phase);
                        if (shouldShow) {
                            phase.Hidden = false;
                        }
                    });

                    //refresh phase subjects by current phase
                    angular.forEach(vm.subjects, function (subject) {
                        subject.Hidden = true;
                        var shouldShow = shouldShowThisPhaseSubject(vm, subject);
                        if (shouldShow) {
                            subject.Hidden = false;
                        }
                    });

                    //refresh phase grade by current phase
                    angular.forEach(vm.grades, function (grade) {
                        grade.Hidden = true;
                        var shouldShow = shouldShowThisPhaseGrade(vm, grade);
                        if (shouldShow) {
                            grade.Hidden = false;
                        }
                    });

                    //二次筛选
                    //refresh phase subject grade by current phase
                    angular.forEach(vm.grades, function (grade) {

                        //var needChangeToEmptyGrade = true;

                        //如果学段和学科同时不为空，则需要二次筛选
                        if(!(isEmptyItem(vm.phase)) && !isEmptyItem(vm.subject)) {
                            var shouldShow = shouldShowThisPhaseSubjectGrade(vm, grade);
                            if (!shouldShow) {
                                grade.Hidden = true;
                            } else {
                                if (sameCodeItem(grade, vm.grade)) {
                                    //console.log("当前的选中的年级在隐藏的按钮中: " + grade.Code + ',' + theVm.selectResult.Grade.Code);
                                    //needChangeToEmptyGrade = false;
                                }
                            }
                        }
                    });
                };
                var resultChanged = function (event) {
                    if(!event) {
                        return;
                    }
                    if (event.ChangeBy === 'Org') {
                        vm.phase = emptyPhase;
                        vm.subject = emptySubject;
                        vm.grade = emptyGrade;
                        return;
                    }

                    if (event.ChangeBy === 'Phase') {
                        vm.subject = emptySubject;
                        vm.grade = emptyGrade;
                        return;
                    }

                    if (event.ChangeBy === 'Subject') {
                        vm.grade = emptyGrade;
                        return;
                    }
                    //do nothing for grade
                };

                vm = {
                        initCodes: initCodes,
                        org: emptyOrg,
                        orgs: null,
                        phase: emptyPhase,
                        phases: null,
                        subject: emptySubject,
                        subjects: null,
                        grade: emptyGrade,
                        grades: null,
                        getSelectCodes: getSelectCodes,
                        initItems: initItems,
                        initCurrent: initCurrent,
                        initRelation: initRelation,
                        resultChanged: resultChanged,
                        updateView: updateView
                    };
                return vm;
            };

        return dicVm;
    };
    _.createDicCatalogVm = function () {
        return createDicCatalogVm();
    };
}) (zqnb || { });