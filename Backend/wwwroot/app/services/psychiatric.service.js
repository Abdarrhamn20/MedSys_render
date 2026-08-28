(function() {
    'use strict';

    angular.module('medicalApp').factory('PsychiatricService', PsychiatricService);

    PsychiatricService.$inject = ['$http'];

    function PsychiatricService($http) {
        var PSYCH_API = '/api/psychiatric';
        var ASSESS_API = '/api/assessments';

        return {
            // === MSE Records ===
            getPsychiatricRecord: function(recordId) {
                return $http.get(PSYCH_API + '/records/' + recordId).then(r);
            },
            savePsychiatricRecord: function(recordId, data) {
                return $http.post(PSYCH_API + '/records/' + recordId, data).then(r);
            },

            // === SOAP Notes ===
            getSoapNote: function(recordId) {
                return $http.get(PSYCH_API + '/soap/' + recordId).then(r);
            },
            saveSoapNote: function(recordId, data) {
                return $http.post(PSYCH_API + '/soap/' + recordId, data).then(r);
            },

            // === Patient Risk Level ===
            getPatientRisk: function(patientId) {
                return $http.get(PSYCH_API + '/patient-risk/' + patientId).then(r);
            },
            updatePatientRisk: function(patientId, data) {
                return $http.put(PSYCH_API + '/patient-risk/' + patientId, data).then(r);
            },

            // === Questionnaire Templates ===
            getTemplates: function() {
                return $http.get(ASSESS_API + '/templates').then(r);
            },
            createTemplate: function(data) {
                return $http.post(ASSESS_API + '/templates', data).then(r);
            },
            deleteTemplate: function(id) {
                return $http.delete(ASSESS_API + '/templates/' + id).then(r);
            },

            // === Assignment ===
            assignAssessment: function(data) {
                return $http.post(ASSESS_API + '/assign', data).then(r);
            },

            // === Patient Flows ===
            getPatientPending: function() {
                return $http.get(ASSESS_API + '/patient/pending').then(r);
            },
            getPatientCompleted: function() {
                return $http.get(ASSESS_API + '/patient/completed').then(r);
            },
            submitAnswers: function(id, data) {
                return $http.post(ASSESS_API + '/patient/submit/' + id, data).then(r);
            },

            // === Doctor Results & Lists ===
            getAssessmentResults: function(id) {
                return $http.get(ASSESS_API + '/results/' + id).then(r);
            },
            getPatientAssessments: function(patientUserId) {
                return $http.get(ASSESS_API + '/patient-list/' + patientUserId).then(r);
            },
            getPendingCount: function() {
                return $http.get(ASSESS_API + '/stats/pending-count').then(r);
            }
        };

        function r(response) {
            return response.data;
        }
    }
})();
