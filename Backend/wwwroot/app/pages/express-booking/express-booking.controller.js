(function() {
    'use strict';

    angular.module('medicalApp').controller('ExpressBookingController', ExpressBookingController);

    ExpressBookingController.$inject = ['CommissionsService', 'UsersService', 'AuthService', 'toastr', '$window'];

    function ExpressBookingController(CommissionsService, UsersService, AuthService, toastr, $window) {
        var vm = this;

        vm.user = AuthService.getUser() || {};
        vm.role = vm.user.role;
        vm.canBook = vm.role === 'Admin' || vm.role === 'Receptionist';
        vm.canViewCashReport = vm.role === 'Admin' || vm.role === 'Cashier';
        vm.isReceptionist = vm.role === 'Receptionist';

        vm.doctors = [];
        vm.loadingDoctors = true;
        vm.submitting = false;

        // Express Booking Form State
        vm.booking = {
            patientName: '',
            patientPhone: '',
            gender: 'ذكر',
            doctorId: null,
            paymentMethod: 'Cash',
            consultationFee: 50.00,
            notes: ''
        };

        // Patient Search (البحث عن مريض موجود)
        vm.patientSearch = '';
        vm.patientResults = [];
        vm.searchingPatients = false;
        vm.searchPatients = searchPatients;
        vm.selectPatient = selectPatient;
        vm.clearPatientSearch = clearPatientSearch;

        // Ticket Receipt Modal
        vm.showTicketModal = false;
        vm.receipt = null;

        // Daily Cash Report State
        vm.reportDate = new Date();
        vm.cashReport = null;
        vm.loadingReport = false;
        vm.activeTab = vm.role === 'Cashier' ? 'cashReport' : 'booking';

        // Functions
        vm.loadDoctors = loadDoctors;
        vm.onDoctorChange = onDoctorChange;
        vm.submitExpressBooking = submitExpressBooking;
        vm.printTicket = printTicket;
        vm.loadCashReport = loadCashReport;
        vm.resetForm = resetForm;

        activate();

        function activate() {
            if (vm.canBook) {
                loadDoctors();
            }
            if (vm.canViewCashReport) {
                loadCashReport();
            }
        }

        function loadDoctors() {
            vm.loadingDoctors = true;
            UsersService.getUsers({ role: 'Doctor', pageSize: 100 }).then(function(res) {
                vm.doctors = res.data || [];
                if (vm.doctors.length > 0 && !vm.booking.doctorId) {
                    vm.booking.doctorId = vm.doctors[0].userID;
                    onDoctorChange();
                }
            }).finally(function() {
                vm.loadingDoctors = false;
            });
        }

        function onDoctorChange() {
            var selectedDoc = vm.doctors.find(function(d) { return d.userID === vm.booking.doctorId; });
            if (selectedDoc) {
                vm.booking.consultationFee = selectedDoc.consultationFee || selectedDoc.ConsultationFee || 50.00;
            }
        }

        function searchPatients() {
            var q = (vm.patientSearch || '').trim();
            if (q.length < 2) {
                vm.patientResults = [];
                return;
            }
            vm.searchingPatients = true;
            UsersService.getUsers({ search: q, role: 'Patient', pageSize: 8 }).then(function(res) {
                vm.patientResults = res.data || [];
            }).finally(function() {
                vm.searchingPatients = false;
            });
        }

        function selectPatient(p) {
            vm.booking.patientName = p.fullName;
            vm.booking.patientPhone = p.phone || '';
            vm.patientResults = [];
            vm.patientSearch = '';
        }

        function clearPatientSearch() {
            vm.patientResults = [];
        }

        function submitExpressBooking() {
            if (!vm.booking.patientName) {
                toastr.warning('يرجى إدخال اسم المريض');
                return;
            }
            if (!vm.booking.doctorId) {
                toastr.warning('يرجى اختيار الطبيب المعالج');
                return;
            }

            vm.submitting = true;
            CommissionsService.processExpressBooking(vm.booking).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.receipt = res.data;
                    vm.showTicketModal = true;
                    resetForm();
                    if (vm.canViewCashReport) {
                        loadCashReport();
                    }
                } else {
                    toastr.error(res.message);
                }
            }).catch(function(err) {
                toastr.error(err.data ? err.data.message : 'حدث خطأ في عملية الحجز السريع');
            }).finally(function() {
                vm.submitting = false;
            });
        }

        function resetForm() {
            vm.booking.patientName = '';
            vm.booking.patientPhone = '';
            vm.booking.notes = '';
        }

        function printTicket() {
            $window.print();
        }

        function loadCashReport() {
            vm.loadingReport = true;
            var targetDate = vm.reportDate ? vm.reportDate.toISOString().split('T')[0] : null;

            CommissionsService.getDailyCashReport(targetDate).then(function(res) {
                if (res.success) {
                    vm.cashReport = res.data;
                }
            }).finally(function() {
                vm.loadingReport = false;
            });
        }
    }
})();
