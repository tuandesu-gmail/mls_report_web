app.controller('chartCtrl', ['$scope', 'GridService', '$uibModal', function ($scope, GridService, $uibModal) {
    $scope.url = url;
    $scope.menus = [];
    $scope.charts = [];
    $scope.waiting = false;

    $scope.getMenu = function () {
        var params = {};
        GridService.getMenu(params).then(function (result) {
            $scope.menus = result;
        }, function (result) {
        });
    }

    $scope.getCharts = function () {
        $scope.waiting = true;
        //var params = {};
        var params = { chart_id: chart_id, report_id: report_id };
        GridService.getCharts(params).then(function (result) {
            $scope.charts = result;
            
            if (result == undefined || result.length == 0) {
                document.getElementById("chartNotExisting").style.visibility = "visible";
            } else {
                document.getElementById("chartNotExisting").style.visibility = "hidden";
            }
            $scope.waiting = false;
            //document.getElementById("chartAddNewDiv").style.visibility = "visible";
        }, function (result) {
        });
    }

    $scope.getMenu();
    $scope.getCharts();
}]);

app.factory('GridService', ['$q', '$filter', '$timeout', '$http', function ($q, $filter, $timeout, $http) {
    var service = {};

    service.getMenu = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/GetMenu', params)
            .success(function (result) {
                //note, the server passes the information about the data set size
                deferred.resolve(result);
            }).error(function () {
                deferred.reject("error");
            });

        return deferred.promise;
    };

    service.getCharts = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/GetCharts', params)
            .success(function (result) {
                //note, the server passes the information about the data set size
                deferred.resolve(result);
            }).error(function () {
                deferred.reject("error");
            });

        return deferred.promise;
    };

    return service;
}]);
