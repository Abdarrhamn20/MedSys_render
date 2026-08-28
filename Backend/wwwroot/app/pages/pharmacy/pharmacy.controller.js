(function() {
    'use strict';

    angular.module('medicalApp').controller('PharmacyController', PharmacyController);

    PharmacyController.$inject = ['PharmacyService', 'AuthService', 'toastr'];

    function PharmacyController(PharmacyService, AuthService, toastr) {
        var vm = this;

        var user = AuthService.getUser() || {};
        vm.role = user.role;
        vm.activeTab = 'inventory';

        // Inventory
        vm.medications = [];
        vm.medSearch = '';
        vm.medCategory = '';
        vm.categories = [];
        vm.showMedModal = false;
        vm.editingMed = null;
        vm.medForm = getEmptyMed();
        vm.savingMed = false;
        vm.medPage = 1;
        vm.medTotalPages = 1;

        // Dispensing
        vm.pendingPrescriptions = [];
        vm.pendingPrescriptionGroups = [];
        vm.dispenseSearch = '';
        vm.showDispenseModal = false;
        vm.selectedPrescription = null;
        vm.dispenseForm = {};
        vm.dispensing = false;

        // History
        vm.dispenseHistory = [];
        vm.historyPage = 1;
        vm.historyTotalPages = 1;

        // Dashboard
        vm.stats = {};
        vm.lowStockItems = [];

        // Functions
        vm.setTab = setTab;
        vm.loadMedications = loadMedications;
        vm.openAddMed = openAddMed;
        vm.openEditMed = openEditMed;
        vm.saveMedication = saveMedication;
        vm.deleteMedication = deleteMedication;
        vm.openDispense = openDispense;
        vm.confirmDispense = confirmDispense;
        vm.medPageChange = medPageChange;
        vm.historyPageChange = historyPageChange;
        vm.printGroupPrescription = printGroupPrescription;

        activate();

        function activate() {
            loadDashboard();
            loadMedications();
            loadPending();
            loadCategories();
        }

        function setTab(tab) {
            vm.activeTab = tab;
            if (tab === 'inventory') loadMedications();
            else if (tab === 'dispense') loadPending();
            else if (tab === 'history') loadHistory();
            else if (tab === 'dashboard') loadDashboard();
        }

        // =================== Dashboard ===================
        function loadDashboard() {
            PharmacyService.getDashboard().then(function(res) {
                if (res.success) vm.stats = res.data;
            });
            PharmacyService.getLowStock().then(function(res) {
                if (res.success) vm.lowStockItems = res.data;
            });
        }

        // =================== Medications ===================
        function loadMedications() {
            PharmacyService.getMedications({ search: vm.medSearch, category: vm.medCategory, page: vm.medPage, pageSize: 15 })
                .then(function(res) {
                    vm.medications = res.data || [];
                    vm.medTotalPages = res.totalPages || 1;
                });
        }

        function loadCategories() {
            PharmacyService.getCategories().then(function(res) {
                if (res.success) vm.categories = res.data || [];
            });
        }

        function openAddMed() {
            vm.editingMed = null;
            vm.medForm = getEmptyMed();
            vm.showMedModal = true;
        }

        function openEditMed(med) {
            vm.editingMed = med;
            vm.medForm = angular.copy(med);
            if (vm.medForm.expiryDate) vm.medForm.expiryDate = new Date(vm.medForm.expiryDate);
            vm.showMedModal = true;
        }

        function saveMedication() {
            if (!vm.medForm.nameAr) { toastr.warning('أدخل اسم الدواء'); return; }
            vm.savingMed = true;

            var promise;
            if (vm.editingMed) {
                promise = PharmacyService.updateMedication(vm.editingMed.medicationID, vm.medForm);
            } else {
                promise = PharmacyService.addMedication(vm.medForm);
            }

            promise.then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showMedModal = false;
                    loadMedications();
                    loadCategories();
                } else { toastr.error(res.message); }
            })
            .catch(function() { toastr.error('حدث خطأ'); })
            .finally(function() { vm.savingMed = false; });
        }

        function deleteMedication(med) {
            if (!confirm('هل أنت متأكد من حذف "' + med.nameAr + '"؟')) return;
            PharmacyService.deleteMedication(med.medicationID).then(function(res) {
                if (res.success) { toastr.success(res.message); loadMedications(); }
            });
        }

        function medPageChange(dir) {
            vm.medPage += dir;
            if (vm.medPage < 1) vm.medPage = 1;
            loadMedications();
        }

        function getEmptyMed() {
            return { name: '', nameAr: '', category: '', dosageForm: 'أقراص', unit: 'قرص', quantityInStock: 0, minStockLevel: 10, purchasePrice: 0, sellingPrice: 0, manufacturer: '', expiryDate: null };
        }

        // =================== Dispensing ===================
        function loadPending() {
            PharmacyService.getPendingPrescriptions().then(function(res) {
                if (res.success) {
                    var raw = res.data || [];
                    vm.pendingPrescriptions = raw;
                    
                    // Group by RecordID
                    var groups = {};
                    angular.forEach(raw, function(p) {
                        var key = p.recordID;
                        if (!groups[key]) {
                            groups[key] = {
                                recordID: p.recordID,
                                patientName: p.patientName,
                                patientPhone: p.patientPhone,
                                doctorName: p.doctorName,
                                appointmentDate: p.appointmentDate,
                                prescriptions: []
                            };
                        }
                        groups[key].prescriptions.push(p);
                    });
                    
                    // Convert back to array
                    vm.pendingPrescriptionGroups = Object.keys(groups).map(function(k) { return groups[k]; });
                }
            });
        }

        function printGroupPrescription(group) {
            if (!group) return;
            var dateStr = new Date(group.appointmentDate).toLocaleDateString('ar-EG');

            var prescRows = '';
            angular.forEach(group.prescriptions, function(p, i) {
                prescRows += '<tr>' +
                    '<td style="padding:10px;border:1px solid #ddd;text-align:center;">' + (i + 1) + '</td>' +
                    '<td style="padding:10px;border:1px solid #ddd;font-weight:600;color:#0077B6;">' + (p.medicationName || '') + '</td>' +
                    '<td style="padding:10px;border:1px solid #ddd;">' + (p.dosage || '-') + '</td>' +
                    '<td style="padding:10px;border:1px solid #ddd;">' + (p.frequency || '-') + '</td>' +
                    '<td style="padding:10px;border:1px solid #ddd;">' + (p.duration || '-') + '</td>' +
                    '<td style="padding:10px;border:1px solid #ddd;font-size:0.85rem;">' + (p.instructions || '-') + '</td>' +
                    '</tr>';
            });

            var html = '<!DOCTYPE html><html dir="rtl" lang="ar"><head><meta charset="UTF-8">' +
                '<title>وصفة طبية - صيدلية</title>' +
                '<link href="https://fonts.googleapis.com/css2?family=Cairo:wght@400;600;700&display=swap" rel="stylesheet">' +
                '<style>*{margin:0;padding:0;box-sizing:border-box}body{font-family:"Cairo",sans-serif;padding:30px;color:#333}' +
                '.header{text-align:center;border-bottom:3px solid #0077B6;padding-bottom:20px;margin-bottom:24px}' +
                '.header h1{color:#0077B6;font-size:1.6rem;margin-bottom:4px}' +
                '.header p{color:#666;font-size:0.85rem}' +
                '.info-grid{display:grid;grid-template-columns:1fr 1fr;gap:12px;margin-bottom:24px;padding:16px;background:#f8f9fa;border-radius:8px}' +
                '.info-grid div{font-size:0.9rem}.info-grid strong{color:#0077B6}' +
                'table{width:100%;border-collapse:collapse;margin-bottom:24px}' +
                'th{background:#0077B6;color:white;padding:12px;font-size:0.9rem}' +
                'tr:nth-child(even){background:#f8f9fa}' +
                '.footer{margin-top:40px;display:flex;justify-content:space-between;padding-top:20px;border-top:2px dashed #ccc}' +
                '.footer div{text-align:center}.signature-line{width:180px;border-top:1px solid #333;margin:8px auto 0;padding-top:6px;font-size:0.85rem}' +
                '@media print{body{padding:20px}}</style></head><body>' +
                '<div class="header"><h1>🏥 صيدلية العيادة الافتراضية</h1><p>أمر صرف وصفة طبية — رقم السجل: ' + group.recordID + '</p></div>' +
                '<div class="info-grid">' +
                '<div><strong>المريض:</strong> ' + (group.patientName || '') + '</div>' +
                '<div><strong>الطبيب:</strong> د. ' + (group.doctorName || '') + '</div>' +
                '<div><strong>تاريخ الاستشارة:</strong> ' + dateStr + '</div>' +
                '<div><strong>رقم الجوال:</strong> ' + (group.patientPhone || 'غير مسجل') + '</div>' +
                '</div>' +
                '<h3 style="color:#0077B6;margin-bottom:12px;font-size:1rem;">💊 الوصفات المعتمدة للصرف</h3>' +
                '<table><thead><tr><th>#</th><th>الدواء</th><th>الجرعة</th><th>التكرار</th><th>المدة</th><th>التعليمات</th></tr></thead>' +
                '<tbody>' + prescRows + '</tbody></table>' +
                '<div class="footer"><div><div class="signature-line">توقيع الصيدلاني</div></div><div><div class="signature-line">ختم الصيدلية</div></div></div>' +
                '</body></html>';

            var printWindow = window.open('', '_blank');
            printWindow.document.write(html);
            printWindow.document.close();
            printWindow.focus();
            setTimeout(function() { printWindow.print(); }, 600);
        }

        function openDispense(prescription) {
            vm.selectedPrescription = prescription;
            vm.dispenseForm = {
                prescriptionID: prescription.prescriptionID,
                medicationID: null,
                quantity: prescription.quantity || 1,
                notes: ''
            };
            vm.showDispenseModal = true;
        }

        function confirmDispense() {
            if (vm.dispenseForm.quantity <= 0) { toastr.warning('أدخل كمية صحيحة'); return; }
            vm.dispensing = true;

            PharmacyService.dispense(vm.dispenseForm).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showDispenseModal = false;
                    loadPending();
                    loadDashboard();
                } else { toastr.error(res.message); }
            })
            .catch(function(err) { toastr.error(err.data ? err.data.message : 'حدث خطأ'); })
            .finally(function() { vm.dispensing = false; });
        }

        // =================== History ===================
        function loadHistory() {
            PharmacyService.getDispenseHistory({ page: vm.historyPage, pageSize: 15 }).then(function(res) {
                vm.dispenseHistory = res.data || [];
                vm.historyTotalPages = res.totalPages || 1;
            });
        }

        function historyPageChange(dir) {
            vm.historyPage += dir;
            if (vm.historyPage < 1) vm.historyPage = 1;
            loadHistory();
        }
    }
})();
