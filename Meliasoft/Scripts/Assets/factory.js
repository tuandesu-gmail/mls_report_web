app.factory('LookupService', ['$q', '$filter', '$timeout', '$http', function ($q, $filter, $timeout, $http) {
    var service = {};

    service.checkDataForLookup = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/CheckDataForLookup', params)
            .success(function (result) {
                //note, the server passes the information about the data set size
                deferred.resolve(result);
            }).error(function () {
                deferred.reject("error");
            });

        return deferred.promise;
    };

    service.getDataForLookup = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/GetDataForLookup', params)
            .success(function (result) {
                //note, the server passes the information about the data set size
                deferred.resolve(result);
            }).error(function () {
                deferred.reject("error");
            });

        return deferred.promise;
    };

    service.getDetailOfDmPr = function (params) {
        var deferred = $q.defer();
        $http.post(url + '/GetDetailOfDmPr', params)
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
