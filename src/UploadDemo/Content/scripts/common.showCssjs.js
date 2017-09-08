(function($) {
    $(function() {

        var cssJsInfo = "";
        cssJsInfo += "<h2>load scripts</h2>";
        $("script").each(function () {
            cssJsInfo += "<p>" + this.src + "</p>";
        });

        cssJsInfo += "<h2>load styles</h2>";
        $("link").each(function() {
            cssJsInfo += "<p>" + this.href + "</p>";
        });
        cssJsInfo += "<h2>load imgs</h2>";
        cssJsInfo += "<p>bitch.jpg</p>";
        $('#_showCssjs').html(cssJsInfo);
    });

}(jQuery));