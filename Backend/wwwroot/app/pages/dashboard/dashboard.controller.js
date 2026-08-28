(function() {
    'use strict';

    angular.module('medicalApp').controller('DashboardController', DashboardController);

    DashboardController.$inject = ['AuthService', 'UsersService', '$http', '$timeout'];

    function DashboardController(AuthService, UsersService, $http, $timeout) {
        var vm = this;

        vm.user = AuthService.getUser() || {};
        vm.stats = {};
        vm.recentAppointments = [];
        vm.recentInvoices = [];
        vm.loading = true;

        // Psychiatric detection
        var spec = (vm.user.specialty || '').toLowerCase();
        vm.isPsychiatric = spec === 'الطب النفسي' || spec === 'طب نفسي أطفال ومراهقين' || spec.indexOf('نفس') !== -1 || spec.indexOf('psych') !== -1;
        vm.pendingAssessments = 0;
        vm.templateCount = 0;

        var options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
        vm.today = new Date().toLocaleDateString('ar-EG', options);

        activate();

        function activate() {
            // Load stats
            UsersService.getDashboardStats()
                .then(function(response) {
                    if (response.success) vm.stats = response.data;
                })
                .finally(function() { vm.loading = false; });

            // Load recent appointments
            UsersService.getRecentAppointments()
                .then(function(response) {
                    if (response.success) vm.recentAppointments = response.data;
                });

            // Load recent invoices for Doctor
            if (vm.user.role === 'Doctor') {
                $http.get('/api/billing/invoices', { params: { page: 1, pageSize: 5 } })
                    .then(function(res) {
                        vm.recentInvoices = res.data.data || [];
                    });
            }

            // Load psychiatric stats
            if (vm.user.role === 'Doctor' && vm.isPsychiatric) {
                $http.get('/api/assessments/templates').then(function(res) {
                    if (res.data.success) {
                        vm.templateCount = (res.data.data || []).length;
                    }
                });
                $http.get('/api/assessments/stats/pending-count').then(function(res) {
                    if (res.data.success) {
                        vm.pendingAssessments = res.data.data || 0;
                    }
                }).catch(function() { vm.pendingAssessments = 0; });
            }

            // Load doctors for Patient Dashboard to show the Doctor Price Browser
            if (vm.user.role === 'Patient') {
                vm.doctorsLoading = true;
                UsersService.getDoctors({ page: 1, pageSize: 100 })
                    .then(function(response) {
                        vm.doctors = response.data || [];
                    })
                    .finally(function() {
                        vm.doctorsLoading = false;
                    });
            }

            // Load charts for Admin only
            if (vm.user.role === 'Admin') {
                $timeout(function() { loadCharts(); }, 500);
            }

            // Load Pharmacist dashboard data
            if (vm.user.role === 'Pharmacist') {
                $http.get('/api/pharmacy/dashboard').then(function(res) {
                    if (res.data.success) vm.pharm = res.data.data;
                });
                $http.get('/api/pharmacy/requests', { params: { isResolved: false, pageSize: 5 } }).then(function(res) {
                    if (res.data && res.data.data) vm.pendingRequests = res.data.data;
                });
            }

            // Load Lab Technician pending orders
            if (vm.user.role === 'LabTechnician') {
                $http.get('/api/lab/orders', { params: { status: 'Requested' } }).then(function(res) {
                    if (res.data.success) vm.labPending = (res.data.data || []).slice(0, 5);
                });
            }

            // Load Radiologist pending orders
            if (vm.user.role === 'Radiologist') {
                $http.get('/api/radiology/orders', { params: { status: 'Requested' } }).then(function(res) {
                    if (res.data.success) vm.radioPending = (res.data.data || []).slice(0, 5);
                });
            }

            // Load Warehouse Keeper low stock
            if (vm.user.role === 'WarehouseKeeper') {
                $http.get('/api/warehouse/low-stock').then(function(res) {
                    if (res.data.success) vm.whLowStock = (res.data.data || []).slice(0, 5);
                });
            }
        }

        function loadCharts() {
            // Weekly Chart
            $http.get('/api/dashboard/charts/weekly').then(function(res) {
                if (res.data.success && res.data.data) {
                    var data = res.data.data;
                    createBarChart('weeklyChart', {
                        labels: data.map(function(d) { return d.dayName; }),
                        datasets: [
                            {
                                label: 'إجمالي',
                                data: data.map(function(d) { return d.total; }),
                                backgroundColor: 'rgba(0, 119, 182, 0.7)',
                                borderColor: '#0077B6',
                                borderWidth: 2,
                                borderRadius: 6
                            },
                            {
                                label: 'مكتمل',
                                data: data.map(function(d) { return d.completed; }),
                                backgroundColor: 'rgba(45, 198, 83, 0.7)',
                                borderColor: '#2DC653',
                                borderWidth: 2,
                                borderRadius: 6
                            }
                        ]
                    });
                }
            });

            // Priority Distribution
            $http.get('/api/dashboard/charts/priorities').then(function(res) {
                if (res.data.success && res.data.data) {
                    var data = res.data.data;
                    createDoughnutChart('priorityChart', {
                        labels: data.map(function(d) { return d.levelNameAr; }),
                        datasets: [{
                            data: data.map(function(d) { return d.count; }),
                            backgroundColor: data.map(function(d) { return d.colorCode; }),
                            borderWidth: 3,
                            borderColor: '#fff'
                        }]
                    });
                }
            });

            // Top Specialties
            $http.get('/api/dashboard/charts/specialties').then(function(res) {
                if (res.data.success && res.data.data) {
                    var data = res.data.data;
                    var colors = ['#0077B6', '#00B4D8', '#2DC653', '#FF9F1C', '#E63946', '#8338EC'];
                    createBarChart('specialtiesChart', {
                        labels: data.map(function(d) { return d.specialty; }),
                        datasets: [{
                            label: 'عدد المواعيد',
                            data: data.map(function(d) { return d.count; }),
                            backgroundColor: data.map(function(_, i) { return colors[i % colors.length] + 'B3'; }),
                            borderColor: data.map(function(_, i) { return colors[i % colors.length]; }),
                            borderWidth: 2,
                            borderRadius: 6
                        }]
                    }, true);
                }
            });

            // Doctors Performance
            $http.get('/api/dashboard/charts/doctors-performance').then(function(res) {
                if (res.data.success && res.data.data) {
                    var data = res.data.data;
                    createBarChart('performanceChart', {
                        labels: data.map(function(d) { return d.doctorName; }),
                        datasets: [
                            {
                                label: 'مكتمل',
                                data: data.map(function(d) { return d.completed; }),
                                backgroundColor: 'rgba(45, 198, 83, 0.7)',
                                borderColor: '#2DC653',
                                borderWidth: 2,
                                borderRadius: 6
                            },
                            {
                                label: 'في الانتظار',
                                data: data.map(function(d) { return d.pending; }),
                                backgroundColor: 'rgba(255, 159, 28, 0.7)',
                                borderColor: '#FF9F1C',
                                borderWidth: 2,
                                borderRadius: 6
                            }
                        ]
                    }, false, true);
                }
            });
        }

        function createBarChart(canvasId, chartData, horizontal, stacked) {
            var ctx = document.getElementById(canvasId);
            if (!ctx) return;
            new Chart(ctx, {
                type: horizontal ? 'bar' : 'bar',
                data: chartData,
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    indexAxis: horizontal ? 'y' : 'x',
                    plugins: {
                        legend: { display: chartData.datasets.length > 1, position: 'top', rtl: true, labels: { font: { family: 'Cairo', size: 12 }, usePointStyle: true } }
                    },
                    scales: {
                        x: { stacked: !!stacked, grid: { display: false }, ticks: { font: { family: 'Cairo', size: 11 } } },
                        y: { stacked: !!stacked, beginAtZero: true, ticks: { font: { family: 'Cairo', size: 11 }, stepSize: 1 } }
                    }
                }
            });
        }

        function createDoughnutChart(canvasId, chartData) {
            var ctx = document.getElementById(canvasId);
            if (!ctx) return;
            new Chart(ctx, {
                type: 'doughnut',
                data: chartData,
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    cutout: '60%',
                    plugins: {
                        legend: { position: 'bottom', rtl: true, labels: { font: { family: 'Cairo', size: 12 }, usePointStyle: true, padding: 16 } }
                    }
                }
            });
        }
    }
})();
