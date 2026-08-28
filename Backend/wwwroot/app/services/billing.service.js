(function() {
    'use strict';

    angular.module('medicalApp').factory('BillingService', BillingService);

    BillingService.$inject = ['$http'];

    function BillingService($http) {
        var API = '/api/billing';

        return {
            getInvoices: function(params) {
                return $http.get(API + '/invoices', { params: params }).then(r);
            },
            getInvoice: function(id) {
                return $http.get(API + '/invoices/' + id).then(r);
            },
            payWithCard: function(id, data) {
                return $http.post(API + '/invoices/' + id + '/pay', data).then(r);
            },
            payWithCash: function(id) {
                return $http.post(API + '/invoices/' + id + '/pay-cash', {}).then(r);
            },
            getStats: function() {
                return $http.get(API + '/stats').then(r);
            }
        };

        function r(response) {
            return response.data;
        }
    }
})();
