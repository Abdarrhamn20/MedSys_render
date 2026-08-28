(function() {
    'use strict';

    angular.module('medicalApp').factory('AccountingService', AccountingService);

    AccountingService.$inject = ['$http'];

    function AccountingService($http) {
        var API = '/api/accounting';

        return {
            getChart: function() {
                return $http.get(API + '/chart').then(r);
            },
            getFlatAccounts: function() {
                return $http.get(API + '/chart/flat').then(r);
            },
            createAccount: function(data) {
                return $http.post(API + '/chart', data).then(r);
            },
            updateAccount: function(id, data) {
                return $http.put(API + '/chart/' + id, data).then(r);
            },

            getJournalEntries: function(params) {
                return $http.get(API + '/journal-entries', { params: params }).then(r);
            },
            getJournalEntry: function(id) {
                return $http.get(API + '/journal-entries/' + id).then(r);
            },
            createJournalEntry: function(data) {
                return $http.post(API + '/journal-entries', data).then(r);
            },
            postJournalEntry: function(id) {
                return $http.post(API + '/journal-entries/' + id + '/post', {}).then(r);
            },
            reverseJournalEntry: function(id) {
                return $http.post(API + '/journal-entries/' + id + '/reverse', {}).then(r);
            },

            getLedger: function(accountId, params) {
                return $http.get(API + '/ledger/' + accountId, { params: params }).then(r);
            },
            getTrialBalance: function(params) {
                return $http.get(API + '/trial-balance', { params: params }).then(r);
            },
            getSummary: function() {
                return $http.get(API + '/summary').then(r);
            }
        };

        function r(response) {
            return response.data;
        }
    }
})();
