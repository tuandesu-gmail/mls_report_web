app.controller('ctrl', ['$scope', '$q', 'GridService', '$uibModal', '$http', function ($scope, $q, GridService, $uibModal, $http) {
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
    if (value.Value && value.Value.indexOf("/Date(") == 0) {//if (value.Value && value.Value.startsWith("/Date(")) {
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
        console.log('report.js: $scope.menus == null || $scope.menus.length == 0');
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

  var dataBindingFlag = true;
  var pagingIncrement = 50;
  var loadedRowsCounter = 0;
  var previousTop = 0;

  $scope.getData = function () {
    $scope.waiting = true;

    var cloned = [].concat($scope.param);
    angular.forEach(cloned, function (value, index) {
      if (typeof (value.Value) === "undefined") {
        value.Value = "";
      } else if (value.Value && typeof (value.Value.title) !== "undefined") {
        value.Value = value.Value.title;
      } else {
        if (angular.isDate(value.Value)) {
          //var parsedDate = Date.parse(date);
          //var _date = $filter('date')(value.Value, 'dd/MM/yyyy');
          var options = {
            year: "numeric",
            month: "2-digit",
            day: "numeric"
          };
          value.Value = value.Value.toLocaleString("vi", options);
        }

      }
    });

    //function fixHeaderEllipsis() {
    //  var $links = $("#grid .k-grid-header .k-header .k-link");
    //  $links.each(function () {
    //    var t = $(this).text().trim();
    //    $(this).removeClass("needs-ellipsis no-ellipsis");
    //    if (t.length > 40) $(this).addClass("needs-ellipsis");
    //    else $(this).addClass("no-ellipsis");
    //  });
    //}

    var params = { id: id, key_detail: keyDetail, param: cloned };
    GridService.getData(params).then(function (result) {
      if (typeof (result.Success) === "undefined") {

        var voiceMsgValue = result[result.length - 1];
        $("#hiddenChartVoiceMsg").val(voiceMsgValue.VoiceMsgValue);
        //var val = "voiceMsg";
        //var filteredObj = data.find(function (item, i) {
        //    if (item.voiceMsg_Name === val) {                        
        //        return item.voiceMsg_Value;
        //    }
        //});

        //var single_object = $scope('$scope')(result, function (d) { return d.voiceMsg !== ""; })[0];
        result.splice(result.length - 1, 1);

        angular.forEach(result, function (value, index) {
          //value.IsCheck = false;
          value.IsModify = false;
        });




        $scope.rowCollection = result;
        if (result.length > 0) {
          $scope.selected = 0;
        }

        $("#grid").data('kendoGrid').dataSource.data([]);

        //if (ishaslockedcolumn == 1) {
        var grid = $("#grid").data("kendoGrid");

        //if (grid) {
        //  // chạy lần đầu
        //  fixHeaderEllipsis();

        //  // chạy lại khi data render / đổi kích thước / show-hide cột / resize cột
        //  grid.bind("dataBound", fixHeaderEllipsis);
        //  grid.bind("columnShow", fixHeaderEllipsis);
        //  grid.bind("columnHide", fixHeaderEllipsis);
        //  grid.bind("columnResize", fixHeaderEllipsis);
        //}

        var dataSource = new kendo.data.DataSource({ data: result });
        grid.setDataSource(dataSource);
        grid.dataSource.read();
        //} else {
        //    // New code for Auto loading when scroll
        //    var gridElement = $("#grid")
        //    //var pagingIncrement = 30; //new
        //    loadedRowsCounter = pagingIncrement;
        //    var scrollbarWidth = kendo.support.scrollbar();
        //    var dataBindingFlag = true;                    

        //    gridElement.kendoGrid({
        //        dataSource: {
        //            type: "json",
        //            //transport: { 
        //            //    read: "http://demos.kendoui.com/service/Northwind.svc/Orders"
        //            //},
        //            data: result,
        //            //total: function (data) {
        //            //    return data.data.length;
        //            //},
        //            schema: {
        //                model: {
        //                    fields: dummy
        //                }
        //            },
        //            pageSize: pagingIncrement,

        //            serverPaging: false
        //        },
        //        navigatable: false,
        //        //pageable: true,
        //        pageable: {
        //            refresh: false,
        //            previousNext: false,
        //            numeric: false
        //        },
        //        //scrollable: { virtual: true },

        //        //dataBound: function () {
        //        //    dataBindingFlag = true;
        //        //},


        //        dataBound: function (e) {
        //            dataBindingFlag = true;
        //            //e.sender.pager.element.hide();
        //            //e.sender.pager.element.css = "height:0px;";
        //            //e.sender.pager.element.offsetHeight = 0;
        //            //e.sender.pager.element.scrollHeight = 0;

        //            //if (e.sender.dataSource.total() <= e.sender.dataSource.pageSize()) {
        //            //    e.sender.pager.element.hide();
        //            //} else {
        //            //    e.sender.pager.element.show();
        //            //}   
        //            this.pager.element.hide();
        //            //e.sender.scrollHeight += 30;
        //            //k-grid-content k-auto-scrollable
        //            //this.refresh();
        //            //$('[class*="k-pager-nav"]').hide();
        //            var newHeight = $(".k-grid-content").height();
        //            $(".k-grid-content").height(newHeight + 37);

        //            var dataItems = e.sender.dataSource.view();
        //            for (var j = 0; j < dataItems.length; j++) {
        //                var bold = dataItems[j].Bold;

        //                var row = e.sender.tbody.find("[data-uid='" + dataItems[j].uid + "']");

        //                if (bold == "C") {
        //                    row.addClass("bold");

        //                    if (e.sender.lockedContent) {
        //                        row = e.sender.lockedContent.find("[data-uid='" + dataItems[j].uid + "']");
        //                        row.addClass("bold");
        //                    }
        //                }

        //                var bColor = dataItems[j].BColor;
        //                if (bColor !== undefined) {
        //                    row.css("background-color", bColor);
        //                }

        //                //angular.forEach(fields, function (value, index) {
        //                //    var cell = row.children().eq(value.index);
        //                //    if (cell[0].innerText == "0") {
        //                //        cell[0].innerText = "";
        //                //    }
        //                //    if (cell[0].innerText == "01/01/1900") {
        //                //        cell[0].innerText = "";
        //                //    }
        //                //});
        //            }
        //        },
        //        //-----------------------------------------

        //        columns: dummy //dummy //fields
        //    });


        //    var gridDataSource = gridElement.data("kendoGrid").dataSource;

        //    gridElement.children(".k-grid-content")
        //        .on("scroll", function (e) {
        //            if (dataBindingFlag) {
        //                var dataDiv = e.target;
        //                var currentPageSize = gridDataSource.pageSize();
        //                var currentIdx = gridDataSource.page();
        //                var newIdx = currentIdx;
        //                var currentTop = dataDiv.scrollTop;
        //                loadedRowsCounter = pagingIncrement * currentIdx;

        //                var scrollHeight = dataDiv.scrollHeight - dataDiv.offsetHeight - scrollbarWidth;
        //                var scrollingFlg = false;
        //                var scrollTo = -50;

        //                if (dataDiv.scrollTop >= dataDiv.scrollHeight - dataDiv.offsetHeight - scrollbarWidth && gridDataSource.total() > currentPageSize) {
        //                    dataBindingFlag = false;
        //                    gridDataSource.pageSize(currentPageSize + pagingIncrement);
        //                }
        //                //if (gridDataSource.total() > loadedRowsCounter) {

        //                    //if (dataDiv.scrollTop >= scrollHeight) {
        //                    //    if (previousTop < currentTop) {
        //                    //        scrollingFlg = true;
        //                    //        newIdx++;
        //                    //    }
        //                    //} else if (dataDiv.scrollTop <= scrollbarWidth && newIdx > 1) {
        //                    //    if (previousTop > currentTop) {
        //                    //        scrollingFlg = true;
        //                    //        scrollTo = scrollHeight + 50;
        //                    //        newIdx--;
        //                    //    }                                        
        //                    //} 
        //                //} else if (dataDiv.scrollTop <= scrollbarWidth && newIdx > 1) {
        //                //    if (previousTop > currentTop) {
        //                //        scrollingFlg = true;
        //                //        scrollTo = scrollHeight + 50;
        //                //        newIdx--;
        //                //    }
        //                //}

        //                //if (scrollingFlg) {
        //                //    dataBindingFlag = false;
        //                //    gridDataSource.pageSize(currentPageSize + pagingIncrement);

        //                //    //gridDataSource.page(currentIdx + 1); //gridDataSource.page() + 1

        //                //    setTimeout(function () {

        //                //        gridDataSource.page(newIdx);

        //                //        //if (newIdx > currentIdx) {
        //                //            $('.k-grid-content').animate({
        //                //                scrollTop: scrollTo
        //                //            }, 1);
        //                //        //}                                        
        //                //    }, 500);                                    

        //                //    setTimeout(function () {
        //                //        dataBindingFlag = true;
        //                //        //ChangeDataBindingFlag();
        //                //    }, 1000);

        //                //    previousTop = 0;
        //                //} else {
        //                //    previousTop = currentTop;
        //                //}

        //            }
        //        });

        //}




        $scope.waiting = false;
      } else {
        $scope.waiting = false;
        alert(result.Error);
      }
    }, function (result) {
    });
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

  $scope.getData();


  var addressCache = null;
  var addressPromise = null;

  function initAddress() {
    if (!addressPromise) {
      addressPromise = getCurrentPosition()
        .then(function (pos) { return reverseGeocode($http, pos.coords.latitude, pos.coords.longitude); })
        .catch(function () { return null; })
        .then(function (addr) { addressCache = addr; return addr; });
    }
    return addressPromise; // để ai cần có thể .then()
  }

  // gọi NGAY khi vào trang
  initAddress();

  

  $scope.setData = function () {
    var grid = $("#grid").data("kendoGrid");
    var dataItem = grid.dataSource.data();

    var checkedRows = $("#checkedRows").val();

    //////var grid = $("#grid").data("kendoGrid");
    //////var lockedRow = grid.lockedTable.find("tr").eq(finder.index());

    //////lockedRow.find(":checkbox").prop("checked", true);

    ////var finder = $(this).parent(); // the clicked row element

    //////var grid = $("#grid").data("kendoGrid"); // get reference to the Grid widget

    ////var lockedRow = grid
    ////    .k-grid-content-locked // reference to the locked table
    ////    .find("tr").eq(finder.index()); // finds the row with the same index as the one clicked (the finder)

    ////// finds the checkbox inside the row and set it as checked
    ////// here we assume that the checkbox is always inside the locked portion of the grid
    ////lockedRow.find(":checkbox").prop("checked", true);

    ////---------------------------------------
    //var changed = false;
    //for (var idx = 0; idx < dataItem.length; idx++) {
    //    if (dataItem[idx].dirty || (checkedRows.indexOf("," + idx + ",") >= 0)) {
    //        dataItem[idx].IsCheck = true;
    //        dataItem[idx].IsModify = true;
    //        changed = true;
    //        //break;
    //    }
    //}

    //if (changed) { //if (true || changed) {
    //    var params = { id: id, param: dataItem, changed: changed };
    //    //var params = { id: id, param: $scope.rowCollection };
    //    GridService.setData(params).then(function (result) {
    //        if (result.Success) {
    //            alert("Success");
    //        } else {
    //            alert(result.Error);
    //        }
    //    }, function (result) {
    //    });
    //}

    var changed = false;
    var postRows = [];                           // <— mảng chỉ chứa dòng cần gửi
    var rows = dataItem;
    var checked = $("#checkedRows").val() || ""; // ",0,3,7,"

    for (var idx = 0; idx < rows.length; idx++) {
      var row = rows[idx];
      var isChecked = checked.indexOf("," + idx + ",") >= 0 || row.IsCheck === true;
      var isDirty = !!row.dirty || row.IsModify === true;

      if (isChecked || isDirty) {
        row.IsCheck = true;
        row.IsModify = true;
        postRows.push(row);                      // <— chỉ push dòng cần lưu
        changed = true;
      }
    }

    if (!changed) {
      alert("Không có dòng nào thay đổi để lưu.");
      return;
    }

    //var params = { id: id, param: postRows, changed: true };  // <— gửi CHỈ postRows
    //GridService.setData(params).then(function (result) {
    //    if (result.Success) alert("Success");
    //    else showMsg(result.Error);  //alert(result.Error);
    //}, function () { alert("error"); });

    // nếu đã có cache -> dùng ngay, nếu chưa xong -> đợi promise
    var addrP = (addressCache !== null) ? $q.when(addressCache) : (addressPromise || $q.when(null));

    addrP.then(function (addr) {
      var params = { id: id, param: postRows, changed: true, clientAddress: addr };
      return GridService.setData(params);
    }).then(function (result) {
      if (result.Success) alert("Success"); else showMsg(result.Error); // alert(result.Error);
    }, function () { alert("error"); });

  }

  function showMsg(msg) {
    var text = (typeof msg === 'string') ? msg : JSON.stringify(msg, null, 2);
    window.prompt("Sao chép nội dung bên dưới (Ctrl+C):", text);
  }

  // đặt ở report.js (cùng file bạn đang gọi $http)
  var LOCATIONIQ_API_KEY = "pk.1d9661f0c3df58c63a0769504ee85e76";

  // Promise lấy toạ độ trình duyệt (user phải Allow)
  function getCurrentPosition() {
    return new Promise(function (resolve, reject) {
      if (!navigator.geolocation) return reject(new Error("No geolocation"));
      navigator.geolocation.getCurrentPosition(resolve, reject, {
        enableHighAccuracy: false, timeout: 10000, maximumAge: 60000
      });
    });
  }

  // Reverse geocoding qua LocationIQ (có thể dùng $http thay axios)
  function reverseGeocode($http, lat, lon) {
    return $http.get("https://us1.locationiq.com/v1/reverse", {
      params: { key: LOCATIONIQ_API_KEY, lat: lat, lon: lon, format: "json" }
    }).then(function (resp) {
      return (resp.data && resp.data.display_name) ? resp.data.display_name : null;
    }, function () { return null; });
  }



  $scope.setDefaultChartData = function () {
    try {
      var params = { report_id: id };
      GridService.setDefaultChartData(params).then(function (result) {
        if (result.Success) {
          //alert("Success");
          window.location.href = '/Report/Home?report_id=' + id;
        } else {
          alert(result.Error);
        }
      }, function (result) {
      });
    } catch (err) {
      alert("report.setDefaultChartData() : " + err.message);
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
    try {
      var deferred = $q.defer();
      $http.post(url + '/GetMenu', params)
        .success(function (result) {
          console.log('report.js: service.getMenu = function (params): success');
          if (typeof (result.Success) === "undefined" || result.Success) {
            //note, the server passes the information about the data set size
            deferred.resolve(result);
          } else {
            alert(result.Error);
          }

        }).error(function () {
          console.log('report.js: service.getMenu = function (params): error');
          deferred.reject("error");
        });

      return deferred.promise;
    } catch (err) {
      console.log('report.js: service.getMenu = function (params): catch = ', err.message);
      alert("report.getMenu() : " + err.message);
    }
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

  //service.setData = function (params) {
  //    var deferred = $q.defer();
  //    $http.post(url + '/SetData', params)
  //        .success(function (result) {
  //            //note, the server passes the information about the data set size
  //            deferred.resolve(result);
  //        }).error(function () {
  //            deferred.reject("error");
  //        });

  //    return deferred.promise;
  //};
  service.setData = function (params) {
    var deferred = $q.defer();

    // Lấy token nếu trang có render
    var tokenEl = document.querySelector('input[name="__RequestVerificationToken"]');
    var token = tokenEl ? tokenEl.value : null;

    $http({
      method: 'POST',
      url: url + '/SetData',
      headers: angular.extend(
        { 'Content-Type': 'application/json; charset=utf-8' },
        token ? { 'RequestVerificationToken': token } : {}
      ),
      data: params
    })
      .success(function (result) { deferred.resolve(result); })
      .error(function () { deferred.reject("error"); });

    return deferred.promise;
  };


  service.setDefaultChartData = function (params) {
    var deferred = $q.defer();
    $http.post(url + '/UpdateDefaultChart', params)
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
