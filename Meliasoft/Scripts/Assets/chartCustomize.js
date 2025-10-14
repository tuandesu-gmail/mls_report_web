app.controller('chartCtrl', ['$scope', 'GridService', '$uibModal', '$http', function ($scope, GridService, $uibModal, $http) {
    //for DatePicker
    $scope.dateOptions = {
        formatYear: 'yy',
        startingDay: 1
    };

    $scope.format = 'dd/MM/yyyy';
    $scope.altInputFormats = ['d!/M!/yyyy'];

    //$scope.popup = popup;

    //$scope.openPopup = function (index) {
    //    $scope.popup[index] = true;
    //};
    //for DatePicker

    $scope.waiting = false;
    $scope.errmsg = '';

    //for Kendo Grid - Columns - start
    $scope.mainGridOptions_Columns = {
        dataSource: {
            transport: {
                //read: "http://localhost:5000/Home/Test",
                dataType: "json"
            }
        },
        sortable: false,
        columns: chartColumns_Dummy,
        //editable: true,
        dataBound: function (e) {
            //this.expandRow(this.tbody.find("tr.k-master-row").first());

            var objThis = this;
            angular.forEach(chartColumns_Fields, function (value, index) {
                value.index = objThis.wrapper.find(".k-grid-header [data-field=" + value.field + "]").index();
                //if (value.field !== "HeaderText") {
                //    this.hideColumn(value.field);
                //}
            });

            //$("#gridSelectColumns tbody").on("click", "tr", function (e) {
            //    var rowElement = this;
            //    var row = $(rowElement);
            //    //var grid = $("#gridSelectColumns").getKendoGrid();
            //    if (row.hasClass("k_state_selected_tmp")) {
            //        row.removeClass('k_state_selected_tmp');
            //        row.find('[type=checkbox]').prop('checked', false);
            //        $('.checkAllCls').prop('checked', false);
            //        //e.stopPropagation();
            //    } else {
            //        //grid.select(row)
            //        row.find('[type=checkbox]').prop('checked', true);
            //        //e.stopPropagation();
            //    }
            //});

            ////-----------
            //$(document).on("click", "#checkAll", function () {
            //    $("#InverterGrid tbody input:checkbox").attr("checked", this.checked);
            //});

            //----------- for "gridSelectColumns" grid -------------
            $(".checkAllCls").on("click", function () {
                var ele = this;
                var state = $(ele).is(':checked');
                var grid = $('#gridSelectColumns').data('kendoGrid');
                if (state == true) {
                    $('.check-box-inner').prop('checked', true).closest('tr').addClass('k_state_selected_tmp'); //k-state-selected
                    //$('.check-box-inner').prop('checked', true);
                }
                else {
                    $('.check-box-inner').prop('checked', false).closest('tr').removeClass('k_state_selected_tmp'); //k-state-selected
                    //$('.check-box-inner').prop('checked', false);
                }
            });  

            $(".check-box-inner").on("click", function () {
                var ele = this;
                var state = $(ele).is(':checked');
                if (!state) {
                    $('.checkAllCls').prop('checked', false);
                }
            });  

            //----------- for "gridSelectRows" grid -------------
            $(".checkAllRowsCls").on("click", function () {
                var ele = this;
                var state = $(ele).is(':checked');
                var grid = $('#gridSelectRows').data('kendoGrid');
                if (state == true) {
                    $('.check-box-inner-rows').prop('checked', true).closest('tr').addClass('k_state_selected_tmp'); //k-state-selected
                    //$('.check-box-inner').prop('checked', true);
                }
                else {
                    $('.check-box-inner-rows').prop('checked', false).closest('tr').removeClass('k_state_selected_tmp'); //k-state-selected
                    //$('.check-box-inner').prop('checked', false);
                }
            });

            $(".check-box-inner-rows").on("click", function () {
                var ele = this;
                var state = $(ele).is(':checked');
                if (!state) {
                    $('.checkAllRowsCls').prop('checked', false);
                }
            });  


        }
    };
    //for Kendo Grid - Columns - end

    //for Kendo Grid - Columns - start
    $scope.mainGridOptions_Rows = {
        dataSource: {
            transport: {
                //read: "http://localhost:5000/Home/Test",
                dataType: "json"
            }
        },
        sortable: false,
        columns: chartRows_Dummy,
        //editable: true,
        dataBound: function (e) {
            //this.expandRow(this.tbody.find("tr.k-master-row").first());

            var objThis = this;
            angular.forEach(chartRows_Fields, function (value, index) {
                value.index = objThis.wrapper.find(".k-grid-header [data-field=" + value.field + "]").index();
                //if (value.field !== "HeaderText") {
                //    this.hideColumn(value.field);
                //}
            });


        }
    };
    //for Kendo Grid - Columns - end

    //$scope.param = param;

    function dateFormatDMY(inputDate) {
        var month = '' + (inputDate.getMonth() + 1),
            day = '' + inputDate.getDate(),
            year = inputDate.getFullYear();

        if (month.length < 2) month = '0' + month;
        if (day.length < 2) day = '0' + day;

        return [day, month, year].join('/');
    }

    //angular.forEach($scope.param, function (value, index) {
    //    if (value.Value && value.Value.indexOf("/Date(") == 0) {//if (value.Value && value.Value.startsWith("/Date(")) {
    //        //$scope.param[index].Value = new Date(parseInt(value.Value.substr(6)));
    //        var d = new Date(parseInt(value.Value.substr(6)));

    //         $scope.param[index].Value = dateFormatDMY(d); // [day, month, year].join('/');
    //    }

    //});

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


        $scope.getChartColumns();
        $scope.getChartRows();
        $scope.getChartTypes();

        $(".rpt-filter-pane").removeClass("active");
    };

    //$scope.rowCollection = [];
    $scope.ChartTypes = [];

    $scope.check = false;
    //$scope.checkAll = function () {
    //    angular.forEach($scope.rowCollection, function (value, index) {
    //        value.IsCheck = $scope.check;
    //    });
    //};

    //$scope.getChartInfo = function () {
    //    $scope.waiting = true;
    //    //var params = {};
    //    var params = { chart_id: chart_id, report_id: report_id };
    //    GridService.getChartInfo(params).then(function (result) {
    //        $scope.charts = result;

    //        $scope.waiting = false;
    //        //document.getElementById("chartAddNewDiv").style.visibility = "visible";
    //    }, function (result) {
    //    });
    //}

    $scope.getChartColumns = function () {
        $scope.waiting = true;
        //var previousSelectedColumns = "," + document.getElementById('checkedChartColumns').value + ",";

        try {

            var params = { chart_id: chart_id, report_id: report_id };
            GridService.getChartColumns(params).then(function (result) {
                if (typeof (result.Success) === "undefined") {
                    angular.forEach(result, function (value, index) {
                        value.IsCheck = false;
                        var idx = index + 1;
                        if (value.SELECTED > 0) {
                            value.IsCheck = true;
                            //} else if (previousSelectedColumns !== undefined && previousSelectedColumns.indexOf("," + idx + ",") >= 0) {
                            //    value.IsCheck = true;
                        }
                        //value.IsModify = false;
                    });


                    //$scope.rowCollection = result;
                    //if (result.length > 0) {
                    //    $scope.selected = 0;
                    //}

                    $("#gridSelectColumns").data('kendoGrid').dataSource.data([]);


                    var grid = $("#gridSelectColumns").data("kendoGrid");
                    var dataSource = new kendo.data.DataSource({ data: result });
                    grid.setDataSource(dataSource);
                    grid.dataSource.read();


                    $scope.waiting = false;
                } else {
                    $scope.waiting = false;
                    alert(result.Error);
                }
            }, function (result) {
            });
        } catch (err) {
            alert("chartCustomize.getChartColumns() : " + err.message);
        } 

    };

    $scope.getChartRows = function () {
        $scope.waiting = true;

        try {
            var previousSelectedRows = "," + document.getElementById('checkedChartRows').value + ",";
            var params = { chart_id: chart_id, report_id: report_id };
            GridService.getChartRows(params).then(function (result) {
                if (typeof (result.Success) === "undefined") {

                    angular.forEach(result, function (value, index) {
                        value.IsCheck = false;
                        var idx = index + 1;
                        if (previousSelectedRows !== undefined && previousSelectedRows.indexOf("," + idx + ",") >= 0) {
                            value.IsCheck = true;
                        }
                        //value.IsModify = false;
                    });


                    //$scope.rowCollection = result;
                    //if (result.length > 0) {
                    //    $scope.selected = 0;
                    //}

                    $("#gridSelectRows").data('kendoGrid').dataSource.data([]);


                    var grid = $("#gridSelectRows").data("kendoGrid");
                    var dataSource = new kendo.data.DataSource({ data: result });
                    grid.setDataSource(dataSource);
                    grid.dataSource.read();


                    $scope.waiting = false;
                } else {
                    $scope.waiting = false;
                    alert(result.Error);
                }
            }, function (result) {
            });
        } catch (err) {
            alert("chartCustomize.getChartRows() : " + err.message);
        }
        
    };

    $scope.getChartTypes = function () {
        $scope.waiting = true;

        try {
            var selectedChartTypeId = $("#selectedTypeId").val();

            var params = { selected_chart_type_id: selectedChartTypeId };
            GridService.getChartTypes(params).then(function (result) {
                if (typeof (result.Success) === "undefined") {

                    var chartTypes_Options = [];
                    //var chartTypes_SelectedOption = {};
                    var chartIdx = 0;

                    angular.forEach(result, function (value, index) {
                        chartTypes_Options.push(value);
                        if (selected_chart_type_id == value.TypeId) {
                            //chartTypes_SelectedOption = value;
                            $scope.currentIndex = chartIdx;
                        }
                        chartIdx++;
                    });

                    document.getElementById('selectedChartIndex').textContent = $scope.currentIndex + 1;

                    $scope.slides = chartTypes_Options;
                    //$scope.ChartTypes = {
                    //    availableOptions: chartTypes_Options,
                    //    selectedOption: chartTypes_SelectedOption //This sets the default value of the select in the ui
                    //};

                    $scope.waiting = false;
                } else {
                    $scope.waiting = false;
                    alert(result.Error);
                }
            }, function (result) {
            });
        } catch (err) {
            alert("chartCustomize.getChartTypes() : " + err.message);
        }
        
    };

    $scope.direction = 'left';
    $scope.currentIndex = 0;

    $scope.setCurrentSlideIndex = function (index) {
        $scope.direction = (index > $scope.currentIndex) ? 'left' : 'right';
        $scope.currentIndex = index;
        document.getElementById('selectedChartIndex').textContent = $scope.currentIndex + 1;
    };

    $scope.isCurrentSlideIndex = function (index) {
        return $scope.currentIndex === index;
    };

    $scope.prevSlide = function () {
        $scope.direction = 'left';
        $scope.currentIndex = ($scope.currentIndex < $scope.slides.length - 1) ? ++$scope.currentIndex : 0;

        document.getElementById('selectedChartIndex').textContent = $scope.currentIndex + 1;
    };

    $scope.nextSlide = function () {
        $scope.direction = 'right';
        $scope.currentIndex = ($scope.currentIndex > 0) ? --$scope.currentIndex : $scope.slides.length - 1;

        document.getElementById('selectedChartIndex').textContent = $scope.currentIndex + 1;
    };

    function ChangeDataBindingFlag() {

        dataBindingFlag = true;
    }

    var onActivate = function (e) {
        // Get main object to get grid
        var oTab = window[JSTabsCurrent]["getTab"]();
        // Onglet 0 ?
        if (oTab.oTabs.select().index() == 0) {
            // Just resize then call scrollSunc
            oTab.oGrid.resize(true);
            oTab.oFuns.scrollSync();

        }
    }

    $scope.getChartColumns();
    $scope.getChartRows();
    $scope.getChartTypes();


    $scope.triggerDefaultColumns = function () {       
        $scope.getChartColumns();
    };

    $scope.triggerDefaultRows = function () {
        $scope.getChartRows();
    };
    

    $scope.triggerSelectAllColumns = function () {
        var gridColumns = $("#gridSelectColumns").data("kendoGrid");
        var dataItem_Columns = gridColumns.dataSource.data();
      
        angular.forEach(dataItem_Columns, function (obj) {
            obj.IsChecked = true;                
        });

    };

    $scope.triggerUnselectAllColumns = function () {
        var gridColumns = $("#gridSelectColumns").data("kendoGrid");
        var dataItem_Columns = gridColumns.dataSource.data();
        angular.forEach(dataItem_Columns, function (obj) {
            obj.IsChecked = true;
        });

    };

    $scope.updateChart = function () {

        try {
            var gridColumns = $("#gridSelectColumns").data("kendoGrid");
            //var dataItem_Columns = gridColumns.dataSource.data();

            //var selectedChartColumns = $("#checkedChartColumns").val();
            var selectedFields = "";

            //for (var idx = 0; idx < dataItem_Columns.length; idx++) {
            //    if (dataItem_Columns[idx].dirty || (selectedChartColumns.indexOf("," + idx + ",") >= 0)) {
            //        dataItem_Columns[idx].IsCheck = true;
            //        dataItem_Columns[idx].IsModify = true;//DataFieldName
            //        changed = true;

            //        if (selectedFields == "") {
            //            selectedFields = dataItem_Columns[idx].DataFieldName;
            //        } else {
            //            selectedFields += "," + dataItem_Columns[idx].DataFieldName;
            //        }
            //        //break;
            //    }
            //}


            //get all selected rows (those which have the checkbox checked)  
            var selCols = $("input:checked", gridColumns.tbody).closest("tr");
            // Get data item for each
            //var items = [];
            $.each(selCols, function (idx, row) {
                var item = gridColumns.dataItem(row);

                if (selectedFields == "") {
                    selectedFields = item.DataFieldName;
                } else {
                    selectedFields += "," + item.DataFieldName;
                }
            });


            //-----------------------------------
            var gridRows = $("#gridSelectRows").data("kendoGrid");
            //var dataItem_Rows = gridRows.dataSource.data();

            //var selectedChartRows = $("#checkedChartRows").val();
            var selectedRows = "";

            //for (var idx = 0; idx < dataItem_Rows.length; idx++) {
            //    if (dataItem_Rows[idx].dirty || (selectedChartRows.indexOf("," + idx + ",") >= 0)) {
            //        dataItem_Rows[idx].IsCheck = true;
            //        dataItem_Rows[idx].IsModify = true;//DataFieldName
            //        changed = true;

            //        if (selectedRows == "") {
            //            selectedRows = idx + 1;
            //        } else {
            //            selectedRows += "," + idx + 1;
            //        }
            //        //break;
            //    }
            //}

            //get all selected rows (those which have the checkbox checked)  
            var selRows = $("input:checked", gridRows.tbody).closest("tr");
            // Get data item for each

            $.each(selRows, function (idx, row) {
                //var item = gridRows.dataItem(row);

                var selectedIdx = row.rowIndex + 1; // idx + 1;
                if (selectedRows == "") {
                    selectedRows = selectedIdx;
                } else {
                    selectedRows += "," + selectedIdx;
                }
            });

            //---------------------------

            //var selectedChartTypeId = $("#chartType").val();
            //var chartTitle = $("#txtChartTitle").val();
            //var chartSubTitle = $("#txtChartSubTitle").val();
            //var chartVisible = 0; //$scope.chartVisible; // $("#chkChartVisible").val();
            //if (document.getElementById('chkChartVisible').checked) {
            //    chartVisible = 1;
            //}
            //var chartNote = $("#txtChartNote").val();

            if (selectedFields != "") {
                //, title: chartTitle, sub_title: chartSubTitle, visible: chartVisible, chart_note: chartNote, chart_type_id: selectedChartTypeId
                var chartTypeIdx = $scope.currentIndex;

                var showAtHomePage = 0;
                if (document.getElementById('chkShowAtHomePage').checked) {
                    showAtHomePage = 1;
                }
                var autoSpeech = 0;
                if (document.getElementById('chkAutoSpeech').checked) {
                    autoSpeech = 1;
                }
                var params = {
                    chart_id: chart_id, report_id: report_id, selected_fields: selectedFields, selected_rows: selectedRows,
                    chart_type_idx: chartTypeIdx, show_at_home_page: showAtHomePage, auto_speech: autoSpeech
                };

                GridService.updateChart(params).then(function (result) {
                    if (result.Success) {
                        alert("Lưu thành công!");

                        var url = '/Report/Home?chart_id=' + chart_id + '&report_id=' + report_id;
                        window.location.href = url;
                    } else {
                        alert(result.Error);
                    }
                }, function (result) {
                });
            } else {
                alert("Chưa nhập tiêu đề hoặc chưa chọn cột. Vui lòng xem lại!");
            }
        } catch (err) {
            alert("chartCustomize.updateChart() : " + err.message);
        }
        
    }

    $scope.selected = null;
    $scope.setSelected = function (val) {
        $scope.selected = val;
    };

    $scope.displayedCollection = [].concat($scope.rowCollection);

  
}])
    .animation('.slide-animation', function () {
        return {
            beforeAddClass: function (element, className, done) {
                var scope = element.scope();

                if (className == 'ng-hide') {
                    var finishPoint = element.parent().width();
                    if (scope.direction !== 'right') {
                        finishPoint = -finishPoint;
                    }
                    TweenMax.to(element, 0.5, { left: finishPoint, onComplete: done });
                }
                else {
                    done();
                }
            },
            removeClass: function (element, className, done) {
                var scope = element.scope();

                if (className == 'ng-hide') {
                    element.removeClass('ng-hide');

                    var startPoint = element.parent().width();
                    if (scope.direction === 'right') {
                        startPoint = -startPoint;
                    }

                    TweenMax.fromTo(element, 0.5, { left: startPoint }, { left: 0, onComplete: done });
                }
                else {
                    done();
                }
            }
        };
    });

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

    //service.getChartInfo = function (params) {
    //    var deferred = $q.defer();
    //    $http.post(url + '/GetChartInfo', params)
    //        .success(function (result) {
    //            //note, the server passes the information about the data set size
    //            deferred.resolve(result);
    //        }).error(function () {
    //            deferred.reject("error");
    //        });

    //    return deferred.promise;
    //};

    service.getChartColumns = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/GetChartColumns', params)
            .success(function (result) {
                //note, the server passes the information about the data set size
                deferred.resolve(result);
            }).error(function () {
                deferred.reject("error");
            });

        return deferred.promise;
    };

    service.getChartRows = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/GetChartRows', params)
            .success(function (result) {
                //note, the server passes the information about the data set size
                deferred.resolve(result);
            }).error(function () {
                deferred.reject("error");
            });

        return deferred.promise;
    };

    service.getChartTypes = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/GetChartTypes', params)
            .success(function (result) {
                //note, the server passes the information about the data set size
                deferred.resolve(result);
            }).error(function () {
                deferred.reject("error");
            });

        return deferred.promise;
    };

    service.updateChart = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/UpdateChart', params)
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
