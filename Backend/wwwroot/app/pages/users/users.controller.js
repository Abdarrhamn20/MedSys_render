(function() {
    'use strict';

    angular.module('medicalApp').controller('UsersController', UsersController);

    UsersController.$inject = ['UsersService', 'TreasuryService', 'toastr'];

    function UsersController(UsersService, TreasuryService, toastr) {
        var vm = this;

        vm.users = [];
        vm.userStats = {};
        vm.loading = true;
        vm.saving = false;
        vm.searchQuery = '';
        vm.filterRole = '';
        vm.page = 1;
        vm.pageSize = 10;
        vm.totalCount = 0;
        vm.totalPages = 0;

        // Modals
        vm.showAddModal = false;
        vm.showDeleteModal = false;
        vm.showProfileModal = false;
        vm.editingUser = null;
        vm.selectedUser = null;
        vm.selectedProfileUser = null;
        vm.newUser = getEmptyUser();

        // Functions
        vm.loadUsers = loadUsers;
        vm.saveUser = saveUser;
        vm.editUser = editUser;
        vm.deleteUser = deleteUser;
        vm.confirmDelete = confirmDelete;
        vm.toggleActive = toggleActive;
        vm.viewUserProfile = viewUserProfile;
        vm.getRoleAr = getRoleAr;

        activate();

        function activate() {
            loadUsers();
            loadStats();
            loadTreasuries();
        }

        function loadTreasuries() {
            TreasuryService.getTreasuries().then(function(res) {
                vm.treasuries = (res && res.data) ? res.data : [];
            }).catch(function() {
                vm.treasuries = [];
            });
        }

        function loadUsers() {
            vm.loading = true;
            UsersService.getUsers({
                search: vm.searchQuery,
                role: vm.filterRole,
                page: 1,
                pageSize: 500
            }).then(function(response) {
                vm.users = response.data;
                vm.totalCount = response.totalCount;
                vm.totalPages = response.totalPages;
            }).catch(function() {
                toastr.error('حدث خطأ في تحميل البيانات');
            }).finally(function() {
                vm.loading = false;
            });
        }

        function loadStats() {
            UsersService.getUserStats().then(function(response) {
                if (response.success) vm.userStats = response.data;
            });
        }

        function saveUser() {
            vm.saving = true;
            var promise;

            if (vm.editingUser) {
                promise = UsersService.updateUser(vm.editingUser.userID, vm.newUser);
            } else {
                promise = UsersService.createUser(vm.newUser);
            }

            promise.then(function(response) {
                if (response.success) {
                    toastr.success(response.message);
                    vm.showAddModal = false;
                    vm.newUser = getEmptyUser();
                    vm.editingUser = null;
                    loadUsers();
                    loadStats();
                } else {
                    toastr.error(response.message);
                }
            }).catch(function(err) {
                toastr.error(err.data ? err.data.message : 'حدث خطأ');
            }).finally(function() {
                vm.saving = false;
            });
        }

        function editUser(user) {
            vm.editingUser = user;
            vm.newUser = {
                fullName: user.fullName || user.FullName || '',
                email: user.email || user.Email || '',
                phone: user.phone || user.Phone || '',
                role: user.role || user.Role || 'Patient',
                assignedTreasuryID: user.assignedTreasuryID || null,
                password: '',
                specialty: user.specialty || user.Specialty || (user.doctorProfile ? user.doctorProfile.specialty : ''),
                licenseNumber: user.licenseNumber || user.LicenseNumber || (user.doctorProfile ? user.doctorProfile.licenseNumber : ''),
                consultationFee: user.consultationFee || user.ConsultationFee || (user.doctorProfile ? user.doctorProfile.consultationFee : 0),
                bloodType: user.bloodType || user.BloodType || (user.patientProfile ? user.patientProfile.bloodType : ''),
                gender: user.gender || user.Gender || (user.patientProfile ? user.patientProfile.gender : '')
            };
            vm.showAddModal = true;
        }

        function deleteUser(user) {
            vm.selectedUser = user;
            vm.showDeleteModal = true;
        }

        function confirmDelete() {
            UsersService.deleteUser(vm.selectedUser.userID).then(function(response) {
                if (response.success) {
                    toastr.success(response.message);
                    vm.showDeleteModal = false;
                    loadUsers();
                    loadStats();
                }
            }).catch(function() {
                toastr.error('حدث خطأ في حذف المستخدم');
            });
        }

        function toggleActive(user) {
            UsersService.toggleActive(user.userID).then(function(response) {
                if (response.success) {
                    toastr.success(response.message);
                    loadUsers();
                }
            });
        }

        function viewUserProfile(user) {
            vm.selectedProfileUser = user;
            vm.showProfileModal = true;
        }

        function getRoleAr(role) {
            var roles = { 
                'Admin': 'مدير', 
                'Accountant': 'محاسب',
                'Doctor': 'طبيب', 
                'Patient': 'مريض',
                'Pharmacist': 'صيدلاني',
                'LabTechnician': 'فني مختبرات',
                'Radiologist': 'أخصائي أشعة',
                'Receptionist': 'موظف استقبال',
                'Cashier': 'كاشير',
                'WarehouseKeeper': 'أمين مخزن'
            };
            return roles[role] || role;
        }

        function getEmptyUser() {
            return { fullName: '', email: '', password: '', phone: '', role: 'Patient', assignedTreasuryID: null, specialty: '', licenseNumber: '', consultationFee: 0, bloodType: '', gender: '' };
        }
    }
})();
