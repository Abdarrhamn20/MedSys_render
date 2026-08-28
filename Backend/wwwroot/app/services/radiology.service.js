(function() {
    'use strict';

    angular.module('medicalApp').factory('RadiologyService', RadiologyService);

    RadiologyService.$inject = ['$http'];

    function RadiologyService($http) {
        var service = {
            getTemplates: getTemplates,
            getRadiologyOrders: getRadiologyOrders,
            createRadiologyOrder: createRadiologyOrder,
            updateRadiologyReport: updateRadiologyReport,
            uploadImage: uploadImage
        };

        return service;

        function getTemplates() {
            return $http.get('/api/radiology/templates').then(function(res) {
                return res.data;
            });
        }

        function getRadiologyOrders(status, patientUserId) {
            var params = {};
            if (status) params.status = status;
            if (patientUserId) params.patientUserId = patientUserId;

            return $http.get('/api/radiology/orders', { params: params }).then(function(res) {
                return res.data;
            });
        }

        function createRadiologyOrder(data) {
            return $http.post('/api/radiology/orders', data).then(function(res) {
                return res.data;
            });
        }

        function updateRadiologyReport(id, data) {
            return $http.put('/api/radiology/orders/' + id + '/report', data).then(function(res) {
                return res.data;
            });
        }

        function uploadImage(file) {
            var fd = new FormData();
            fd.append('file', file);
            return $http.post('/api/radiology/upload', fd, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            }).then(function(res) {
                return res.data;
            });
        }
    }
})();
