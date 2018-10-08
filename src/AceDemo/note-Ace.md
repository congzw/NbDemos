#Ace

## Ace Files


- ace.js 包含网站的主要功能事件处理，例如边框条，盒子容器等。
- ace-elements.js 包含自定义插件的包装，例如输入框，滚动条，选色板，向导，树控件，文本编辑器等（当然也应该包含相应的插件）。
- ace-extra.js 包含cookie和localStorage等用户设置的存取，边框条的状态和页面设置等。
- x-editable/ace-editable.js包含了五个额外的在线编辑插件：date, slider, spinner, wysiwyg and image.


其中，ace.js和ace-elements.js是由Javascript builder合成，包含：assets/js/src的下列文件：

- ace.js
- ace.sidebar.js
- ace.widget-box.js
- elements.scroller.js
- elements.fileinput.js
- ...


## Layout

1) Navbar
2) Sidebar
3) Breadcrumbs (inside "main-content")
4) Page content (inside "main-content")
5) Settings box (inside "page-content")
6) Footer

