define(function () {
    var zqnb = {};

    zqnb.helloworld = function () {
        var msg = "hello world from zqnb";
        return msg;
    };

    zqnb.sayHi = function (msg) {
        console.log(msg);
    };

    //extensions for raw javascripts
    var subString = function (str, start, len, suffix) {
        if (!str) {
            return "";
        }
        var l = 0;
        var a = str.split("");
        for (var i = 0; i < a.length; i++) {
            if (a[i].charCodeAt(0) < 299) {
                l++;
            } else {
                l += 2;
            }
            if (l > len) {
                str = str.substring(0, i);
                break;
            }
        }
        if (!!suffix) {
            str += suffix;
        }
        return str + "";

    };
    String.prototype.subString = function (start, len, suffix) {
        return subString(this, start, len, suffix);
    };

    return zqnb;
});
