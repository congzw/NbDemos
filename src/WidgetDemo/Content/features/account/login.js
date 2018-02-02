(function ($) {
    'use strict';
    var canvs$ = $(".layui-canvs");
    canvs$.width($(window).width());
    canvs$.height($(window).height());
    canvs$.jParticle({
        background: "#141414",
        color: "#E6E6E6"
    });
}(jQuery));