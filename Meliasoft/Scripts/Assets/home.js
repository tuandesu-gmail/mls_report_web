var scopeHolder;
app.controller('ctrl', ['$scope', 'GridService', '$uibModal', function ($scope, GridService, $uibModal) {
    $scope.url = url;
    $scope.menus = [];
    $scope.charts = [];
    $scope.msgcount = 0;
    $scope.alerts = [];

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
        try {
            if (report_id != undefined) {
                var params = { report_id: report_id }; //var params = {};
                GridService.getCharts(params).then(function (result) {
                    $scope.charts = result;
                    $scope.waiting = false;
                }, function (result) {
                });
            }
            
        } catch (err) {
            alert("home.getCharts() : " + err.message);
        }
        
    }

    $scope.getAlerts = function () {

        $scope.getReportMsgCount();

        try {

            var msgCount = $("#msgCount").text();

            if (msgCount > 0) {
                $("#alertTitle").text("THÔNG BÁO:");
                $scope.waiting = true;

                var params = {};
                GridService.getAlerts(params).then(function (result) {
                    $scope.alerts = result;
                    $("#msgCount").text("0");
                    $scope.waiting = false;
                }, function (result) {
                });
            } else {
                $scope.alerts = [];
                $("#alertTitle").text("Không có thông báo!");
            }
        } catch (err) {
            alert("home.getAlerts() : " + err.message);
        }        
    }

    $scope.getReportMsgCount = function () {
        $scope.waiting = true;

        try {

            var params = {};
          GridService.getReportMsgCount(params).then(function (result) {
            if (result[0] != null && result[0] != undefined) {
              //$scope.msgcount = result.MsgCount;
              //document.getElementById('msgCount').value = 'msgCount = ' + result[0].MsgCount;
              $("#msgCount").text(result[0]._No);

              if (result[0]._No > 0) {
                // $scope.alerts = [];
                $('#envelopeIcon').css('visibility', 'visible');
                $('#envelopeIcon').css('height', '45px');
              } else {
                $('#envelopeIcon').css('visibility', 'hidden');
                $('#envelopeIcon').css('height', '0px');
              }
            } else {
              $('#envelopeIcon').css('visibility', 'hidden');
              $('#envelopeIcon').css('height', '0px');
            }
                

                $scope.waiting = false;
            }, function (result) {
            });
        } catch (err) {
            alert("home.getReportMsgCount() : " + err.message);
        }
    }

    scopeHolder = $scope;
    $scope.getMenu();
    $scope.getCharts();
    $scope.getReportMsgCount();
    
}]);

app.factory('GridService', ['$q', '$filter', '$timeout', '$http', function ($q, $filter, $timeout, $http) {
    var service = {};

    service.getMenu = function (params) {
        try {
            var deferred = $q.defer();
            $http.post(url + '/GetMenu', params)
                .success(function (result) {
                    if (typeof (result.Success) === "undefined" || result.Success) {
                        //note, the server passes the information about the data set size
                        deferred.resolve(result);
                    } else {
                        alert(result.Error);
                    }
                    
                }).error(function () {
                    deferred.reject("error");
            });

            return deferred.promise;
        } catch (err) {
            console.log('report.js: service.getMenu = function (params): catch = ', err.message);
            alert("report.getMenu() : " + err.message);
        }
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

    service.getAlerts = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/GetAlerts', params)
            .success(function (result) {
                //note, the server passes the information about the data set size
                deferred.resolve(result);
            }).error(function () {
                deferred.reject("error");
            });

        return deferred.promise;
    };

    service.getReportMsgCount = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/GetAlertCount', params)
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
