define(function() {

    //------ log 日志组件（仿照Log4Net的思路）-------
    var log = {};
    log.levels = {
        DEBUG: 1,
        INFO: 2,
        WARN: 3,
        ERROR: 4,
        FATAL: 5,
        OFF: 100
    };
    log.level = log.levels.DEBUG;

    var getLogLevelDesc = function(logLevel) {

        var logLevelInfo = "OFF";
        for (let key in log.levels) {
            if (log.levels.hasOwnProperty(key)) {
                if (logLevel === log.levels[key]) {
                    logLevelInfo = key;
                    break;
                }
            }
        }
        return logLevelInfo;
    };
    log.getLogLevelDesc = getLogLevelDesc;

    log.isOFF = function() {
        return log.level === log.levels.OFF;
    }

    log.log = function(logObject, logLevel) {
        if (!window.console || !window.console.log) {
            return;
        }

        if (logLevel != undefined && logLevel < log.level) {
            return;
        }

        var logLevelInfo = getLogLevelDesc(log.level);
        if (logLevelInfo === "OFF") {
            return;
        }
        console.log(logLevelInfo + ' -> ' + logObject);
    };
    log.debug = function(logObject) {
        //log.log("DEBUG: ", log.levels.DEBUG);
        log.log(logObject, log.levels.DEBUG);
    };
    log.info = function(logObject) {
        log.log("INFO: ", log.levels.INFO);
        log.log(logObject, log.levels.INFO);
    };
    log.warn = function(logObject) {
        log.log("WARN: ", log.levels.WARN);
        log.log(logObject, log.levels.WARN);
    };
    log.error = function(logObject) {
        log.log("ERROR: ", log.levels.ERROR);
        log.log(logObject, log.levels.ERROR);
    };
    log.fatal = function(logObject) {
        log.log("FATAL: ", log.levels.FATAL);
        log.log(logObject, log.levels.FATAL);
    };

    return log;
});