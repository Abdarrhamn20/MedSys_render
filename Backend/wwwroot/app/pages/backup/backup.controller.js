(function() {
    'use strict';

    angular.module('medicalApp').controller('BackupController', BackupController);

    BackupController.$inject = ['AuthService', 'toastr', '$timeout'];

    function BackupController(AuthService, toastr, $timeout) {
        var vm = this;

        // === Mock Data (معاينة تصميمية فقط — لا اتصال بالباك اند) ===
        vm.overview = {
            lastBackup: '2026/08/05 02:00',
            lastSize: '48.7 MB',
            totalStorage: '154.3 MB',
            retentionDays: 14,
            backupsCount: 6,
            healthy: true
        };

        vm.settings = {
            scheduleEnabled: true,
            scheduleTime: '02:00',
            localPath: 'C:\\MedicalSystemBackups\\Medical_System',
            externalPath: 'D:\\IVS_Backups\\Medical_System',
            externalEnabled: true,
            keepDaily: 14,
            keepWeekly: 8,
            keepMonthly: 12
        };

        vm.backups = [
            { id: 1, name: 'backup_20260805_020000.zip', date: '2026/08/05 02:00', type: 'يومية', size: '48.7 MB', status: 'success', verified: true, auto: true },
            { id: 2, name: 'backup_20260804_020000.zip', date: '2026/08/04 02:00', type: 'يومية', size: '48.1 MB', status: 'success', verified: true, auto: true },
            { id: 3, name: 'backup_20260803_020000.zip', date: '2026/08/03 02:00', type: 'يومية', size: '47.9 MB', status: 'success', verified: true, auto: true },
            { id: 4, name: 'backup_20260802_020000.zip', date: '2026/08/02 02:00', type: 'يومية', size: '47.5 MB', status: 'success', verified: true, auto: true },
            { id: 5, name: 'backup_20260801_020000.zip', date: '2026/08/01 02:00', type: 'أسبوعية', size: '46.9 MB', status: 'success', verified: true, auto: true },
            { id: 6, name: 'backup_20260701_020000.zip', date: '2026/07/01 02:00', type: 'شهرية', size: '44.2 MB', status: 'success', verified: true, auto: true }
        ];

        vm.recentActivity = [
            { action: 'نسخ احتياطي تلقائي', target: 'backup_20260805_020000.zip', date: '2026/08/05 02:00', user: 'تلقائي' },
            { action: 'تنزيل نسخة', target: 'backup_20260804_020000.zip', date: '2026/08/04 10:15', user: 'مدير النظام' },
            { action: 'نسخ احتياطي تلقائي', target: 'backup_20260804_020000.zip', date: '2026/08/04 02:00', user: 'تلقائي' },
            { action: 'فحص سلامة نسخة', target: 'backup_20260803_020000.zip', date: '2026/08/03 02:05', user: 'تلقائي' },
            { action: 'تغيير إعدادات الجدولة', target: 'الوقت: 02:00', date: '2026/08/01 15:30', user: 'مدير النظام' }
        ];

        // === State ===
        vm.creatingBackup = false;
        vm.showRestoreModal = false;
        vm.restoreTarget = null;
        vm.showScheduleModal = false;
        vm.showActivityModal = false;

        // === Actions ===
        vm.createBackup = createBackup;
        vm.downloadBackup = downloadBackup;
        vm.deleteBackup = deleteBackup;
        vm.openRestore = openRestore;
        vm.confirmRestore = confirmRestore;
        vm.openSchedule = openSchedule;
        vm.saveSchedule = saveSchedule;
        vm.testPath = testPath;
        vm.viewActivity = viewActivity;
        vm.getTypeClass = getTypeClass;

        function createBackup() {
            if (vm.creatingBackup) return;
            vm.creatingBackup = true;
            toastr.info('معاينة تصميمية: سيتم بدء النسخ الاحتياطي عند ربط الباك اند', 'معاينة');
            $timeout(function() {
                vm.creatingBackup = false;
                toastr.success('تم إنشاء نسخة احتياطية جديدة بنجاح (تجريبي)', 'معاينة');
            }, 1800);
        }

        function downloadBackup(backup) {
            toastr.info('تنزيل النسخة: ' + backup.name + ' (تجريبي)', 'معاينة');
        }

        function deleteBackup(backup) {
            toastr.warning('سيتم حذف النسخة: ' + backup.name + ' (تجريبي)', 'معاينة');
        }

        function openRestore(backup) {
            vm.restoreTarget = backup;
            vm.showRestoreModal = true;
        }

        function confirmRestore() {
            vm.showRestoreModal = false;
            toastr.warning('تم بدء الاستعادة التجريبية — سيتم استرجاع البيانات بعد تأكيد الباك اند', 'معاينة');
        }

        function openSchedule() {
            vm.showScheduleModal = true;
        }

        function saveSchedule() {
            vm.showScheduleModal = false;
            toastr.success('تم حفظ إعدادات النسخ الاحتياطي (تجريبي)', 'معاينة');
        }

        function testPath(path, label) {
            if (!path) {
                toastr.warning('أدخل المسار أولاً قبل الفحص', 'معاينة');
                return;
            }
            toastr.info('معاينة تصميمية: سيُفحص المسار على جهاز السيرفر نفسه (وجود المجلد + صلاحية الكتابة)', label);
            $timeout(function() {
                toastr.success('المسار متاح والكتابة عليه مسموحة (محاكاة)', label);
            }, 1200);
        }

        function viewActivity() {
            vm.showActivityModal = true;
        }

        function getTypeClass(type) {
            if (type === 'يومية') return 'badge-primary';
            if (type === 'أسبوعية') return 'badge-info';
            return 'badge-normal';
        }
    }
})();
