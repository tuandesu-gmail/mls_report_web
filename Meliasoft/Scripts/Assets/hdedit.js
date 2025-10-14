app.controller('ctrl', ['$scope', 'GridService', 'LookupService', '$uibModal', function ($scope, GridService, LookupService, $uibModal) {
    $scope.keyCode = "";
    $scope.keydown = function (e) {
        $scope.keyCode = e.which;
    };

    $scope.title = "";

    $scope.rowCollectionPr = [];
    $scope.rowCollectionVt = [];

    $scope.active = 0;

    $scope.dateOptions = {
        formatYear: 'yy',
        maxDate: new Date(2020, 5, 22),
        minDate: new Date(),
        startingDay: 1
    };

    $scope.open1 = function () {
        $scope.popup1.opened = true;
    };

    $scope.format = 'dd/MM/yyyy';
    $scope.altInputFormats = ['d!/M!/yyyy'];

    $scope.popup1 = {
        opened: false
    };

    $scope.getDataDetail = function (key) {
        var params = { id: key };
        GridService.getDataDetail(params).then(function (result) {
            $scope.selectedPr = 0;
            $scope.selectedVt = 0;
            $scope.model = result.Model;
            if ($scope.model.UID === 0) {
                $scope.title = "Thêm hợp đồng";
                $scope.model.NGAY_HD = new Date();
            } else {
                $scope.title = "Sửa hợp đồng";
                $scope.model.NGAY_HD = new Date(parseInt($scope.model.NGAY_HD.substr(6)));
            }

            $scope.rowCollectionPr = result.DataPr;

            $.each(result.DataVt, function () {
                processDVTS(this);
            });

            $scope.rowCollectionVt = result.DataVt;
        }, function (result) {
        });
    }

    $scope.getDataDetail(uid);

    /*setSelected*/
    $scope.setSelected = function (val, row) {
        if ($scope.active === 0) {
            $scope.setSelectedPr(val, row);
        } else {
            $scope.setSelectedVt(val, row);
        }
    }

    $scope.changePr = false;

    $scope.selectedPr = null;
    $scope.setSelectedPr = function (val, row) {
        $scope.selectedPr = val;
        $scope.changePr = false;
        if (typeof row !== "undefined" && row.MA_PR !== null && row.MA_PR !== "") {
            if (val === $scope.rowCollectionPr.length - 1) {
                $scope.addItem(true);
            }
        }
    }

    $scope.selectedVt = null;
    $scope.setSelectedVt = function (val, row) {
        $scope.selectedVt = val;
        if (typeof row !== "undefined" && row.MA_VT !== null && row.MA_VT !== "") {
            if (val === $scope.rowCollectionVt.length - 1) {
                $scope.addItem(true);
            }
        }
    }
    /*setSelected*/

    /*focusLookup*/
    $scope.focusLookup = function (val, row) {
        if ($scope.active === 0) {
            $scope.focusLookupPr(val, row);
        } else {
            $scope.focusLookupVt(val, row);
        }
    }

    $scope.focusLookupPr = function (val, row) {
        $scope.setSelectedPr(val, row);
        //$("table[st-table='displayedCollectionDetail'] tbody tr:eq(" + val + ") td:eq(0) span").show();
    }

    $scope.focusLookupVt = function (val, row) {
        $scope.setSelectedVt(val, row);
        //$("table[st-table='displayedCollectionDetail'] tbody tr:eq(" + val + ") td:eq(0) span").show();
    }
    /*focusLookup*/

    $scope.blurLookupPr = function (val) {
        //$("table[st-table='displayedCollectionDetail'] tbody tr:eq(" + val + ") td:eq(0) span").hide();
    }

    $scope.blurLookupVt = function (val) {
        //$("table[st-table='displayedCollectionDetail'] tbody tr:eq(" + val + ") td:eq(0) span").hide();
    }

    /*changeRow*/
    $scope.changeRow = function (row, col) {
        if ($scope.active === 0) {
            $scope.changeRowPr(row, col);
        } else {
            $scope.changeRowVt(row, col);
        }
    }

    $scope.changeRowPr = function (row, col) {
        $scope.changePr = true;
    }

    $scope.changeRowVt = function (row, col, event) {
        if (typeof col !== "undefined" && col == "DVT") {
        } else {
            row.TIEN = (row.SO_LUONG0 * row.GIA).toFixed(0);
            $scope.totalSum();
        }
    }

    $scope.totalSum = function () {
        var sum = $scope.rowCollectionVt.reduce(function (sum, current) {
            return sum + parseFloat(current.TIEN);
        }, 0);

        $scope.model.TRI_GIA = sum;
    }
    /*changeRow*/

    $scope.blurPr = function (row) {
        if ($scope.changePr && row.MA_PR !== null && row.MA_PR !== "") {
            row.MA_PR = row.MA_PR.toUpperCase();
            var params = { id: row.MA_PR };
            GridService.getDataDetailPr(params).then(function (result) {
                if (result.Success) {
                    var data = result.DataPr;
                    if (data.length > 0) {
                        if ($scope.rowCollectionVt.length === 1 && ($scope.rowCollectionVt[0].MA_VT === null || $scope.rowCollectionVt[0].MA_VT === "")) {
                            $scope.rowCollectionVt.splice(0, 1);
                        }

                        var i = 0;
                        while (i < $scope.rowCollectionVt.length) {
                            if ($scope.rowCollectionVt[i].MA_PR === row.MA_PR) {
                                $scope.rowCollectionVt.splice(i, 1);
                            } else {
                                i++;
                            }
                        }

                        $.each(data, function () {
                            var item = jQuery.extend({
                                MA_PR: row.MA_PR,
                                MA_HD: ""
                            }, this);

                            item.SO_LUONG0 = row.SO_LUONG * item.SO_LUONG0;
                            item.HE_SO0 = row.HE_SO0;
                            item.SO_LUONG = row.SO_LUONG * item.SO_LUONG;
                            item.TIEN = (item.SO_LUONG0 * item.GIA).toFixed(0);

                            processDVTS(item);

                            $scope.rowCollectionVt.push(item);
                        });

                        $scope.totalSum();
                    }
                }
            }, function (result) {
            });
        }
    }

    /*addItem*/
    $scope.addItem = function (notSelect) {
        if ($scope.active === 0) {
            $scope.addItemPr(notSelect);
        } else {
            $scope.addItemVt(notSelect);
        }
    }

    $scope.addItemPr = function (notSelect) {
        $scope.rowCollectionPr.push({
            UID: 0,
            MA_HD: "",
            MA_PR: "",
            TEN_PR: "",
            SO_LUONG: 0
        });

        if (typeof notSelect !== "boolean" || !notSelect) {
            var val = $scope.rowCollectionPr.length - 1;
            $scope.setSelectedPr(val);
        }
        //$("table[st-table='displayedCollectionDetail'] tbody tr:eq(" + val + ")").focus();
    }

    $scope.addItemVt = function (notSelect) {
        $scope.rowCollectionVt.push({
            UID: 0,
            MA_HD: "",
            MA_PR: "",
            MA_VT: "",
            TEN_VT: "",
            DVT: "",
            SO_LUONG0: 0,
            HE_SO0: 1,
            SO_LUONG: 0,
            GIA: 0,
            GIA_NT: 0,
            TIEN: 0,
            TIEN_NT: 0
        });

        if (typeof notSelect !== "boolean" || !notSelect) {
            var val = $scope.rowCollectionVt.length - 1;
            $scope.setSelectedVt(val);
        }
        //$("table[st-table='displayedCollectionDetail'] tbody tr:eq(" + val + ")").focus();
    }
    /*addItem*/

    /*removeItem*/
    $scope.removeItem = function (index) {
        if ($scope.active === 0) {
            $scope.removeItemPr(index);
        } else {
            $scope.removeItemVt(index);
        }
    }

    $scope.removeItemPr = function (index) {
        $scope.rowCollectionPr.splice(index, 1);
    }

    $scope.removeItemVt = function (index) {
        $scope.rowCollectionVt.splice(index, 1);
        $scope.totalSum();
    }
    /*removeItem*/

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
                    return $scope.model.NGAY_HD;
                },
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
            var params = { key: key, search: val, date: $scope.model.NGAY_HD };
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

    $scope.callbackPr = function (row, item) {
        row.TEN_PR = item.TEN_CODE;
    }

    $scope.callbackVt = function (row, item) {
        row.TEN_VT = item.TEN_CODE;
        if (row.DVT === null || row.DVT == "") {
            processDVTS(item);
            row.DVTS = item.DVTS;

            row.DVT = item.DVT;
        }
        if (row.GIA === null || row.GIA == 0) {
            row.GIA = item.GIA_BAN;
            $scope.changeRowVt(row);
        }
    }

    $scope.save = function () {
        var error = false;

        error |= validateField($scope.model.MA_HD, "MA_HD");
        error |= validateField($scope.model.TEN_HD, "TEN_HD");
        error |= validateField($scope.model.MA_VV, "MA_VV");

        if (error) {
            return;
        }

        var params = { model: $scope.model, dataPr: $scope.rowCollectionPr, dataVt: $scope.rowCollectionVt };
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

    $scope.displayedCollectionPr = [].concat($scope.rowCollectionPr);
    $scope.displayedCollectionVt = [].concat($scope.rowCollectionVt);
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

    service.getDataDetailPr = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/GetDataDetailPr', params)
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
