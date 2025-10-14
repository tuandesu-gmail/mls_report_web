app.controller('ctrl', ['$scope', 'GridService', 'LookupService', '$uibModal', function ($scope, GridService, LookupService, $uibModal) {
    $scope.keyCode = "";
    $scope.keydown = function (e) {
        $scope.keyCode = e.which;
    };

    $scope.title = "";
    $scope.getDataDetail = function (key) {
        var params = { id: key };
        GridService.getDataDetail(params).then(function (result) {
            $scope.model = result.Model;
            if ($scope.model.UID === 0) {
                $scope.title = "Thêm đối tượng";
            } else {
                $scope.title = "Sửa đối tượng";
            }
        }, function (result) {
        });
    }

    $scope.getDataDetail(uid);

    $scope.alert = function (msg) {
        myAlert($scope, $uibModal, msg);
    }

    $scope.lookup = function (path, key, callback, val) {
        var modalInstance = $uibModal.open({
            animation: false,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: '/Scripts/Assets/myLookup.html',
            controller: 'MyLookup',
            controllerAs: '$ctrl',
            //size: size,
            resolve: {
                key: function () {
                    return key;
                },
                parents: function () {
                    return $scope;
                },
                val: function () {
                    return val;
                },
                date: function () {
                    return null;
                }
            }
        });

        modalInstance.result.then(function (item) {
            if (typeof path === "string") {
                eval("$scope." + path + key + " = '" + item.MA_CODE + "'");
            } else {
                path[key] = item.MA_CODE;
                if (typeof callback === "function") {
                    callback(path, item);
                }
            }
        }, function () {
            //$log.info('Modal dismissed at: ' + new Date());
        });
    }

    $scope.lookupBlur = function (path, key, callback) {
        var val;
        if (typeof path === "string") {
            val = eval("$scope." + path + key);
        } else {
            val = path[key];
        }

        if (val !== null && val !== "") {
            var params = { key: key, search: val };
            LookupService.checkDataForLookup(params).then(function (result) {
                if (result.length !== 1) {
                    $scope.lookup(path, key, callback, val);
                } else {
                    if (typeof callback === "function") {
                        callback(path, result[0]);
                    }
                }
            }, function (result) {
            });
        }
    }

    $scope.save = function () {
        var error = false;

        error |= validateField($scope.model.MA_DT, "MA_DT");
        error |= validateField($scope.model.TEN_DT, "TEN_DT");
        error |= validateField($scope.model.MA_NH_DT, "MA_NH_DT");

        if (error) {
            return;
        }

        var params = { model: $scope.model };
        GridService.saveData(params).then(function (result) {
            if (!result.Success) {
                $scope.alert(result.Error);
            } else {
                redirectToIndex();
            }
        }, function (result) {
        });
    }
    $scope.cancel = function () {
        redirectToIndex();
    }
}]);

app.factory('GridService', ['$q', '$filter', '$timeout', '$http', function ($q, $filter, $timeout, $http) {
    var service = {};

    service.getDataDetail = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/GetDataDetailForEdit', params)
            .success(function (result) {
                //note, the server passes the information about the data set size
                deferred.resolve(result);
            }).error(function () {
                deferred.reject("error");
            });

        return deferred.promise;
    };

    service.saveData = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/SaveData', params)
            .success(function (result) {
                //note, the server passes the information about the data set size
                deferred.resolve(result);
            }).error(function (result) {
                deferred.reject("error");
            });

        return deferred.promise;
    };

    return service;
}]);
