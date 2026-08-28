(function() {
    'use strict';

    angular.module('medicalApp').controller('RegisterController', RegisterController);

    RegisterController.$inject = ['AuthService', '$state', 'toastr'];

    function RegisterController(AuthService, $state, toastr) {
        var vm = this;

        vm.userData = {
            fullName: '',
            email: '',
            password: '',
            phone: '',
            role: 'Patient',
            specialty: '',
            licenseNumber: '',
            bloodType: '',
            gender: ''
        };
        vm.loading = false;
        vm.error = null;
        vm.showPassword = false;

        vm.register = register;

        // === Claim Account Logic ===
        vm.mode = 'new'; // 'new' or 'claim'
        vm.claimStep = 1;
        vm.claimData = { phone: '', password: '', email: '' };
        vm.foundPatient = null;
        vm.checkClaimPhone = checkClaimPhone;
        vm.submitClaimAccount = submitClaimAccount;

        function checkClaimPhone() {
            if (!vm.claimData.phone) {
                toastr.warning('يرجى إدخال رقم الهاتف المسجل لدى العيادة');
                return;
            }
            vm.loading = true;
            vm.error = null;
            AuthService.checkClaimAccount(vm.claimData.phone)
                .then(function(res) {
                    if (res.success) {
                        vm.foundPatient = res.data;
                        vm.claimData.email = res.data.email && !res.data.email.includes('@clinic.com') ? res.data.email : '';
                        vm.claimStep = 2;
                        toastr.success(res.message);
                    } else {
                        vm.error = res.message;
                    }
                })
                .catch(function(err) {
                    vm.error = err.data ? err.data.message : 'لم نجد حساباً مسجلاً لدى العيادة بهذا الرقم';
                })
                .finally(function() {
                    vm.loading = false;
                });
        }

        function submitClaimAccount() {
            if (!vm.claimData.password) {
                toastr.warning('يرجى تحديد كلمة المرور السريّة الخاصة بك');
                return;
            }
            vm.loading = true;
            vm.error = null;
            AuthService.claimAccount(vm.claimData)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message, 'أهلاً بك!');
                        $state.go('app.dashboard');
                    } else {
                        vm.error = res.message;
                    }
                })
                .catch(function(err) {
                    vm.error = err.data ? err.data.message : 'حدث خطأ أثناء تفعيل الحساب';
                })
                .finally(function() {
                    vm.loading = false;
                });
        }

        function register() {
            vm.loading = true;
            vm.error = null;

            // Handle custom specialty when "أخرى" is selected
            var data = angular.copy(vm.userData);
            if (data.role === 'Doctor' && data.specialty === 'أخرى' && data.customSpecialty) {
                data.specialty = data.customSpecialty;
            }
            delete data.customSpecialty;

            AuthService.register(data)
                .then(function(response) {
                    if (response.success) {
                        toastr.success(response.message, 'تم بنجاح');
                        $state.go('app.dashboard');
                    } else {
                        vm.error = response.message;
                    }
                })
                .catch(function(err) {
                    vm.error = err.data ? err.data.message : 'حدث خطأ في الاتصال بالخادم';
                })
                .finally(function() {
                    vm.loading = false;
                });
        }
    }
})();
