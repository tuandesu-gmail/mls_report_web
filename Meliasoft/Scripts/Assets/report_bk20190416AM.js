app.controller('ctrl', ['$scope', 'GridService', '$uibModal', '$http', function ($scope, GridService, $uibModal, $http) {
    //for DatePicker
    $scope.dateOptions = {
        formatYear: 'yy',
        startingDay: 1
    };

    $scope.format = 'dd/MM/yyyy';
    $scope.altInputFormats = ['d!/M!/yyyy'];

    $scope.popup = popup;

    $scope.openPopup = function (index) {
        $scope.popup[index] = true;
    };
    //for DatePicker

    $scope.waiting = false;
    $scope.errmsg = '';

    //for Kendo Grid
    $scope.mainGridOptions = {
        dataSource: {
            transport: {
                //read: "http://localhost:5000/Home/Test",
                dataType: "json"
            }
        },
        sortable: false,
        columns: dummy,
        //editable: true,
        dataBound: function (e) {
            //this.expandRow(this.tbody.find("tr.k-master-row").first());

            var objThis = this;
            angular.forEach(fields, function (value, index) {
                value.index = objThis.wrapper.find(".k-grid-header [data-field=" + value.field + "]").index();
            });

            var dataItems = e.sender.dataSource.view();
            for (var j = 0; j < dataItems.length; j++) {
                var bold = dataItems[j].Bold;

                var row = e.sender.tbody.find("[data-uid='" + dataItems[j].uid + "']");

                if (bold == "C") {
                    row.addClass("bold");

                    if (e.sender.lockedContent) {
                        row = e.sender.lockedContent.find("[data-uid='" + dataItems[j].uid + "']");
                        row.addClass("bold");
                    }
                }

                var bColor = dataItems[j].BColor;
                if (bColor !== undefined) {
                    row.css("background-color", bColor);
                }

                //angular.forEach(fields, function (value, index) {
                //    var cell = row.children().eq(value.index);
                //    if (cell[0].innerText == "0") {
                //        cell[0].innerText = "";
                //    }
                //    if (cell[0].innerText == "01/01/1900") {
                //        cell[0].innerText = "";
                //    }
                //});
            }
        }
    };
    //for Kendo Grid

    $scope.param = param;

    function dateFormatDMY(inputDate) {
        var month = '' + (inputDate.getMonth() + 1),
            day = '' + inputDate.getDate(),
            year = inputDate.getFullYear();

        if (month.length < 2) month = '0' + month;
        if (day.length < 2) day = '0' + day;

        return [day, month, year].join('/');
    }

    angular.forEach($scope.param, function (value, index) {
        if (value.Value && value.Value.startsWith("/Date(")) {
            //$scope.param[index].Value = new Date(parseInt(value.Value.substr(6)));
            var d = new Date(parseInt(value.Value.substr(6)));

            //var month = '' + (d.getMonth() + 1),
            //    day = '' + d.getDate(),
            //    year = d.getFullYear();

            //if (month.length < 2) month = '0' + month;
            //if (day.length < 2) day = '0' + day;

            $scope.param[index].Value = dateFormatDMY(d); // [day, month, year].join('/');
        }

        //var val = $.cookie('meliasoft_param_' + id + '_' + value.Name);
        //if (typeof val !== "undefined") {
        //    if (value.Name.startsWith("_Ngay") || value.Value && value.Value.startsWith("/Date(")) {
        //        //$scope.param[index].Value = new Date(val);
        //        var d2 = new Date(val);

        //        //var month2 = '' + (d2.getMonth() + 1),
        //        //    day2 = '' + d2.getDate(),
        //        //    year2 = d2.getFullYear();

        //        //if (month2.length < 2) month2 = '0' + month2;
        //        //if (day2.length < 2) day2 = '0' + day2;

        //        $scope.param[index].Value = dateFormatDMY(d2); //[day2, month2, year2].join('/');
        //    } else {
        //        //if (val === "true") {
        //        //    $scope.param[index].Value = "1";
        //        //} else if (val === "false") {
        //        //    $scope.param[index].Value = "0";
        //        //} else {
        //        //    $scope.param[index].Value = val;
        //        //}
        //        $scope.param[index].Value = val;
        //    }
        //}
    });

    $scope.menus = [];

    $scope.getMenu = function () {
        var params = {};
        GridService.getMenu(params).then(function (result) {
            $scope.menus = result;
            if ($scope.menus == null || $scope.menus.length == 0) {
                $scope.errmsg = 'Có lỗi hệ thống, hãy kiểm tra kết nối đến server!';
            }
            
        }, function (result) {
        });
    };
    $scope.getMenu();

    $scope.refreshData = function () {
        $scope.waiting = true;

        //angular.forEach($scope.param, function (value, index) {
        //    $.cookie('meliasoft_param_' + id + '_' + value.Name, value.Value);
        //});

        //if (changed) {
        //    $scope.setData();
        //}

        $scope.getData();

        $(".rpt-filter-pane").removeClass("active");
    };

    $scope.rowCollection = [];

    $scope.check = false;
    $scope.checkAll = function () {
        angular.forEach($scope.rowCollection, function (value, index) {
            value.IsCheck = $scope.check;
        });
    };

    $scope.getData = function () {
        $scope.waiting = true;

        var cloned = [].concat($scope.param);
        angular.forEach(cloned, function (value, index) {
            if (typeof (value.Value) === "undefined") {
                value.Value = "";
            } else if (value.Value && typeof (value.Value.title) !== "undefined") {
                value.Value = value.Value.title;
            }
        });

        var params = { id: id, param: cloned };
        GridService.getData(params).then(function (result) {
            if (typeof (result.Success) === "undefined") {
                angular.forEach(result, function (value, index) {
                    //value.IsCheck = false;
                    value.IsModify = false;
                });

                $scope.rowCollection = result;
                if (result.length > 0) {
                    $scope.selected = 0;
                }

                $("#grid").data('kendoGrid').dataSource.data([]);

                //alert("ishaslockedcolumn = " + ishaslockedcolumn);

                
                if (ishaslockedcolumn == 1) {
                    var grid = $("#grid").data("kendoGrid");
                    var dataSource = new kendo.data.DataSource({ data: result });
                    grid.setDataSource(dataSource);
                    grid.dataSource.read();
                } else {
                    // New code for Auto loading when scroll
                    var gridElement = $("#grid")
                    var pagingIncrement = 20; //new
                    var scrollbarWidth = kendo.support.scrollbar();
                    var dataBindingFlag = true;

                    gridElement.kendoGrid({
                        dataSource: {
                            type: "json",
                            //transport: { 
                            //    read: "http://demos.kendoui.com/service/Northwind.svc/Orders"
                            //},
                            data: result,
                            //total: function (data) {
                            //    return data.data.length;
                            //},
                            schema: {
                                model: {
                                    //fields: {
                                    //    OrderID: { type: "number" },
                                    //    Freight: { type: "number" },
                                    //    ShipName: { type: "string" },
                                    //    OrderDate: { type: "date" }
                                    //}
                                    fields: dummy
                                }
                            },
                            pageSize: pagingIncrement,

                            serverPaging: false
                        },
                        //navigatable: true,
                        //pageable: true,
                        pageable: {
                            refresh: false,
                            previousNext: true,
                            numeric: false
                        },
                        scrollable: { virtual: true },

                        //dataBound: function () {
                        //    dataBindingFlag = true;
                        //},
                        dataBound: function (e) {
                            dataBindingFlag = true;
                            //e.sender.pager.element.hide();
                            //e.sender.pager.element.css = "height:0px;";
                            //e.sender.pager.element.offsetHeight = 0;
                            //e.sender.pager.element.scrollHeight = 0;


                            //if (e.sender.dataSource.total() <= e.sender.dataSource.pageSize()) {
                            //    e.sender.pager.element.hide();
                            //} else {
                            //    e.sender.pager.element.show();
                            //}   

                            var dataItems = e.sender.dataSource.view();
                            for (var j = 0; j < dataItems.length; j++) {
                                var bold = dataItems[j].Bold;

                                var row = e.sender.tbody.find("[data-uid='" + dataItems[j].uid + "']");

                                if (bold == "C") {
                                    row.addClass("bold");

                                    if (e.sender.lockedContent) {
                                        row = e.sender.lockedContent.find("[data-uid='" + dataItems[j].uid + "']");
                                        row.addClass("bold");
                                    }
                                }

                                var bColor = dataItems[j].BColor;
                                if (bColor !== undefined) {
                                    row.css("background-color", bColor);
                                }

                                //angular.forEach(fields, function (value, index) {
                                //    var cell = row.children().eq(value.index);
                                //    if (cell[0].innerText == "0") {
                                //        cell[0].innerText = "";
                                //    }
                                //    if (cell[0].innerText == "01/01/1900") {
                                //        cell[0].innerText = "";
                                //    }
                                //});
                            }
                        },

                        //columns: [{
                        //    field: "OrderID"
                        //}, {
                        //    field: "Freight"
                        //}, {
                        //    field: "OrderDate",
                        //    title: "Order Date",
                        //    format: "{0:MM/dd/yyyy}"
                        //}, {
                        //    field: "ShipName",
                        //    title: "Ship Name"
                        //}]
                        //columns: '@ViewBag.Dummy'
                        columns: dummy //dummy //fields
                    });

                    //var gridDataSource = gridElement.data("kendoGrid").dataSource;

                    //gridElement.children(".k-grid-content")
                    //    .on("scroll", function (e) {
                    //        if (dataBindingFlag) {
                    //            var dataDiv = e.target;
                    //            var currentPageSize = gridDataSource.pageSize();
                    //            if (dataDiv.scrollTop >= dataDiv.scrollHeight - dataDiv.offsetHeight - scrollbarWidth && gridDataSource.total() > currentPageSize) {
                    //                dataBindingFlag = false;
                    //                gridDataSource.pageSize(currentPageSize + pagingIncrement);
                    //            }
                    //        }
                    //    });

                }
                

                

                $scope.waiting = false;
            } else {
                $scope.waiting = false;
                alert(result.Error);
            }
        }, function (result) {
        });
    };

    $scope.getData();

    $scope.setData = function () {
        var grid = $("#grid").data("kendoGrid");
        var dataItem = grid.dataSource.data();

        var checkedRows = $("#checkedRows").val();

        ////var grid = $("#grid").data("kendoGrid");
        ////var lockedRow = grid.lockedTable.find("tr").eq(finder.index());

        ////lockedRow.find(":checkbox").prop("checked", true);

        //var finder = $(this).parent(); // the clicked row element

        ////var grid = $("#grid").data("kendoGrid"); // get reference to the Grid widget

        //var lockedRow = grid
        //    .k-grid-content-locked // reference to the locked table
        //    .find("tr").eq(finder.index()); // finds the row with the same index as the one clicked (the finder)

        //// finds the checkbox inside the row and set it as checked
        //// here we assume that the checkbox is always inside the locked portion of the grid
        //lockedRow.find(":checkbox").prop("checked", true);


        var changed = false;
        for (var idx = 0; idx < dataItem.length; idx++) {
            if (dataItem[idx].dirty || (checkedRows.indexOf("," + idx) >= 0)) {
                dataItem[idx].IsCheck = true;
                dataItem[idx].IsModify = true;
                changed = true;
                //break;
            }
        }

        if (changed) { //if (true || changed) {
            var params = { id: id, param: dataItem, changed: changed };
            //var params = { id: id, param: $scope.rowCollection };
            GridService.setData(params).then(function (result) {
                if (result.Success) {
                    alert("Success");
                } else {
                    alert(result.Error);
                }
            }, function (result) {
            });
        }
    }

    $scope.selected = null;
    $scope.setSelected = function (val) {
        $scope.selected = val;
    };

    $scope.displayedCollection = [].concat($scope.rowCollection);

    $scope.searchAPI = function (idField, userInputString, timeoutPromise) {
        return $http.post(url + '/GetLookup', { id: idField, value: userInputString }, { timeout: timeoutPromise });
    };

    $scope.alert = function (msg) {
        myAlert($scope, $uibModal, msg);
    };
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

    service.getData = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/GetData', params)
            .success(function (result) {
                //note, the server passes the information about the data set size
                deferred.resolve(result);
            }).error(function () {
                deferred.reject("error");
            });

        return deferred.promise;
    };

    service.setData = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/SetData', params)
            .success(function (result) {
                //note, the server passes the information about the data set size
                deferred.resolve(result);
            }).error(function () {
                deferred.reject("error");
            });

        return deferred.promise;
    };

    service.getLookup = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/GetLookup', params)
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
