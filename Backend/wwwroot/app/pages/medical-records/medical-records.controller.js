(function() {
    'use strict';

    angular.module('medicalApp').controller('MedicalRecordsController', MedicalRecordsController);

    MedicalRecordsController.$inject = ['MedicalService', 'AttachmentService', 'AuthService', 'AppointmentService', 'PharmacyService', 'PsychiatricService', 'toastr', '$rootScope'];

    function MedicalRecordsController(MedicalService, AttachmentService, AuthService, AppointmentService, PharmacyService, PsychiatricService, toastr, $rootScope) {
        var vm = this;

        var user = AuthService.getUser() || {};
        vm.role = user.role;
        vm.records = [];
        vm.detail = null;
        vm.showDetail = false;
        vm.showCreateModal = false;
        vm.showAddPrescription = false;
        vm.saving = false;
        vm.newRecord = getEmptyRecord();
        vm.newPrescription = {};
        vm.tempPrescription = {};
        
        vm.attachments = [];
        vm.newAttachment = {};
        vm.uploadingAttachment = false;

        vm.doctorAppointments = [];
        vm.loadingAppointments = false;

        // Medication Selection variables
        vm.medications = [];
        vm.medicationMode = 'dropdown';
        vm.searchQuery = '';
        vm.medicationNotFound = false;
        vm.requestNotes = '';
        vm.sendingRequest = false;

        // Psychiatric MSE
        vm.activeDetailTab = 'general';
        vm.mseData = {};
        vm.mseLoading = false;
        vm.mseSaving = false;
        vm.isPsychiatricDoctor = false;

        // === NEW: SOAP Note State ===
        vm.soapData = {};
        vm.soapLoading = false;
        vm.soapSaving = false;
        
        // === NEW: Risk Level State ===
        vm.patientRisk = { riskLevel: 'Stable', riskLevelAr: 'مستقر 🟢', riskLevelNotes: '' };
        vm.riskLoading = false;
        vm.riskSaving = false;
        vm.showRiskUpdateForm = false;
        vm.riskUpdateData = { riskLevel: 'Stable', notes: '' };

        // Assessments
        vm.assessmentTemplates = [];
        vm.patientAssessments = [];
        vm.showCreateTemplate = false;
        vm.showAssignAssessment = false;
        vm.showAssessmentResult = false;
        vm.newTemplate = { title: '', description: '', questions: [] };
        vm.newQuestion = { text: '', type: 'text' };
        vm.assignData = {};
        vm.assessmentResult = null;
        vm.templateSaving = false;
        vm.assignSaving = false;

        vm.viewRecord = viewRecord;
        vm.createRecord = createRecord;
        vm.savePrescription = savePrescription;
        vm.deletePrescription = deletePrescription;
        vm.addTempPrescription = addTempPrescription;
        vm.printPrescription = printPrescription;
        vm.printFullMedicalRecord = printFullMedicalRecord;
        vm.uploadAttachment = uploadAttachment;
        vm.deleteAttachment = deleteAttachment;
        vm.openCreateModal = openCreateModal;
        vm.loadDoctorAppointments = loadDoctorAppointments;

        vm.loadMedications = loadMedications;
        vm.searchMedication = searchMedication;
        vm.sendMedicationRequest = sendMedicationRequest;
        vm.selectMedication = selectMedication;
        vm.changeMedicationMode = changeMedicationMode;
        vm.sendPrescriptionsToPharmacy = sendPrescriptionsToPharmacy;
        vm.getDispenseStatusAr = getDispenseStatusAr;
        vm.hasDraftPrescriptions = hasDraftPrescriptions;

        // Psychiatric Functions
        vm.switchDetailTab = switchDetailTab;
        vm.loadMSE = loadMSE;
        vm.saveMSE = saveMSE;
        vm.loadTemplates = loadTemplates;
        vm.createTemplate = createTemplate;
        vm.deleteTemplate = deleteTemplate;
        vm.addQuestionToTemplate = addQuestionToTemplate;
        vm.removeQuestion = removeQuestion;
        vm.assignAssessment = assignAssessment;
        vm.loadPatientAssessments = loadPatientAssessments;
        vm.viewAssessmentResult = viewAssessmentResult;
        
        // === NEW: SOAP Notes & Risk Level ===
        vm.loadSoapNote = loadSoapNote;
        vm.saveSoapNote = saveSoapNote;
        vm.loadPatientRisk = loadPatientRisk;
        vm.updatePatientRisk = updatePatientRisk;
        vm.getRiskBadgeClass = getRiskBadgeClass;
        vm.getRiskBadgeText = getRiskBadgeText;
        
        // === NEW: MSE Quick-Pick Values (Interactive Elements) ===
        vm.mseQuickPicks = getMseQuickPicks();
        vm.appendMSEField = appendMSEField;
        vm.clearMSEField = clearMSEField;
        vm.toggleDictation = toggleDictation;
        vm.isDictating = false;
        vm.dictationField = null;
        
        // === NEW: Assessment Trend Graph ===
        vm.loadAssessmentTrend = loadAssessmentTrend;
        vm.assessmentTrend = [];

        activate();

        function activate() { 
            loadRecords(); 
            if (vm.role === 'Doctor') {
                loadMedications();
            }
        }

        function openCreateModal() {
            vm.showCreateModal = true;
            loadDoctorAppointments();
        }

        function loadDoctorAppointments() {
            vm.loadingAppointments = true;
            AppointmentService.getAppointments({ pageSize: 100 }).then(function(res) {
                var allApps = res.data || [];
                // فقط المرضى الذين اكتمل موعدهم مع الطبيب (تم الكشف/المكالمة) ولم يُنشأ لهم سجل طبي بعد
                vm.doctorAppointments = allApps.filter(function(app) {
                    return app.status === 'Completed' && !app.hasMedicalRecord;
                });
            }).finally(function() {
                vm.loadingAppointments = false;
            });
        }

        function loadRecords() {
            MedicalService.getRecords({ pageSize: 50 }).then(function(res) {
                vm.records = res.data || [];
            });
        }

        function viewRecord(record) {
            MedicalService.getRecord(record.recordID).then(function(res) {
                if (res.success) {
                    vm.detail = res.data;
                    vm.showDetail = true;
                    vm.showAddPrescription = false;
                    loadAttachments(vm.detail.recordID);
                    vm.activeDetailTab = 'general';
                    // Full separation between General medicine and Psychiatric:
                    // - General mode: no psychiatric tabs at all
                    // - Psychiatric mode: all records are psychiatric
                    // - Hybrid mode: depends on the record's doctor specialty
                    var spec = (vm.detail.doctorSpecialty || '').toLowerCase();
                    var mode = ($rootScope.facilityMode || 'General');
                    if (mode === 'General') {
                        vm.isPsychiatricDoctor = false;
                    } else if (mode === 'Psychiatric') {
                        vm.isPsychiatricDoctor = true;
                    } else {
                        vm.isPsychiatricDoctor = spec === 'الطب النفسي' || spec === 'طب نفسي أطفال ومراهقين' || spec.indexOf('نفس') !== -1 || spec.indexOf('psych') !== -1;
                    }
                }
            });
        }

        function loadAttachments(recordId) {
            AttachmentService.getRecordAttachments(recordId).then(function(res) {
                if (res.success) {
                    vm.attachments = res.data;
                }
            });
        }

        function uploadAttachment() {
            var fileInput = document.getElementById('attachmentFile');
            var file = fileInput.files[0];
            
            if (!file) {
                toastr.warning('الرجاء اختيار ملف أولاً');
                return;
            }

            vm.uploadingAttachment = true;
            AttachmentService.uploadAttachment(file, vm.detail.recordID, vm.detail.patientID, vm.newAttachment.description)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message);
                        fileInput.value = '';
                        vm.newAttachment = {};
                        loadAttachments(vm.detail.recordID);
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) { toastr.error(err.data ? err.data.message : 'حدث خطأ في رفع الملف'); })
                .finally(function() { vm.uploadingAttachment = false; });
        }

        function deleteAttachment(att) {
            if (!confirm('هل أنت متأكد من حذف هذا المرفق؟')) return;
            AttachmentService.deleteAttachment(att.attachmentID).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    loadAttachments(vm.detail.recordID);
                }
            });
        }

        function createRecord() {
            if (!vm.newRecord.appID) { toastr.warning('أدخل رقم الموعد'); return; }
            vm.saving = true;

            MedicalService.createRecord(vm.newRecord)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message);
                        vm.showCreateModal = false;
                        vm.newRecord = getEmptyRecord();
                        loadRecords();
                    } else { toastr.error(res.message); }
                })
                .catch(function(err) { toastr.error(err.data ? err.data.message : 'حدث خطأ'); })
                .finally(function() { vm.saving = false; });
        }

        function savePrescription() {
            if (!vm.newPrescription.medicationName) { toastr.warning('أدخل اسم الدواء'); return; }

            MedicalService.addPrescription(vm.detail.recordID, vm.newPrescription)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message);
                        vm.newPrescription = {};
                        vm.showAddPrescription = false;
                        viewRecord({ recordID: vm.detail.recordID });
                    }
                });
        }

        function deletePrescription(p) {
            if (!confirm('هل أنت متأكد من حذف هذه الوصفة؟')) return;
            MedicalService.deletePrescription(p.prescriptionID).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    viewRecord({ recordID: vm.detail.recordID });
                }
            });
        }

        function addTempPrescription() {
            if (!vm.tempPrescription.medicationName) return;
            vm.newRecord.prescriptions.push(angular.copy(vm.tempPrescription));
            vm.tempPrescription = {};
        }

        function getEmptyRecord() {
            return { appID: null, diagnosisAr: '', treatmentPlan: '', doctorNotes: '', followUpDate: null, followUpNotes: '', prescriptions: [], sendToPharmacy: true };
        }

        function sendPrescriptionsToPharmacy(recordId) {
            if (!recordId) return;
            vm.sendingRequest = true;
            MedicalService.sendPrescriptionsToPharmacy(recordId)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم إرسال الوصفات للصيدلية بنجاح');
                        viewRecord({ recordID: recordId });
                        loadRecords();
                    } else {
                        toastr.error(res.message || 'فشل إرسال الوصفات');
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ في الاتصال بالخادم');
                })
                .finally(function() {
                    vm.sendingRequest = false;
                });
        }

        function getDispenseStatusAr(status) {
            var map = {
                'Draft': 'مسودة (غير مرسلة للصيدلية)',
                'Pending': 'بانتظار الصرف بالصيدلية ⏳',
                'Dispensed': 'تم الصرف بنجاح ✅',
                'PartiallyDispensed': 'تم الصرف جزئياً'
            };
            return map[status] || status;
        }

        function hasDraftPrescriptions(record) {
            if (!record || !record.prescriptions) return false;
            return record.prescriptions.some(function(p) {
                return p.dispenseStatus === 'Draft';
            });
        }

        function printPrescription() {
            if (!vm.detail) return;
            var d = vm.detail;
            var dateStr = new Date(d.appointmentDate).toLocaleDateString('ar-EG');

            var prescRows = '';
            angular.forEach(d.prescriptions, function(p, i) {
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
                '<title>وصفة طبية</title>' +
                '<link href="https://fonts.googleapis.com/css2?family=Cairo:wght@400;600;700&display=swap" rel="stylesheet">' +
                '<style>*{margin:0;padding:0;box-sizing:border-box}body{font-family:"Cairo",sans-serif;padding:30px;color:#333}' +
                '.header{text-align:center;border-bottom:3px solid #0077B6;padding-bottom:20px;margin-bottom:24px}' +
                '.header h1{color:#0077B6;font-size:1.6rem;margin-bottom:4px}' +
                '.header p{color:#666;font-size:0.85rem}' +
                '.info-grid{display:grid;grid-template-columns:1fr 1fr;gap:12px;margin-bottom:24px;padding:16px;background:#f8f9fa;border-radius:8px}' +
                '.info-grid div{font-size:0.9rem}.info-grid strong{color:#0077B6}' +
                '.diagnosis{background:#e8f4f8;padding:16px;border-radius:8px;margin-bottom:24px;border-right:4px solid #0077B6}' +
                '.diagnosis h3{color:#0077B6;margin-bottom:8px;font-size:1rem}' +
                'table{width:100%;border-collapse:collapse;margin-bottom:24px}' +
                'th{background:#0077B6;color:white;padding:12px;font-size:0.9rem}' +
                'tr:nth-child(even){background:#f8f9fa}' +
                '.footer{margin-top:40px;display:flex;justify-content:space-between;padding-top:20px;border-top:2px dashed #ccc}' +
                '.footer div{text-align:center}.signature-line{width:180px;border-top:1px solid #333;margin:8px auto 0;padding-top:6px;font-size:0.85rem}' +
                '@media print{body{padding:20px}}</style></head><body>' +
                '<div class="header"><h1>🏥 منظومة العيادة الافتراضية</h1><p>وصفة طبية — رقم السجل: ' + d.recordID + '</p></div>' +
                '<div class="info-grid">' +
                '<div><strong>المريض:</strong> ' + (d.patientName || '') + '</div>' +
                '<div><strong>الطبيب:</strong> د. ' + (d.doctorName || '') + '</div>' +
                '<div><strong>التخصص:</strong> ' + (d.doctorSpecialty || '') + '</div>' +
                '<div><strong>التاريخ:</strong> ' + dateStr + '</div>' +
                '<div><strong>فصيلة الدم:</strong> ' + (d.patientBloodType || 'غير محدد') + '</div>' +
                '<div><strong>الحساسية:</strong> ' + (d.patientAllergies || 'لا يوجد') + '</div>' +
                '</div>' +
                '<div class="diagnosis"><h3>التشخيص</h3><p>' + (d.diagnosisAr || d.diagnosis || '') + '</p></div>' +
                (d.treatmentPlan ? '<div class="diagnosis" style="border-color:#2DC653;background:#e8f8ed;margin-bottom:24px"><h3 style="color:#2DC653">خطة العلاج</h3><p>' + d.treatmentPlan + '</p></div>' : '') +
                '<h3 style="color:#0077B6;margin-bottom:12px;font-size:1rem;">💊 الوصفات الطبية</h3>' +
                '<table><thead><tr><th>#</th><th>الدواء</th><th>الجرعة</th><th>التكرار</th><th>المدة</th><th>التعليمات</th></tr></thead>' +
                '<tbody>' + prescRows + '</tbody></table>' +
                (d.doctorNotes ? '<p style="background:#fff3cd;padding:12px;border-radius:8px;font-size:0.9rem;margin-bottom:24px"><strong>ملاحظات:</strong> ' + d.doctorNotes + '</p>' : '') +
                '<div class="footer"><div><div class="signature-line">توقيع الطبيب</div></div><div><div class="signature-line">ختم العيادة</div></div></div>' +
                '</body></html>';

            var printWindow = window.open('', '_blank');
            printWindow.document.write(html);
            printWindow.document.close();
            printWindow.focus();
            setTimeout(function() { printWindow.print(); }, 600);
        }

        function printFullMedicalRecord() {
            if (!vm.detail) return;
            var d = vm.detail;
            var dateStr = new Date(d.appointmentDate).toLocaleDateString('ar-EG');
            
            var prescRows = '';
            if (d.prescriptions && d.prescriptions.length > 0) {
                angular.forEach(d.prescriptions, function(p, i) {
                    prescRows += '<tr>' +
                        '<td style="padding:10px;border:1px solid #ddd;text-align:center;">' + (i + 1) + '</td>' +
                        '<td style="padding:10px;border:1px solid #ddd;font-weight:600;color:#1d3557;">' + (p.medicationName || '') + '</td>' +
                        '<td style="padding:10px;border:1px solid #ddd;">' + (p.dosage || '-') + '</td>' +
                        '<td style="padding:10px;border:1px solid #ddd;">' + (p.frequency || '-') + '</td>' +
                        '<td style="padding:10px;border:1px solid #ddd;">' + (p.duration || '-') + '</td>' +
                        '<td style="padding:10px;border:1px solid #ddd;font-size:0.85rem;">' + (p.instructions || '-') + '</td>' +
                        '</tr>';
                });
            } else {
                prescRows = '<tr><td colspan="6" style="padding:15px;text-align:center;color:#888;">لا توجد وصفات طبية مسجلة في هذا السجل.</td></tr>';
            }

            var html = '<!DOCTYPE html><html dir="rtl" lang="ar"><head><meta charset="UTF-8">' +
                '<title>التقرير الطبي الرسمي</title>' +
                '<link href="https://fonts.googleapis.com/css2?family=Cairo:wght@400;600;700;800&display=swap" rel="stylesheet">' +
                '<style>*{margin:0;padding:0;box-sizing:border-box}body{font-family:"Cairo",sans-serif;padding:40px;color:#333;background:#fff}' +
                '.header{display:flex;justify-content:space-between;align-items:center;border-bottom:3px solid #1d3557;padding-bottom:20px;margin-bottom:30px}' +
                '.header-logo{font-size:1.8rem;font-weight:800;color:#1d3557;display:flex;align-items:center;gap:10px}' +
                '.header-title{text-align:left;font-size:0.85rem;color:#666}' +
                '.doc-title{text-align:center;margin-bottom:25px;font-size:1.4rem;font-weight:800;color:#1d3557;text-transform:uppercase;letter-spacing:1px}' +
                '.section-title{font-size:1.05rem;font-weight:700;color:#1d3557;border-bottom:2px solid #efefef;padding-bottom:6px;margin-bottom:15px;display:flex;align-items:center;gap:8px}' +
                '.info-grid{display:grid;grid-template-columns:1fr 1fr;gap:15px;margin-bottom:30px;padding:20px;background:#f8f9fa;border-radius:10px;border:1px solid #eee}' +
                '.info-grid div{font-size:0.9rem;line-height:1.6}.info-grid strong{color:#1d3557}' +
                '.medical-content{margin-bottom:30px;display:grid;grid-template-columns:1fr;gap:20px}' +
                '.content-box{background:#f8f9fa;padding:16px;border-radius:8px;border-right:4px solid #1d3557;border-left:1px solid #eee;border-top:1px solid #eee;border-bottom:1px solid #eee}' +
                '.content-box h4{color:#1d3557;margin-bottom:8px;font-size:0.95rem;font-weight:700}' +
                '.content-box p{font-size:0.9rem;line-height:1.6;color:#444}' +
                'table{width:100%;border-collapse:collapse;margin-bottom:35px;border-radius:8px;overflow:hidden}' +
                'th{background:#1d3557;color:white;padding:12px;font-size:0.9rem;text-align:right}' +
                'td{padding:12px;border-bottom:1px solid #efefef;font-size:0.9rem}' +
                'tr:nth-child(even){background:#fcfcfc}' +
                '.followup-box{background:#e8f4f8;border-right:4px solid #0077b6;padding:15px;border-radius:8px;margin-bottom:30px;font-size:0.9rem}' +
                '.footer{margin-top:50px;display:flex;justify-content:space-between;padding-top:25px;border-top:2px dashed #ddd}' +
                '.footer div{text-align:center}.signature-line{width:180px;border-top:1px solid #333;margin:15px auto 0;padding-top:6px;font-size:0.85rem;color:#666}' +
                '@media print{body{padding:10px}.no-print{display:none}}</style></head><body>' +
                '<div class="header">' +
                '<div class="header-logo">🏥 العيادة الافتراضية الذكية</div>' +
                '<div class="header-title"><p>رقم التقرير: <strong>MR-' + d.recordID + '</strong></p><p>تاريخ الطباعة: ' + new Date().toLocaleDateString('ar-EG') + '</p></div>' +
                '</div>' +
                '<h2 class="doc-title">التقرير الطبي والتشخيص الشامل</h2>' +
                
                '<h3 class="section-title">👤 البيانات الشخصية والطبية للمريض</h3>' +
                '<div class="info-grid">' +
                '<div><strong>اسم المريض:</strong> ' + (d.patientName || '') + '</div>' +
                '<div><strong>الرقم التعريفي:</strong> Patient-' + (d.patientUserID || '') + '</div>' +
                '<div><strong>رقم الجوال:</strong> ' + (d.patientPhone || 'غير مسجل') + '</div>' +
                '<div><strong>فصيلة الدم:</strong> <span style="background:#e63946;color:white;padding:2px 8px;border-radius:4px;font-weight:bold;font-size:0.8rem;">' + (d.patientBloodType || 'غير معروفة') + '</span></div>' +
                '<div><strong>الحساسية والأدوية المستبعدة:</strong> ' + (d.patientAllergies || 'لا توجد حساسية معروفة') + '</div>' +
                '<div><strong>الأمراض المزمنة:</strong> ' + (d.patientChronicDiseases || 'لا توجد أمراض مزمنة') + '</div>' +
                '</div>' +

                '<h3 class="section-title">🩺 تفاصيل الاستشارة الطبية والزيارة</h3>' +
                '<div class="info-grid">' +
                '<div><strong>الطبيب المعالج:</strong> د. ' + (d.doctorName || '') + '</div>' +
                '<div><strong>التخصص الطبي:</strong> ' + (d.doctorSpecialty || '') + '</div>' +
                '<div><strong>تاريخ الاستشارة:</strong> ' + dateStr + '</div>' +
                '<div><strong>الرقم المهني للطبيب:</strong> ' + (d.doctorLicenseNumber || '—') + '</div>' +
                '</div>' +

                '<h3 class="section-title">📋 التشخيص الطبي وخطة العلاج</h3>' +
                '<div class="medical-content">' +
                '<div class="content-box"><h4>التشخيص والتقييم السريري (Diagnosis):</h4><p>' + (d.diagnosisAr || d.diagnosis || 'لم يحدد') + '</p></div>' +
                (d.treatmentPlan ? '<div class="content-box" style="border-right-color:#2DC653;"><h4>الخطة العلاجية الموصى بها (Treatment Plan):</h4><p>' + d.treatmentPlan + '</p></div>' : '') +
                (d.doctorNotes ? '<div class="content-box" style="border-right-color:#FF9F1C;"><h4>ملاحظات وتوصيات الطبيب (Doctor Notes):</h4><p>' + d.doctorNotes + '</p></div>' : '') +
                '</div>' +

                (d.followUpDate ? '<div class="followup-box"><strong>📅 موعد المتابعة القادم:</strong> ' + new Date(d.followUpDate).toLocaleDateString('ar-EG') + (d.followUpNotes ? ' — ملاحظة: ' + d.followUpNotes : '') + '</div>' : '') +

                '<h3 class="section-title">💊 الوصفة الدوائية المعتمدة (Prescription)</h3>' +
                '<table><thead><tr><th>#</th><th>اسم الدواء</th><th>الجرعة اليومية</th><th>التكرار والجرعة</th><th>المدة</th><th>تعليمات الاستخدام</th></tr></thead>' +
                '<tbody>' + prescRows + '</tbody></table>' +

                '<div style="margin-top: 30px; font-size: 0.8rem; color: #888; text-align: center; background: #fff3cd; padding: 10px; border-radius: 6px;">' +
                '<p>تم إصدار هذا التقرير الطبي إلكترونياً وبشكل معتمد بالكامل من سجلات العيادة الافتراضية الذكية.</p>' +
                '</div>' +

                '<div class="footer"><div><div class="signature-line">توقيع الطبيب المعالج</div></div><div><div class="signature-line">ختم المنشأة والعيادة</div></div></div>' +
                '</body></html>';

            var printWindow = window.open('', '_blank');
            printWindow.document.write(html);
            printWindow.document.close();
            printWindow.focus();
            setTimeout(function() { printWindow.print(); }, 600);
        }

        // ==========================================
        //  Psychiatric MSE & Assessment Functions
        // ==========================================

        function switchDetailTab(tab) {
            vm.activeDetailTab = tab;
            if (tab === 'mse' && vm.detail) {
                loadMSE(vm.detail.recordID);
            } else if (tab === 'soap' && vm.detail) {
                loadSoapNote(vm.detail.recordID);
            } else if (tab === 'risk' && vm.detail) {
                loadPatientRisk(vm.detail.patientID);
            } else if (tab === 'trend' && vm.detail) {
                loadPatientRisk(vm.detail.patientID);
                loadAssessmentTrend(vm.detail.patientUserID);
            } else if (tab === 'assessments' && vm.detail) {
                loadTemplates();
                loadPatientAssessments(vm.detail.patientUserID);
            }
        }

        function loadMSE(recordId) {
            vm.mseLoading = true;
            PsychiatricService.getPsychiatricRecord(recordId)
                .then(function(res) {
                    if (res.success) {
                        vm.mseData = res.data || {};
                    }
                })
                .catch(function() { toastr.error('حدث خطأ في تحميل بيانات MSE'); })
                .finally(function() { vm.mseLoading = false; });
        }

        function saveMSE() {
            if (!vm.detail) return;
            vm.mseSaving = true;
            var data = {
                appearance: vm.mseData.appearance || '',
                behavior: vm.mseData.behavior || '',
                speech: vm.mseData.speech || '',
                moodAndAffect: vm.mseData.moodAndAffect || '',
                thoughtProcess: vm.mseData.thoughtProcess || '',
                thoughtContent: vm.mseData.thoughtContent || '',
                perception: vm.mseData.perception || '',
                cognition: vm.mseData.cognition || '',
                insightAndJudgment: vm.mseData.insightAndJudgment || '',
                isSpeechToTextUsed: vm.mseData.isSpeechToTextUsed || false
            };
            PsychiatricService.savePsychiatricRecord(vm.detail.recordID, data)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم حفظ فحص الحالة العقلية بنجاح');
                        vm.mseData = res.data || vm.mseData;
                    } else {
                        toastr.error(res.message || 'فشل حفظ البيانات');
                    }
                })
                .catch(function(err) { toastr.error(err.data ? err.data.message : 'حدث خطأ'); })
                .finally(function() { vm.mseSaving = false; });
        }

        function loadTemplates() {
            PsychiatricService.getTemplates()
                .then(function(res) {
                    if (res.success) {
                        vm.assessmentTemplates = res.data || [];
                    }
                });
        }

        function createTemplate() {
            if (!vm.newTemplate.title) { toastr.warning('يرجى إدخال عنوان القالب'); return; }
            if (vm.newTemplate.questions.length === 0) { toastr.warning('يرجى إضافة سؤال واحد على الأقل'); return; }
            vm.templateSaving = true;
            var data = {
                title: vm.newTemplate.title,
                description: vm.newTemplate.description,
                schemaJson: JSON.stringify(vm.newTemplate.questions)
            };
            PsychiatricService.createTemplate(data)
                .then(function(res) {
                    if (res.success) {
                        toastr.success('تم إنشاء قالب الاستبيان بنجاح');
                        vm.showCreateTemplate = false;
                        vm.newTemplate = { title: '', description: '', questions: [] };
                        loadTemplates();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) { toastr.error(err.data ? err.data.message : 'حدث خطأ'); })
                .finally(function() { vm.templateSaving = false; });
        }

        function deleteTemplate(template) {
            if (!confirm('هل أنت متأكد من حذف هذا القالب؟')) return;
            PsychiatricService.deleteTemplate(template.templateID)
                .then(function(res) {
                    if (res.success) {
                        toastr.success('تم حذف القالب');
                        loadTemplates();
                    }
                });
        }

        function addQuestionToTemplate() {
            if (!vm.newQuestion.text) { toastr.warning('يرجى إدخال نص السؤال'); return; }
            var q = angular.copy(vm.newQuestion);
            if (q.type === 'choice' && q.optionsText) {
                q.options = q.optionsText.split(',').map(function(o) { return o.trim(); }).filter(function(o) { return o; });
                delete q.optionsText;
            }
            vm.newTemplate.questions.push(q);
            vm.newQuestion = { text: '', type: 'text' };
        }

        function removeQuestion(index) {
            vm.newTemplate.questions.splice(index, 1);
        }

        function assignAssessment() {
            if (!vm.assignData.templateID) { toastr.warning('يرجى اختيار الاستبيان'); return; }
            vm.assignSaving = true;
            var data = {
                templateID: vm.assignData.templateID,
                patientUserID: vm.detail.patientUserID
            };
            PsychiatricService.assignAssessment(data)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم إسناد الاستبيان للمريض بنجاح');
                        vm.showAssignAssessment = false;
                        vm.assignData = {};
                        loadPatientAssessments(vm.detail.patientUserID);
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) { toastr.error(err.data ? err.data.message : 'حدث خطأ'); })
                .finally(function() { vm.assignSaving = false; });
        }

        function loadPatientAssessments(patientUserId) {
            if (!patientUserId) return;
            PsychiatricService.getPatientAssessments(patientUserId)
                .then(function(res) {
                    if (res.success) {
                        vm.patientAssessments = res.data || [];
                    }
                });
        }

        function viewAssessmentResult(assessment) {
            PsychiatricService.getAssessmentResults(assessment.assessmentID)
                .then(function(res) {
                    if (res.success) {
                        vm.assessmentResult = res.data;
                        vm.assessmentResult.parsedAnswers = {};
                        vm.assessmentResult.parsedSchema = [];
                        try {
                            vm.assessmentResult.parsedAnswers = JSON.parse(res.data.answersJson || '{}');
                            vm.assessmentResult.parsedSchema = JSON.parse(res.data.templateSchema || '[]');
                        } catch(e) {}
                        vm.showAssessmentResult = true;
                    }
                });
        }

        function loadMedications() {
            PharmacyService.getMedications({ pageSize: 1000 }).then(function(res) {
                if (res && res.data) {
                    vm.medications = res.data.filter(function(m) {
                        return m.isActive;
                    });
                }
            });
        }

        function changeMedicationMode(mode) {
            vm.medicationMode = mode;
            vm.medicationNotFound = false;
            vm.searchQuery = '';
            vm.requestNotes = '';
            vm.tempPrescription.medicationName = '';
            vm.newPrescription.medicationName = '';
        }

        function selectMedication(med) {
            if (!med) return;
            var name = med.nameAr + ' (' + med.name + ')';
            vm.tempPrescription.medicationName = name;
            vm.newPrescription.medicationName = name;
        }

        function searchMedication(context) {
            var query = vm.searchQuery ? vm.searchQuery.trim().toLowerCase() : '';
            if (!query) {
                toastr.warning('يرجى كتابة اسم الدواء للبحث');
                return;
            }

            var found = false;
            var matchedMed = null;
            
            angular.forEach(vm.medications, function(med) {
                if ((med.name && med.name.toLowerCase().indexOf(query) !== -1) || 
                    (med.nameAr && med.nameAr.toLowerCase().indexOf(query) !== -1)) {
                    if (med.quantityInStock > 0) {
                        found = true;
                        matchedMed = med;
                    }
                }
            });

            if (found && matchedMed) {
                vm.medicationNotFound = false;
                var name = matchedMed.nameAr + ' (' + matchedMed.name + ')';
                if (context === 'temp') {
                    vm.tempPrescription.medicationName = name;
                } else {
                    vm.newPrescription.medicationName = name;
                }
                toastr.success('تم العثور على الدواء في المخزون: ' + matchedMed.nameAr);
            } else {
                vm.medicationNotFound = true;
                if (context === 'temp') {
                    vm.tempPrescription.medicationName = '';
                } else {
                    vm.newPrescription.medicationName = '';
                }
                toastr.error('الدواء غير متوفر في الصيدلية حالياً');
            }
        }

        function sendMedicationRequest(context) {
            var medName = vm.searchQuery ? vm.searchQuery.trim() : '';
            if (!medName) {
                toastr.warning('يرجى تحديد اسم الدواء المطلوب توفيره');
                return;
            }

            vm.sendingRequest = true;
            var data = {
                medicationName: medName,
                notes: vm.requestNotes || 'مطلوب توفيره للمريض بصورة عاجلة'
            };

            PharmacyService.createMedicationRequest(data)
                .then(function(res) {
                    if (res.success) {
                        toastr.success('تم إرسال طلب توفير الدواء للمدير والصيدلاني بنجاح');
                        vm.medicationNotFound = false;
                        vm.searchQuery = '';
                        vm.requestNotes = '';
                    } else {
                        toastr.error(res.message || 'فشل إرسال الطلب');
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data ? err.data.message : 'حدث خطأ أثناء إرسال الطلب');
                })
                .finally(function() {
                    vm.sendingRequest = false;
                });
        }

        // ==========================================
        //  SOAP NOTES FUNCTIONS
        // ==========================================

        function loadSoapNote(recordId) {
            vm.soapLoading = true;
            PsychiatricService.getSoapNote(recordId)
                .then(function(res) {
                    if (res.success) {
                        vm.soapData = res.data || {};
                    }
                })
                .catch(function() { toastr.error('حدث خطأ في تحميل SOAP Note'); })
                .finally(function() { vm.soapLoading = false; });
        }

        function saveSoapNote() {
            if (!vm.detail) return;
            vm.soapSaving = true;
            var data = {
                subjective: vm.soapData.subjective || '',
                objective: vm.soapData.objective || '',
                assessment: vm.soapData.assessment || '',
                plan: vm.soapData.plan || ''
            };
            PsychiatricService.saveSoapNote(vm.detail.recordID, data)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم حفظ SOAP Note بنجاح');
                        vm.soapData = res.data || vm.soapData;
                    } else {
                        toastr.error(res.message || 'فشل حفظ البيانات');
                    }
                })
                .catch(function(err) { toastr.error(err.data ? err.data.message : 'حدث خطأ'); })
                .finally(function() { vm.soapSaving = false; });
        }

        // ==========================================
        //  RISK LEVEL FUNCTIONS
        // ==========================================

        function loadPatientRisk(patientId) {
            if (!patientId) return;
            // note: the endpoint expects PatientID (PatientProfile.PatientID), not UserID
            vm.riskLoading = true;
            PsychiatricService.getPatientRisk(patientId)
                .then(function(res) {
                    if (res.success) {
                        vm.patientRisk = res.data || vm.patientRisk;
                    }
                })
                .catch(function() { })
                .finally(function() { vm.riskLoading = false; });
        }

        function updatePatientRisk() {
            if (!vm.detail) return;
            vm.riskSaving = true;
            var data = {
                riskLevel: vm.riskUpdateData.riskLevel,
                notes: vm.riskUpdateData.notes || ''
            };
            PsychiatricService.updatePatientRisk(vm.detail.patientID, data)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم تحديث مستوى الخطورة بنجاح');
                        vm.patientRisk = res.data || vm.patientRisk;
                        vm.showRiskUpdateForm = false;
                    } else {
                        toastr.error(res.message || 'فشل تحديث مستوى الخطورة');
                    }
                })
                .catch(function(err) { toastr.error(err.data ? err.data.message : 'حدث خطأ'); })
                .finally(function() { vm.riskSaving = false; });
        }

        function getRiskBadgeClass(level) {
            var map = {
                'Stable': 'badge-normal',
                'Monitoring': 'badge-urgent',
                'Critical': 'badge-danger'
            };
            return map[level] || 'badge-secondary';
        }

        function getRiskBadgeText(level) {
            var map = {
                'Stable': 'مستقر 🟢',
                'Monitoring': 'تحت الملاحظة 🟡',
                'Critical': 'حرج/خطر 🔴'
            };
            return map[level] || level || 'غير محدد';
        }

        // ==========================================
        //  MSE QUICK-PICK VALUES (Interactive Elements)
        // ==========================================

        function getMseQuickPicks() {
            return {
                appearance: [
                    { label: 'مرتب ونظيف', value: 'يبدو مرتباً ونظيفاً، مهتم بمظهره' },
                    { label: 'مهمل', value: 'مظهر مهمل، غير مهتم بنظافته الشخصية' },
                    { label: 'غريب/غير اعتيادي', value: 'مظهر غريب أو غير اعتيادي (ملابس غير مناسبة)' },
                    { label: 'مستغرب', value: 'يبدو مستغرباً أو مرتبكاً' }
                ],
                behavior: [
                    { label: 'هادئ ومتعاون', value: 'هادئ ومتعاون خلال الجلسة' },
                    { label: 'متوتر وقلق', value: 'متوتر وقلق، صعوبة في الجلوس' },
                    { label: 'عدائي', value: 'سلوك عدائي أو مهدد' },
                    { label: 'بطيء حركياً', value: 'بطء في الحركة والاستجابة (Psychomotor Retardation)' },
                    { label: 'منفعل', value: 'انفعال حركي زائد (Psychomotor Agitation)' }
                ],
                speech: [
                    { label: 'طبيعي', value: 'كلام طبيعي في السرعة والحجم والنبرة' },
                    { label: 'بطيء', value: 'كلام بطيء ومتردد' },
                    { label: 'سريع', value: 'كلام سريع ومندفع (Pressure of Speech)' },
                    { label: 'عالٍ', value: 'صوت مرتفع ونبرة حادة' },
                    { label: 'منخفض', value: 'صوت منخفض بالكاد يُسمع' }
                ],
                moodAndAffect: [
                    { label: 'مكتئب', value: 'مزاج مكتئب، حزين، باكٍ' },
                    { label: 'قلق', value: 'مزاج قلق وخائف' },
                    { label: 'مبتهج', value: 'مزاج مبتهج أو مرح بشكل غير مناسب' },
                    { label: 'غاضب', value: 'مزاج غاضب وسريع الانفعال' },
                    { label: 'متسطح وجدانياً', value: 'وجدان متسطح أو محدود (Flat/Blunted Affect)' },
                    { label: 'متقلب', value: 'وجدان متقلب وغير مستقر (Labile Affect)' }
                ],
                thoughtProcess: [
                    { label: 'منطقي', value: 'مجرى تفكير منطقي ومنظم' },
                    { label: 'متطاير', value: 'تطاير أفكار (Flight of Ideas)' },
                    { label: 'ظرفي', value: 'تفكير ظرفي ومطنب (Circumstantial)' },
                    { label: 'فضفاض', value: 'ترابط فضفاض (Loose Associations)' },
                    { label: 'متوقف', value: 'توقف الفكر (Thought Blocking)' }
                ],
                thoughtContent: [
                    { label: 'لا أوهام', value: 'لا توجد أوهام أو أفكار غير عادية' },
                    { label: 'أوهام اضطهاد', value: 'أوهام اضطهادية (Paranoid Delusions)' },
                    { label: 'أوهام عظمة', value: 'أوهام عظمة (Grandiose Delusions)' },
                    { label: 'وساوس', value: 'أفكار وسواسية متكررة' },
                    { label: 'أفكار انتحارية', value: 'أفكار انتحارية أو إيذاء النفس' }
                ],
                perception: [
                    { label: 'لا هلاوس', value: 'لا توجد هلاوس حسية' },
                    { label: 'هلاوس سمعية', value: 'هلاوس سمعية (سماع أصوات)' },
                    { label: 'هلاوس بصرية', value: 'هلاوس بصرية (رؤية أشياء غير موجودة)' },
                    { label: 'هلاوس لمسية', value: 'هلاوس لمسية' },
                    { label: 'تبدد الواقع', value: 'تبدد الواقع أو تبدد الشخصية' }
                ],
                cognition: [
                    { label: 'متوجه', value: 'متوجه نحو الزمان والمكان والشخص' },
                    { label: 'غير متوجه', value: 'غير متوجه للزمان أو المكان أو الشخص' },
                    { label: 'ضعف تركيز', value: 'ضعف في التركيز والانتباه' },
                    { label: 'ضعف ذاكرة', value: 'ضعف في الذاكرة القصيرة والطويلة' }
                ],
                insightAndJudgment: [
                    { label: 'بصيرة كاملة', value: 'بصيرة كاملة — يدرك مرضه ويحتاج للعلاج' },
                    { label: 'بصيرة جزئية', value: 'بصيرة جزئية — يقر ببعض الأعراض لكن لا يدرك خطورتها' },
                    { label: 'فقدان بصيرة', value: 'فقدان البصيرة — لا يدرك مرضه ولا حاجته للعلاج' },
                    { label: 'حكم ضعيف', value: 'حكم ضعيف على المخاطر والقرارات' }
                ]
            };
        }

        function appendMSEField(field, value) {
            var current = vm.mseData[field] || '';
            vm.mseData[field] = current ? current + '\n' + value : value;
        }

        function clearMSEField(field) {
            vm.mseData[field] = '';
        }

        // ==========================================
        //  DICTATION MODE (Web Speech API)
        // ==========================================

        function toggleDictation(field) {
            if (!('webkitSpeechRecognition' in window) && !('SpeechRecognition' in window)) {
                toastr.error('ميزة التعرف على الصوت غير مدعومة في هذا المتصفح. استخدم Chrome.');
                return;
            }

            if (vm.isDictating) {
                // Stop
                if (vm.dictationRecognition) {
                    vm.dictationRecognition.stop();
                }
                vm.isDictating = false;
                vm.dictationField = null;
                toastr.info('تم إيقاف التعرف على الصوت');
                return;
            }

            var SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
            var recognition = new SpeechRecognition();
            recognition.lang = 'ar-SA';
            recognition.continuous = true;
            recognition.interimResults = true;
            recognition.maxAlternatives = 5;

            recognition.onresult = function(event) {
                var transcript = '';
                for (var i = event.resultIndex; i < event.results.length; i++) {
                    var result = event.results[i];
                    if (result.isFinal) {
                        transcript += result[0].transcript + ' ';
                    }
                }
                if (transcript) {
                    var current = vm.mseData[field] || vm.soapData[field] || '';
                    if (field.indexOf('.') !== -1) {
                        var parts = field.split('.');
                        if (parts[0] === 'soap') {
                            vm.soapData[parts[1]] = (vm.soapData[parts[1]] || '') + transcript;
                        }
                    } else {
                        vm.mseData[field] = current ? current + transcript : transcript;
                    }
                    // Use $digest to update the view (scope is not directly available, use manual approach)
                    var rootEl = document.querySelector('[ng-app]');
                    if (rootEl) {
                        var scope = angular.element(rootEl).scope();
                        if (scope) { try { scope.$digest(); } catch(e) {} }
                    }
                }
            };

            recognition.onerror = function(event) {
                toastr.error('خطأ في التعرف على الصوت: ' + event.error);
                vm.isDictating = false;
                vm.dictationField = null;
            };

            recognition.onend = function() {
                if (vm.isDictating) {
                    // Auto-restart if still dictating
                    setTimeout(function() { recognition.start(); }, 100);
                }
            };

            vm.dictationRecognition = recognition;
            vm.isDictating = true;
            vm.dictationField = field;
            recognition.start();
            toastr.info('جاري الاستماع... تحدث الآن');
        }

        // ==========================================
        //  ASSESSMENT TREND GRAPH (PHQ-9 / GAD-7)
        // ==========================================

        function loadAssessmentTrend(patientUserId) {
            if (!patientUserId) return;
            PsychiatricService.getPatientAssessments(patientUserId)
                .then(function(res) {
                    if (res.success) {
                        var all = res.data || [];
                        // Filter for completed PHQ-9 and GAD-7 only
                        vm.assessmentTrend = all.filter(function(a) {
                            return a.status === 'Completed' &&
                                (a.templateTitle && (a.templateTitle.indexOf('PHQ') !== -1 || a.templateTitle.indexOf('GAD') !== -1));
                        });
                        if (vm.assessmentTrend.length > 0) {
                            // Load full results for each to calculate scores
                            angular.forEach(vm.assessmentTrend, function(a, idx) {
                                PsychiatricService.getAssessmentResults(a.assessmentID)
                                    .then(function(resultRes) {
                                        if (resultRes.success && resultRes.data) {
                                            a.parsedAnswers = {};
                                            a.score = 0;
                                            try {
                                                a.parsedAnswers = JSON.parse(resultRes.data.answersJson || '{}');
                                                var schema = [];
                                                try { schema = JSON.parse(resultRes.data.templateSchema || '[]'); } catch(e) {}
                                                // Calculate score from answers
                                                angular.forEach(schema, function(q) {
                                                    if (q.type === 'scoring' && q.weights) {
                                                        var answer = a.parsedAnswers[q.text];
                                                        if (answer !== undefined && answer !== null) {
                                                            var idx = q.options.indexOf(answer);
                                                            if (idx !== -1 && idx < q.weights.length) {
                                                                a.score = (a.score || 0) + q.weights[idx];
                                                            }
                                                        }
                                                    }
                                                });
                                            } catch(e) {}
                                            a.score = a.score || 0;
                                        }
                                        if (idx === vm.assessmentTrend.length - 1 && vm.assessmentTrend.length > 1) {
                                            // All loaded, render chart after a cycle
                                            setTimeout(function() { renderTrendChart(); }, 300);
                                        }
                                    });
                            });
                        }
                    }
                });
        }

        function renderTrendChart() {
            var canvas = document.getElementById('assessmentTrendChart');
            if (!canvas || vm.assessmentTrend.length < 2) return;
            
            var sorted = vm.assessmentTrend.slice().sort(function(a, b) {
                return new Date(a.completedAt) - new Date(b.completedAt);
            });

            var labels = sorted.map(function(a) {
                return new Date(a.completedAt).toLocaleDateString('ar-EG', { month: 'short', day: 'numeric' });
            });

            var phqData = [];
            var gadData = [];

            sorted.forEach(function(a) {
                if (a.templateTitle.indexOf('PHQ') !== -1) {
                    phqData.push({ x: new Date(a.completedAt), y: a.score || 0 });
                } else if (a.templateTitle.indexOf('GAD') !== -1) {
                    gadData.push({ x: new Date(a.completedAt), y: a.score || 0 });
                }
            });

            if (phqData.length < 2 && gadData.length < 2) return;

            var datasets = [];
            if (phqData.length >= 2) {
                datasets.push({
                    label: 'PHQ-9 (الاكتئاب)',
                    data: phqData,
                    borderColor: '#E63946',
                    backgroundColor: 'rgba(230,57,70,0.1)',
                    borderWidth: 2,
                    pointRadius: 4,
                    pointBackgroundColor: '#E63946',
                    tension: 0.3,
                    fill: true
                });
                // Reference line for moderate depression threshold (10)
                var ctx = canvas.getContext('2d');
            }
            if (gadData.length >= 2) {
                datasets.push({
                    label: 'GAD-7 (القلق)',
                    data: gadData,
                    borderColor: '#6C5CE7',
                    backgroundColor: 'rgba(108,92,231,0.1)',
                    borderWidth: 2,
                    pointRadius: 4,
                    pointBackgroundColor: '#6C5CE7',
                    tension: 0.3,
                    fill: true
                });
            }

            new Chart(canvas, {
                type: 'line',
                data: { datasets: datasets },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: { position: 'top', rtl: true, labels: { font: { family: 'Cairo', size: 11 }, usePointStyle: true } },
                        tooltip: {
                            rtl: true,
                            callbacks: {
                                title: function(items) {
                                    return new Date(items[0].raw.x).toLocaleDateString('ar-EG', { year: 'numeric', month: 'long', day: 'numeric' });
                                }
                            }
                        }
                    },
                    scales: {
                        x: {
                            type: 'time',
                            time: { unit: 'day', displayFormats: { day: 'd MMM' } },
                            ticks: { font: { family: 'Cairo', size: 10 } },
                            grid: { display: false }
                        },
                        y: {
                            beginAtZero: true,
                            max: 30,
                            ticks: { font: { family: 'Cairo', size: 10 }, stepSize: 5 },
                            grid: { color: 'rgba(0,0,0,0.05)' },
                            title: { display: true, text: 'الدرجة', font: { family: 'Cairo', size: 11 } }
                        }
                    }
                }
            });
        }
    }
})();
