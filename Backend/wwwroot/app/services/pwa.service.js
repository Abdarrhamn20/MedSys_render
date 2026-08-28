(function() {
    'use strict';

    angular.module('medicalApp').factory('PWAService', PWAService);

    PWAService.$inject = ['$http', '$window', '$rootScope'];

    function PWAService($http, $window, $rootScope) {
        var deferredPrompt = null;
        var pwaEnabled = false;

        var service = {
            checkAndInitPWA: checkAndInitPWA,
            installApp: installApp,
            togglePWASetting: togglePWASetting,
            isPWAEnabled: function() { return pwaEnabled; },
            canInstall: false
        };

        return service;

        function checkAndInitPWA() {
            return $http.get('/api/settings/mobile-pwa').then(function(res) {
                var data = (res && res.data) ? res.data : (res || {});
                pwaEnabled = !!(data.enabled || (data.data && data.data.enabled));

                if (pwaEnabled) {
                    injectManifest();
                    registerServiceWorker();
                    setupInstallPrompt();
                } else {
                    removeManifest();
                }
                return pwaEnabled;
            }).catch(function() {
                pwaEnabled = false;
                return false;
            });
        }

        function injectManifest() {
            if (!document.querySelector('link[rel="manifest"]')) {
                var link = document.createElement('link');
                link.rel = 'manifest';
                link.href = 'manifest.json';
                document.head.appendChild(link);
            }
        }

        function removeManifest() {
            var link = document.querySelector('link[rel="manifest"]');
            if (link) {
                link.parentNode.removeChild(link);
            }
        }

        function registerServiceWorker() {
            if ('serviceWorker' in navigator) {
                navigator.serviceWorker.register('service-worker.js').then(function(reg) {
                    console.log('PWA ServiceWorker registered successfully:', reg.scope);
                }).catch(function(err) {
                    console.warn('PWA ServiceWorker registration failed:', err);
                });
            }
        }

        function setupInstallPrompt() {
            $window.addEventListener('beforeinstallprompt', function(e) {
                if (!pwaEnabled) return;
                e.preventDefault();
                deferredPrompt = e;
                service.canInstall = true;
                $rootScope.$broadcast('pwa:can-install', true);
                $rootScope.$applyAsync();
            });
        }

        function installApp() {
            if (!pwaEnabled) {
                var unpwaMsg = '🔒 ميزة تطبيق الموبايل غير مفعّلة حالياً لهذه العيادة!\n\n' +
                               'إن ميزة تثبيت واستخدام المنظومة كتطبيق هاتف ذكي مستقل غير مفعّلة على سيرفر هذه العيادة.\n\n' +
                               '📢 يُرجى التواصل مع إدارة النظام/المطور للاتفاق على شراء وتفعيل ترخيص نسختك الخاصة من تطبيق الموبايل.';
                alert(unpwaMsg);
                return;
            }

            if (deferredPrompt) {
                deferredPrompt.prompt();
                deferredPrompt.userChoice.then(function(choiceResult) {
                    if (choiceResult.outcome === 'accepted') {
                        console.log('User accepted the PWA install prompt');
                    }
                    deferredPrompt = null;
                    service.canInstall = false;
                    $rootScope.$broadcast('pwa:can-install', false);
                    $rootScope.$applyAsync();
                });
            } else {
                var isIOS = /iPad|iPhone|iPod/.test(navigator.userAgent) && !window.MSStream;
                var msg = isIOS 
                    ? '📲 لتثبيت التطبيق على آيفون: انقر زر المشاركة (⎘) أسفل المتصفح ثم اختر "إضافة إلى الشاشة الرئيسية (Add to Home Screen)".'
                    : '📲 لتثبيت التطبيق على أندرويد: انقر خيارات المتصفح (⋮) أعلى اليسار ثم اختر "تثبيت التطبيق" أو "إضافة إلى الشاشة الرئيسية".';
                
                alert(msg);
            }
        }

        function togglePWASetting(enabled) {
            return $http.post('/api/settings/mobile-pwa', { enabled: enabled }).then(function(res) {
                return checkAndInitPWA();
            });
        }
    }
})();
