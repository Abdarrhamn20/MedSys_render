(function() {
    'use strict';

    angular.module('medicalApp')
        .config(appConfig)
        .run(appRun);

    // === App Configuration ===
    appConfig.$inject = ['$httpProvider'];

    function appConfig($httpProvider) {
        // Register HTTP Interceptor
        $httpProvider.interceptors.push('AuthInterceptor');
    }

    // === App Run - Route Protection ===
    appRun.$inject = ['$rootScope', '$state', 'AuthService', '$window'];

    function appRun($rootScope, $state, AuthService, $window) {

        $rootScope.$on('$stateChangeStart', function(event, toState) {
            var data = toState.data || {};
            var requiresAuth = data.requiresAuth === true;
            var isLoggedIn = AuthService.isLoggedIn();

            // حراسة كل مسارات التطبيق الداخلية حتى لو لم تَرِث data.requiresAuth من الوالد (abstract)
            var isProtectedRoute = toState.name === 'app' || toState.name.indexOf('app.') === 0;

            if ((requiresAuth || isProtectedRoute) && !isLoggedIn) {
                event.preventDefault();
                $state.go('login');
                return;
            }

            // حراسة الأدوار: رفض الوصول لمسارات لا يملك المستخدم دوراً فيها
            var allowedRoles = data.roles;
            if (allowedRoles && allowedRoles.length) {
                var user = AuthService.getUser();
                var role = user ? user.role : null;
                if (allowedRoles.indexOf(role) === -1) {
                    event.preventDefault();
                    $state.go('app.dashboard');
                    return;
                }
            }

            // Redirect logged-in users away from login/register
            if (!requiresAuth && isLoggedIn && (toState.name === 'login' || toState.name === 'register')) {
                event.preventDefault();
                $state.go('app.dashboard');
                return;
            }

            // Set page title
            if (toState.data && toState.data.pageTitle) {
                $rootScope.pageTitle = toState.data.pageTitle;
                $rootScope.pageIcon = toState.data.icon || 'fa-home';
            }
        });

        // Set current user info in rootScope
        if (AuthService.isLoggedIn()) {
            $rootScope.currentUser = AuthService.getUser();
        }

        // === جلسة واحدة لكل متصفح (سلوك متسق) ===
        // عند تغيّر الجلسة من تبويب آخر (تسجيل دخول/خروج)، يعيد التطبيق التحميل بنظافة
        // ليمنع الحالة المختلطة (اسم حساب آخر + صلاحيات هذا التبويب).
        // يُسمح بفتح التبويب الثاني لنفس المستخدم دون إعادة تحميل.
        $window.addEventListener('storage', function(event) {
            if (!event || (event.key !== 'medical_token' && event.key !== 'medical_user')) return;

            // تسجيل خروج من تبويب آخر — انهِ هذه الجلسة أيضاً بنظافة
            if (event.newValue === null) {
                $window.location.reload();
                return;
            }

            var localUser = AuthService.getUser();
            var incomingUser = null;
            try {
                incomingUser = event.key === 'medical_user' ? JSON.parse(event.newValue) : null;
            } catch (e) {
                incomingUser = null;
            }

            // عند تغيّر التوكن: المستخدم الجديد يُعرف من localStorage مباشرة (كتب التوكن قبل المستخدم)
            if (!incomingUser) {
                var newUser = AuthService.getUser();
                if (newUser && localUser && newUser.UserID === localUser.UserID) return;
                incomingUser = newUser;
            }

            // نفس الحساب — لا حاجة لإعادة التحميل
            if (localUser && incomingUser && localUser.UserID === incomingUser.UserID) return;

            // حساب مختلف في تبويب آخر — أعد البناء كاملاً بالحساب الجديد
            $window.location.reload();
        });

        // === شبكة أمان: عند تثبيت نسخة SW جديدة، أعد التحميل تلقائياً ===
        if ('serviceWorker' in $window.navigator && $window.navigator.serviceWorker) {
            $window.navigator.serviceWorker.addEventListener('message', function(event) {
                if (event.data && event.data.type === 'ivs-sw-updated') {
                    $window.location.reload();
                }
            });
        }

        // === مراقبة دورية للجلسة (شبكة أمان إضافية) ===
        // تكتشف أي تغيير في الجلسة من تبويب آخر حتى لو فات حدث storage
        var lastSeenSessionKey = AuthService.getSessionKey();
        $window.setInterval(function() {
            var current = AuthService.getSessionKey();
            if (current === lastSeenSessionKey) return;
            lastSeenSessionKey = current;
            // إن لم يكن هذا التبويب هو من كتب التغيير → جلسة خارجية → أعد البناء
            if (current !== AuthService.getLastWrittenSessionKey()) {
                $window.location.reload();
            }
        }, 1000);
    }
})();
