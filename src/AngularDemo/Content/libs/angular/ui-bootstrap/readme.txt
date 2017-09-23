

https://github.com/angular-ui/bootstrap
http://www.bootcdn.cn/angular-ui-bootstrap/

UI Bootstrap 1.0 and higher requires Angular 1.4.x or higher and it has been tested with Angular 1.4.8.
UI Bootstrap 0.14.3 is the last version that supports Angular 1.3.x.
UI Bootstrap 0.12.0 is the last version that supports Angular 1.2.x.


Angular-animate (the version should match with your angular's, tested with 1.6.1) if you plan in using animations, you need to load angular-animate as well.
Angular-touch (the version should match with your angular's, tested with 1.6.1) if you plan in using swipe actions, you need to load angular-touch as well.
Bootstrap CSS (tested with version 3.3.7). This version of the library (2.5.0) works only with Bootstrap CSS in version 3.x. 0.8.0 is the last version of this library that supports Bootstrap CSS in version 2.3.x.

Installation
As soon as you've got all the files downloaded and included in your page you just need to declare a dependency on the ui.bootstrap module:
angular.module('myModule', ['ui.bootstrap']);