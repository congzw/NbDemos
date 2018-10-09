define(["jquery"], function ($) {
    "use strict";

    var activeNavItem = function(currentActiveId, openAll) {

        //somehow get a reference to our newly clicked(selected) element's parent "LI"

        var $current = $("#" + currentActiveId);
        var new_active = $current.parent();

        if (openAll) {
            //打开所有的菜单
            $('.nav-list li').addClass('open');
        }

        //remove ".active" class from all (previously) ".active" elements
        $('.nav-list li.active').removeClass('active hover');

        //add ".active" class to our newly selected item and all its parent "LI" elements
        new_active.addClass('active open hover').parents('.nav-list li').addClass('active open');

        //you can also update breadcrumbs:
        var breadcrumb_items = [];
        //$(this) is a reference to our clicked/selected element
        $current.parents('.nav-list li').each(function() {
            var link = $(this).find('> a');
            var text = link.text();
            var href = link.attr('href');
            breadcrumb_items.push({ 'text': text, 'href': href });
        });
        //now we have a breadcrumbs list and can replace breadcrumbs area
        return breadcrumb_items;
    };

    var updateBreadcrumbs = function (breadcrumbs) {
        //todo
        console.log('todo updateBreadcrumbs');
        console.log(breadcrumbs);
    };

    return {
        activeNavItem: activeNavItem,
        updateBreadcrumbs: updateBreadcrumbs
    };
});