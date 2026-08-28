// Medical System Service Worker for Offline & Mobile App Shell Support
const CACHE_NAME = 'ivs-medical-app-v51';
const ASSETS_TO_CACHE = [
    './',
    './index.html',
    './manifest.json',
    './assets/css/variables.css',
    './assets/css/main.css?v=1.1',
    './assets/css/components.css',
    './assets/css/layout.css',
    './assets/css/login.css',
    './assets/css/telemedicine.css?v=1.0',
    './assets/css/responsive.css?v=2.9',
    './assets/icons/icon-192.png',
    './assets/icons/icon-512.png',
    './assets/sounds/ring.wav',
    'https://cdnjs.cloudflare.com/ajax/libs/angular.js/1.8.3/angular.min.js',
    'https://cdnjs.cloudflare.com/ajax/libs/angular-ui-router/1.1.0/angular-ui-router.min.js',
    'https://cdnjs.cloudflare.com/ajax/libs/Chart.js/4.4.1/chart.umd.min.js',
    'https://cdn.jsdelivr.net/npm/chartjs-adapter-date-fns@3.0.0/dist/chartjs-adapter-date-fns.bundle.min.js',
    'https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.7/signalr.min.js',
    './app/app.module.js',
    './app/app.routes.js?v=2.2',
    './app/app.config.js?v=1.3',
    './app/services/auth.service.js?v=1.1',
    './app/services/api.interceptor.js?v=1.1',
    './app/services/toast.service.js',
    './app/services/users.service.js',
    './app/services/appointment.service.js',
    './app/services/medical.service.js',
    './app/services/attachment.service.js',
    './app/services/pharmacy.service.js',
    './app/services/billing.service.js',
    './app/services/accounting.service.js',
    './app/services/treasury.service.js?v=1.0',
    './app/services/warehouse.service.js?v=1.1',
    './app/services/psychiatric.service.js',
    './app/services/inpatient.service.js?v=7.0',
    './app/services/pwa.service.js?v=3.0',
    './app/services/commissions.service.js?v=1.0',
    './app/services/lab.service.js?v=1.1',
    './app/services/employees.service.js?v=1.2',
    './app/services/radiology.service.js?v=1.1',
    './app/services/telemedicine.service.js?v=1.2',
    './app/services/notification.service.js?v=1.0',
    './app/services/push.service.js?v=1.2',
    './app/services/realtime.service.js?v=1.0',
    './app/pages/login/login.html',
    './app/pages/register/register.html?v=1.1',
    './app/pages/login/login.controller.js?v=1.1',
    './app/pages/register/register.controller.js?v=1.1',
    './app/pages/dashboard/dashboard.html?v=1.4',
    './app/pages/dashboard/dashboard.controller.js?v=1.2',
    './app/pages/users/users.html?v=1.4',
    './app/pages/users/users.controller.js?v=2.2',
    './app/pages/appointments/appointments.html?v=1.2',
    './app/pages/appointments/appointments.controller.js?v=1.2',
    './app/pages/book-appointment/book-appointment.html?v=1.3',
    './app/pages/book-appointment/book-appointment.controller.js?v=1.2',
    './app/pages/medical-records/medical-records.html?v=1.6',
    './app/pages/medical-records/medical-records.controller.js?v=1.6',
    './app/pages/profile/profile.html',
    './app/pages/profile/profile.controller.js?v=1.3',
    './app/pages/pharmacy/pharmacy.html?v=1.3',
    './app/pages/pharmacy/pharmacy.controller.js?v=1.3',
    './app/pages/billing/billing.html?v=1.5',
    './app/pages/billing/billing.controller.js?v=1.2',
    './app/pages/fee-management/fee-management.html?v=1.2',
    './app/pages/fee-management/fee-management.controller.js?v=1.3',
    './app/pages/patient-assessments/patient-assessments.html?v=1.0',
    './app/pages/patient-assessments/patient-assessments.controller.js?v=1.0',
    './app/pages/inpatient/inpatient.html?v=7.0',
    './app/pages/inpatient/inpatient.controller.js?v=7.0',
    './app/pages/doctor-ledger/doctor-ledger.html?v=1.0',
    './app/pages/doctor-ledger/doctor-ledger.controller.js?v=1.0',
    './app/pages/accounting/accounting.html?v=1.1',
    './app/pages/accounting/accounting.controller.js?v=1.1',
    './app/pages/treasury/treasury.html?v=1.1',
    './app/pages/treasury/treasury.controller.js?v=1.1',
    './app/pages/warehouse/warehouse.html?v=1.1',
    './app/pages/warehouse/warehouse.controller.js?v=1.1',
    './app/pages/express-booking/express-booking.html?v=1.1',
    './app/pages/express-booking/express-booking.controller.js?v=1.2',
    './app/pages/lab/lab.html?v=1.1',
    './app/pages/lab/lab.controller.js?v=1.1',
    './app/pages/patients/patients.html?v=1.1',
    './app/pages/patients/patients.controller.js?v=1.0',
    './app/pages/employees/employees.html?v=1.3',
    './app/pages/employees/employees.controller.js?v=1.1',
    './app/pages/audit-logs/audit-logs.html?v=1.0',
    './app/pages/audit-logs/audit-logs.controller.js?v=1.0',
    './app/pages/radiology/radiology.html?v=1.3',
    './app/pages/radiology/radiology.controller.js?v=1.2',
    './app/pages/telemedicine/telemedicine.html?v=1.0',
    './app/pages/telemedicine/telemedicine.controller.js?v=1.2',
    './app/pages/backup/backup.controller.js?v=1.1',
    './app/pages/backup/backup.html?v=1.1',
    './app/components/layout/layout.html?v=1.8',
    './app/components/sidebar/sidebar.controller.js?v=2.1'
];

