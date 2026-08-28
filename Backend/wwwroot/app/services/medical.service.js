(function() {
    'use strict';

    angular.module('medicalApp').factory('MedicalService', MedicalService);

    MedicalService.$inject = ['$http'];

    function MedicalService($http) {
        var API = '/api/medicalrecords';

        return {
            getRecords: function(params) { return $http.get(API, { params: params }).then(r); },
            getRecord: function(id) { return $http.get(API + '/' + id).then(r); },
            createRecord: function(data) { return $http.post(API, data).then(r); },
            updateRecord: function(id, data) { return $http.put(API + '/' + id, data).then(r); },
            addPrescription: function(recordId, data) { return $http.post(API + '/' + recordId + '/prescriptions', data).then(r); },
            deletePrescription: function(id) { return $http.delete(API + '/prescriptions/' + id).then(r); },
            sendPrescriptionsToPharmacy: function(recordId) { return $http.post(API + '/' + recordId + '/send-prescriptions', {}).then(r); }
        };

        function r(response) { return response.data; }
    }
})();
