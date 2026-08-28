(function() {
    'use strict';

    angular.module('medicalApp').factory('LabService', LabService);

    LabService.$inject = ['$http'];

    function LabService($http) {
        var API = '/api/lab';

        var service = {
            getLabTests: getLabTests,
            getLabOrders: getLabOrders,
            getLabOrder: getLabOrder,
            createLabOrder: createLabOrder,
            updateLabResult: updateLabResult,
            createLabTest: createLabTest,
            updateLabTest: updateLabTest,
            deleteLabTest: deleteLabTest,
            addPanelMember: addPanelMember,
            saveCulture: saveCulture,
            addSensitivity: addSensitivity,
            getCulture: getCulture,
            getDevices: getDevices,
            createDevice: createDevice,
            updateDevice: updateDevice,
            captureDeviceResult: captureDeviceResult
        };

        return service;

        function getLabTests() {
            return $http.get(API + '/tests').then(r);
        }

        function getLabOrders(status, patientUserId) {
            var params = {};
            if (status) params.status = status;
            if (patientUserId) params.patientUserId = patientUserId;
            return $http.get(API + '/orders', { params: params }).then(r);
        }

        function getLabOrder(id) {
            return $http.get(API + '/orders/' + id).then(r);
        }

        function createLabOrder(data) {
            return $http.post(API + '/orders', data).then(r);
        }

        function updateLabResult(id, itemId, data) {
            return $http.put(API + '/orders/' + id + '/items/' + itemId + '/result', data).then(r);
        }

        function createLabTest(data) {
            return $http.post(API + '/tests', data).then(r);
        }

        function updateLabTest(id, data) {
            return $http.put(API + '/tests/' + id, data).then(r);
        }

        function deleteLabTest(id) {
            return $http.delete(API + '/tests/' + id).then(r);
        }

        function addPanelMember(id, data) {
            return $http.post(API + '/tests/' + id + '/panel', data).then(r);
        }

        function saveCulture(id, itemId, data) {
            return $http.post(API + '/orders/' + id + '/items/' + itemId + '/culture', data).then(r);
        }

        function addSensitivity(cultureId, data) {
            return $http.post(API + '/culture/' + cultureId + '/sensitivities', data).then(r);
        }

        function getCulture(id, itemId) {
            return $http.get(API + '/orders/' + id + '/items/' + itemId + '/culture').then(r);
        }

        function getDevices() {
            return $http.get(API + '/devices').then(r);
        }

        function createDevice(data) {
            return $http.post(API + '/devices', data).then(r);
        }

        function updateDevice(id, data) {
            return $http.put(API + '/devices/' + id, data).then(r);
        }

        function captureDeviceResult(id, data) {
            return $http.post(API + '/devices/' + id + '/capture', data).then(r);
        }

        function r(response) { return response.data; }
    }
})();
