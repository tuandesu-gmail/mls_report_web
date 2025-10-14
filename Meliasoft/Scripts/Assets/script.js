function validateField(value, id) {
    var error = false;

    if (value === null || value === "") {
        var title = $("#" + id).attr("data-val-required");
        $("span[data-valmsg-for='" + id + "']").html("<img src='/Images/required.png' height='20px' title='" + title + "' />");
        error = true;
    } else {
        $("span[data-valmsg-for='" + id + "']").html("");
    }

    return error;
}

function myAlert($scope, $uibModal, msg) {
    var modalInstance = $uibModal.open({
        animation: false,
        ariaLabelledBy: 'modal-title',
        ariaDescribedBy: 'modal-body',
        templateUrl: '/Scripts/Assets/myAlert.html',
        controller: 'MyAlert',
        controllerAs: '$ctrl',
        //size: size,
        resolve: {
            msg: function () {
                return msg;
            },
            parents: function () {
                return $scope;
            },
        }
    });

    modalInstance.result.then(function (item) {
    }, function () {
    });
}

function processDVTS(row) {
    row.DVTS = [];
    if (row.DVT_QD0 !== null && row.DVT_QD0 !== "") {
        row.DVTS.push({ DVT: row.DVT_QD0, HS_QD: 1 });
    }
    if (row.DVT_QD1 !== null && row.DVT_QD1 !== "") {
        if (row.HS_QD1 === null || row.HS_QD1 == 0) {
            row.HS_QD1 = 1;
        }
        row.DVTS.push({ DVT: row.DVT_QD1, HS_QD: row.HS_QD1 });
    }
    if (row.DVT_QD2 !== null && row.DVT_QD2 !== "") {
        if (row.HS_QD2 === null || row.HS_QD2 == 0) {
            row.HS_QD2 = 1;
        }
        row.DVTS.push({ DVT: row.DVT_QD2, HS_QD: row.HS_QD2 });
    }

    return row;
}
