(function() {
    'use strict';

    angular.module('medicalApp').controller('LabController', LabController);

    LabController.$inject = ['LabService', 'UsersService', 'AuthService', 'toastr', '$window'];

    function LabController(LabService, UsersService, AuthService, toastr, $window) {
        var vm = this;

        vm.currentUser = AuthService.getUser() || {};
        vm.labTests = [];
        vm.orders = [];
        vm.patients = [];
        vm.devices = [];
        vm.loading = true;
        vm.activeTab = 'orders';

        var canManageTests = vm.currentUser.role === 'Admin' || vm.currentUser.role === 'LabTechnician';
        var canOrder = vm.currentUser.role === 'Admin' || vm.currentUser.role === 'Doctor';
        var canResult = vm.currentUser.role === 'Admin' || vm.currentUser.role === 'Doctor' || vm.currentUser.role === 'LabTechnician';
        var canManageDevices = vm.currentUser.role === 'Admin' || vm.currentUser.role === 'LabTechnician';
        vm.canManageTests = canManageTests;
        vm.canOrder = canOrder;
        vm.canResult = canResult;
        vm.canManageDevices = canManageDevices;

        // Tabs
        vm.setTab = setTab;

        // Orders
        vm.newOrder = {
            patientUserId: null,
            testIds: [],
            resultNotes: ''
        };
        vm.toggleTestSelection = toggleTestSelection;
        vm.openOrderModal = openOrderModal;
        vm.submitCreateOrder = submitCreateOrder;

        // Result
        vm.showResultModal = false;
        vm.selectedOrder = null;
        vm.selectedItem = null;
        vm.resultInput = { resultValue: '', technicianNotes: '' };
        vm.openResultModal = openResultModal;
        vm.submitResult = submitResult;

        // Culture & Sensitivity
        vm.showCultureModal = false;
        vm.culture = null;
        vm.cultureInput = {};
        vm.sensitivityInput = {};
        vm.openCultureModal = openCultureModal;
        vm.saveCulture = saveCulture;
        vm.addSensitivity = addSensitivity;

        // Print
        vm.showPrintModal = false;
        vm.openPrintModal = openPrintModal;
        vm.printReport = printReport;

        // Test Management
        vm.showTestModal = false;
        vm.testForm = null;
        vm.testEditMode = false;
        vm.newRange = {};
        vm.openCreateTestModal = openCreateTestModal;
        vm.openEditTestModal = openEditTestModal;
        vm.addRange = addRange;
        vm.removeRange = removeRange;
        vm.submitTest = submitTest;
        vm.deleteTest = deleteTest;

        // Panel members
        vm.panelMemberTestId = null;
        vm.addPanelMember = addPanelMember;

        // Devices
        vm.showDeviceModal = false;
        vm.deviceForm = {};
        vm.deviceEditMode = false;
        vm.openCreateDeviceModal = openCreateDeviceModal;
        vm.openEditDeviceModal = openEditDeviceModal;
        vm.submitDevice = submitDevice;

        // Device capture
        vm.showCaptureModal = false;
        vm.captureDevice = null;
        vm.captureInput = { itemId: null, value: '', notes: '' };
        vm.openCaptureModal = openCaptureModal;
        vm.submitCapture = submitCapture;

        vm.pendingItems = [];

        activate();

        function activate() {
            loadTests();
            loadOrders();
            if (vm.currentUser.role !== 'Patient') {
                loadPatients();
            }
            if (canManageDevices) {
                loadDevices();
            }
        }

        function setTab(tab) {
            vm.activeTab = tab;
        }

        // ============================================================
        //  Orders
        // ============================================================

        function loadOrders() {
            vm.loading = true;
            LabService.getLabOrders().then(function(res) {
                if (res.success) {
                    vm.orders = res.data;
                }
            }).finally(function() {
                vm.loading = false;
            });
        }

        function loadPatients() {
            UsersService.getUsers({ role: 'Patient', pageSize: 100 }).then(function(res) {
                vm.patients = res.data || [];
            });
        }

        function loadTests() {
            LabService.getLabTests().then(function(res) {
                if (res.success) {
                    vm.labTests = res.data;
                }
            });
        }

        function loadDevices() {
            LabService.getDevices().then(function(res) {
                if (res.success) {
                    vm.devices = res.data;
                }
            });
        }

        function openOrderModal() {
            vm.newOrder.patientUserId = null;
            vm.newOrder.testIds = [];
            vm.newOrder.resultNotes = '';
            vm.showOrderModal = true;
        }

        function toggleTestSelection(testId) {
            var idx = vm.newOrder.testIds.indexOf(testId);
            if (idx > -1) {
                vm.newOrder.testIds.splice(idx, 1);
            } else {
                vm.newOrder.testIds.push(testId);
            }
        }

        function isTestSelected(testId) {
            return vm.newOrder.testIds.indexOf(testId) > -1;
        }
        vm.isTestSelected = isTestSelected;

        function submitCreateOrder() {
            if (!vm.newOrder.patientUserId) {
                toastr.warning('يرجى اختيار المريض');
                return;
            }
            if (!vm.newOrder.testIds.length) {
                toastr.warning('يرجى اختيار فحص أو بانل واحد على الأقل');
                return;
            }

            LabService.createLabOrder({
                patientUserId: vm.newOrder.patientUserId,
                labTestIDs: vm.newOrder.testIds,
                resultNotes: vm.newOrder.resultNotes
            }).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showOrderModal = false;
                    loadOrders();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        // ============================================================
        //  Item Result
        // ============================================================

        function openResultModal(order, item) {
            vm.selectedOrder = order;
            vm.selectedItem = item;
            vm.resultInput.resultValue = item.resultValue || '';
            vm.resultInput.technicianNotes = item.technicianNotes || '';
            vm.showResultModal = true;
        }

        function submitResult() {
            if (!vm.resultInput.resultValue) {
                toastr.warning('يرجى إدخال قيمة النتيجة');
                return;
            }

            LabService.updateLabResult(vm.selectedOrder.labOrderID, vm.selectedItem.labOrderItemID, vm.resultInput).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showResultModal = false;
                    loadOrders();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        // ============================================================
        //  Culture & Sensitivity
        // ============================================================

        function openCultureModal(order, item) {
            vm.selectedOrder = order;
            vm.selectedItem = item;
            vm.culture = null;
            vm.cultureInput = {
                organism: '',
                gramStain: '',
                cultureStatus: 'NoGrowth',
                quantitativeResult: ''
            };
            vm.sensitivityInput = { antibioticName: '', interpretation: 'Sensitive', zoneDiameter: null };
            vm.showCultureModal = true;

            LabService.getCulture(order.labOrderID, item.labOrderItemID).then(function(res) {
                if (res.success) {
                    vm.culture = res.data;
                    vm.cultureInput.organism = res.data.organism || '';
                    vm.cultureInput.gramStain = res.data.gramStain || '';
                    vm.cultureInput.cultureStatus = res.data.cultureStatus || 'NoGrowth';
                    vm.cultureInput.quantitativeResult = res.data.quantitativeResult || '';
                }
            }).catch(function() {
                // لا توجد مزرعة بعد — نمط طبيعي لعنصر جديد
            });
        }

        function saveCulture() {
            LabService.saveCulture(vm.selectedOrder.labOrderID, vm.selectedItem.labOrderItemID, vm.cultureInput).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.culture = res.data;
                } else {
                    toastr.error(res.message);
                }
            });
        }

        function addSensitivity() {
            if (!vm.sensitivityInput.antibioticName) {
                toastr.warning('يرجى إدخال اسم المضاد الحيوي');
                return;
            }
            if (!vm.culture) {
                toastr.warning('يرجى حفظ بيانات المزرعة أولاً');
                return;
            }

            LabService.addSensitivity(vm.culture.cultureSensitivityID, vm.sensitivityInput).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.culture.sensitivityResults.push(res.data);
                    vm.sensitivityInput = { antibioticName: '', interpretation: 'Sensitive', zoneDiameter: null };
                } else {
                    toastr.error(res.message);
                }
            });
        }

        // ============================================================
        //  Print
        // ============================================================

        function openPrintModal(order) {
            vm.selectedOrder = order;
            vm.showPrintModal = true;
        }

        function printReport() {
            $window.print();
        }

        // ============================================================
        //  Tests & Panels
        // ============================================================

        function emptyTestForm() {
            return {
                testName: '',
                code: '',
                category: 'General',
                price: 25,
                unit: 'mg/dL',
                isPanel: false,
                deviceID: null,
                referenceRanges: [{ gender: 'All', minAge: 0, maxAge: 120, normalMin: 0, normalMax: 100, rangeNotes: '' }]
            };
        }

        function openCreateTestModal() {
            vm.testEditMode = false;
            vm.testForm = emptyTestForm();
            vm.showTestModal = true;
        }

        function openEditTestModal(test) {
            vm.testEditMode = true;
            vm.testForm = {
                testName: test.testName,
                code: test.code,
                category: test.category,
                price: test.price,
                unit: test.unit,
                isPanel: test.isPanel,
                deviceID: test.deviceID,
                referenceRanges: (test.referenceRanges || []).map(function(r) {
                    return { gender: r.gender, minAge: r.minAge, maxAge: r.maxAge, normalMin: r.normalMin, normalMax: r.normalMax, rangeNotes: r.rangeNotes };
                })
            };
            if (!vm.testForm.referenceRanges.length) {
                vm.testForm.referenceRanges.push({ gender: 'All', minAge: 0, maxAge: 120, normalMin: 0, normalMax: 100, rangeNotes: '' });
            }
            vm.editingTestId = test.labTestID;
            vm.showTestModal = true;
        }

        function addRange() {
            vm.testForm.referenceRanges.push({ gender: 'All', minAge: 0, maxAge: 120, normalMin: 0, normalMax: 100, rangeNotes: '' });
        }

        function removeRange(idx) {
            vm.testForm.referenceRanges.splice(idx, 1);
        }

        function submitTest() {
            if (!vm.testForm.testName || !vm.testForm.code) {
                toastr.warning('اسم الفحص وكوده مطلوبان');
                return;
            }

            var op = vm.testEditMode
                ? LabService.updateLabTest(vm.editingTestId, vm.testForm)
                : LabService.createLabTest(vm.testForm);

            op.then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showTestModal = false;
                    loadTests();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        function deleteTest(test) {
            if (test.isPanel || (test.panelChildren && test.panelChildren.length > 0)) {
                // بانل أو أب لعناصر — حذف سيقوم الخادم بفحص الارتباط
            }
            if (!confirm('حذف الفحص "' + test.testName + '"؟ لا يمكن حذف فحص مرتبط بطلبات سابقة.')) return;
            LabService.deleteLabTest(test.labTestID).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    loadTests();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        function addPanelMember(test) {
            if (!vm.panelMemberTestId) {
                toastr.warning('اختر الفحص الفرعي لإضافته إلى البانل');
                return;
            }
            LabService.addPanelMember(test.labTestID, { memberTestID: vm.panelMemberTestId }).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.panelMemberTestId = null;
                    loadTests();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        // ============================================================
        //  Devices
        // ============================================================

        function emptyDeviceForm() {
            return { deviceName: '', deviceCode: '', deviceModel: '', connectionType: 'Manual', isActive: true };
        }

        function openCreateDeviceModal() {
            vm.deviceEditMode = false;
            vm.deviceForm = emptyDeviceForm();
            vm.showDeviceModal = true;
        }

        function openEditDeviceModal(device) {
            vm.deviceEditMode = true;
            vm.deviceForm = {
                deviceName: device.deviceName,
                deviceCode: device.deviceCode,
                deviceModel: device.deviceModel,
                connectionType: device.connectionType,
                isActive: device.isActive
            };
            vm.editingDeviceId = device.labDeviceID;
            vm.showDeviceModal = true;
        }

        function submitDevice() {
            if (!vm.deviceForm.deviceName || !vm.deviceForm.deviceCode) {
                toastr.warning('اسم الجهاز وكوده مطلوبان');
                return;
            }
            var op = vm.deviceEditMode
                ? LabService.updateDevice(vm.editingDeviceId, vm.deviceForm)
                : LabService.createDevice(vm.deviceForm);
            op.then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showDeviceModal = false;
                    loadDevices();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        function openCaptureModal(device) {
            vm.captureDevice = device;
            vm.captureInput = { itemId: null, value: '', notes: '' };
            // قائمة العناصر المعلّقة (بانتظار النتيجة) لالتقاط نتيجتها من الجهاز
            vm.pendingItems = [];
            vm.orders.forEach(function(o) {
                (o.items || []).forEach(function(item) {
                    if (item.resultStatus === 'Pending' && item.labTest && (!item.labTest.deviceID || item.labTest.deviceID === device.labDeviceID)) {
                        vm.pendingItems.push({
                            id: item.labOrderItemID,
                            label: o.patientUser.fullName + ' — ' + item.labTest.testName + ' (#' + o.labOrderID + ')'
                        });
                    }
                });
            });
            if (!vm.pendingItems.length) {
                toastr.warning('لا توجد عناصر معلّقة تنتظر النتيجة لهذا الجهاز');
                return;
            }
            vm.showCaptureModal = true;
        }

        function submitCapture() {
            if (!vm.captureInput.itemId || !vm.captureInput.value) {
                toastr.warning('اختر العنصر وأدخل القيمة');
                return;
            }
            LabService.captureDeviceResult(vm.captureDevice.labDeviceID, {
                labOrderItemID: vm.captureInput.itemId,
                value: vm.captureInput.value,
                notes: vm.captureInput.notes
            }).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showCaptureModal = false;
                    loadOrders();
                } else {
                    toastr.error(res.message);
                }
            });
        }
    }
})();
