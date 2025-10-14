jQuery(window).load(function () {
    jQuery('.scrollbar-inner').scrollbar();
    jQuery('.scrollbar-inner').on("scroll", function (o) {
        console.log(o);
    });
});

var app = angular.module('app', ['ui.bootstrap', 'smart-table', /*'blockUI',*/ 'ngTouch', 'angucomplete', 'fcsa-number', 'angucomplete-alt', 'kendo.directives']);

//app.config(["blockUIConfig", function (blockUIConfig) {
//    // Change the default overlay message
//    blockUIConfig.message = 'Đang thực hiện...';

//    // Change the default delay to 100ms before the blocking is visible
//    blockUIConfig.delay = 100;
//}]);
