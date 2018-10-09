(function () {
    "use strict";
    require(['ace_const'], function (ace_const) {

        //console.log("enter ace_main => ");
        //console.log(ace_const);
        var rootPath = ace_const.rootPath;
        var contentPath = ace_const.contentPath;
        var acePath = ace_const.acePath;

        require.config({
            baseUrl: rootPath,
            waitSeconds: 2000,
            paths: {
                'jquery': acePath + '/components/jquery/dist/jquery.min'
                , 'bootstrap': acePath + '/components/bootstrap/dist/js/bootstrap.min'
                , 'ace': acePath + '/assets/js/ace'
                , 'ace-elements': acePath + '/assets/js/ace-elements'
                , 'ace-extra': acePath + '/assets/js/ace-extra'
                , 'ace_main': contentPath + '/scripts/ace_main'
                , 'ace_nav': contentPath + '/scripts/ace_nav'
            },
            shim: {
                'bootstrap': {
                    deps: ['jquery']
                },
                'ace': {
                    deps: ['jquery', 'bootstrap']
                },
                'ace-elements': {
                    deps: ['ace']
                }
            }
        });
        
        require(['jquery', 'bootstrap', 'ace', 'ace-extra', 'ace-elements'], function ($) {
            try {
                ace.demo.init(true);//true means the call is not from inside a jQuery document ready event
            } catch (e) { }

            /**
            //you can restore state of elements now (ace-extra.js is loaded)
            $('.ace-save-state').each(function() {
                ace.settings.loadState(this);
            });
    
            //or as soon as you render an element such as sidebar, etc ...
              ace.settings.loadState('sidebar');
            */
        });
    });
})();