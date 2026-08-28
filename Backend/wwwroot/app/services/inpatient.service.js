(function() {
    'use strict';

    angular.module('medicalApp').factory('InpatientService', InpatientService);

    InpatientService.$inject = ['$http'];

    function InpatientService($http) {
        var API = '/api/inpatient';

        return {
            getWards: function() { return $http.get(API + '/wards').then(r); },
            createWard: function(data) { return $http.post(API + '/wards', data).then(r); },
            getBedGrid: function() { return $http.get(API + '/bed-grid').then(r); },
            createRoom: function(data) { return $http.post(API + '/rooms', data).then(r); },
            createBed: function(data) { return $http.post(API + '/beds', data).then(r); },
            getAdmissions: function(status) {
                var url = API + '/admissions';
                if (status) url += '?status=' + status;
                return $http.get(url).then(r);
            },
            getAdmissionById: function(id) { return $http.get(API + '/admissions/' + id).then(r); },
            createAdmission: function(data) { return $http.post(API + '/admissions', data).then(r); },
            dischargePatient: function(id, data) { return $http.post(API + '/admissions/' + id + '/discharge', data).then(r); },
            addDailyLog: function(id, data) { return $http.post(API + '/admissions/' + id + '/logs', data).then(r); },
            createCareOrder: function(data) { return $http.post(API + '/orders', data).then(r); },
            getCareOrders: function(admissionId) { return $http.get(API + '/admissions/' + admissionId + '/orders').then(r); },
            executeCareOrder: function(orderId, data) { return $http.post(API + '/orders/' + orderId + '/execute', data).then(r); },
            getNursingDashboard: function() { return $http.get(API + '/nursing-dashboard').then(r); }
        };

        function r(response) {
            return response.data;
        }
    }
})();
