(function () {
    'use strict';

    var log = function(position, text) {
        console.log('----[ ' + position + ',' + text + ' ]----');
    };

    var index = 0;

    //A
    function demo (foo) {
        log("A", foo);

        //B
        function demo(bar) {
            log("B", bar);
        }

        demo(index++);
    };

    demo(index++);
}());