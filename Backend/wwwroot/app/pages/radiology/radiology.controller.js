(function() {
    'use strict';

    angular.module('medicalApp').controller('RadiologyController', RadiologyController);

    RadiologyController.$inject = ['RadiologyService', 'UsersService', 'AuthService', 'toastr', '$window'];

    function RadiologyController(RadiologyService, UsersService, AuthService, toastr, $window) {
        var vm = this;

        vm.currentUser = AuthService.getUser() || {};
        vm.templates = [];
        vm.orders = [];
        vm.patients = [];
        vm.loading = true;

        // Modals State
        vm.showOrderModal = false;
        vm.showReportModal = false;
        vm.showViewerModal = false;
        vm.showImageModal = false;
        vm.uploadingImage = false;
        vm.selectedOrder = null;

        // Form models
        vm.newOrder = {
            patientUserId: null,
            templateID: null,
            modality: 'X-Ray',
            bodyPart: 'Chest'
        };

        vm.reportInput = {
            reportText: '',
            imagePath: ''
        };

        // Functions
        vm.loadOrders = loadOrders;
        vm.loadTemplates = loadTemplates;
        vm.loadPatients = loadPatients;
        vm.openOrderModal = openOrderModal;
        vm.submitCreateOrder = submitCreateOrder;
        vm.openReportModal = openReportModal;
        vm.applyTemplate = applyTemplate;
        vm.submitReport = submitReport;
        vm.openViewerModal = openViewerModal;
        vm.openImageModal = openImageModal;
        vm.uploadImage = uploadImage;
        vm.printReport = printReport;

        activate();

        function activate() {
            loadTemplates();
            loadOrders();
            if (vm.currentUser.role !== 'Patient') {
                loadPatients();
            }
        }

        function loadTemplates() {
            RadiologyService.getTemplates().then(function(res) {
                if (res.success) {
                    vm.templates = res.data;
                }
            });
        }

        function loadPatients() {
            UsersService.getUsers({ role: 'Patient', pageSize: 100 }).then(function(res) {
                vm.patients = res.data || [];
            });
        }

        function loadOrders() {
            vm.loading = true;
            RadiologyService.getRadiologyOrders().then(function(res) {
                if (res.success) {
                    vm.orders = res.data;
                }
            }).finally(function() {
                vm.loading = false;
            });
        }

        function openOrderModal() {
            vm.showOrderModal = true;
        }

        function submitCreateOrder() {
            if (!vm.newOrder.patientUserId) {
                toastr.warning('يرجى اختيار المريض');
                return;
            }

            RadiologyService.createRadiologyOrder(vm.newOrder).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showOrderModal = false;
                    vm.newOrder = { patientUserId: null, templateID: null, modality: 'X-Ray', bodyPart: 'Chest' };
                    loadOrders();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        function openReportModal(order) {
            vm.selectedOrder = order;
            vm.reportInput.reportText = order.reportText || '';
            vm.reportInput.imagePath = order.imagePath || '';
            vm.showReportModal = true;
        }

        function applyTemplate(tpl) {
            if (tpl && tpl.defaultReportText) {
                vm.reportInput.reportText = tpl.defaultReportText;
                toastr.info('تم تطبيق قالب التقرير الجاهز');
            }
        }

        function submitReport() {
            if (!vm.reportInput.reportText) {
                toastr.warning('يرجى إدخال نص التقرير');
                return;
            }

            RadiologyService.updateRadiologyReport(vm.selectedOrder.radiologyOrderID, vm.reportInput).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showReportModal = false;
                    loadOrders();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        function openViewerModal(order) {
            vm.selectedOrder = order;
            vm.showViewerModal = true;
        }

        function openImageModal() {
            vm.showImageModal = true;
        }

        function uploadImage() {
            var input = document.getElementById('radiologyImageFile');
            if (!input || !input.files || !input.files.length) {
                toastr.warning('اختر ملف صورة أولاً.');
                return;
            }
            var file = input.files[0];
            vm.uploadingImage = true;
            RadiologyService.uploadImage(file).then(function(res) {
                if (res && res.success && res.data) {
                    vm.reportInput.imagePath = res.data;
                    toastr.success('تم رفع الصورة بنجاح وأُدرج مسارها في الحقل.');
                } else {
                    toastr.error((res && res.message) || 'فشل رفع الصورة.');
                }
            }).catch(function(err) {
                var msg = (err && err.data && err.data.message) || 'فشل رفع الصورة إلى الخادم.';
                toastr.error(msg);
            }).finally(function() {
                vm.uploadingImage = false;
            });
        }

        function printReport() {
            $window.print();
        }
    }
})();
