(function() {
    'use strict';

    angular.module('medicalApp').controller('SidebarController', SidebarController);

    SidebarController.$inject = ['AuthService', '$state', 'toastr', '$rootScope', '$http', '$window', 'PWAService', 'NotificationService', '$scope', 'PushService', 'RealtimeService'];

    function SidebarController(AuthService, $state, toastr, $rootScope, $http, $window, PWAService, NotificationService, $scope, PushService, RealtimeService) {
        var layout = this;

        layout.user = AuthService.getUser() || {};
        layout.sidebarOpen = false;
        layout.searchQuery = '';
        layout.urgentCount = 0;
        layout.pendingCount = 0;
        layout.isDarkMode = false;
        layout.pwa = PWAService;
        layout.isOnline = $window.navigator.onLine;

        $window.addEventListener('online', function() {
            layout.isOnline = true;
            toastr.success('تمت استعادة الاتصال بالشبكة (Online)');
            $rootScope.$applyAsync();
        });

        $window.addEventListener('offline', function() {
            layout.isOnline = false;
            toastr.warning('تم انقطاع الاتصال بالشبكة (نمط Offline)');
            $rootScope.$applyAsync();
        });

        layout.toggleSidebar = toggleSidebar;
        layout.closeSidebar = closeSidebar;
        layout.logout = logout;
        layout.getRoleAr = getRoleAr;
        layout.toggleDarkMode = toggleDarkMode;
        
        // Notifications & Medication Requests State
        layout.medRequests = [];
        layout.notificationsOpen = false;
        layout.hasNotifications = false;
        layout.toggleNotifications = toggleNotifications;
        layout.resolveMedRequest = resolveMedRequest;
        layout.loadMedRequests = loadMedRequests;
        layout.notifications = [];
        layout.unreadCount = 0;
        layout.loadSystemNotifications = loadSystemNotifications;
        layout.markNotificationRead = markNotificationRead;
        layout.markAllNotificationsRead = markAllNotificationsRead;
        layout.facilityMode = 'General'; // General, Psychiatric, Hybrid
        layout.loadFacilityMode = loadFacilityMode;
        layout.updateFacilityMode = updateFacilityMode;

        // Push Notifications State & Enable
        layout.push = PushService;
        layout.pushState = {};
        layout.enablingPush = false;
        layout.enablePush = enablePush;
        layout.refreshPushState = refreshPushState;

        // Incoming Video Call (ringing) State
        layout.showIncomingCall = false;
        layout.incomingCall = null;
        layout.joinIncomingCall = joinIncomingCall;
        layout.dismissIncomingCall = dismissIncomingCall;

        var pollingTimer = null;
        var ringAudio = null;
        var vibrateTimer = null;
        var lastRungNotificationID = null;
        var currentCallAppointmentID = null;

        $rootScope.$on('facilityModeChanged', function(event, mode) {
            layout.facilityMode = mode;
        });

        activate();

        function activate() {
            loadNotifications();
            loadMedRequests();
            loadSystemNotifications();
            initDarkMode();
            loadFacilityMode();
            PWAService.checkAndInitPWA();
            refreshPushState();
            PushService.init();
            RealtimeService.start();
            listenRealtime();
            startPolling();
        }

        function listenRealtime() {
            $rootScope.$on('realtime:notification', function(event, notification) {
                refreshNotificationsFromRealtime(notification);
                if (notification && notification.type === 'TelemedicineStarted') {
                    handleIncomingCall(notification);
                }
            });
            $scope.$on('$destroy', function() {
                stopIncomingCall();
                RealtimeService.stop();
            });
        }

        function refreshNotificationsFromRealtime(notification) {
            loadSystemNotifications();
        }

        function handleIncomingCall(notification) {
            var appointmentId = notification.relatedEntityID;
            if (!appointmentId) return;
            // لا نرن لنفس الإشعار مرتين
            if (notification.notificationID && notification.notificationID === lastRungNotificationID) return;
            lastRungNotificationID = notification.notificationID || null;
            // لا نعرض النافذة إن كان المستخدم داخل نفس المكالمة بالفعل
            if ($state.current && $state.current.name === 'app.telemedicine' &&
                currentCallAppointmentID === appointmentId) return;
            currentCallAppointmentID = appointmentId;

            layout.incomingCall = {
                title: notification.title || 'مكالمة فيديو واردة 📹',
                message: notification.message || '',
                appointmentId: appointmentId
            };
            layout.showIncomingCall = true;
            $scope.$applyAsync();
            startRinging();
        }

        function startRinging() {
            stopRinging();
            if ($window.Audio) {
                ringAudio = new Audio('/assets/sounds/ring.wav');
                ringAudio.loop = true;
                ringAudio.volume = 0.9;
                ringAudio.play().catch(function() {
                    ringAudio = null;
                });
            }
            if ($window.navigator.vibrate) {
                vibrateTimer = $window.setInterval(function() {
                    $window.navigator.vibrate([250, 120, 250]);
                }, 3000);
                $window.navigator.vibrate([250, 120, 250]);
            }
        }

        function stopRinging() {
            if (ringAudio) {
                ringAudio.pause();
                ringAudio.src = '';
                ringAudio = null;
            }
            if (vibrateTimer) {
                $window.clearInterval(vibrateTimer);
                vibrateTimer = null;
            }
            if ($window.navigator.vibrate) $window.navigator.vibrate(0);
        }

        function stopIncomingCall() {
            stopRinging();
            layout.showIncomingCall = false;
            layout.incomingCall = null;
            currentCallAppointmentID = null;
        }

        function joinIncomingCall() {
            var appointmentId = layout.incomingCall ? layout.incomingCall.appointmentId : currentCallAppointmentID;
            stopIncomingCall();
            if (appointmentId) {
                $state.go('app.telemedicine', { appointmentId: appointmentId });
            }
        }

        function dismissIncomingCall() {
            stopIncomingCall();
        }

        function refreshPushState() {
            layout.pushState = PushService.status();
        }

        function enablePush() {
            layout.enablingPush = true;
            PushService.enable().then(function(res) {
                layout.enablingPush = false;
                refreshPushState();
                if (res && res.ok) {
                    toastr.success('تم تفعيل إشعارات الجوال بنجاح');
                } else if (res && res.reason === 'needInstall') {
                    toastr.info('لتفعيل الإشعارات على آيفون: ثبّت التطبيق أولاً من زر "تثبيت التطبيق" (إضافة إلى الشاشة الرئيسية) ثم أعد الضغط على "تفعيل الإشعارات".');
                } else if (res && res.reason === 'denied') {
                    toastr.warning('تم رفض إذن الإشعارات. فعّله من إعدادات المتصفح ثم أعد المحاولة.');
                } else if (res && res.reason === 'disabled') {
                    toastr.warning('إشعارات الدفع غير مفعلة على الخادم.');
                } else if (res && res.reason === 'unsupported') {
                    toastr.warning('متصفحك لا يدعم إشعارات الدفع.');
                } else {
                    toastr.error('تعذر تفعيل الإشعارات. حاول مرة أخرى.');
                }
            });
        }

        function startPolling() {
            stopPolling();
            pollingTimer = $window.setInterval(function() {
                loadSystemNotifications();
                loadNotifications();
            }, 30000);
            $scope.$on('$destroy', stopPolling);
        }

        function stopPolling() {
            if (pollingTimer) {
                $window.clearInterval(pollingTimer);
                pollingTimer = null;
            }
        }

        function loadFacilityMode() {
            $http.get('/api/settings/facility-mode').then(function(res) {
                if (res.data && res.data.data) {
                    layout.facilityMode = res.data.data.facilityMode || 'General';
                    $rootScope.facilityMode = layout.facilityMode;
                }
            }).catch(function() {
                layout.facilityMode = 'General';
                $rootScope.facilityMode = 'General';
            });
        }

        function updateFacilityMode(mode) {
            return $http.post('/api/settings/facility-mode', { facilityMode: mode }).then(function(res) {
                if (res.data.success) {
                    layout.facilityMode = mode;
                    $rootScope.facilityMode = mode;
                    toastr.success(res.data.message || 'تم تحديث نمط تشغيل المنظومة بنجاح');
                    $rootScope.$broadcast('facilityModeChanged', mode);
                } else {
                    toastr.error(res.data.message);
                }
            });
        }

        function togglePWAFeature(enable) {
            PWAService.togglePWASetting(enable).then(function() {
                if (enable) {
                    toastr.success('تم تفعيل ترخيص وميزة تطبيق الموبايل PWA بنجاح للعيادة');
                } else {
                    toastr.info('تم حظر وتعطيل ترخيص تطبيق الموبايل PWA للعميل (يعمل موقع ويب فقط)');
                }
            });
        }

        function initDarkMode() {
            var savedTheme = $window.localStorage.getItem('medical_theme');
            if (savedTheme === 'dark') {
                layout.isDarkMode = true;
                document.body.setAttribute('data-theme', 'dark');
            } else {
                layout.isDarkMode = false;
                document.body.removeAttribute('data-theme');
            }
        }

        function toggleDarkMode() {
            layout.isDarkMode = !layout.isDarkMode;
            if (layout.isDarkMode) {
                document.body.setAttribute('data-theme', 'dark');
                $window.localStorage.setItem('medical_theme', 'dark');
            } else {
                document.body.removeAttribute('data-theme');
                $window.localStorage.setItem('medical_theme', 'light');
            }
        }

        function toggleNotifications() {
            layout.notificationsOpen = !layout.notificationsOpen;
            if (layout.notificationsOpen) {
                loadSystemNotifications();
                loadMedRequests();
            }
        }

        function loadSystemNotifications() {
            NotificationService.getAll(1, 10).then(function(res) {
                // getAll يعيد جسم PaginatedResponse مباشرة → القائمة في res.Data
                var items = (res && (res.Data || res.data)) ? (res.Data || res.data) : [];
                layout.notifications = Array.isArray(items) ? items : [];
                NotificationService.getUnreadCount().then(function(res2) {
                    var data = (res2 && res2.data) ? res2.data : res2;
                    layout.unreadCount = (data && data.count) ? data.count : 0;
                    layout.hasNotifications = layout.unreadCount > 0 || layout.medRequests.length > 0;
                });
            }).catch(function() {
                layout.notifications = [];
            });
        }

        function markNotificationRead(n) {
            if (n && !n.isRead) {
                NotificationService.markRead(n.notificationID).then(function() {
                    n.isRead = true;
                    if (layout.unreadCount > 0) layout.unreadCount--;
                    layout.hasNotifications = layout.unreadCount > 0 || layout.medRequests.length > 0;
                });
            }
            layout.notificationsOpen = false;
            navigateToNotification(n);
        }

        function markAllNotificationsRead() {
            NotificationService.markAllRead().then(function() {
                angular.forEach(layout.notifications, function(n) { n.isRead = true; });
                layout.unreadCount = 0;
                layout.hasNotifications = layout.medRequests.length > 0;
            });
        }

        function navigateToNotification(n) {
            if (!n) return;
            if (n.type === 'TelemedicineStarted' && layout.user.role === 'Patient' && n.relatedEntityID) {
                $state.go('app.telemedicine', { appointmentId: n.relatedEntityID });
            } else if (n.relatedEntityType === 'Appointment') {
                $state.go('app.appointments');
            } else {
                $state.go('app.dashboard');
            }
        }

        function loadMedRequests() {
            if (layout.user.role === 'Admin' || layout.user.role === 'Pharmacist') {
                $http.get('/api/pharmacy/requests', { params: { isResolved: false, page: 1, pageSize: 20 } })
                    .then(function(res) {
                        layout.medRequests = res.data.data || [];
                        layout.hasNotifications = layout.unreadCount > 0 || layout.medRequests.length > 0;
                    });
            }
        }

        function resolveMedRequest(id, event) {
            if (event) {
                event.stopPropagation();
                event.preventDefault();
            }
            
            $http.put('/api/pharmacy/requests/' + id + '/resolve', {})
                .then(function(res) {
                    if (res.data.success) {
                        toastr.success(res.data.message || 'تم توفير الدواء وحل الطلب بنجاح');
                        layout.medRequests = layout.medRequests.filter(function(r) {
                            return r.requestId !== id;
                        });
                        layout.hasNotifications = layout.unreadCount > 0 || layout.medRequests.length > 0;
                        // Trigger dashboard reload if we are on pharmacy page
                        $rootScope.$broadcast('medicationRequestResolved');
                    } else {
                        toastr.error(res.data.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء معالجة الطلب');
                });
        }

        function loadNotifications() {
            if (layout.user.role === 'Doctor') {
                $http.get('/api/dashboard/stats').then(function(res) {
                    if (res.data.success) {
                        layout.urgentCount = res.data.data.urgentCases || 0;
                        layout.pendingCount = res.data.data.pendingAppointments || 0;
                    }
                });
            } else if (layout.user.role === 'Admin') {
                $http.get('/api/dashboard/stats').then(function(res) {
                    if (res.data.success) {
                        layout.urgentCount = res.data.data.emergencies || 0;
                        layout.pendingCount = res.data.data.pendingAppointments || 0;
                    }
                });
            } else if (layout.user.role === 'Patient') {
                $http.get('/api/dashboard/stats').then(function(res) {
                    if (res.data.success) {
                        layout.pendingCount = res.data.data.upcomingAppointments || 0;
                    }
                });
            } else if (layout.user.role === 'Pharmacist') {
                $http.get('/api/pharmacy/dashboard').then(function(res) {
                    if (res.data.success) {
                        layout.pendingCount = res.data.data.pendingPrescriptions || 0;
                    }
                });
            }
        }

        function toggleSidebar() {
            layout.sidebarOpen = !layout.sidebarOpen;
        }

        function closeSidebar() {
            layout.sidebarOpen = false;
        }

        function logout() {
            PushService.unsubscribe();
            AuthService.logout();
            toastr.info('تم تسجيل الخروج بنجاح', 'إلى اللقاء');
            $state.go('login');
        }

        function getRoleAr() {
            var roles = {
                'Admin': 'مدير النظام',
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
            return roles[layout.user.role] || layout.user.role;
        }
    }
})();
