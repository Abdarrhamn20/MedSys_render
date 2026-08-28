(function() {
    'use strict';

    angular.module('medicalApp').factory('TreasuryService', TreasuryService);

    TreasuryService.$inject = ['$http'];

    function TreasuryService($http) {
        var API = '/api/treasury';

        return {
            getTreasuries: function() {
                return $http.get(API).then(r);
            },
            createTreasury: function(data) {
                return $http.post(API, data).then(r);
            },
            updateTreasury: function(id, data) {
                return $http.put(API + '/' + id, data).then(r);
            },
            deleteTreasury: function(id) {
                return $http.delete(API + '/' + id).then(r);
            },

            getVouchers: function(params) {
                return $http.get(API + '/vouchers', { params: params }).then(r);
            },
            getVoucher: function(id) {
                return $http.get(API + '/vouchers/' + id).then(r);
            },
            createVoucher: function(data) {
                return $http.post(API + '/vouchers', data).then(r);
            },
            postVoucher: function(id) {
                return $http.post(API + '/vouchers/' + id + '/post', {}).then(r);
            },
            reverseVoucher: function(id) {
                return $http.post(API + '/vouchers/' + id + '/reverse', {}).then(r);
            },

            getDailyJournal: function(params) {
                return $http.get(API + '/daily-journal', { params: params }).then(r);
            },
            getReceivables: function(params) {
                return $http.get(API + '/receivables', { params: params }).then(r);
            },
            getClosure: function() {
                return $http.get(API + '/closure').then(r);
            },
            setClosure: function(closedThrough) {
                return $http.post(API + '/closure', { closedThrough: closedThrough }).then(r);
            },
            openClosure: function() {
                return $http.post(API + '/closure/open', {}).then(r);
            }
        };

        function r(response) {
            return response.data;
        }
    }
})();
