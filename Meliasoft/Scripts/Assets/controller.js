app.controller('MyAlert', ["$uibModalInstance", "msg", function ($uibModalInstance, msg) {
    var $ctrl = this;

    $ctrl.title = "Lỗi";
    $ctrl.msg = msg;

    $ctrl.ok = function () {
        $uibModalInstance.close();
    };

    $ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}]);

app.controller('MyLookup', ["$uibModalInstance", "LookupService", '$uibModal', "key", "val", "date", function ($uibModalInstance, LookupService, $uibModal, key, val, date) {
    var $ctrl = this;

    $ctrl.title = "Chọn danh mục";
    $ctrl.rowCollection = [];
    $ctrl.rowCollectionDetail = [];

    $ctrl.showDVT = (key === "MA_VT");
    $ctrl.showDetail = (key === "MA_PR");
    $ctrl.date = date;

    $ctrl.selected = null;
    $ctrl.setSelected = function (val) {
        $ctrl.selected = val;
        if ($ctrl.showDetail) {
            var value = $ctrl.rowCollection[val].MA_CODE;
            $ctrl.getDetailOfDmPr(value);
        }
    }

    $ctrl.detail = function (key) {
        //alert(val);

        var modalInstanceDetail = $uibModal.open({
            animation: false,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: '/Scripts/Assets/myLookupDetail.html',
            controller: 'MyLookupDetail',
            controllerAs: '$ctrl',
            //size: size,
            resolve: {
                key: function () {
                    return key;
                },
            }
        });

        modalInstanceDetail.result.then(function (item) {
        }, function () {
            //$log.info('Modal dismissed at: ' + new Date());
        });
    }

    $ctrl.choiceSelected = function (val) {
        $ctrl.setSelected(val);
        $ctrl.ok();
    }

    $ctrl.getDataForLookup = function () {
        var params = { key: key, search: $ctrl.lookup, date: $ctrl.date };
        LookupService.getDataForLookup(params).then(function (result) {
            $ctrl.rowCollection = result;
        }, function (result) {
        });
    }

    $ctrl.getDetailOfDmPr = function (value) {
        var params = { key: value };
        LookupService.getDetailOfDmPr(params).then(function (result) {
            $ctrl.rowCollectionDetail = result;
        }, function (result) {
        });
    }

    $ctrl.search = function () {
        $ctrl.getDataForLookup();
    }

    if (typeof val !== "undefined") {
        $ctrl.lookup = val;
        $ctrl.search();
    } else {
        $ctrl.lookup = "";
    }

    $ctrl.lookupKey = function ($event) {
        if ($event.keyCode === 13) {
            $ctrl.getDataForLookup();
        }
    }

    $ctrl.ok = function () {
        if ($ctrl.selected !== null) {
            $uibModalInstance.close($ctrl.rowCollection[$ctrl.selected]);
        }
    };

    $ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $ctrl.displayedCollection = [].concat($ctrl.rowCollection);
    $ctrl.displayedCollectionDetail = [].concat($ctrl.rowCollectionDetail);
}]);

app.controller('MyLookupDetail', ["$uibModalInstance", "LookupService", "key", function ($uibModalInstance, LookupService, key) {
    var $ctrl = this;

    $ctrl.title = "Chi tiết";
    $ctrl.rowCollection = [];

    $ctrl.ok = function () {
        $uibModalInstance.dismiss('ok');
    };

    $ctrl.getDetailOfDmPr = function () {
        var params = { key: key };
        LookupService.getDetailOfDmPr(params).then(function (result) {
            $ctrl.rowCollection = result;
        }, function (result) {
        });
    }

    $ctrl.getDetailOfDmPr();

    $ctrl.displayedCollection = [].concat($ctrl.rowCollection);
}]);
