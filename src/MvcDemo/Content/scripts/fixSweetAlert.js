(function () {
    "use strict";

    var tryMaxCount = 10;
    var tryCount = 0;
    var log = function (msg) {
        //for debug
        //console.log(msg);
    }

    var loadingPluginScripts = false;
    var loadPluginScripts = function () {
        loadingPluginScripts = true;
        var newscript = document.createElement('script');
        newscript.type = 'text/javascript';
        newscript.async = true;
        newscript.src = '/Content/libs/SweetAlert/sweetalert2.js';
        (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(newscript);
    }

    function checkPlugin() {
        tryCount++;
        log('try loading plugin:' + tryCount + '/' + tryMaxCount);
        if (tryCount > tryMaxCount) {
            log('loadPlugin fail: swal');
            if (!loadingPluginScripts) {
                loadPluginScripts();
            }
        }
        if (window.swal) {
            pluginLoaded();
        }
        else {
            window.setTimeout(checkPlugin, 100);
        }
    }
    checkPlugin();

    function pluginLoaded() {
        log('plugin load suceess: swal');

        window.old_alert = window.alert;
        window.alert = function (message) {
            swal(message);
        };
    }
}());