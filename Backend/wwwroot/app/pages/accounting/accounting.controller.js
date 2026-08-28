(function() {
    'use strict';

    angular.module('medicalApp').controller('AccountingController', AccountingController);

    AccountingController.$inject = ['AccountingService', 'AuthService', 'toastr'];

    function AccountingController(AccountingService, AuthService, toastr) {
        var vm = this;

        var user = AuthService.getUser() || {};
        vm.role = user.role;
        vm.isAdmin = vm.role === 'Admin' || vm.role === 'Accountant';

        // Tabs: chart | entries | ledger | trial
        vm.activeTab = 'chart';
        vm.setTab = setTab;

        // ==== Summary Cards ====
        vm.summary = null;
        vm.loadSummary = loadSummary;

        // ==== Chart of Accounts ====
        vm.chart = [];
        vm.chartRows = [];
        vm.flatAccounts = [];
        vm.expanded = {};
        vm.loadChart = loadChart;
        vm.toggleExpand = toggleExpand;
        vm.accountTypeAr = accountTypeAr;
        vm.openAccountModal = openAccountModal;
        vm.closeAccountModal = closeAccountModal;
        vm.saveAccount = saveAccount;
        vm.openAccountModal = openAccountModal;
        vm.accountForm = null;
        vm.editingAccount = null;
        vm.showAccountModal = false;
        vm.savingAccount = false;

        // ==== Journal Entries ====
        vm.entries = [];
        vm.filterStatus = '';
        vm.filterFrom = '';
        vm.filterTo = '';
        vm.page = 1;
        vm.pageSize = 10;
        vm.totalPages = 1;
        vm.entriesLoading = false;
        vm.loadEntries = loadEntries;
        vm.applyEntryFilters = applyEntryFilters;
        vm.pageChange = pageChange;

        // New entry modal
        vm.showEntryModal = false;
        vm.entryForm = null;
        vm.entryTotalDebit = 0;
        vm.entryTotalCredit = 0;
        vm.savingEntry = false;
        vm.openEntryModal = openEntryModal;
        vm.closeEntryModal = closeEntryModal;
        vm.addEntryLine = addEntryLine;
        vm.removeEntryLine = removeEntryLine;
        vm.recalcEntry = recalcEntry;
        vm.saveEntry = saveEntry;

        // Entry detail modal
        vm.showEntryDetail = false;
        vm.currentEntry = null;
        vm.openEntryDetail = openEntryDetail;
        vm.closeEntryDetail = closeEntryDetail;
        vm.postEntry = postEntry;
        vm.reverseEntry = reverseEntry;

        // ==== Ledger ====
        vm.ledgerAccountID = '';
        vm.ledgerFrom = '';
        vm.ledgerTo = '';
        vm.ledger = null;
        vm.ledgerLoading = false;
        vm.loadLedger = loadLedger;

        // ==== Trial Balance ====
        vm.trialAsOf = '';
        vm.trial = null;
        vm.trialLoading = false;
        vm.loadTrialBalance = loadTrialBalance;

        activate();

        function activate() {
            loadChart();
            loadEntries();
            if (vm.isAdmin) loadSummary();
        }

        function setTab(tab) {
            vm.activeTab = tab;
            if (tab === 'entries') loadEntries();
            if (tab === 'chart') loadChart();
            if (tab === 'trial') loadTrialBalance();
        }

        // ============================================================
        //  Summary
        // ============================================================
        function loadSummary() {
            if (!vm.isAdmin) return;
            AccountingService.getSummary()
                .then(function(res) {
                    if (res.success) vm.summary = res.data;
                })
                .catch(function() {
                    toastr.error('فشل تحميل ملخص المحاسبة');
                });
        }

        // ============================================================
        //  Chart of Accounts
        // ============================================================
        function loadChart() {
            AccountingService.getChart()
                .then(function(res) {
                    if (res.success) {
                        vm.chart = res.data || [];
                        flattenChart();
                    }
                })
                .catch(function() {
                    toastr.error('فشل تحميل شجرة الحسابات');
                });

            AccountingService.getFlatAccounts()
                .then(function(res) {
                    if (res.success) {
                        vm.flatAccounts = res.data || [];
                        var ledgerSel = document.getElementById('ledgerAccountSel');
                        if (ledgerSel && !vm.ledgerAccountID && vm.flatAccounts.length > 0) {
                            vm.ledgerAccountID = vm.flatAccounts[0].accountID;
                        }
                    }
                })
                .catch(function() {});
        }

        function toggleExpand(code) {
            vm.expanded[code] = !vm.expanded[code];
            flattenChart();
        }

        function flattenChart() {
            vm.chartRows = [];
            walk(vm.chart, 0);
            function walk(nodes, depth) {
                (nodes || []).forEach(function(node) {
                    vm.chartRows.push({
                        accountID: node.accountID,
                        accountCode: node.accountCode,
                        accountNameAr: node.accountNameAr,
                        accountType: node.accountType,
                        openingBalance: node.openingBalance,
                        isActive: node.isActive,
                        depth: depth,
                        hasChildren: node.children && node.children.length > 0,
                        expanded: !!vm.expanded[node.accountCode]
                    });
                    if (vm.expanded[node.accountCode] && node.children) {
                        walk(node.children, depth + 1);
                    }
                });
            }
        }

        function accountTypeAr(type) {
            var map = {
                'Asset': 'أصل',
                'Liability': 'خصم',
                'Equity': 'حقوق ملكية',
                'Revenue': 'إيراد',
                'Expense': 'مصروف'
            };
            return map[type] || type;
        }

        function openAccountModal(account) {
            vm.editingAccount = account || null;
            if (vm.editingAccount && !vm.editingAccount.hasOwnProperty('parentAccountID')) {
                // الصف المعروض مبسّط — نستعيد بيانات الحساب الكاملة من القائمة المسطحة
                var full = null;
                for (var i = 0; i < vm.flatAccounts.length; i++) {
                    if (vm.flatAccounts[i].accountID === vm.editingAccount.accountID) {
                        full = vm.flatAccounts[i];
                        break;
                    }
                }
                if (full) vm.editingAccount = full;
            }
            vm.accountForm = vm.editingAccount ? {
                accountCode: account.accountCode,
                accountName: account.accountName || '',
                accountNameAr: account.accountNameAr,
                accountType: account.accountType,
                parentAccountID: account.parentAccountID || null,
                openingBalance: account.openingBalance || 0,
                isActive: account.isActive
            } : {
                accountCode: '',
                accountName: '',
                accountNameAr: '',
                accountType: 'Asset',
                parentAccountID: null,
                openingBalance: 0,
                isActive: true
            };
            vm.showAccountModal = true;
        }

        function closeAccountModal() {
            vm.showAccountModal = false;
            vm.editingAccount = null;
        }

        function saveAccount() {
            var form = vm.accountForm;
            if (!form.accountCode || !form.accountNameAr) {
                toastr.warning('رقم الحساب واسمه بالعربية مطلوبان');
                return;
            }

            var payload = {
                accountCode: form.accountCode,
                accountName: form.accountName || form.accountNameAr,
                accountNameAr: form.accountNameAr,
                accountType: form.accountType,
                parentAccountID: form.parentAccountID ? Number(form.parentAccountID) : null,
                openingBalance: Number(form.openingBalance) || 0,
                isActive: !!form.isActive
            };

            vm.savingAccount = true;
            var promise = vm.editingAccount
                ? AccountingService.updateAccount(vm.editingAccount.accountID, payload)
                : AccountingService.createAccount(payload);

            promise
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم حفظ الحساب بنجاح');
                        vm.closeAccountModal();
                        loadChart();
                        if (vm.isAdmin) loadSummary();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء حفظ الحساب');
                })
                .finally(function() {
                    vm.savingAccount = false;
                });
        }

        // ============================================================
        //  Journal Entries
        // ============================================================
        function loadEntries() {
            vm.entriesLoading = true;
            var params = {
                status: vm.filterStatus || null,
                from: vm.filterFrom || null,
                to: vm.filterTo || null,
                page: vm.page,
                pageSize: vm.pageSize
            };

            AccountingService.getJournalEntries(params)
                .then(function(res) {
                    vm.entries = res.data || [];
                    vm.totalPages = Math.ceil(res.totalCount / vm.pageSize) || 1;
                })
                .catch(function() {
                    toastr.error('فشل تحميل القيود المحاسبية');
                })
                .finally(function() {
                    vm.entriesLoading = false;
                });
        }

        function applyEntryFilters() {
            vm.page = 1;
            loadEntries();
        }

        function pageChange(dir) {
            vm.page += dir;
            if (vm.page < 1) vm.page = 1;
            if (vm.page > vm.totalPages) vm.page = vm.totalPages;
            loadEntries();
        }

        function openEntryModal() {
            if (!vm.isAdmin) return;
            vm.entryForm = {
                entryDate: new Date(),
                description: '',
                sourceModule: 'Manual',
                lines: [
                    { accountID: null, debit: null, credit: null, notes: '' },
                    { accountID: null, debit: null, credit: null, notes: '' }
                ]
            };
            recalcEntry();
            vm.showEntryModal = true;
        }

        function closeEntryModal() {
            vm.showEntryModal = false;
        }

        function addEntryLine() {
            vm.entryForm.lines.push({ accountID: null, debit: null, credit: null, notes: '' });
            recalcEntry();
        }

        function removeEntryLine(index) {
            if (vm.entryForm.lines.length <= 2) {
                toastr.warning('يجب أن يتكون القيد من سطرين على الأقل');
                return;
            }
            vm.entryForm.lines.splice(index, 1);
            recalcEntry();
        }

        function recalcEntry() {
            if (!vm.entryForm || !vm.entryForm.lines) return;
            var d = 0, c = 0;
            vm.entryForm.lines.forEach(function(line) {
                d += Number(line.debit) || 0;
                c += Number(line.credit) || 0;
            });
            vm.entryTotalDebit = d;
            vm.entryTotalCredit = c;
        }

        function saveEntry() {
            var form = vm.entryForm;
            if (!form.description) {
                toastr.warning('يرجى إدخال بيان القيد');
                return;
            }
            if (!form.entryDate) {
                toastr.warning('يرجى إدخال تاريخ القيد');
                return;
            }

            var lines = [];
            var hasValue = false;
            for (var i = 0; i < form.lines.length; i++) {
                var line = form.lines[i];
                if (!line.accountID) continue;
                var debit = Number(line.debit) || 0;
                var credit = Number(line.credit) || 0;
                if (debit > 0 || credit > 0) {
                    hasValue = true;
                    lines.push({ accountID: Number(line.accountID), debit: debit, credit: credit, notes: line.notes || null });
                }
            }

            if (!hasValue) {
                toastr.warning('أدخل قيمة مدين/دائن على الأقل في سطر واحد');
                return;
            }
            if (lines.length < 2) {
                toastr.warning('يجب تعبئة سطرين على الأقل (مدين ودائن)');
                return;
            }

            var totalDebit = 0, totalCredit = 0;
            lines.forEach(function(l) { totalDebit += l.debit; totalCredit += l.credit; });
            if (totalDebit !== totalCredit) {
                toastr.warning('القيد غير متوازن: مجموع المدين (' + totalDebit.toFixed(2) + ') لا يساوي مجموع الدائن (' + totalCredit.toFixed(2) + ')');
                return;
            }

            vm.savingEntry = true;
            AccountingService.createJournalEntry({
                entryDate: new Date(form.entryDate).toISOString(),
                description: form.description,
                sourceModule: 'Manual',
                lines: lines
            })
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم إنشاء القيد بنجاح');
                        vm.closeEntryModal();
                        loadEntries();
                        if (vm.isAdmin) loadSummary();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء إنشاء القيد');
                })
                .finally(function() {
                    vm.savingEntry = false;
                });
        }

        function openEntryDetail(entry) {
            AccountingService.getJournalEntry(entry.journalEntryID)
                .then(function(res) {
                    if (res.success) {
                        vm.currentEntry = res.data;
                        vm.showEntryDetail = true;
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function() {
                    toastr.error('فشل تحميل تفاصيل القيد');
                });
        }

        function closeEntryDetail() {
            vm.showEntryDetail = false;
            vm.currentEntry = null;
        }

        function postEntry(entry) {
            if (!vm.isAdmin) return;
            if (!confirm('هل أنت متأكد من ترحيل القيد ' + entry.entryNumber + '؟ بعد الترحيل لا يمكن تعديله')) return;

            AccountingService.postJournalEntry(entry.journalEntryID)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم ترحيل القيد');
                        vm.closeEntryDetail();
                        loadEntries();
                        if (vm.isAdmin) loadSummary();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء الترحيل');
                });
        }

        function reverseEntry(entry) {
            if (!vm.isAdmin) return;
            if (!confirm('سيتم إنشاء قيد عكسي للقيد ' + entry.entryNumber + ' (يُرحَّل لاحقاً يدوياً). هل تريد المتابعة؟')) return;

            AccountingService.reverseJournalEntry(entry.journalEntryID)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم إنشاء القيد العكسي');
                        vm.closeEntryDetail();
                        loadEntries();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء إنشاء القيد العكسي');
                });
        }

        // ============================================================
        //  Ledger (كشف حساب)
        // ============================================================
        function loadLedger() {
            if (!vm.ledgerAccountID) {
                toastr.warning('اختر الحساب لعرض كشف حسابه');
                return;
            }
            vm.ledgerLoading = true;
            var params = {
                from: vm.ledgerFrom || null,
                to: vm.ledgerTo || null
            };

            AccountingService.getLedger(vm.ledgerAccountID, params)
                .then(function(res) {
                    if (res.success) {
                        vm.ledger = res.data;
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function() {
                    toastr.error('فشل تحميل كشف الحساب');
                })
                .finally(function() {
                    vm.ledgerLoading = false;
                });
        }

        // ============================================================
        //  Trial Balance (ميزان المراجعة)
        // ============================================================
        function loadTrialBalance() {
            vm.trialLoading = true;
            var params = {
                asOf: vm.trialAsOf || null
            };

            AccountingService.getTrialBalance(params)
                .then(function(res) {
                    if (res.success) {
                        vm.trial = res.data;
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function() {
                    toastr.error('فشل تحميل ميزان المراجعة');
                })
                .finally(function() {
                    vm.trialLoading = false;
                });
        }
    }
})();
