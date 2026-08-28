(function() {
    'use strict';

    angular.module('medicalApp').controller('InpatientController', InpatientController);

    InpatientController.$inject = ['InpatientService', 'UsersService', 'toastr', 'AuthService', '$state'];

    function InpatientController(InpatientService, UsersService, toastr, AuthService, $state) {
        var vm = this;

        vm.activeTab = 'grid'; // grid, admissions, management
        vm.loading = false;
        vm.role = AuthService.getUserRole();
        vm.user = AuthService.getUser() || {};

        // Data arrays
        vm.wards = [];
        vm.admissions = [];
        vm.doctors = [];
        vm.patients = [];

        // Stats summary
        vm.stats = {
            totalWards: 0,
            totalRooms: 0,
            totalBeds: 0,
            occupiedBeds: 0,
            vacantBeds: 0,
            occupancyRate: 0
        };

        // Modals state
        vm.showAdmissionModal = false;
        vm.showDischargeModal = false;
        vm.showLogModal = false;
        vm.showAddWardModal = false;
        vm.showAddRoomModal = false;
        vm.showAddBedModal = false;

        vm.selectedBed = null;
        vm.selectedAdmission = null;

        // Form Models
        vm.admissionForm = { patientID: '', doctorID: '', bedID: '', admissionReason: '' };
        vm.dischargeForm = { dischargeSummary: '' };
        vm.logForm = { temperature: '37.0', bloodPressure: '120/80', pulseRate: '75', oxygenLevel: '98%', doctorNotes: '', nursingNotes: '' };
        vm.wardForm = { wardName: '', wardNameAr: '', genderType: 'Mixed', floorNumber: 1 };
        vm.roomForm = { wardID: '', roomNumber: '', roomType: 'General', dailyRate: 200, maxBeds: 2 };
        vm.bedForm = { roomID: '', bedNumber: '', notes: '' };

        // Management sub-tab
        vm.managementSubTab = 'ward'; // ward, room, bed

        vm.nursingOrders = [];
        vm.showCreateOrderModal = false;
        vm.showExecuteOrderModal = false;

        function getCleanDate() {
            var d = new Date();
            d.setSeconds(0, 0);
            return d;
        }

        vm.orderForm = {
            admissionID: '',
            orderType: 'Medication',
            orderDescription: '',
            frequency: 'Once',
            scheduledTime: getCleanDate(),
            unitPrice: 0
        };

        vm.executeForm = {
            notes: '',
            vitalTemperature: '',
            vitalBloodPressure: '',
            vitalPulse: '',
            vitalOxygen: ''
        };

        // Methods
        vm.setTab = setTab;
        vm.loadBedGrid = loadBedGrid;
        vm.loadAdmissions = loadAdmissions;
        vm.loadMyStay = loadMyStay;
        vm.loadNursingDashboard = loadNursingDashboard;
        vm.openAdmissionModal = openAdmissionModal;
        vm.submitAdmission = submitAdmission;
        vm.openDischargeModal = openDischargeModal;
        vm.submitDischarge = submitDischarge;
        vm.openLogModal = openLogModal;
        vm.submitDailyLog = submitDailyLog;
        vm.openCreateOrderModal = openCreateOrderModal;
        vm.submitCreateOrder = submitCreateOrder;
        vm.openExecuteOrderModal = openExecuteOrderModal;
        vm.submitExecuteOrder = submitExecuteOrder;
        vm.submitWard = submitWard;
        vm.submitRoom = submitRoom;
        vm.submitBed = submitBed;

        init();

        function init() {
            if (vm.role === 'Patient') {
                vm.activeTab = 'myStay';
                loadMyStay();
            } else {
                vm.activeTab = 'grid';
                loadBedGrid();
                loadAdmissions();
                loadPrerequisites();
            }
        }

        function setTab(tab) {
            vm.activeTab = tab;
            if (tab === 'grid') loadBedGrid();
            if (tab === 'admissions') loadAdmissions();
            if (tab === 'myStay') loadMyStay();
            if (tab === 'nursing') loadNursingDashboard();
        }

        function loadNursingDashboard() {
            InpatientService.getNursingDashboard().then(function(res) {
                var list = (res && res.data) ? res.data : (res || []);
                vm.nursingOrders = list;
            });
        }

        function openCreateOrderModal(admission) {
            vm.selectedAdmission = admission;
            var admissionID = admission ? (admission.admissionID || admission.AdmissionID) : '';
            vm.orderForm = {
                admissionID: admissionID,
                orderType: 'Medication',
                orderDescription: '',
                frequency: 'Once',
                scheduledTime: getCleanDate(),
                unitPrice: 0
            };
            vm.showCreateOrderModal = true;
        }

        function submitCreateOrder() {
            if (!vm.orderForm.admissionID || !vm.orderForm.orderDescription) {
                toastr.warning('يرجى اختيار المريض وتحديد وصف الخدمة أو الجرعة');
                return;
            }
            vm.loading = true;
            var payload = {
                admissionID: parseInt(vm.orderForm.admissionID),
                orderType: vm.orderForm.orderType,
                orderDescription: vm.orderForm.orderDescription,
                frequency: vm.orderForm.frequency,
                scheduledTime: vm.orderForm.scheduledTime || new Date(),
                unitPrice: parseFloat(vm.orderForm.unitPrice || 0)
            };

            InpatientService.createCareOrder(payload).then(function(res) {
                toastr.success('تم جدولة الخدمة/الجرعة التمريضية بنجاح');
                vm.showCreateOrderModal = false;
                loadNursingDashboard();
                loadAdmissions();
            }).catch(function(err) {
                toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء الجدولة');
            }).finally(function() {
                vm.loading = false;
            });
        }

        function openExecuteOrderModal(order) {
            vm.selectedOrder = order;
            vm.executeForm = {
                notes: '',
                vitalTemperature: '',
                vitalBloodPressure: '',
                vitalPulse: '',
                vitalOxygen: ''
            };
            vm.showExecuteOrderModal = true;
        }

        function submitExecuteOrder() {
            if (!vm.selectedOrder) return;
            var orderID = vm.selectedOrder.orderID || vm.selectedOrder.OrderID;
            vm.loading = true;

            InpatientService.executeCareOrder(orderID, vm.executeForm).then(function(res) {
                toastr.success('تم توثيق تنفيذ الخدمة التمريضية بنجاح');
                vm.showExecuteOrderModal = false;
                loadNursingDashboard();
                loadAdmissions();
            }).catch(function(err) {
                toastr.error('فشل توثيق تنفيذ الخدمة');
            }).finally(function() {
                vm.loading = false;
            });
        }

        function loadMyStay() {
            vm.loadingMyStay = true;
            InpatientService.getAdmissions('').then(function(res) {
                var list = (res && res.data) ? res.data : (res || []);
                if (angular.isArray(list) && list.length > 0) {
                    var active = list.find(function(a) { return (a.status || a.Status) === 'Active'; }) || list[0];
                    var id = active.admissionID || active.AdmissionID;
                    InpatientService.getAdmissionById(id).then(function(detailsRes) {
                        vm.myStay = (detailsRes && detailsRes.data) ? detailsRes.data : detailsRes;
                    });
                } else {
                    vm.myStay = null;
                }
            }).finally(function() {
                vm.loadingMyStay = false;
            });
        }

        function loadBedGrid() {
            vm.loading = true;
            InpatientService.getBedGrid().then(function(res) {
                var responseData = (res && res.data) ? res.data : (res || []);
                vm.wards = responseData;
                calculateStats(responseData);
            }).catch(function(err) {
                toastr.error('حدث خطأ في تحميل خريطة الأسرة والتجهيزات');
            }).finally(function() {
                vm.loading = false;
            });
        }

        function calculateStats(wards) {
            var totalRooms = 0;
            var totalBeds = 0;
            var occupied = 0;
            var vacant = 0;

            if (angular.isArray(wards)) {
                wards.forEach(function(w) {
                    var rooms = w.rooms || w.Rooms || [];
                    totalRooms += rooms.length;
                    rooms.forEach(function(r) {
                        var beds = r.beds || r.Beds || [];
                        totalBeds += beds.length;
                        beds.forEach(function(b) {
                            var status = b.status || b.Status;
                            if (status === 'Occupied') occupied++;
                            else if (status === 'Vacant') vacant++;
                        });
                    });
                });
            }

            vm.stats.totalWards = wards.length;
            vm.stats.totalRooms = totalRooms;
            vm.stats.totalBeds = totalBeds;
            vm.stats.occupiedBeds = occupied;
            vm.stats.vacantBeds = vacant;
            vm.stats.occupancyRate = totalBeds > 0 ? Math.round((occupied / totalBeds) * 100) : 0;
        }

        function loadAdmissions() {
            InpatientService.getAdmissions('Active').then(function(res) {
                var data = (res && res.data) ? res.data : (res || []);
                vm.admissions = data;
            });
        }

        function loadPrerequisites() {
            UsersService.getDoctors().then(function(res) {
                var data = (res && res.data) ? res.data : (res || []);
                vm.doctors = data;
            });
            UsersService.getPatients().then(function(res) {
                var data = (res && res.data) ? res.data : (res || []);
                vm.patients = data;
            });
        }

        function openAdmissionModal(bed) {
            var status = bed.status || bed.Status;
            if (status === 'Occupied') {
                var admission = bed.currentAdmission || bed.CurrentAdmission;
                if (admission) {
                    var admissionID = admission.admissionID || admission.AdmissionID;
                    openLogModal(admission);
                } else {
                    toastr.info('هذا السرير مشغول بالفعل');
                }
                return;
            }

            var bedID = bed.bedID || bed.BedID;
            vm.selectedBed = bed;
            vm.admissionForm = {
                bedID: bedID,
                patientID: '',
                doctorID: '',
                admissionReason: ''
            };
            vm.showAdmissionModal = true;
        }

        function submitAdmission() {
            if (!vm.admissionForm.patientID || !vm.admissionForm.doctorID || !vm.admissionForm.admissionReason) {
                toastr.warning('يرجى تعبئة كافة البيانات المطلوبة');
                return;
            }

            vm.loading = true;
            var payload = {
                patientID: parseInt(vm.admissionForm.patientID),
                doctorID: parseInt(vm.admissionForm.doctorID),
                bedID: parseInt(vm.admissionForm.bedID),
                admissionReason: vm.admissionForm.admissionReason
            };

            InpatientService.createAdmission(payload).then(function(res) {
                toastr.success('تم تنويم المريض وتسكينه بالسرير بنجاح');
                vm.showAdmissionModal = false;
                loadBedGrid();
                loadAdmissions();
            }).catch(function(err) {
                toastr.error(err.data && err.data.message ? err.data.message : 'فشل تنويم المريض');
            }).finally(function() {
                vm.loading = false;
            });
        }

        function openDischargeModal(admission) {
            vm.selectedAdmission = admission;
            vm.dischargeForm = { dischargeSummary: '' };
            vm.showDischargeModal = true;
        }

        function submitDischarge() {
            if (!vm.dischargeForm.dischargeSummary) {
                toastr.warning('يرجى كتابة ملخص تقرير الخروج');
                return;
            }

            var admissionID = vm.selectedAdmission.admissionID || vm.selectedAdmission.AdmissionID;
            vm.loading = true;

            InpatientService.dischargePatient(admissionID, { dischargeSummary: vm.dischargeForm.dischargeSummary }).then(function(res) {
                toastr.success(res.data.message || 'تم تسجيل خروج المريض وتفريغ السرير بنجاح');
                vm.showDischargeModal = false;
                loadBedGrid();
                loadAdmissions();
            }).catch(function(err) {
                toastr.error('حدث خطأ في إجراء الخروج');
            }).finally(function() {
                vm.loading = false;
            });
        }

        function openLogModal(admission) {
            vm.selectedAdmission = admission;
            vm.selectedAdmissionLogs = [];
            vm.logForm = {
                temperature: '37.0',
                bloodPressure: '120/80',
                pulseRate: '75',
                oxygenLevel: '98%',
                doctorNotes: '',
                nursingNotes: ''
            };

            var admissionID = admission.admissionID || admission.AdmissionID;
            InpatientService.getAdmissionById(admissionID).then(function(res) {
                var data = (res && res.data) ? res.data : res;
                vm.selectedAdmissionLogs = data.dailyLogs || data.DailyLogs || [];
            });

            vm.showLogModal = true;
        }

        function submitDailyLog() {
            var admissionID = vm.selectedAdmission.admissionID || vm.selectedAdmission.AdmissionID;
            vm.loading = true;

            InpatientService.addDailyLog(admissionID, vm.logForm).then(function(res) {
                toastr.success('تم تسجيل العلامات الحيوية والملاحظات اليومية بنجاح');
                
                // Refresh logs list in modal
                InpatientService.getAdmissionById(admissionID).then(function(detailsRes) {
                    var data = (detailsRes && detailsRes.data) ? detailsRes.data : detailsRes;
                    vm.selectedAdmissionLogs = data.dailyLogs || data.DailyLogs || [];
                });

                vm.logForm.doctorNotes = '';
                vm.logForm.nursingNotes = '';
                loadAdmissions();
            }).catch(function(err) {
                toastr.error('حدث خطأ أثناء حفظ السجل الحيوي');
            }).finally(function() {
                vm.loading = false;
            });
        }

        function submitWard() {
            if (!vm.wardForm.wardNameAr || !vm.wardForm.wardName) {
                toastr.warning('يرجى كتابة اسم القسم بالعربي والإنجليزي');
                return;
            }
            InpatientService.createWard(vm.wardForm).then(function(res) {
                toastr.success('تم إضافة الجناح بنجاح');
                vm.showAddWardModal = false;
                vm.wardForm = { wardName: '', wardNameAr: '', genderType: 'Mixed', floorNumber: 1 };
                loadBedGrid();
            });
        }

        function submitRoom() {
            if (!vm.roomForm.wardID || !vm.roomForm.roomNumber) {
                toastr.warning('يرجى اختيار القسم وتحديد رقم الغرفة');
                return;
            }
            var payload = {
                wardID: parseInt(vm.roomForm.wardID),
                roomNumber: vm.roomForm.roomNumber,
                roomType: vm.roomForm.roomType,
                dailyRate: parseFloat(vm.roomForm.dailyRate),
                maxBeds: parseInt(vm.roomForm.maxBeds)
            };
            InpatientService.createRoom(payload).then(function(res) {
                toastr.success('تم إضافة الغرفة بنجاح');
                vm.showAddRoomModal = false;
                vm.roomForm = { wardID: '', roomNumber: '', roomType: 'General', dailyRate: 200, maxBeds: 2 };
                loadBedGrid();
            });
        }

        function submitBed() {
            if (!vm.bedForm.roomID || !vm.bedForm.bedNumber) {
                toastr.warning('يرجى اختيار الغرفة وتحديد رقم السرير');
                return;
            }
            var payload = {
                roomID: parseInt(vm.bedForm.roomID),
                bedNumber: vm.bedForm.bedNumber,
                notes: vm.bedForm.notes
            };
            InpatientService.createBed(payload).then(function(res) {
                toastr.success('تم إضافة السرير بنجاح');
                vm.showAddBedModal = false;
                vm.bedForm = { roomID: '', bedNumber: '', notes: '' };
                loadBedGrid();
            });
        }
    }
})();
