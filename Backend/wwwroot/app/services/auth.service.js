(function() {
    'use strict';

    angular.module('medicalApp').factory('AuthService', AuthService);

    AuthService.$inject = ['$http', '$window', '$rootScope'];

    function AuthService($http, $window, $rootScope) {
        var API_URL = '/api/auth';
        var TOKEN_KEY = 'medical_token';
        var USER_KEY = 'medical_user';

        var service = {
            login: login,
            register: register,
            checkClaimAccount: checkClaimAccount,
            claimAccount: claimAccount,
            logout: logout,
            getProfile: getProfile,
            changePassword: changePassword,
            isLoggedIn: isLoggedIn,
            getToken: getToken,
            getUser: getUser,
            getUserRole: getUserRole,
            getSessionKey: getSessionKey,
            getLastWrittenSessionKey: function() { return lastWrittenSessionKey; }
        };

        // آخر جلسة كتبها هذا التبويب نفسه — للتمييز بين التغيير الذاتي والتغيير من تبويب آخر
        var lastWrittenSessionKey = '';

        return service;

        function getSessionKey() {
            return getToken() + '|' + (getUser() ? getUser().UserID : '');
        }

        function checkClaimAccount(phone) {
            return $http.post(API_URL + '/check-claim', { phone: phone })
                .then(function(response) { return response.data; });
        }

        function claimAccount(data) {
            return $http.post(API_URL + '/claim', data)
                .then(function(response) {
                    if (response.data.success) {
                        $window.localStorage.setItem(TOKEN_KEY, response.data.data.token);
                        $window.localStorage.setItem(USER_KEY, JSON.stringify(response.data.data.user));
                        $rootScope.currentUser = response.data.data.user;
                        lastWrittenSessionKey = getSessionKey();
                    }
                    return response.data;
                });
        }

        // === Login ===
        function login(credentials) {
            return $http.post(API_URL + '/login', credentials)
                .then(function(response) {
                    if (response.data.success) {
                        $window.localStorage.setItem(TOKEN_KEY, response.data.data.token);
                        $window.localStorage.setItem(USER_KEY, JSON.stringify(response.data.data.user));
                        $rootScope.currentUser = response.data.data.user;
                        lastWrittenSessionKey = getSessionKey();
                    }
                    return response.data;
                });
        }

        // === Register ===
        function register(userData) {
            return $http.post(API_URL + '/register', userData)
                .then(function(response) {
                    if (response.data.success) {
                        $window.localStorage.setItem(TOKEN_KEY, response.data.data.token);
                        $window.localStorage.setItem(USER_KEY, JSON.stringify(response.data.data.user));
                        $rootScope.currentUser = response.data.data.user;
                        lastWrittenSessionKey = getSessionKey();
                    }
                    return response.data;
                });
        }

        // === Logout ===
        function logout() {
            $window.localStorage.removeItem(TOKEN_KEY);
            $window.localStorage.removeItem(USER_KEY);
            $rootScope.currentUser = null;
            lastWrittenSessionKey = '';
        }

        // === Get Profile ===
        function getProfile() {
            return $http.get(API_URL + '/profile')
                .then(function(response) { return response.data; });
        }

        // === Change Password ===
        function changePassword(data) {
            return $http.post(API_URL + '/change-password', data)
                .then(function(response) { return response.data; });
        }

        // === Helpers ===
        function isLoggedIn() {
            return !!$window.localStorage.getItem(TOKEN_KEY);
        }

        function getToken() {
            return $window.localStorage.getItem(TOKEN_KEY);
        }

        function getUser() {
            var user = $window.localStorage.getItem(USER_KEY);
            return user ? JSON.parse(user) : null;
        }

        function getUserRole() {
            var user = getUser();
            return user ? user.role : null;
        }
    }
})();
