var zqnb = zqnb || {};

(function () {
    'use strict';

    if (zqnb.inited) {
        //防止页面重复加载
        return;
    }

    String.prototype.equalIgnoreCase = function (str) {
        return (str != null
                && typeof str === 'string'
                && this.toUpperCase() === str.toUpperCase());
    }
    Array.prototype.contains = function (element) {
        for (var i = 0; i < this.length; i++) {
            if (this[i] === element) {
                return true;
            }
        }
        return false;
    };

    zqnb.inited = true;
    console.log('>> zqnb inited');
}());
