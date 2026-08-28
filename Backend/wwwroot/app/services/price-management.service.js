(function() {
    'use strict';

    angular.module('medicalApp').factory('PriceManagementService', PriceManagementService);

    PriceManagementService.$inject = ['$http'];

    function PriceManagementService($http) {
        var API = '/api/pricemanagement';

        var service = {
            getOverview: getOverview,
            getDoctorFees: getDoctorFees,
            updateDoctorFee: updateDoctorFee,
            getLabTestPrices: getLabTestPrices,
            updateLabTestPrice: updateLabTestPrice,
            createLabTest: createLabTest,
            deleteLabTest: deleteLabTest,
            getRadiologyPrices: getRadiologyPrices,
            updateRadiologyPrice: updateRadiologyPrice,
            createRadiologyTemplate: createRadiologyTemplate,
            deleteRadiologyTemplate: deleteRadiologyTemplate,
            getMedicationPrices: getMedicationPrices,
            updateMedicationPrices: updateMedicationPrices,
            getRoomPrices: getRoomPrices,
            updateRoomRate: updateRoomRate,
            getInventoryPrices: getInventoryPrices,
            updateInventoryPrices: updateInventoryPrices,
            adjustPrices: adjustPrices,
            getHealthServices: getHealthServices,
            createHealthService: createHealthService,
            updateHealthService: updateHealthService,
            deleteHealthService: deleteHealthService
        };

        return service;

        function getOverview() { return $http.get(API + '/overview').then(r); }
        function getDoctorFees() { return $http.get(API + '/doctors').then(r); }
        function updateDoctorFee(id, price) { return $http.put(API + '/doctors/' + id, { price: price }).then(r); }
        function getLabTestPrices() { return $http.get(API + '/lab-tests').then(r); }
        function updateLabTestPrice(id, price) { return $http.put(API + '/lab-tests/' + id, { price: price }).then(r); }
        function createLabTest(data) { return $http.post(API + '/lab-tests', data).then(r); }
        function deleteLabTest(id) { return $http.delete(API + '/lab-tests/' + id).then(r); }
        function getRadiologyPrices() { return $http.get(API + '/radiology-templates').then(r); }
        function updateRadiologyPrice(id, price) { return $http.put(API + '/radiology-templates/' + id, { price: price }).then(r); }
        function createRadiologyTemplate(data) { return $http.post(API + '/radiology-templates', data).then(r); }
        function deleteRadiologyTemplate(id) { return $http.delete(API + '/radiology-templates/' + id).then(r); }
        function getMedicationPrices(search, page, pageSize) {
            var params = { page: page || 1, pageSize: pageSize || 50 };
            if (search) params.search = search;
            return $http.get(API + '/medications', { params: params }).then(r);
        }
        function updateMedicationPrices(id, data) { return $http.put(API + '/medications/' + id + '/prices', data).then(r); }
        function getRoomPrices() { return $http.get(API + '/rooms').then(r); }
        function updateRoomRate(id, price) { return $http.put(API + '/rooms/' + id, { price: price }).then(r); }
        function getInventoryPrices(search, page, pageSize) {
            var params = { page: page || 1, pageSize: pageSize || 50 };
            if (search) params.search = search;
            return $http.get(API + '/inventory', { params: params }).then(r);
        }
        function updateInventoryPrices(id, data) { return $http.put(API + '/inventory/' + id + '/prices', data).then(r); }
        function adjustPrices(entityType, percentage) { return $http.post(API + '/adjust-prices', { entityType: entityType, percentage: percentage }).then(r); }
        function getHealthServices() { return $http.get(API + '/health-services').then(r); }
        function createHealthService(data) { return $http.post(API + '/health-services', data).then(r); }
        function updateHealthService(id, data) { return $http.put(API + '/health-services/' + id, data).then(r); }
        function deleteHealthService(id) { return $http.delete(API + '/health-services/' + id).then(r); }

        function r(response) { return response.data; }
    }
})();