self.addEventListener('install', (event) => {
    event.waitUntil(
        caches.open(CACHE_NAME).then((cache) => {
            return Promise.allSettled(ASSETS_TO_CACHE.map((asset) => cache.add(asset)));
        }).then(() => self.skipWaiting())
    );
});

self.addEventListener('activate', (event) => {
    event.waitUntil(
        caches.keys().then((keys) => {
            return Promise.all(
                keys.map((key) => {
                    if (key !== CACHE_NAME) {
                        return caches.delete(key);
                    }
                })
            );
        }).then(() => self.clients.claim()).then(() => {
            // أبلغ كل النوافذ المفتوحة بإعادة التحميل فور تثبيت نسخة جديدة
            // حتى تلتقط التبويبات القديمة آخر تحديثات الكود تلقائياً
            return self.clients.matchAll({ type: 'window', includeUncontrolled: true }).then((clients) => {
                clients.forEach((client) => client.postMessage({ type: 'ivs-sw-updated' }));
            });
        })
    );
});

self.addEventListener('fetch', (event) => {
    // Only cache GET requests that are static assets, bypass API calls
    if (event.request.method === 'GET' && !event.request.url.includes('/api/')) {
        // الصفحات والقالب HTML تُجلب من الشبكة أولاً ثم كاش — لضمان وصول كل التحديثات دائماً
        const isHtml = event.request.mode === 'navigate' || event.request.url.includes('.html');
        if (isHtml) {
            event.respondWith(
                fetch(event.request).then((response) => {
                    const copy = response.clone();
                    caches.open(CACHE_NAME).then((cache) => cache.put(event.request, copy));
                    return response;
                }).catch(() => caches.match(event.request).then((c) => c || caches.match('./index.html')))
            );
            return;
        }

        event.respondWith(
            caches.match(event.request).then((cachedResponse) => {
                if (cachedResponse) {
                    return cachedResponse;
                }
                return fetch(event.request).then((response) => {
                    // تخزين تشغيلي: أي ملف يُجلب بنجاح يُخزَّن ليعمل Offline لاحقاً
                    if (response && response.ok) {
                        const copy = response.clone();
                        caches.open(CACHE_NAME).then((cache) => cache.put(event.request, copy));
                    }
                    return response;
                }).catch(() => {
                    // Fallback to main app shell if network fails
                    return caches.match('./index.html');
                });
            })
        );
    }
});

// === Push Notifications ===
self.addEventListener('push', (event) => {
    let data = {
        title: 'إشعار جديد',
        body: '',
        icon: './assets/icons/icon-192.png',
        badge: './assets/icons/icon-192.png',
        type: '',
        relatedEntityType: '',
        relatedEntityID: null,
        timestamp: null
    };

    if (event.data) {
        try {
            data = Object.assign(data, event.data.json());
        } catch (e) {
            data.title = event.data.text();
        }
    }

    const isCall = data.type === 'TelemedicineStarted';
    const isReminder = data.type === 'AppointmentTimeReached';

    const options = {
        body: data.body || '',
        icon: data.icon,
        badge: data.badge,
        dir: 'rtl',
        lang: 'ar',
        data: {
            type: data.type,
            relatedEntityType: data.relatedEntityType,
            relatedEntityID: data.relatedEntityID,
            url: getTargetUrl(data)
        },
        renotify: true,
        // إشعار الجلسة لا يُلغى حتى يتفاعل المستخدم، مع اهتزاز كالرنين
        requireInteraction: isCall || isReminder,
        vibrate: isCall ? [250, 120, 250] : [100, 80, 100],
        // معرّف فريد لكل نوع/كيان حتى لا تلغي الإشعارات بعضها
        tag: (data.type || 'ivs') + '-' + (data.relatedEntityID || Date.now())
    };

    if (isCall) {
        options.sound = './assets/sounds/ring.wav';
    }

    event.waitUntil(self.registration.showNotification(data.title, options));
});

self.addEventListener('notificationclick', (event) => {
    event.notification.close();

    const target = (event.notification.data && event.notification.data.url) || './#/dashboard';

    event.waitUntil(
        clients.matchAll({ type: 'window', includeUncontrolled: true }).then((clientList) => {
            for (const client of clientList) {
                if ('focus' in client) {
                    client.navigate(target);
                    return client.focus();
                }
            }
            return clients.openWindow(target);
        })
    );
});

function getTargetUrl(data) {
    const base = './#/';
    if (data.type === 'TelemedicineStarted' && data.relatedEntityID) {
        return base + 'telemedicine/' + data.relatedEntityID;
    }
    if (data.relatedEntityType === 'Appointment') {
        return base + 'appointments';
    }
    return base + 'dashboard';
}
