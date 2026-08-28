(function() {
    'use strict';

    angular.module('medicalApp').controller('LoginController', LoginController);

    LoginController.$inject = ['AuthService', '$state', 'toastr', '$window'];

    function LoginController(AuthService, $state, toastr, $window) {
        var vm = this;

        vm.credentials = { email: '', password: '' };
        vm.loading = false;
        vm.error = null;
        vm.showPassword = false;

        vm.login = login;

        function login() {
            vm.loading = true;
            vm.error = null;

            // اعتراض ودود: جلسة نشطة بحساب مختلف في هذا المتصفح
            var activeUser = AuthService.getUser();
            if (AuthService.isLoggedIn() && activeUser && vm.credentials.email &&
                activeUser.Email && activeUser.Email.toLowerCase() !== vm.credentials.email.toLowerCase()) {
                var confirmed = $window.confirm(
                    'يوجد حساب نشط آخر باسم "' + activeUser.FullName + '" (' + activeUser.Email + ').' +
                    '\nالدخول بهذا الحساب سيُنهي الجلسة الحالية في كل النوافذ الأخرى.' +
                    '\n\nهل تريد المتابعة؟'
                );
                if (!confirmed) {
                    vm.loading = false;
                    return;
                }
            }

            AuthService.login(vm.credentials)
                .then(function(response) {
                    if (response.success) {
                        toastr.success(response.message, 'مرحباً');
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
