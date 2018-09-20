var setCustomziePhaseSubjects = function (phase, subjects, customizePhaseSubjects) {
    //set customizes if necessary
    if (customizePhaseSubjects == null || customizePhaseSubjects.length === 0) {
        return;
    }

    var getCustomizePhaseSubjectKey = function (item) {
        return createRelationKey(item.PhaseCode, item.SubjectCode);
    };
    //for better preformance, convert to hashtable
    var customizePhaseSubjectTable = createHashTable(getCustomizePhaseSubjectKey, customizePhaseSubjects);
    for (var i = 0; i < subjects.length; i++) {
        var subject = subjects[i];
        var customizePhaseSubjectKey = createRelationKey(phase.Code, subject.Code);
        var theOne = customizePhaseSubjectTable[customizePhaseSubjectKey];
        if (theOne) {
            subject.Name = theOne.CustomizeName;
            subject.SortNum = theOne.CustomizeSortNum;
            subject.InUse = theOne.CustomizeInUse;
            var hideGradeCodeArray = [];
            if (theOne.HideGradeCodes.trim()) {
                hideGradeCodeArray = theOne.HideGradeCodes.split(',');
            }
            for (var j = 0; j < subject.Grades.length; j++) {
                var grade = subject.Grades[j];
                grade.InUse = !hasCode(grade.Code, hideGradeCodeArray);
            }
        }
    }
    //fix orders
    subjects.sort(function (a, b) { return a.SortNum - b.SortNum });
};