app.controller('ctrl', ['$scope', 'GridService', '$uibModal', function ($scope, GridService, $uibModal) {
    $scope.keyCode = "";
    $scope.keydown = function (e) {
        $scope.keyCode = e.which;
    };

    $scope.rowCollectionMaster = [];
    $scope.rowCollectionDetail = [];

    $scope.getDataMaster = function () {
        var params = {};
        GridService.getDataMaster(params).then(function (result) {
            $.each(result, function () {
                this.NGAY_HD = new Date(parseInt(this.NGAY_HD.substr(6)));
            });
            $scope.rowCollectionMaster = result;
            if (result.length > 0) {
                $scope.selectedMaster = result.length - 1;
            }
        }, function (result) {
        });
    }

    $scope.doComplete = function () {
        var container = $("#master")
        var scrollTo = $("#master table tbody tr:eq(" + $scope.selectedMaster + ")");
        container.scrollTop(scrollTo.offset().top - container.offset().top + container.scrollTop());
    }

    $scope.getDataDetail = function (key) {
        var params = { key: key };
        GridService.getDataDetail(params).then(function (result) {
            $scope.selectedDetail = 0;
            $scope.rowCollectionDetail = result;
        }, function (result) {
        });
    }

    $scope.$watch('selectedMaster', function () {
        if ($scope.selectedMaster < $scope.rowCollectionMaster.length) {
            $scope.getDataDetail($scope.rowCollectionMaster[$scope.selectedMaster].MA_HD);
        }
    });

    $scope.getDataMaster();

    $scope.selectedMaster = null;
    $scope.setSelectedMaster = function (val) {
        $scope.selectedMaster = val;
    }

    $scope.selectedDetail = null;
    $scope.setSelectedDetail = function (val) {
        $scope.selectedDetail = val;
    }

    $scope.newItem = function () {
        window.location = getLinkEdit();
    }

    $scope.editItem = function () {
        if ($scope.selectedMaster !== null && $scope.selectedMaster >= 0 && $scope.selectedMaster < $scope.displayedCollectionMaster.length) {
            var id = $scope.displayedCollectionMaster[$scope.selectedMaster].UID;
            window.location = getLinkEdit() + "/" + id;
        }
    }

    $scope.deleteItem = function () {
        if ($scope.selectedMaster !== null && $scope.selectedMaster >= 0 && $scope.selectedMaster < $scope.displayedCollectionMaster.length) {
            if (!confirm("Are you sure to delete this row?")) {
                return;
            }

            var row = $scope.displayedCollectionMaster[$scope.selectedMaster];
            var params = { key: row.UID };
            GridService.deleteData(params).then(function (result) {
                if (result.Success) {
                    $scope.getDataMaster();
                } else {
                    alert("Error");
                }
            }, function (result) {
            });
        }
    }

    $scope.dblclick = function (index) {
        $scope.selectedMaster = index;
        $scope.editItem();
    }

    $scope.displayedCollectionMaster = [].concat($scope.rowCollectionMaster);
    $scope.displayedCollectionDetail = [].concat($scope.rowCollectionDetail);
}]);

app.factory('GridService', ['$q', '$filter', '$timeout', '$http', function ($q, $filter, $timeout, $http) {
    var service = {};

    service.getDataMaster = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/GetDataMaster', params)
            .success(function (result) {
                //note, the server passes the information about the data set size
                deferred.resolve(result);
            }).error(function () {
                deferred.reject("error");
            });

        return deferred.promise;
    };

    service.getDataDetail = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/GetDataDetail', params)
            .success(function (result) {
                //note, the server passes the information about the data set size
                deferred.resolve(result);
            }).error(function () {
                deferred.reject("error");
            });

        return deferred.promise;
    };

    service.deleteData = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/DeleteData', params)
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
