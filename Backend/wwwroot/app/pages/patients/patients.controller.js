(function() {
    'use strict';

    angular.module('medicalApp').controller('PatientsController', PatientsController);

    PatientsController.$inject = ['UsersService', 'AuthService', 'toastr'];

    function PatientsController(UsersService, AuthService, toastr) {
        var vm = this;

        vm.currentUser = AuthService.getUser() || {};
        vm.patients = [];
        vm.searchQuery = '';
        vm.page = 1;
        vm.pageSize = 15;
        vm.totalPages = 1;
        vm.totalCount = 0;
        vm.loading = false;

        // Modals
        vm.showFormModal = false;
        vm.editMode = false;
        vm.showMergeModal = false;

        vm.form = {};
        vm.merge = { sourcePatientId: null, targetPatientId: null };

        vm.loadPatients = loadPatients;
        vm.goPage = goPage;
        vm.openAddModal = openAddModal;
        vm.openEditModal = openEditModal;
        vm.closeFormModal = closeFormModal;
        vm.submitForm = submitForm;
        vm.selectMergeSource = selectMergeSource;
        vm.submitMerge = submitMerge;

        activate();

        function activate() {
            loadPatients();
        }

        function loadPatients() {
            vm.loading = true;
            var params = { page: vm.page, pageSize: vm.pageSize };
            if (vm.searchQuery) params.search = vm.searchQuery;

            UsersService.getPatients(params).then(function(res) {
                vm.patients = res.data || [];
                vm.totalCount = res.totalCount || 0;
                vm.totalPages = res.totalPages || 1;
            }).finally(function() {
                vm.loading = false;
            });
        }

        function goPage(p) {
            if (p < 1 || p > vm.totalPages) return;
            vm.page = p;
            loadPatients();
        }

        function resetForm() {
            vm.form = {
                firstName: '',
                fatherName: '',
                grandfatherName: '',
                familyName: '',
                fileNumber: '',
                phone: '',
                gender: '',
                dateOfBirth: null,
                bloodType: '',
                chronicDiseases: '',
                allergies: '',
                generalNotes: ''
            };
        }

        function openAddModal() {
            resetForm();
            vm.editMode = false;
            // طلب رقم الملف التالي تلقائياً
            UsersService.getNextFileNumber().then(function(res) {
                if (res.success && res.data) vm.form.fileNumber = res.data.fileNumber;
            });
            vm.showFormModal = true;
        }

        function openEditModal(p) {
            vm.editMode = true;
            vm.form = {
                patientId: p.patientID,
                firstName: p.firstName || '',
                fatherName: p.fatherName || '',
                grandfatherName: p.grandfatherName || '',
                familyName: p.familyName || '',
                fileNumber: p.fileNumber || '',
                phone: p.phone || '',
                gender: p.gender || '',
                dateOfBirth: p.dateOfBirth ? new Date(p.dateOfBirth) : null,
                bloodType: p.bloodType || '',
                chronicDiseases: p.chronicDiseases || '',
                allergies: p.allergies || '',
                generalNotes: p.generalNotes || ''
            };
            vm.showFormModal = true;
        }

        function closeFormModal() {
            vm.showFormModal = false;
        }

        function submitForm() {
            if (!vm.form.firstName) {
                toastr.warning('الاسم الأول مطلوب');
                return;
            }

            var payload = {
                firstName: vm.form.firstName,
                fatherName: vm.form.fatherName,
                grandfatherName: vm.form.grandfatherName,
                familyName: vm.form.familyName,
                gender: vm.form.gender,
                dateOfBirth: vm.form.dateOfBirth || null,
                bloodType: vm.form.bloodType,
                chronicDiseases: vm.form.chronicDiseases,
                allergies: vm.form.allergies,
                generalNotes: vm.form.generalNotes
            };

            // إعادة بناء الاسم الكامل من المكونات على الواجهة لعرض فوري
            var parts = [vm.form.firstName, vm.form.fatherName, vm.form.grandfatherName, vm.form.familyName]
                .filter(function(x) { return x && x.trim(); });
            payload.fullName = parts.join(' ');

            var op = vm.editMode
                ? UsersService.updatePatient(vm.form.patientId, payload)
                : createWalkInPatient(payload);

            op.then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showFormModal = false;
                    loadPatients();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        // إنشاء ملف مريض جديد: يُنشئ حساب مريض عبر API المستخدمين (أدمن فقط)
        function createWalkInPatient(payload) {
            if (vm.currentUser.role !== 'Admin') {
                toastr.warning('إنشاء ملف جديد متاح للمدير فقط — يمكنك تعديل ملف موجود أو استخدام الحجز السريع.');
                return { then: function() {} };
            }
            var email = 'patient_' + Date.now() + '@clinic.com';
            return UsersService.createUser({
                fullName: payload.fullName,
                email: email,
                password: 'Patient123!',
                phone: payload.phone,
                role: 'Patient',
                firstName: payload.firstName,
                fatherName: payload.fatherName,
                grandfatherName: payload.grandfatherName,
                familyName: payload.familyName,
                gender: payload.gender,
                dateOfBirth: payload.dateOfBirth,
                bloodType: payload.bloodType
            });
        }

        function selectMergeSource(p) {
            vm.merge = { sourcePatientId: p.patientID, targetPatientId: null };
            vm.showMergeModal = true;
        }

        function submitMerge() {
            if (!vm.merge.sourcePatientId || !vm.merge.targetPatientId) {
                toastr.warning('يرجى اختيار الملفين (المصدر والوجهة)');
                return;
            }
            if (vm.merge.sourcePatientId === vm.merge.targetPatientId) {
                toastr.warning('لا يمكن دمج الملف مع نفسه');
                return;
            }

            var source = vm.patients.filter(function(p) { return p.patientID === vm.merge.sourcePatientId; })[0];
            var target = vm.patients.filter(function(p) { return p.patientID === vm.merge.targetPatientId; })[0];

            if (!confirm('سيتم دمج ' + source.fullName + ' (' + source.fileNumber + ') إلى ' + target.fullName + ' (' + target.fileNumber + '). متابعة؟')) {
                return;
            }

            UsersService.mergePatients({
                sourcePatientID: vm.merge.sourcePatientId,
                targetPatientID: vm.merge.targetPatientId
            }).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showMergeModal = false;
                    vm.merge = { sourcePatientId: null, targetPatientId: null };
                    loadPatients();
                } else {
                    toastr.error(res.message);
                }
            });
        }
    }
})();
