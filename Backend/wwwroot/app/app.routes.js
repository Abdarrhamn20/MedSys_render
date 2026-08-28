(function() {
    'use strict';

    angular.module('medicalApp').config(routeConfig);

    routeConfig.$inject = ['$stateProvider', '$urlRouterProvider'];

    function routeConfig($stateProvider, $urlRouterProvider) {

        $urlRouterProvider.otherwise('/login');

        $stateProvider
            // === Auth ===
            .state('login', {
                url: '/login',
                templateUrl: 'app/pages/login/login.html',
                controller: 'LoginController',
                controllerAs: 'vm',
                data: { requiresAuth: false, pageTitle: 'تسجيل الدخول' }
            })
            .state('register', {
                url: '/register',
                templateUrl: 'app/pages/register/register.html?v=1.1',
                controller: 'RegisterController',
                controllerAs: 'vm',
                data: { requiresAuth: false, pageTitle: 'إنشاء حساب جديد' }
            })

            // === Main Layout ===
            .state('app', {
                abstract: true,
                templateUrl: 'app/components/layout/layout.html?v=1.8',
                controller: 'SidebarController',
                controllerAs: 'layout',
                data: { requiresAuth: true }
            })

            // === Dashboard ===
            .state('app.dashboard', {
                url: '/dashboard',
                templateUrl: 'app/pages/dashboard/dashboard.html?v=1.5',
                controller: 'DashboardController',
                controllerAs: 'vm',
                data: { pageTitle: 'لوحة التحكم', icon: 'fa-home' }
            })

            // === Users (Admin) ===
            .state('app.users', {
                url: '/users',
                templateUrl: 'app/pages/users/users.html?v=1.4',
                controller: 'UsersController',
                controllerAs: 'vm',
                data: { pageTitle: 'إدارة المستخدمين', icon: 'fa-users-cog', roles: ['Admin'] }
            })

            // === Backup & Restore (Admin) ===
            .state('app.backup', {
                url: '/backup',
                templateUrl: 'app/pages/backup/backup.html?v=1.1',
                controller: 'BackupController',
                controllerAs: 'vm',
                data: { pageTitle: 'النسخ الاحتياطي', icon: 'fa-database', roles: ['Admin'] }
            })

            // === Appointments ===
            .state('app.appointments', {
                url: '/appointments',
                templateUrl: 'app/pages/appointments/appointments.html?v=1.2',
                controller: 'AppointmentsController',
                controllerAs: 'vm',
                data: { pageTitle: 'المواعيد', icon: 'fa-calendar-alt', roles: ['Admin', 'Doctor', 'Patient', 'Receptionist'] }
            })

            // === Book Appointment ===
            .state('app.bookAppointment', {
                url: '/book-appointment?doctorId',
                templateUrl: 'app/pages/book-appointment/book-appointment.html?v=1.3',
                controller: 'BookAppointmentController',
                controllerAs: 'vm',
                data: { pageTitle: 'حجز موعد', icon: 'fa-calendar-plus', roles: ['Patient'] }
            })

            .state('app.medicalRecords', {
    url: '/medical-records',
    templateUrl: 'app/pages/medical-records/medical-records.html?v=1.6',
                controller: 'MedicalRecordsController',
                controllerAs: 'vm',
                data: { pageTitle: 'السجلات الطبية', icon: 'fa-file-medical', roles: ['Admin', 'Doctor', 'Patient'] }
            })

            // === Profile ===
            .state('app.profile', {
                url: '/profile',
                templateUrl: 'app/pages/profile/profile.html',
                controller: 'ProfileController',
                controllerAs: 'vm',
                data: { pageTitle: 'الملف الشخصي', icon: 'fa-user-circle' }
            })

            // === Pharmacy ===
            .state('app.pharmacy', {
                url: '/pharmacy',
                templateUrl: 'app/pages/pharmacy/pharmacy.html?v=1.3',
                controller: 'PharmacyController',
                controllerAs: 'vm',
                data: { pageTitle: 'الصيدلية', icon: 'fa-pills', roles: ['Admin', 'Pharmacist', 'Doctor'] }
            })

            // === Billing & Payments ===
            .state('app.billing', {
                url: '/billing',
                templateUrl: 'app/pages/billing/billing.html?v=1.5',
                controller: 'BillingController',
                controllerAs: 'vm',
                data: { pageTitle: 'الفواتير والمدفوعات', icon: 'fa-file-invoice-dollar', roles: ['Admin', 'Doctor', 'Patient', 'Pharmacist', 'Cashier'] }
            })

            // === Fee Management (Doctor / Admin) ===
            .state('app.feeManagement', {
                url: '/fee-management',
                templateUrl: 'app/pages/fee-management/fee-management.html?v=1.2',
                controller: 'FeeManagementController',
                controllerAs: 'vm',
                data: { pageTitle: 'إدارة أسعار الكشوفات', icon: 'fa-hand-holding-usd', roles: ['Admin', 'Doctor'] }
            })

            // === Centralized Price Management (Admin only) ===
            .state('app.priceManagement', {
                url: '/price-management',
                templateUrl: 'app/pages/price-management/price-management.html?v=1.0',
                controller: 'PriceManagementController',
                controllerAs: 'vm',
                data: { pageTitle: 'إدارة الأسعار المركزية', icon: 'fa-tags', roles: ['Admin'] }
            })

            // === Patient Assessments ===
            .state('app.patientAssessments', {
                url: '/patient-assessments',
                templateUrl: 'app/pages/patient-assessments/patient-assessments.html?v=1.0',
                controller: 'PatientAssessmentsController',
                controllerAs: 'vm',
                data: { pageTitle: 'الاستبيانات النفسية', icon: 'fa-clipboard-check', roles: ['Admin', 'Doctor', 'Patient'] }
            })

            // === Inpatient & Bed Management ===
            .state('app.inpatient', {
                url: '/inpatient',
                templateUrl: 'app/pages/inpatient/inpatient.html?v=7.0',
                controller: 'InpatientController',
                controllerAs: 'vm',
                data: { pageTitle: 'إدارة الإيواء والتنويم', icon: 'fa-bed-pulse', roles: ['Admin', 'Doctor', 'Patient'] }
            })

            // === Doctor Financial Ledger ===
            .state('app.doctorLedger', {
                url: '/doctor-ledger',
                templateUrl: 'app/pages/doctor-ledger/doctor-ledger.html?v=1.0',
                controller: 'DoctorLedgerController',
                controllerAs: 'vm',
                data: { pageTitle: 'كشف الأرباح والمستحقات', icon: 'fa-chart-line', roles: ['Admin', 'Doctor'] }
            })

            // === Accounting (النظام المحاسبي) ===
            .state('app.accounting', {
                url: '/accounting',
                templateUrl: 'app/pages/accounting/accounting.html?v=1.1',
                controller: 'AccountingController',
                controllerAs: 'vm',
                data: { pageTitle: 'النظام المحاسبي', icon: 'fa-book', roles: ['Admin', 'Accountant', 'Cashier'] }
            })

            // === Treasury (الخزائن والسندات) ===
            .state('app.treasury', {
                url: '/treasury',
                templateUrl: 'app/pages/treasury/treasury.html?v=1.2',
                controller: 'TreasuryController',
                controllerAs: 'vm',
                data: { pageTitle: 'الخزائن والسندات', icon: 'fa-vault', roles: ['Admin', 'Accountant', 'Cashier'] }
            })

            // === Warehouse (المخازن والأصناف) ===
            .state('app.warehouse', {
                url: '/warehouse',
                templateUrl: 'app/pages/warehouse/warehouse.html?v=1.0',
                controller: 'WarehouseController',
                controllerAs: 'vm',
                data: { pageTitle: 'المخازن والأصناف', icon: 'fa-warehouse', roles: ['Admin', 'WarehouseKeeper'] }
            })

            // === Express Booking & Cash Register ===
            .state('app.expressBooking', {
                url: '/express-booking',
                templateUrl: 'app/pages/express-booking/express-booking.html?v=1.1',
                controller: 'ExpressBookingController',
                controllerAs: 'vm',
                data: { pageTitle: 'الحجز السريع وتقارير الخزينة', icon: 'fa-bolt', roles: ['Admin', 'Receptionist', 'Cashier'] }
            })

            // === Laboratory Module (LIS) ===
            .state('app.lab', {
                url: '/lab',
                templateUrl: 'app/pages/lab/lab.html?v=1.1',
                controller: 'LabController',
                controllerAs: 'vm',
                data: { pageTitle: 'قسم المختبر والتحاليل الطبية', icon: 'fa-flask', roles: ['Admin', 'Doctor', 'LabTechnician', 'Patient'] }
            })

            // === Patient Files (ملفات المرضى المحسّنة) ===
            .state('app.patients', {
                url: '/patients',
                templateUrl: 'app/pages/patients/patients.html?v=1.1',
                controller: 'PatientsController',
                controllerAs: 'vm',
                data: { pageTitle: 'ملفات المرضى', icon: 'fa-folder-open', roles: ['Admin', 'Doctor', 'Receptionist'] }
            })

            // === Employees & HR (الموارد البشرية والرواتب) ===
            .state('app.employees', {
                url: '/employees',
                templateUrl: 'app/pages/employees/employees.html?v=1.3',
                controller: 'EmployeesController',
                controllerAs: 'vm',
                data: { pageTitle: 'الموارد البشرية', icon: 'fa-id-card', roles: ['Admin', 'Accountant', 'Doctor', 'Pharmacist', 'LabTechnician', 'Radiologist', 'Receptionist', 'Cashier', 'WarehouseKeeper'] }
            })

            // === Audit Logs (سجل التدقيق) ===
            .state('app.auditLogs', {
                url: '/audit-logs',
                templateUrl: 'app/pages/audit-logs/audit-logs.html?v=1.0',
                controller: 'AuditLogsController',
                controllerAs: 'vm',
                data: { pageTitle: 'سجل التدقيق', icon: 'fa-history', roles: ['Admin'] }
            })

            // === Radiology Module (RIS) ===
            .state('app.radiology', {
                url: '/radiology',
                templateUrl: 'app/pages/radiology/radiology.html?v=1.3',
                controller: 'RadiologyController',
                controllerAs: 'vm',
                data: { pageTitle: 'قسم الأشعة التشخيصية', icon: 'fa-x-ray', roles: ['Admin', 'Doctor', 'Radiologist', 'Patient'] }
            })

            // === Telemedicine Video Call ===
            .state('app.telemedicine', {
                url: '/telemedicine/:appointmentId',
                templateUrl: 'app/pages/telemedicine/telemedicine.html?v=1.0',
                controller: 'TelemedicineController',
                controllerAs: 'vm',
                data: { pageTitle: 'جلسة الفيديو الطبية', icon: 'fa-video', roles: ['Admin', 'Doctor', 'Patient'] }
            });
    }
})();
