(function() {
    'use strict';

    angular.module('medicalApp').controller('PatientAssessmentsController', PatientAssessmentsController);

    PatientAssessmentsController.$inject = ['PsychiatricService', 'AuthService', 'toastr'];

    function PatientAssessmentsController(PsychiatricService, AuthService, toastr) {
        var vm = this;

        vm.activeTab = 'pending';
        vm.pending = [];
        vm.completed = [];
        vm.submitting = false;
        vm.showMyResult = false;
        vm.myResult = null;

        vm.submitAssessment = submitAssessment;
        vm.viewMyResult = viewMyResult;

        activate();

        function activate() {
            loadPending();
            loadCompleted();
        }

        function loadPending() {
            PsychiatricService.getPatientPending()
                .then(function(res) {
                    if (res.success) {
                        vm.pending = (res.data || []).map(function(a) {
                            a.parsedSchema = [];
                            a.answers = {};
                            try { a.parsedSchema = JSON.parse(a.schemaJson || '[]'); } catch(e) {}
                            return a;
                        });
                    }
                });
        }

        function loadCompleted() {
            PsychiatricService.getPatientCompleted()
                .then(function(res) {
                    if (res.success) {
                        vm.completed = res.data || [];
                    }
                });
        }

        function submitAssessment(assessment) {
            vm.submitting = true;
            var data = {
                answersJson: JSON.stringify(assessment.answers)
            };
            PsychiatricService.submitAnswers(assessment.assessmentID, data)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم إرسال إجابات الاستبيان بنجاح');
                        loadPending();
                        loadCompleted();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) { toastr.error(err.data ? err.data.message : 'حدث خطأ'); })
                .finally(function() { vm.submitting = false; });
        }

        function viewMyResult(assessment) {
            PsychiatricService.getAssessmentResults(assessment.assessmentID)
                .then(function(res) {
                    if (res.success) {
                        vm.myResult = res.data;
                        vm.myResult.parsedAnswers = {};
                        vm.myResult.parsedSchema = [];
                        try {
                            vm.myResult.parsedAnswers = JSON.parse(res.data.answersJson || '{}');
                            vm.myResult.parsedSchema = JSON.parse(res.data.templateSchema || '[]');
                        } catch(e) {}
                        vm.showMyResult = true;
                    }
                });
        }
    }
})();
