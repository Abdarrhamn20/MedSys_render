(function() {
    'use strict';

    angular.module('medicalApp').controller('AppointmentsController', AppointmentsController);

    AppointmentsController.$inject = ['AppointmentService', 'AuthService', 'toastr', '$state', 'TelemedicineService'];

    function AppointmentsController(AppointmentService, AuthService, toastr, $state, TelemedicineService) {
        var vm = this;

        var user = AuthService.getUser() || {};
        vm.role = user.role;
        vm.appointments = [];
        vm.filterStatus = '';
        vm.filterPriority = '';
        vm.filterDate = null;
        vm.page = 1;
        vm.pageSize = 10;
        vm.totalCount = 0;
        vm.totalPages = 0;
        vm.showDetails = false;
        vm.detailApp = null;
        vm.sessionHistory = [];
        vm.sessionHistoryLoading = false;
        vm.loading = true;

        vm.load = load;
        vm.viewDetails = viewDetails;
        vm.updateStatus = updateStatus;
        vm.startSession = startSession;
        vm.cancelAppointment = cancelAppointment;
        vm.openCancel = openCancel;
        vm.confirmCancel = confirmCancel;
        vm.closeCancel = closeCancel;
        vm.startVideoCall = startVideoCall;
        vm.getStatusAr = getStatusAr;
        vm.getStatusClass = getStatusClass;
        vm.nextPage = function() { if (vm.page < vm.totalPages) { vm.page++; load(); } };
        vm.prevPage = function() { if (vm.page > 1) { vm.page--; load(); } };

        activate();

        function activate() {
            load();
            // تحميل سياسة الحجز لعرض رسالة نافذة الإلغاء
            AppointmentService.getBookingPolicy().then(function(res) {
                if (res.success) vm.policy = res.data;
            });
        }

        function load() {
            vm.loading = true;
            var params = { page: vm.page, pageSize: vm.pageSize };
            if (vm.filterStatus) params.status = vm.filterStatus;
            if (vm.filterPriority) params.priority = vm.filterPriority;
            if (vm.filterDate) params.date = vm.filterDate;

            AppointmentService.getAppointments(params).then(function(res) {
                vm.appointments = res.data || [];
                vm.totalCount = res.totalCount;
                vm.totalPages = res.totalPages;
            }).catch(function() {
                toastr.error('حدث خطأ في تحميل المواعيد');
            }).finally(function() { vm.loading = false; });
        }

        function viewDetails(app) {
            AppointmentService.getAppointment(app.appID).then(function(res) {
                if (res.success) {
                    vm.detailApp = res.data;
                    vm.showDetails = true;
                    loadSessionHistory(app.appID);
                }
            });
        }

        function loadSessionHistory(appointmentId) {
            vm.sessionHistory = [];
            vm.sessionHistoryLoading = true;
            TelemedicineService.getSessionHistory(appointmentId).then(function(res) {
                if (res.success) {
                    vm.sessionHistory = res.data || [];
                }
            }).finally(function() {
                vm.sessionHistoryLoading = false;
            });
        }

        function updateStatus(app, status) {
            AppointmentService.updateStatus(app.appID, { status: status }).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    load();
                }
            });
        }

        function cancelAppointment(app) {
            if (!confirm('هل أنت متأكد من إلغاء هذا الموعد؟')) return;
            AppointmentService.cancelAppointment(app.appID).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    load();
                }
            });
        }

        function openCancel(app) {
            vm.cancelTarget = app;
            vm.cancelReason = '';
            vm.cancelBlockedMsg = '';
            var start = new Date(app.appointmentDate.substring(0, 10) + 'T' + app.appointmentTime);
            var window = (vm.policy && vm.policy.cancelWindowHours) || 6;
            if (start.getTime() <= Date.now()) {
                vm.cancelBlockedMsg = 'لا يمكن إلغاء موعد انتهى أو بدأ بالفعل';
            } else if ((start.getTime() - Date.now()) / 3600000 < window) {
                vm.cancelBlockedMsg = 'لا يمكن الإلغاء قبل أقل من ' + window + ' ساعات من الموعد. يرجى الاتصال بالعيادة.';
            }
        }

        function confirmCancel() {
            var app = vm.cancelTarget;
            if (!app) return;
            if (vm.role === 'Patient') {
                if (!vm.cancelReason || !vm.cancelReason.trim()) {
                    toastr.warning('يرجى إدخال سبب الإلغاء');
                    return;
                }
                AppointmentService.cancelWithReason(app.appID, vm.cancelReason.trim()).then(function(res) {
                    if (res.success) {
                        toastr.success(res.message);
                        vm.closeCancel();
                        load();
                    } else {
                        toastr.error(res.message);
                    }
                }).catch(function(err) {
                    toastr.error(err.data ? err.data.message : 'حدث خطأ في الإلغاء');
                });
            } else {
                AppointmentService.cancelAppointment(app.appID).then(function(res) {
                    if (res.success) {
                        toastr.success(res.message);
                        vm.closeCancel();
                        load();
                    } else {
                        toastr.error(res.message);
                    }
                }).catch(function(err) {
                    toastr.error(err.data ? err.data.message : 'حدث خطأ في الإلغاء');
                });
            }
        }

        function closeCancel() {
            vm.cancelTarget = null;
            vm.cancelReason = '';
            vm.cancelBlockedMsg = '';
        }

        function getStatusAr(status) {
            var map = { 'Pending': 'في الانتظار', 'Confirmed': 'مؤكد', 'InProgress': 'جاري', 'Completed': 'مكتمل', 'Cancelled': 'ملغي' };
            return map[status] || status;
        }

        function getStatusClass(status) {
            var map = { 'Pending': 'badge-urgent', 'Confirmed': 'badge-primary', 'InProgress': 'badge-info', 'Completed': 'badge-normal', 'Cancelled': 'badge-emergency' };
            return map[status] || 'badge-primary';
        }

        function startVideoCall(app) {
            $state.go('app.telemedicine', { appointmentId: app.appID });
        }

        function startSession(app) {
            if (app.appointmentType === 'Online') {
                startVideoCall(app);
            } else {
                updateStatus(app, 'InProgress');
            }
        }
    }
})();
