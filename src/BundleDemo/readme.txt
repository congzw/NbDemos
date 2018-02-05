## Bundles

相对路径问题e.g.：

background-image: url(../../img/sidebar_toggler_icon_darkblue.png);
src: url('../fonts/fontawesome-webfont.eot?v=4.2.0');

解决方案(按层级分类，使用前缀，分类分别进行bundle)：
https://stackoverflow.com/questions/30447468/asp-net-mvc-css-relative-paths-vs-bundles


## Settings

//this will override web.config setting
//BundleTable.EnableOptimizations = true;
<compilation debug="false" targetFramework="4.5" />

## Resources

https://www.codeproject.com/tips/389545/asp-net-mvc4-bundling-and-minification

