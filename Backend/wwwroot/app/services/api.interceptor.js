(function() {
    'use strict';

    angular.module('medicalApp').factory('AuthInterceptor', AuthInterceptor);

    AuthInterceptor.$inject = ['$window', '$q', '$injector'];

    function AuthInterceptor($window, $q, $injector) {
        return {
            // Attach JWT token to every request
            request: function(config) {
                var token = $window.localStorage.getItem('medical_token');
                if (token) {
                    config.headers = config.headers || {};
                    config.headers.Authorization = 'Bearer ' + token;
                }
                return config;
            },

            // Handle Errors
            responseError: function(rejection) {
                var toastr = $injector.get('toastr');

                // السماح لبعض الطلبات بالتعامل مع أخطائها بنفسها دون إظهار إشعار (مثل فحص بطاقة الموظف)
                if (rejection.config && rejection.config.skipErrorToast) {
                    return $q.reject(rejection);
                }

                if (rejection.status === 401) {
                    $window.localStorage.removeItem('medical_token');
                    $window.localStorage.removeItem('medical_user');
                    var $state = $injector.get('$state');
                    $state.go('login');
                    toastr.error('انتهت صلاحية الجلسة، الرجاء تسجيل الدخول مجدداً');
                } else if (rejection.status === 403) {
                    var msg = (rejection.data && rejection.data.message) ? rejection.data.message : 'ليس لديك الصلاحية للقيام بهذا الإجراء';
                    toastr.warning(msg);
                } else if (rejection.status === 400) {
                    // ASP.NET Core Validation Errors mapping
                    if (rejection.data && rejection.data.errors) {
                        var firstErrorKey = Object.keys(rejection.data.errors)[0];
                        toastr.error(rejection.data.errors[firstErrorKey][0]);
                    } else if (rejection.data && rejection.data.message) {
                        toastr.error(rejection.data.message);
                    } else {
                        toastr.error('يوجد خطأ في البيانات المدخلة');
                    }
                } else if (rejection.status === 404) {
                    toastr.warning('العنصر المطلوب غير موجود');
                } else if (rejection.status === 500) {
                    toastr.error('حدث خطأ في الخادم، الرجاء المحاولة لاحقاً');
                } else if (rejection.status === -1) {
                    toastr.error('لا يوجد اتصال بالخادم');
                }

                return $q.reject(rejection);
            }
        };
    }
})();
