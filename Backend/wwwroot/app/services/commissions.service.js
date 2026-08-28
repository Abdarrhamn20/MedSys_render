(function() {
    'use strict';

    angular.module('medicalApp').factory('CommissionsService', CommissionsService);

    CommissionsService.$inject = ['$http'];

    function CommissionsService($http) {
        var service = {
            getCommissions: getCommissions,
            setCommission: setCommission,
            getDoctorLedger: getDoctorLedger,
            getDailyCashReport: getDailyCashReport,
            processExpressBooking: processExpressBooking
        };

        return service;

        function getCommissions() {
            return $http.get('/api/commissions').then(function(res) {
                return res.data;
            });
        }

        function setCommission(data) {
            return $http.post('/api/commissions', data).then(function(res) {
                return res.data;
            });
        }

        function getDoctorLedger(doctorId, fromDate, toDate) {
            var params = {};
            if (fromDate) params.fromDate = fromDate;
            if (toDate) params.toDate = toDate;

            return $http.get('/api/commissions/doctor/' + doctorId + '/ledger', { params: params }).then(function(res) {
                return res.data;
            });
        }

        function getDailyCashReport(date) {
            var params = {};
            if (date) params.date = date;

            return $http.get('/api/commissions/daily-cash-report', { params: params }).then(function(res) {
                return res.data;
            });
        }

        function processExpressBooking(bookingData) {
            return $http.post('/api/commissions/express-booking', bookingData).then(function(res) {
                return res.data;
            });
        }
    }
})();
