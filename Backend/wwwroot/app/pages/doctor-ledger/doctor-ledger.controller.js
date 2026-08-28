(function() {
    'use strict';

    angular.module('medicalApp').controller('DoctorLedgerController', DoctorLedgerController);

    DoctorLedgerController.$inject = ['CommissionsService', 'UsersService', 'AuthService', 'toastr'];

    function DoctorLedgerController(CommissionsService, UsersService, AuthService, toastr) {
        var vm = this;

        vm.currentUser = AuthService.getUser() || {};
        vm.doctors = [];
        vm.selectedDoctorId = vm.currentUser.role === 'Doctor' ? vm.currentUser.userID : null;
        vm.ledger = null;
        vm.commissions = [];
        vm.loading = true;
        vm.showCommissionModal = false;

        vm.newCommission = {
            doctorId: null,
            commissionType: 'Percentage',
            value: 50.00,
            specialty: ''
        };

        // Date range
        vm.dateRange = 'month'; // 'today', 'week', 'month', 'custom'
        vm.fromDate = null;
        vm.toDate = null;

        // Functions
        vm.loadLedger = loadLedger;
        vm.loadDoctors = loadDoctors;
        vm.loadCommissions = loadCommissions;
        vm.openCommissionModal = openCommissionModal;
        vm.saveCommission = saveCommission;
        vm.setDateFilter = setDateFilter;

        activate();

        function activate() {
            if (vm.currentUser.role === 'Admin') {
                loadDoctors();
                loadCommissions();
            } else {
                loadLedger();
            }
        }

        function loadDoctors() {
            UsersService.getUsers({ role: 'Doctor', pageSize: 100 }).then(function(res) {
                vm.doctors = res.data || [];
                if (vm.doctors.length > 0 && !vm.selectedDoctorId) {
                    vm.selectedDoctorId = vm.doctors[0].userID;
                }
                loadLedger();
            });
        }

        function loadCommissions() {
            CommissionsService.getCommissions().then(function(res) {
                if (res.success) {
                    vm.commissions = res.data;
                }
            });
        }

        function setDateFilter(range) {
            vm.dateRange = range;
            var today = new Date();

            if (range === 'today') {
                vm.fromDate = today;
                vm.toDate = today;
            } else if (range === 'week') {
                var first = today.getDate() - today.getDay();
                vm.fromDate = new Date(today.setDate(first));
                vm.toDate = new Date();
            } else if (range === 'month') {
                vm.fromDate = new Date(today.getFullYear(), today.getMonth(), 1);
                vm.toDate = new Date();
            }
            loadLedger();
        }

        function loadLedger() {
            if (!vm.selectedDoctorId && vm.currentUser.role !== 'Doctor') return;

            var targetId = vm.currentUser.role === 'Doctor' ? vm.currentUser.userID : vm.selectedDoctorId;
            vm.loading = true;

            var fDate = vm.fromDate ? vm.fromDate.toISOString().split('T')[0] : null;
            var tDate = vm.toDate ? vm.toDate.toISOString().split('T')[0] : null;

            CommissionsService.getDoctorLedger(targetId, fDate, tDate).then(function(res) {
                if (res.success) {
                    vm.ledger = res.data;
                } else {
                    toastr.error(res.message);
                }
            }).catch(function() {
                toastr.error('حدث خطأ في تحميل كشف الأرباح والمستحقات');
            }).finally(function() {
                vm.loading = false;
            });
        }

        function openCommissionModal(doc) {
            if (doc) {
                vm.newCommission.doctorId = doc.userID;
                var existing = vm.commissions.find(function(c) { return c.doctorID === doc.userID; });
                if (existing) {
                    vm.newCommission.commissionType = existing.commissionType;
                    vm.newCommission.value = existing.value;
                } else {
                    vm.newCommission.commissionType = 'Percentage';
                    vm.newCommission.value = 50.00;
                }
            }
            vm.showCommissionModal = true;
        }

        function saveCommission() {
            if (!vm.newCommission.doctorId) {
                toastr.warning('اختر الطبيب أولاً');
                return;
            }

            CommissionsService.setCommission({
                doctorID: vm.newCommission.doctorId,
                commissionType: vm.newCommission.commissionType,
                value: vm.newCommission.value,
                specialty: vm.newCommission.specialty
            }).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showCommissionModal = false;
                    loadCommissions();
                    loadLedger();
                } else {
                    toastr.error(res.message);
                }
            });
        }
    }
})();
