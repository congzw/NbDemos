(function (name, definition) {
    if (typeof module !== 'undefined') module.exports = definition();
    else if (typeof define === 'function' && typeof define.amd === 'object') define(definition);
    else this[name] = definition();
}('MyClock', function () {
    "use strict";

    var showCurrentTime = function (date, config) {

        var colorhead = "";
        var yy = date.getYear();
        if (yy < 1900) yy = yy + 1900;
        var MM = date.getMonth() + 1;
        if (MM < 10) MM = '0' + MM;
        var dd = date.getDate();
        if (dd < 10) dd = '0' + dd;
        var hh = date.getHours();
        if (hh < 10) hh = '0' + hh;
        var mm = date.getMinutes();
        if (mm < 10) mm = '0' + mm;
        var ss = date.getSeconds();
        if (ss < 10) ss = '0' + ss;
        var ww = date.getDay();
        if (ww === 0) colorhead = "<font color=\"#FF0000\">";
        if (ww > 0 && ww < 6) colorhead = "<font color=\"#373737\">";
        if (ww === 6) colorhead = "<font color=\"#008000\">";
        if (ww === 0) ww = "星期日";
        if (ww === 1) ww = "星期一";
        if (ww === 2) ww = "星期二";
        if (ww === 3) ww = "星期三";
        if (ww === 4) ww = "星期四";
        if (ww === 5) ww = "星期五";
        if (ww === 6) ww = "星期六";
        var colorfoot = "</font>";

        var showDate = hh + ":" + mm + ":" + ss;
        if (config.withDayOfWeek) {
            showDate = showDate + "  " + ww;
        }
        if (config.withDate) {
            showDate = yy + "-" + MM + "-" + dd + " " + showDate;
        }
        if (config.withColor) {
            showDate = colorhead + showDate + colorfoot;
        }
        return (showDate);
    }

    var myClockHelper = function (systemDate, domId, config) {
        var domIdFix = "myServerClock";
        if (domId) {
            domIdFix = domId;
        }

        var configFix = {
            withColor: false,
            withDayOfWeek: false,
            withDate: false
        };
        if (config != undefined) {
            if (config.withColor) {
                configFix.withColor = config.withColor;
            }
            if (config.withDayOfWeek) {
                configFix.withDayOfWeek = config.withDayOfWeek;
            }
            if (config.withDate) {
                configFix.withDate = config.withDate;
            }
        }

        var diff = systemDate - new Date();
        var tick = function () {
            var today = new Date();
            var jsMilliseconds = today.getTime();
            var fixMilliseconds = jsMilliseconds + diff;
            var fixSystemDate = new Date(fixMilliseconds);
            document.getElementById(domIdFix).innerHTML = showCurrentTime(fixSystemDate, configFix);
        };
        var beginTick = function () {
            tick();
            setTimeout(beginTick, 1000);
        };

        return {
            beginTick: beginTick
        };
    }
    
    var init = function (systemDate, domId, config) {
        var helper = myClockHelper(systemDate, domId, config);
        return helper;
    }

    return {
        init: init
    };
}));