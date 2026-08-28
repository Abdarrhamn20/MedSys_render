(function() {
    'use strict';

    angular.module('medicalApp').factory('AttachmentService', AttachmentService);

    AttachmentService.$inject = ['$http'];

    function AttachmentService($http) {
        var API = '/api/attachments';

        return {
            uploadAttachment: uploadAttachment,
            getRecordAttachments: getRecordAttachments,
            getPatientAttachments: getPatientAttachments,
            deleteAttachment: deleteAttachment
        };

        function uploadAttachment(file, recordId, patientId, description) {
            var formData = new FormData();
            formData.append('file', file);
            if (recordId) formData.append('recordId', recordId);
            if (patientId) formData.append('patientId', patientId);
            if (description) formData.append('description', description);

            return $http.post(API + '/upload', formData, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined } // Let browser set Content-Type with boundary
            }).then(function(response) {
                return response.data;
            });
        }

        function getRecordAttachments(recordId) {
            return $http.get(API + '/record/' + recordId).then(function(response) { return response.data; });
        }

        function getPatientAttachments(patientId) {
            return $http.get(API + '/patient/' + patientId).then(function(response) { return response.data; });
        }

        function deleteAttachment(id) {
            return $http.delete(API + '/' + id).then(function(response) { return response.data; });
        }
    }
})();
