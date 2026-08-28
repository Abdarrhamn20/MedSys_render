(function() {
    'use strict';

    angular.module('medicalApp').controller('TreasuryController', TreasuryController);

    TreasuryController.$inject = ['TreasuryService', 'AccountingService', 'AuthService', 'toastr'];

    function TreasuryController(TreasuryService, AccountingService, AuthService, toastr) {
        var vm = this;

        var user = AuthService.getUser() || {};
        vm.role = user.role;
        vm.isAdmin = vm.role === 'Admin' || vm.role === 'Accountant';
        vm.canCreateVoucher = vm.isAdmin || (vm.role === 'Cashier' && !!user.assignedTreasuryID);
        vm.isCashier = vm.role === 'Cashier';
        vm.myTreasuryID = user.assignedTreasuryID || null;
        vm.noTreasuryWarning = vm.isCashier && !vm.myTreasuryID;

        // Tabs: treasuries | vouchers | journal | receivables | closure
        vm.activeTab = 'vouchers';
        vm.setTab = setTab;

        // ==== Treasuries ====
        vm.treasuries = [];
        vm.loadTreasuries = loadTreasuries;
        vm.treasuryForm = null;
        vm.editingTreasury = null;
        vm.showTreasuryModal = false;
        vm.savingTreasury = false;
        vm.openTreasuryModal = openTreasuryModal;
        vm.closeTreasuryModal = closeTreasuryModal;
        vm.saveTreasury = saveTreasury;
        vm.deleteTreasury = deleteTreasury;

        // ==== Vouchers ====
        vm.vouchers = [];
        vm.filterType = '';
        vm.filterStatus = '';
        vm.filterFrom = '';
        vm.filterTo = '';
        vm.page = 1;
        vm.pageSize = 10;
        vm.totalPages = 1;
        vm.vouchersLoading = false;
        vm.loadVouchers = loadVouchers;
        vm.applyVoucherFilters = applyVoucherFilters;
        vm.pageChange = pageChange;

        // New voucher modal
        vm.showVoucherModal = false;
        vm.voucherForm = null;
        vm.savingVoucher = false;
        vm.openVoucherModal = openVoucherModal;
        vm.closeVoucherModal = closeVoucherModal;
        vm.onVoucherTypeChange = onVoucherTypeChange;
        vm.saveVoucher = saveVoucher;

        // Voucher detail modal
        vm.showVoucherDetail = false;
        vm.currentVoucher = null;
        vm.openVoucherDetail = openVoucherDetail;
        vm.closeVoucherDetail = closeVoucherDetail;
        vm.postVoucher = postVoucher;
        vm.reverseVoucher = reverseVoucher;

        // ==== Daily Journal ====
        vm.journalTreasuryID = '';
        vm.journalFrom = '';
        vm.journalTo = '';
        vm.dailyJournal = null;
        vm.journalLoading = false;
        vm.loadDailyJournal = loadDailyJournal;

        // ==== Receivables ====
        vm.receivablesAsOf = '';
        vm.receivables = [];
        vm.receivablesLoading = false;
        vm.loadReceivables = loadReceivables;

        // ==== Fiscal Closure ====
        vm.closureLoading = false;
        vm.closedThrough = '';
        vm.loadClosure = loadClosure;
        vm.setClosure = setClosure;
        vm.openClosure = openClosure;

        // ==== Chart accounts (for voucher counter-account) ====
        vm.flatAccounts = [];

        activate();

        function activate() {
            loadTreasuries();
            loadVouchers();
            loadClosure();
            AccountingService.getFlatAccounts()
                .then(function(res) {
                    if (res.success) vm.flatAccounts = res.data || [];
                })
                .catch(function() {});
        }

        function setTab(tab) {
            vm.activeTab = tab;
            if (tab === 'treasuries') loadTreasuries();
            if (tab === 'vouchers') loadVouchers();
            if (tab === 'journal') loadDailyJournal();
            if (tab === 'receivables') loadReceivables();
            if (tab === 'closure') loadClosure();
        }

        // ============================================================
        //  Treasuries
        // ============================================================
        function loadTreasuries() {
            TreasuryService.getTreasuries()
                .then(function(res) {
                    if (res.success) vm.treasuries = res.data || [];
                })
                .catch(function() {
                    toastr.error('فشل تحميل الخزائن');
                });
        }

        function openTreasuryModal(treasury) {
            if (!vm.isAdmin) return;
            vm.editingTreasury = treasury || null;
            vm.treasuryForm = vm.editingTreasury ? {
                treasuryName: vm.editingTreasury.treasuryName || '',
                treasuryNameAr: vm.editingTreasury.treasuryNameAr,
                treasuryCode: vm.editingTreasury.treasuryCode,
                accountID: vm.editingTreasury.accountID,
                isActive: vm.editingTreasury.isActive
            } : {
                treasuryName: '',
                treasuryNameAr: '',
                treasuryCode: '',
                accountID: vm.flatAccounts.length > 0 ? vm.flatAccounts[0].accountID : null,
                isActive: true
            };
            vm.showTreasuryModal = true;
        }

        function closeTreasuryModal() {
            vm.showTreasuryModal = false;
            vm.editingTreasury = null;
        }

        function saveTreasury() {
            var form = vm.treasuryForm;
            if (!form.treasuryNameAr || !form.treasuryCode || !form.accountID) {
                toastr.warning('اسم الخزينة بالعربية والكود والحساب المحاسبي مطلوبة');
                return;
            }

            var payload = {
                treasuryName: form.treasuryName || form.treasuryNameAr,
                treasuryNameAr: form.treasuryNameAr,
                treasuryCode: form.treasuryCode,
                accountID: Number(form.accountID),
                isActive: !!form.isActive
            };

            vm.savingTreasury = true;
            var promise = vm.editingTreasury
                ? TreasuryService.updateTreasury(vm.editingTreasury.treasuryID, payload)
                : TreasuryService.createTreasury(payload);

            promise
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم حفظ الخزينة بنجاح');
                        vm.closeTreasuryModal();
                        loadTreasuries();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء حفظ الخزينة');
                })
                .finally(function() {
                    vm.savingTreasury = false;
                });
        }

        function deleteTreasury(treasury) {
            if (!vm.isAdmin) return;
            if (!confirm('هل تريد حذف الخزينة "' + treasury.treasuryNameAr + '"؟')) return;

            TreasuryService.deleteTreasury(treasury.treasuryID)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم حذف الخزينة');
                        loadTreasuries();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء حذف الخزينة');
                });
        }

        // ============================================================
        //  Vouchers
        // ============================================================
        function loadVouchers() {
            if (vm.noTreasuryWarning) {
                vm.vouchers = [];
                vm.totalPages = 1;
                return;
            }
            vm.vouchersLoading = true;
            var params = {
                type: vm.filterType || null,
                status: vm.filterStatus || null,
                from: vm.filterFrom || null,
                to: vm.filterTo || null,
                page: vm.page,
                pageSize: vm.pageSize
            };

            TreasuryService.getVouchers(params)
                .then(function(res) {
                    vm.vouchers = res.data || [];
                    vm.totalPages = Math.ceil(res.totalCount / vm.pageSize) || 1;
                })
                .catch(function() {
                    toastr.error('فشل تحميل السندات');
                })
                .finally(function() {
                    vm.vouchersLoading = false;
                });
        }

        function applyVoucherFilters() {
            vm.page = 1;
            loadVouchers();
        }

        function pageChange(dir) {
            vm.page += dir;
            if (vm.page < 1) vm.page = 1;
            if (vm.page > vm.totalPages) vm.page = vm.totalPages;
            loadVouchers();
        }

        function openVoucherModal() {
            if (!vm.canCreateVoucher) return;
            vm.voucherForm = {
                voucherDate: new Date(),
                voucherType: 'Receipt',
                treasuryID: vm.isCashier && vm.myTreasuryID
                    ? vm.myTreasuryID
                    : (vm.treasuries.length > 0 ? vm.treasuries[0].treasuryID : null),
                toTreasuryID: null,
                accountID: null,
                patientUserID: null,
                invoiceID: null,
                appointmentID: null,
                amount: null,
                description: ''
            };
            onVoucherTypeChange();
            vm.showVoucherModal = true;
        }

        function closeVoucherModal() {
            vm.showVoucherModal = false;
        }

        function onVoucherTypeChange() {
            // لا حاجة لإجراءات معقدة هنا — الحقول تُعرض حسب النوع في الـ HTML
        }

        function saveVoucher() {
            var form = vm.voucherForm;
            if (!form.treasuryID) {
                toastr.warning('اختر الخزينة');
                return;
            }
            if (!form.amount || Number(form.amount) <= 0) {
                toastr.warning('أدخل مبلغاً أكبر من صفر');
                return;
            }
            if (form.voucherType === 'Transfer' && !form.toTreasuryID) {
                toastr.warning('حدد خزينة التحويل إليها');
                return;
            }
            if (form.voucherType !== 'Transfer' && !form.accountID) {
                toastr.warning('حدد الحساب المقابل للسند');
                return;
            }

            var payload = {
                voucherDate: new Date(form.voucherDate).toISOString(),
                voucherType: form.voucherType,
                treasuryID: Number(form.treasuryID),
                toTreasuryID: form.voucherType === 'Transfer' ? Number(form.toTreasuryID) : null,
                accountID: form.voucherType === 'Transfer' ? null : Number(form.accountID),
                patientUserID: form.patientUserID ? Number(form.patientUserID) : null,
                invoiceID: form.invoiceID ? Number(form.invoiceID) : null,
                appointmentID: form.appointmentID ? Number(form.appointmentID) : null,
                amount: Number(form.amount),
                description: form.description || ''
            };

            vm.savingVoucher = true;
            TreasuryService.createVoucher(payload)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم إنشاء السند بنجاح');
                        vm.closeVoucherModal();
                        loadVouchers();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء إنشاء السند');
                })
                .finally(function() {
                    vm.savingVoucher = false;
                });
        }

        function openVoucherDetail(voucher) {
            TreasuryService.getVoucher(voucher.voucherID)
                .then(function(res) {
                    if (res.success) {
                        vm.currentVoucher = res.data;
                        vm.showVoucherDetail = true;
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function() {
                    toastr.error('فشل تحميل تفاصيل السند');
                });
        }

        function closeVoucherDetail() {
            vm.showVoucherDetail = false;
            vm.currentVoucher = null;
        }

        function postVoucher(voucher) {
            var allowed = vm.isAdmin || (vm.isCashier && vm.myTreasuryID && voucher.treasuryID === vm.myTreasuryID);
            if (!allowed) return;
            if (!confirm('سيتم ترحيل السند ' + voucher.voucherNumber + ' وتوليد القيد المحاسبي تلقائياً. هل تريد المتابعة؟')) return;

            TreasuryService.postVoucher(voucher.voucherID)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم ترحيل السند');
                        vm.closeVoucherDetail();
                        loadVouchers();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء ترحيل السند');
                });
        }

        function reverseVoucher(voucher) {
            var allowed = vm.isAdmin || (vm.isCashier && vm.myTreasuryID && voucher.treasuryID === vm.myTreasuryID);
            if (!allowed) return;
            if (!confirm('سيتم عكس السند ' + voucher.voucherNumber + ' وتوليد قيد عكسي. هل تريد المتابعة؟')) return;

            TreasuryService.reverseVoucher(voucher.voucherID)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم عكس السند');
                        vm.closeVoucherDetail();
                        loadVouchers();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء عكس السند');
                });
        }

        // ============================================================
        //  Daily Journal (يومية الخزينة)
        // ============================================================
        function loadDailyJournal() {
            if (vm.noTreasuryWarning) {
                vm.dailyJournal = null;
                return;
            }
            vm.journalLoading = true;
            var params = {
                treasuryId: vm.journalTreasuryID || null,
                from: vm.journalFrom || null,
                to: vm.journalTo || null
            };

            TreasuryService.getDailyJournal(params)
                .then(function(res) {
                    if (res.success) {
                        vm.dailyJournal = res.data;
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function() {
                    toastr.error('فشل تحميل يومية الخزينة');
                })
                .finally(function() {
                    vm.journalLoading = false;
                });
        }

        // ============================================================
        //  Receivables (المذنيه اليومية)
        // ============================================================
        function loadReceivables() {
            vm.receivablesLoading = true;
            var params = {
                asOf: vm.receivablesAsOf || null
            };

            TreasuryService.getReceivables(params)
                .then(function(res) {
                    if (res.success) {
                        vm.receivables = res.data || [];
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function() {
                    toastr.error('فشل تحميل المذنيه اليومية');
                })
                .finally(function() {
                    vm.receivablesLoading = false;
                });
        }

        // ============================================================
        //  Fiscal Closure (الإقفال المالي)
        // ============================================================
        function loadClosure() {
            vm.closureLoading = true;
            TreasuryService.getClosure()
                .then(function(res) {
                    if (res.success && res.data && res.data.closedThrough) {
                        vm.closedThrough = res.data.closedThrough;
                    } else {
                        vm.closedThrough = '';
                    }
                })
                .catch(function() {
                    toastr.error('فشل تحميل حالة الإقفال المالي');
                })
                .finally(function() {
                    vm.closureLoading = false;
                });
        }

        function setClosure() {
            if (!vm.isAdmin) return;
            if (!vm.closedThrough) {
                toastr.warning('حدد تاريخ الإقفال');
                return;
            }
            if (!confirm('سيتم إقفال النظام مالياً حتى ' + vm.closedThrough + ' — لن يُقبل أي ترحيل في هذه الفترة. متابعة؟')) return;

            TreasuryService.setClosure(vm.closedThrough)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم الإقفال المالي');
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء الإقفال المالي');
                });
        }

        function openClosure() {
            if (!vm.isAdmin) return;
            if (!confirm('سيتم فتح النظام وإلغاء الإقفال المالي. هل تريد المتابعة؟')) return;

            TreasuryService.openClosure()
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم فتح الإقفال المالي');
                        vm.closedThrough = '';
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء فتح الإقفال');
                });
        }

        // ============================================================
        //  Helpers
        // ============================================================
        vm.voucherTypeAr = voucherTypeAr;
        vm.statusAr = statusAr;
        vm.statusBadgeClass = statusBadgeClass;

        function voucherTypeAr(type) {
            var map = {
                'Receipt': 'سند قبض',
                'Payment': 'سند صرف',
                'Transfer': 'سند تحويل'
            };
            return map[type] || type;
        }

        function statusAr(status) {
            var map = {
                'Draft': 'مسودة',
                'Posted': 'مرحّل',
                'Reversed': 'معكوس'
            };
            return map[status] || status;
        }

        function statusBadgeClass(status) {
            var map = {
                'Draft': 'acc-badge-draft',
                'Posted': 'acc-badge-posted',
                'Reversed': 'acc-badge-reversed'
            };
            return map[status] || 'acc-badge-draft';
        }
    }
})();
