(function() {
    'use strict';

    angular.module('medicalApp').factory('PharmacyService', PharmacyService);

    PharmacyService.$inject = ['$http'];

    function PharmacyService($http) {
        var API = '/api/pharmacy';

        return {
            // Medications
            getMedications: function(params) { return $http.get(API + '/medications', { params: params }).then(r); },
            addMedication: function(data) { return $http.post(API + '/medications', data).then(r); },
            updateMedication: function(id, data) { return $http.put(API + '/medications/' + id, data).then(r); },
            deleteMedication: function(id) { return $http.delete(API + '/medications/' + id).then(r); },
            getCategories: function() { return $http.get(API + '/medications/categories').then(r); },
            getLowStock: function() { return $http.get(API + '/low-stock').then(r); },

            // Dispensing
            getPendingPrescriptions: function() { return $http.get(API + '/prescriptions/pending').then(r); },
            dispense: function(data) { return $http.post(API + '/dispense', data).then(r); },
            getDispenseHistory: function(params) { return $http.get(API + '/dispense-history', { params: params }).then(r); },

            // Dashboard
            getDashboard: function() { return $http.get(API + '/dashboard').then(r); },

            // Medication Requests
            createMedicationRequest: function(data) { return $http.post(API + '/requests', data).then(r); },
            getMedicationRequests: function(params) { return $http.get(API + '/requests', { params: params }).then(r); },
            resolveMedicationRequest: function(id) { return $http.put(API + '/requests/' + id + '/resolve').then(r); }
        };

        function r(response) { return response.data; }
    }
})();
