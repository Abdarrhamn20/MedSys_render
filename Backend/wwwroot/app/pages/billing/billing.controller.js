(function() {
    'use strict';

    angular.module('medicalApp').controller('BillingController', BillingController);

    BillingController.$inject = ['BillingService', 'AuthService', 'toastr', '$timeout'];

    function BillingController(BillingService, AuthService, toastr, $timeout) {
        var vm = this;

        // User Data
        var user = AuthService.getUser() || {};
        vm.role = user.role;
        vm.userId = user.userID;
        vm.userName = user.fullName;

        // Active Tabs & Filters
        vm.activeTab = (vm.role === 'Admin') ? 'stats' : 'invoices';
        vm.filterStatus = '';
        vm.filterType = '';
        vm.page = 1;
        vm.pageSize = 10;
        vm.totalPages = 1;

        // Lists & Data
        vm.invoices = [];
        vm.stats = {};
        vm.loading = false;
        vm.selectedInvoice = null;

        // Card Payment Modal State
        vm.showPayModal = false;
        vm.processingPayment = false;
        vm.cardFlipped = false;
        vm.cardBrand = 'unknown'; // visa, mastercard, unknown
        vm.cardForm = {
            cardNumber: '',
            cardName: '',
            expiryDate: '',
            cvc: ''
        };

        // Receipt Modal / Printing
        vm.showReceiptModal = false;
        vm.currentReceipt = null;

        // Chart Reference
        var statsChart = null;

        // Functions
        vm.setTab = setTab;
        vm.loadInvoices = loadInvoices;
        vm.loadStats = loadStats;
        vm.applyFilters = applyFilters;
        vm.pageChange = pageChange;
        
        // Actions
        vm.openPayModal = openPayModal;
        vm.closePayModal = closePayModal;
        vm.onCardNumberChange = onCardNumberChange;
        vm.flipCard = flipCard;
        vm.submitCardPayment = submitCardPayment;
        vm.payCash = payCash;
        
        // Patient payment selection methods
        vm.showPaymentMethodModal = false;
        vm.openPaymentMethodSelection = openPaymentMethodSelection;
        vm.closePaymentMethodSelection = closePaymentMethodSelection;
        vm.choosePaymentMethod = choosePaymentMethod;
        
        // Receipt
        vm.openReceipt = openReceipt;
        vm.closeReceipt = closeReceipt;
        vm.printReceipt = printReceipt;
        vm.generateQRText = generateQRText;

        activate();

        function activate() {
            if (vm.role === 'Admin') {
                loadStats();
            }
            loadInvoices();
        }

        function setTab(tab) {
            vm.activeTab = tab;
            if (tab === 'stats' && vm.role === 'Admin') {
                loadStats();
            } else if (tab === 'invoices') {
                loadInvoices();
            }
        }

        // =================== Invoices Loading ===================
        function loadInvoices() {
            vm.loading = true;
            var params = {
                status: vm.filterStatus || null,
                type: vm.filterType || null,
                page: vm.page,
                pageSize: vm.pageSize
            };

            BillingService.getInvoices(params)
                .then(function(res) {
                    vm.invoices = res.data || [];
                    vm.totalPages = Math.ceil(res.totalCount / vm.pageSize) || 1;
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'فشل تحميل الفواتير');
                })
                .finally(function() {
                    vm.loading = false;
                });
        }

        function applyFilters() {
            vm.page = 1;
            loadInvoices();
        }

        function pageChange(dir) {
            vm.page += dir;
            if (vm.page < 1) vm.page = 1;
            if (vm.page > vm.totalPages) vm.page = vm.totalPages;
            loadInvoices();
        }

        // =================== Stats Loading ===================
        function loadStats() {
            if (vm.role !== 'Admin') return;
            
            BillingService.getStats()
                .then(function(res) {
                    if (res.success) {
                        vm.stats = res.data;
                        $timeout(function() {
                            renderCharts();
                        }, 100);
                    }
                })
                .catch(function() {
                    toastr.error('فشل تحميل الإحصائيات المالية');
                });
        }

        // =================== Payment Processing ===================
        function openPaymentMethodSelection(invoice) {
            vm.selectedInvoice = invoice;
            vm.showPaymentMethodModal = true;
        }

        function closePaymentMethodSelection() {
            vm.showPaymentMethodModal = false;
            vm.selectedInvoice = null;
        }

        function choosePaymentMethod(method) {
            vm.showPaymentMethodModal = false;
            if (method === 'Card') {
                openPayModal(vm.selectedInvoice);
            } else if (method === 'Cash') {
                if (confirm('هل أنت متأكد من رغبتك في الدفع نقداً وتأكيد سداد الفاتورة رقم #' + vm.selectedInvoice.invoiceID + '؟')) {
                    BillingService.payWithCash(vm.selectedInvoice.invoiceID)
                        .then(function(res) {
                            if (res.success) {
                                toastr.success(res.message || 'تم تسجيل الدفع النقدي بنجاح وتأكيد الفاتورة!');
                                loadInvoices();
                                if (vm.role === 'Admin') loadStats();
                            } else {
                                toastr.error(res.message);
                            }
                        })
                        .catch(function(err) {
                            toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء معالجة الدفع النقدي');
                        })
                        .finally(function() {
                            vm.selectedInvoice = null;
                        });
                } else {
                    vm.selectedInvoice = null;
                }
            }
        }
        function openPayModal(invoice) {
            vm.selectedInvoice = invoice;
            vm.cardForm = {
                cardNumber: '',
                cardName: vm.userName || '',
                expiryDate: '',
                cvc: ''
            };
            vm.cardBrand = 'unknown';
            vm.cardFlipped = false;
            vm.showPayModal = true;
        }

        function closePayModal() {
            vm.showPayModal = false;
            vm.selectedInvoice = null;
        }

        function onCardNumberChange() {
            var num = vm.cardForm.cardNumber.replace(/\s+/g, '');
            // Format input: add space every 4 digits
            var formatted = num.replace(/(\d{4})/g, '$1 ').trim();
            if (formatted.length > 19) formatted = formatted.substring(0, 19);
            vm.cardForm.cardNumber = formatted;

            // Detect brand
            if (num.startsWith('4')) {
                vm.cardBrand = 'visa';
            } else if (num.startsWith('5') || num.startsWith('2')) {
                vm.cardBrand = 'mastercard';
            } else {
                vm.cardBrand = 'unknown';
            }
        }

        function flipCard(flipped) {
            vm.cardFlipped = flipped;
        }

        function submitCardPayment() {
            if (!vm.cardForm.cardNumber || vm.cardForm.cardNumber.replace(/\s+/g, '').length < 16) {
                toastr.warning('يرجى إدخال رقم بطاقة صالح (16 رقماً)');
                return;
            }
            if (!vm.cardForm.expiryDate || !/^\d{2}\/\d{2}$/.test(vm.cardForm.expiryDate)) {
                toastr.warning('يرجى إدخال تاريخ انتهاء صالح بصيغة MM/YY');
                return;
            }
            if (!vm.cardForm.cvc || vm.cardForm.cvc.length < 3) {
                toastr.warning('يرجى إدخال رمز الأمان (CVC) المكون من 3 أرقام');
                return;
            }

            vm.processingPayment = true;
            
            // Format card number to send
            var payload = {
                cardNumber: vm.cardForm.cardNumber.replace(/\s+/g, ''),
                cardName: vm.cardForm.cardName,
                expiryDate: vm.cardForm.expiryDate,
                cvc: vm.cardForm.cvc
            };

            BillingService.payWithCard(vm.selectedInvoice.invoiceID, payload)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تمت عملية الدفع الإلكتروني بنجاح!');
                        vm.closePayModal();
                        loadInvoices();
                        if (vm.role === 'Admin') loadStats();
                    } else {
                        toastr.error(res.message || 'فشلت عملية الدفع');
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ في عملية الدفع');
                })
                .finally(function() {
                    vm.processingPayment = false;
                });
        }

        function payCash(invoice) {
            if (!confirm('هل أنت متأكد من استلام المبلغ نقداً وتأكيد سداد الفاتورة رقم #' + invoice.invoiceID + '؟')) {
                return;
            }

            BillingService.payWithCash(invoice.invoiceID)
                .then(function(res) {
                    if (res.success) {
                        toastr.success(res.message || 'تم تسجيل الدفع النقدي بنجاح');
                        loadInvoices();
                        if (vm.role === 'Admin') loadStats();
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function(err) {
                    toastr.error(err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء معالجة الدفع النقدي');
                });
        }

        // =================== Receipt Generation & Printing ===================
        function openReceipt(invoice) {
            vm.loading = true;
            BillingService.getInvoice(invoice.invoiceID)
                .then(function(res) {
                    if (res.success) {
                        vm.currentReceipt = res.data;
                        vm.showReceiptModal = true;
                    } else {
                        toastr.error(res.message);
                    }
                })
                .catch(function() {
                    toastr.error('فشل تحميل تفاصيل الفاتورة');
                })
                .finally(function() {
                    vm.loading = false;
                });
        }

        function closeReceipt() {
            vm.showReceiptModal = false;
            vm.currentReceipt = null;
        }

        function printReceipt() {
            $timeout(function() {
                window.print();
            }, 100);
        }

        function generateQRText(invoice) {
            if (!invoice) return '';
            // Basic secure-looking QR content with invoice summary
            return 'IVS-BILL-ID: ' + invoice.invoiceID + '\n' +
                   'Patient: ' + invoice.patientName + '\n' +
                   'Amount: ' + invoice.totalAmount + ' د.ل\n' +
                   'Method: ' + (invoice.paymentMethod === 'Card' ? 'مدفوع إلكترونياً' : 'نقداً') + '\n' +
                   'Date: ' + new Date(invoice.paidAt || invoice.createdAt).toLocaleString('ar-SA') + '\n' +
                   'Ref: ' + invoice.transactionReference;
        }

        // =================== Render Charts ===================
        function renderCharts() {
            if (!vm.stats || !vm.stats.totalRevenue && vm.stats.totalRevenue !== 0) return;

            // Prevent canvas collision
            var ctx = document.getElementById('billingStatsChart');
            if (!ctx) return;

            if (statsChart) {
                statsChart.destroy();
            }

            var labelCash = 'الدفع النقدي (كاش)';
            var labelCard = 'الدفع الإلكتروني (بطاقة)';
            
            statsChart = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: [labelCard, labelCash],
                    datasets: [{
                        data: [vm.stats.cardRevenue || 0, vm.stats.cashRevenue || 0],
                        backgroundColor: ['#4361ee', '#2ec4b6'],
                        borderWidth: 2,
                        hoverOffset: 6
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'bottom',
                            labels: {
                                font: {
                                    family: 'Cairo',
                                    size: 12,
                                    weight: 'bold'
                                }
                            }
                        }
                    }
                }
            });
        }
    }
})();
