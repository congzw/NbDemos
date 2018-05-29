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

    zqnb.inited = true;
    console.log('>> zqnb inited');
}());
