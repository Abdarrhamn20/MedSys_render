(function() {
    'use strict';

    angular.module('medicalApp').controller('PriceManagementController', PriceManagementController);

    PriceManagementController.$inject = ['PriceManagementService', 'toastr'];

    function PriceManagementController(PriceManagementService, toastr) {
        var vm = this;

        vm.activeTab = 'doctors';
        vm.loading = false;
        vm.overview = {};

        // Data arrays
        vm.doctors = [];
        vm.labTests = [];
        vm.radiologyTemplates = [];
        vm.medications = [];
        vm.rooms = [];
        vm.inventory = [];
        vm.healthServices = [];

        // Editing state — uses camelCase matching API response
        vm.editingId = null;
        vm.editPrice = 0;
        vm.editPurchasePrice = 0;
        vm.editSellingPrice = 0;
        vm.editField = 'price';

        // Search + paging
        vm.medSearch = '';
        vm.invSearch = '';
        vm.medPage = 1;
        vm.invPage = 1;
        vm.medTotal = 0;
        vm.invTotal = 0;

        // Adjust prices
        vm.adjustEntityType = 'doctors';
        vm.adjustPercentage = 0;

        // Create forms
        vm.showCreateForm = false;
        vm.newLabTest = {};
        vm.newRadiology = {};
        vm.newHealthService = {};

        // Methods
        vm.setTab = setTab;
        vm.startEdit = startEdit;
        vm.cancelEdit = cancelEdit;
        vm.savePrice = savePrice;
        vm.saveMedPrices = saveMedPrices;
        vm.saveInvPrices = saveInvPrices;
        vm.adjustAllPrices = adjustAllPrices;
        vm.searchMedications = searchMedications;
        vm.searchInventory = searchInventory;
        vm.toggleCreateForm = toggleCreateForm;
        vm.createItem = createItem;
        vm.deleteItem = deleteItem;

        activate();

        function activate() {
            loadOverview();
            loadTab();
        }

        function loadOverview() {
            PriceManagementService.getOverview().then(function(res) {
                if (res.success) vm.overview = res.data;
            });
        }

        function setTab(tab) {
            vm.activeTab = tab;
            vm.editingId = null;
            vm.showCreateForm = false;
            loadTab();
        }

        function loadTab() {
            vm.loading = true;

            switch (vm.activeTab) {
                case 'doctors':
                    PriceManagementService.getDoctorFees().then(function(res) {
                        vm.doctors = res.success ? res.data : [];
                        vm.loading = false;
                    }).catch(function() { vm.loading = false; toastr.error('خطأ في تحميل الأطباء'); });
                    break;

                case 'labTests':
                    PriceManagementService.getLabTestPrices().then(function(res) {
                        vm.labTests = res.success ? res.data : [];
                        vm.loading = false;
                    }).catch(function() { vm.loading = false; toastr.error('خطأ في تحميل التحاليل'); });
                    break;

                case 'radiology':
                    PriceManagementService.getRadiologyPrices().then(function(res) {
                        vm.radiologyTemplates = res.success ? res.data : [];
                        vm.loading = false;
                    }).catch(function() { vm.loading = false; toastr.error('خطأ في تحميل الأشعة'); });
                    break;

                case 'medications':
                    PriceManagementService.getMedicationPrices(vm.medSearch, vm.medPage).then(function(res) {
                        vm.medications = res.data || [];
                        vm.medTotal = res.totalCount || 0;
                        vm.loading = false;
                    }).catch(function() { vm.loading = false; toastr.error('خطأ في تحميل الأدوية'); });
                    break;

                case 'rooms':
                    PriceManagementService.getRoomPrices().then(function(res) {
                        vm.rooms = res.success ? res.data : [];
                        vm.loading = false;
                    }).catch(function() { vm.loading = false; toastr.error('خطأ في تحميل الغرف'); });
                    break;

                case 'inventory':
                    PriceManagementService.getInventoryPrices(vm.invSearch, vm.invPage).then(function(res) {
                        vm.inventory = res.data || [];
                        vm.invTotal = res.totalCount || 0;
                        vm.loading = false;
                    }).catch(function() { vm.loading = false; toastr.error('خطأ في تحميل المخزون'); });
                    break;

                case 'healthServices':
                    PriceManagementService.getHealthServices().then(function(res) {
                        vm.healthServices = res.success ? res.data : [];
                        vm.loading = false;
                    }).catch(function() { vm.loading = false; toastr.error('خطأ في تحميل الخدمات الصحية'); });
                    break;

                case 'adjust':
                    vm.loading = false;
                    break;

                default:
                    vm.loading = false;
                    break;
            }
        }

        // =============================================
        //  EDIT FUNCTIONS — uses camelCase keys from API
        // =============================================

        function startEdit(item) {
            vm.editingId = item.doctorID || item.labTestID || item.templateID || item.roomID || item.medicationID || item.itemID || item.serviceID;
            vm.editPrice = item.price || item.dailyRate || item.sellingPrice || 0;
            vm.editPurchasePrice = item.purchasePrice || 0;
            vm.editSellingPrice = item.sellingPrice || 0;
        }

        function cancelEdit() {
            vm.editingId = null;
            vm.editPrice = 0;
            vm.editField = 'price';
        }

        function savePrice(item, entityType) {
            var id = item.doctorID || item.labTestID || item.templateID || item.roomID || item.serviceID;
            var price = vm.editPrice;

            if (price < 0) {
                toastr.error('السعر لا يمكن أن يكون سالباً');
                return;
            }

            var promise;
            switch (entityType) {
                case 'doctor':
                    promise = PriceManagementService.updateDoctorFee(id, price);
                    break;
                case 'labtest':
                    promise = PriceManagementService.updateLabTestPrice(id, price);
                    break;
                case 'radiology':
                    promise = PriceManagementService.updateRadiologyPrice(id, price);
                    break;
                case 'room':
                    promise = PriceManagementService.updateRoomRate(id, price);
                    break;
                case 'healthService':
                    promise = PriceManagementService.updateHealthService(id, { price: price });
                    break;
            }

            if (promise) {
                promise.then(function(res) {
                    if (res.success) {
                        toastr.success(res.message);
                        vm.editingId = null;
                        loadTab();
                    } else {
                        toastr.error(res.message);
                    }
                }).catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ');
                });
            }
        }

        function saveMedPrices(item) {
            if (vm.editPurchasePrice < 0 || vm.editSellingPrice < 0) {
                toastr.error('الأسعار لا يمكن أن تكون سالبة');
                return;
            }
            PriceManagementService.updateMedicationPrices(item.medicationID, {
                purchasePrice: vm.editPurchasePrice,
                sellingPrice: vm.editSellingPrice
            }).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.editingId = null;
                    loadTab();
                } else {
                    toastr.error(res.message);
                }
            }).catch(function(err) {
                toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ');
            });
        }

        function saveInvPrices(item) {
            if (vm.editPurchasePrice < 0 || vm.editSellingPrice < 0) {
                toastr.error('الأسعار لا يمكن أن تكون سالبة');
                return;
            }
            PriceManagementService.updateInventoryPrices(item.itemID, {
                purchasePrice: vm.editPurchasePrice,
                sellingPrice: vm.editSellingPrice
            }).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.editingId = null;
                    loadTab();
                } else {
                    toastr.error(res.message);
                }
            }).catch(function(err) {
                toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ');
            });
        }

        function adjustAllPrices() {
            if (!vm.adjustPercentage) {
                toastr.error('أدخل النسبة المطلوبة');
                return;
            }
            if (!confirm('هل أنت متأكد من تعديل أسعار "' + getEntityLabel(vm.adjustEntityType) + '" بنسبة ' + vm.adjustPercentage + '%؟')) return;

            PriceManagementService.adjustPrices(vm.adjustEntityType, vm.adjustPercentage).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    loadTab();
                    loadOverview();
                } else {
                    toastr.error(res.message);
                }
            }).catch(function(err) {
                toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ');
            });
        }

        function searchMedications() {
            vm.medPage = 1;
            loadTab();
        }

        function searchInventory() {
            vm.invPage = 1;
            loadTab();
        }

        function getEntityLabel(type) {
            var labels = { doctors: 'رسوم الكشف', labTests: 'التحاليل', radiology: 'الأشعة', rooms: 'الغرف', medications: 'الأدوية', inventory: 'المخزون', healthServices: 'الخدمات الصحية' };
            return labels[type] || type;
        }

        // =============================================
        //  CREATE / DELETE FUNCTIONS
        // =============================================

        function toggleCreateForm() {
            vm.showCreateForm = !vm.showCreateForm;
            vm.newLabTest = {};
            vm.newRadiology = {};
            vm.newHealthService = {};
        }

        function createItem() {
            switch (vm.activeTab) {
                case 'labTests':
                    createLabTest();
                    break;
                case 'radiology':
                    createRadiologyTemplate();
                    break;
                case 'healthServices':
                    createHealthService();
                    break;
            }
        }

        function createLabTest() {
            var d = vm.newLabTest;
            if (!d.testName || !d.code) { toastr.error('اسم الفحص وكوده مطلوبان'); return; }
            if ((d.price || 0) < 0) { toastr.error('السعر لا يمكن أن يكون سالباً'); return; }

            PriceManagementService.createLabTest({
                testName: d.testName,
                code: d.code,
                category: d.category || 'General',
                price: d.price || 0,
                unit: d.unit || 'mg/dL'
            }).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showCreateForm = false;
                    vm.newLabTest = {};
                    loadTab();
                    loadOverview();
                } else {
                    toastr.error(res.message);
                }
            }).catch(function(err) {
                toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ');
            });
        }

        function createRadiologyTemplate() {
            var d = vm.newRadiology;
            if (!d.templateName || !d.modality) { toastr.error('اسم القالب والطريقة مطلوبة'); return; }
            if ((d.price || 0) < 0) { toastr.error('السعر لا يمكن أن يكون سالباً'); return; }

            PriceManagementService.createRadiologyTemplate({
                templateName: d.templateName,
                modality: d.modality,
                bodyPart: d.bodyPart || 'General',
                defaultReportText: d.defaultReportText || '',
                price: d.price || 0
            }).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showCreateForm = false;
                    vm.newRadiology = {};
                    loadTab();
                    loadOverview();
                } else {
                    toastr.error(res.message);
                }
            }).catch(function(err) {
                toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ');
            });
        }

        function createHealthService() {
            var d = vm.newHealthService;
            if (!d.serviceName || !d.serviceNameAr) { toastr.error('اسم الخدمة بالعربية والإنجليزية مطلوب'); return; }
            if ((d.price || 0) < 0) { toastr.error('السعر لا يمكن أن يكون سالباً'); return; }

            PriceManagementService.createHealthService({
                serviceName: d.serviceName,
                serviceNameAr: d.serviceNameAr,
                category: d.category || 'General',
                description: d.description || '',
                price: d.price || 0,
                unit: d.unit || 'مرة'
            }).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showCreateForm = false;
                    vm.newHealthService = {};
                    loadTab();
                    loadOverview();
                } else {
                    toastr.error(res.message);
                }
            }).catch(function(err) {
                toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ');
            });
        }

        function deleteItem(item, entityType) {
            var name = '';
            var id = 0;
            switch (entityType) {
                case 'labtest':
                    id = item.labTestID;
                    name = item.testName;
                    if (!confirm('هل أنت متأكد من حذف فحص "' + name + '"؟')) return;
                    PriceManagementService.deleteLabTest(id).then(function(res) {
                        if (res.success) { toastr.success(res.message); loadTab(); loadOverview(); }
                        else toastr.error(res.message);
                    }).catch(function(err) { toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ'); });
                    break;
                case 'radiology':
                    id = item.templateID;
                    name = item.templateName;
                    if (!confirm('هل أنت متأكد من حذف قالب "' + name + '"؟')) return;
                    PriceManagementService.deleteRadiologyTemplate(id).then(function(res) {
                        if (res.success) { toastr.success(res.message); loadTab(); loadOverview(); }
                        else toastr.error(res.message);
                    }).catch(function(err) { toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ'); });
                    break;
                case 'healthService':
                    id = item.serviceID;
                    name = item.serviceNameAr;
                    if (!confirm('هل أنت متأكد من حذف الخدمة "' + name + '"؟')) return;
                    PriceManagementService.deleteHealthService(id).then(function(res) {
                        if (res.success) { toastr.success(res.message); loadTab(); loadOverview(); }
                        else toastr.error(res.message);
                    }).catch(function(err) { toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ'); });
                    break;
            }
        }
    }
})();
