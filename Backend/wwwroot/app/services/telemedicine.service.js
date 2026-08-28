(function() {
    'use strict';

    angular.module('medicalApp').factory('TelemedicineService', TelemedicineService);

    TelemedicineService.$inject = ['$http'];

    function TelemedicineService($http) {
        var API = '/api/telemedicine';

        return {
            getSession: function(appointmentId) { return $http.get(API + '/sessions/' + appointmentId).then(r); },
            getSessionHistory: function(appointmentId) { return $http.get(API + '/sessions/' + appointmentId + '/history').then(r); },
            createOrGetSession: function(data) { return $http.post(API + '/sessions', data).then(r); },
            startSession: function(id) { return $http.post(API + '/sessions/' + id + '/start').then(r); },
            endSession: function(id, notes) { return $http.post(API + '/sessions/' + id + '/end', notes || {}).then(r); }
        };

        function r(response) { return response.data; }
    }
})();
