var zqnb = zqnb || {};
(function () {
    'use strict';

    if (zqnb.mainApp) {
        //防止页面重复加载
        return;
    }

    zqnb.mainApp = angular.module('mainApp', []);
    var mainApp = zqnb.mainApp;

    console.log('>> zqnb mainApp inited');
}());