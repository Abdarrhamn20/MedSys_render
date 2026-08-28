(function() {
    'use strict';

    angular.module('medicalApp').factory('PushService', PushService);

    PushService.$inject = ['$http', '$window'];

    function PushService($http, $window) {
        var API = '/api/push';
        var registered = false;

        var service = {
            init: init,
            enable: enable,
            status: status,
            requestPermission: enable,
            unsubscribe: unsubscribe,
            isSupported: isSupported
        };

        return service;

        function isSupported() {
            return 'serviceWorker' in $window.navigator && 'PushManager' in $window;
        }

        function isIOS() {
            return /iPad|iPhone|iPod/.test($window.navigator.userAgent) && !$window.MSStream;
        }

        function isStandalone() {
            return $window.matchMedia('(display-mode: standalone)').matches || $window.navigator.standalone === true;
        }

        // مزامنة صامتة: يتحقق من اشتراك سابق ويجدد تسجيله فقط — لا يطلب الإذن تلقائياً
        function init() {
            if (!service.isSupported()) return Promise.resolve(false);

            return $http.get(API + '/vapid-public-key').then(function(res) {
                var data = (res && res.data && res.data.data) ? res.data.data : {};
                if (!data.enabled || !data.publicKey) return false;

                return navigator.serviceWorker.ready.then(function(reg) {
                    return reg.pushManager.getSubscription().then(function(existing) {
                        if (existing) {
                            return saveSubscription(existing);
                        }
                        // لا نطلب الإذن هنا — فقط إذا كان ممنوحاً مسبقاً
                        if ($window.Notification && $window.Notification.permission === 'granted') {
                            return subscribeNew(reg, data.publicKey);
                        }
                        return false;
                    });
                });
            }).catch(function() {
                return false;
            });
        }

        // تفعيل صريح بإذن المستخدم (زر في الواجهة) — يدعم اشتراط آيفون
        function enable() {
            if (!service.isSupported()) {
                return Promise.resolve({ ok: false, reason: 'unsupported' });
            }

            return $http.get(API + '/vapid-public-key').then(function(res) {
                var data = (res && res.data && res.data.data) ? res.data.data : {};
                if (!data.enabled || !data.publicKey) {
                    return { ok: false, reason: 'disabled' };
                }

                // آيفون: الإشعارات تعمل فقط بعد تثبيت التطبيق (إضافة إلى الشاشة الرئيسية)
                if (isIOS() && !isStandalone() && $window.Notification && $window.Notification.permission !== 'granted') {
                    return { ok: false, reason: 'needInstall' };
                }

                return navigator.serviceWorker.ready.then(function(reg) {
                    return reg.pushManager.getSubscription().then(function(existing) {
                        if (existing) {
                            return saveSubscription(existing).then(function() {
                                return { ok: true, subscribed: true };
                            });
                        }
                        if ($window.Notification && $window.Notification.permission === 'default') {
                            return $window.Notification.requestPermission().then(function(permission) {
                                if (permission !== 'granted') return { ok: false, reason: 'denied' };
                                return subscribeNew(reg, data.publicKey).then(function(ok) {
                                    return ok ? { ok: true } : { ok: false, reason: 'error' };
                                });
                            });
                        }
                        if ($window.Notification && $window.Notification.permission === 'denied') {
                            return { ok: false, reason: 'denied' };
                        }
                        return subscribeNew(reg, data.publicKey).then(function(ok) {
                            return ok ? { ok: true } : { ok: false, reason: 'error' };
                        });
                    });
                });
            }).catch(function() {
                return { ok: false, reason: 'error' };
            });
        }

        function status() {
            return {
                supported: service.isSupported(),
                permission: ($window.Notification && $window.Notification.permission) ? $window.Notification.permission : 'unsupported',
                installed: isStandalone(),
                isIOS: isIOS()
            };
        }

        function registerAndSubscribe(vapidPublicKey) {
            var registrationPromise = registered
                ? navigator.serviceWorker.ready
                : navigator.serviceWorker.register('service-worker.js').then(function(reg) {
                      registered = true;
                      return reg;
                  });

            return registrationPromise.then(function(reg) {
                return reg.pushManager.getSubscription().then(function(existing) {
                    if (existing) {
                        return saveSubscription(existing);
                    }
                    return subscribeNew(reg, vapidPublicKey);
                });
            }).catch(function(err) {
                console.warn('Push registration failed:', err);
                return false;
            });
        }

        function subscribeNew(reg, vapidPublicKey) {
            return $window.Notification.requestPermission().then(function(permission) {
                if (permission !== 'granted') return false;
                return reg.pushManager.subscribe({
                    userVisibleOnly: true,
                    applicationServerKey: urlBase64ToUint8Array(vapidPublicKey)
                }).then(saveSubscription).catch(function(err) {
                    console.warn('Push subscribe failed:', err);
                    return false;
                });
            });
        }

        function saveSubscription(subscription) {
            var body = {
                endpoint: subscription.endpoint,
                p256dh: btoa(String.fromCharCode.apply(null, new Uint8Array(subscription.getKey('p256dh')))),
                auth: btoa(String.fromCharCode.apply(null, new Uint8Array(subscription.getKey('auth')))),
                userAgent: $window.navigator.userAgent
            };
            return $http.post(API + '/subscribe', body).then(function() {
                return true;
            }).catch(function(err) {
                console.warn('Saving push subscription failed:', err);
                return false;
            });
        }

        function unsubscribe() {
            if (!isSupported()) return Promise.resolve();
            return navigator.serviceWorker.getRegistration().then(function(reg) {
                if (!reg) return;
                return reg.pushManager.getSubscription().then(function(subscription) {
                    if (!subscription) return;
                    var endpoint = subscription.endpoint;
                    return subscription.unsubscribe().then(function() {
                        return $http.post(API + '/unsubscribe', { endpoint: endpoint }).catch(function() {});
                    });
                });
            });
        }

        function urlBase64ToUint8Array(base64String) {
            var padding = '='.repeat((4 - base64String.length % 4) % 4);
            var base64 = (base64String + padding).replace(/-/g, '+').replace(/_/g, '/');
            var rawData = $window.atob(base64);
            var outputArray = new Uint8Array(rawData.length);
            for (var i = 0; i < rawData.length; ++i) {
                outputArray[i] = rawData.charCodeAt(i);
            }
            return outputArray;
        }
    }
})();
