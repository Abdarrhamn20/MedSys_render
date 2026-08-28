(function() {
    'use strict';

    angular.module('medicalApp').controller('EmployeesController', EmployeesController);

    EmployeesController.$inject = ['EmployeesService', 'AuthService', 'toastr', '$filter'];

    function EmployeesController(EmployeesService, AuthService, toastr, $filter) {
        var vm = this;

        vm.currentUser = AuthService.getUser() || {};
        vm.canManage = (vm.currentUser.role === 'Admin' || vm.currentUser.role === 'Accountant');
        vm.isAdmin = (vm.currentUser.role === 'Admin');
        vm.createAccount = false;

        // === Tabs ===
        vm.activeTab = 'employees';
        vm.setTab = setTab;

        // === Employees list ===
        vm.employees = [];
        vm.searchQuery = '';
        vm.departmentFilter = '';
        vm.departments = [];
        vm.page = 1;
        vm.pageSize = 12;
        vm.totalPages = 1;
        vm.totalCount = 0;
        vm.loadingEmployees = false;
        vm.loadEmployees = loadEmployees;
        vm.goPage = goPage;

        // === Employee form ===
        vm.showFormModal = false;
        vm.editMode = false;
        vm.form = {};
        vm.openAddModal = openAddModal;
        vm.openEditModal = openEditModal;
        vm.closeFormModal = closeFormModal;
        vm.submitForm = submitForm;
        vm.toggleActive = toggleActive;

        vm.compensationModels = [
            { value: 'FixedSalary', label: 'راتب ثابت' },
            { value: 'Commission', label: 'عمولات فقط (طبيب)' },
            { value: 'Mixed', label: 'مختلط (راتب + عمولات)' }
        ];

        vm.employeeRoles = ['Doctor', 'Pharmacist', 'LabTechnician', 'Radiologist', 'Receptionist', 'Cashier', 'WarehouseKeeper', 'Accountant'];

        // === Linkable login accounts (not yet bound to an employee card) ===
        vm.linkableUsers = [];
        vm.loadLinkableUsers = loadLinkableUsers;
        vm.toggleCreateAccount = toggleCreateAccount;
        vm.getRoleLabel = getRoleLabel;

        // === Employee detail ===
        vm.selectedEmployee = null;
        vm.showDetailModal = false;
        vm.openDetail = openDetail;
        vm.closeDetail = closeDetail;

        // === Courses ===
        vm.courses = [];
        vm.courseForm = {};
        vm.showCourseModal = false;
        vm.openCourseModal = openCourseModal;
        vm.closeCourseModal = closeCourseModal;
        vm.submitCourse = submitCourse;
        vm.deleteCourse = deleteCourse;

        // === Leaves ===
        vm.leaves = [];
        vm.leaveFilter = '';
        vm.leavesPage = 1;
        vm.leavesTotalPages = 1;
        vm.loadingLeaves = false;
        vm.loadLeaves = loadLeaves;
        vm.leavesGoPage = leavesGoPage;
        vm.leaveForm = {};
        vm.showLeaveModal = false;
        vm.openLeaveModal = openLeaveModal;
        vm.closeLeaveModal = closeLeaveModal;
        vm.submitLeave = submitLeave;
        vm.updateLeaveStatus = updateLeaveStatus;

        vm.leaveTypes = [
            { value: 'Annual', label: 'سنوية' },
            { value: 'Sick', label: 'مرضية' },
            { value: 'Unpaid', label: 'بدون راتب' },
            { value: 'Other', label: 'أخرى' }
        ];

        // === Payroll ===
        vm.payroll = [];
        vm.payrollPage = 1;
        vm.payrollTotalPages = 1;
        vm.payrollYear = new Date().getFullYear();
        vm.payrollMonth = new Date().getMonth() + 1;
        vm.payrollStatus = '';
        vm.payrollSummary = null;
        vm.loadingPayroll = false;
        vm.loadPayroll = loadPayroll;
        vm.loadPayrollSummary = loadPayrollSummary;
        vm.payrollGoPage = payrollGoPage;
        vm.runPayroll = runPayroll;
        vm.postPayroll = postPayroll;
        vm.reversePayroll = reversePayroll;
        vm.deletePayrollDraft = deletePayrollDraft;
        vm.showAdjustModal = false;
        vm.adjustForm = {};
        vm.openAdjustModal = openAdjustModal;
        vm.closeAdjustModal = closeAdjustModal;
        vm.submitAdjust = submitAdjust;

        vm.months = [
            { value: 1, label: 'يناير' }, { value: 2, label: 'فبراير' }, { value: 3, label: 'مارس' },
            { value: 4, label: 'أبريل' }, { value: 5, label: 'مايو' }, { value: 6, label: 'يونيو' },
            { value: 7, label: 'يوليو' }, { value: 8, label: 'أغسطس' }, { value: 9, label: 'سبتمبر' },
            { value: 10, label: 'أكتوبر' }, { value: 11, label: 'نوفمبر' }, { value: 12, label: 'ديسمبر' }
        ];

        // === Self-service (employee linked account) ===
        vm.me = null;
        vm.meLoading = false;
        vm.myLeaveForm = {};
        vm.showMyLeaveModal = false;
        vm.openMyLeaveModal = openMyLeaveModal;
        vm.closeMyLeaveModal = closeMyLeaveModal;
        vm.submitMyLeave = submitMyLeave;

        vm.getCompensationLabel = getCompensationLabel;
        vm.getLeaveTypeLabel = getLeaveTypeLabel;
        vm.getStatusLabel = getStatusLabel;

        activate();

        function activate() {
            if (vm.canManage) {
                loadEmployees();
                loadDepartments();
                loadLeaves();
                loadPayroll();
                loadPayrollSummary();
            } else {
                loadMyEmployee();
            }
        }

        // === Tabs ===
        function setTab(tab) {
            vm.activeTab = tab;
            if (tab === 'leaves') loadLeaves();
            if (tab === 'payroll') {
                loadPayroll();
                loadPayrollSummary();
            }
        }

        // === Employees list ===
        function loadEmployees() {
            vm.loadingEmployees = true;
            var params = { page: vm.page, pageSize: vm.pageSize };
            if (vm.searchQuery) params.search = vm.searchQuery;
            if (vm.departmentFilter) params.department = vm.departmentFilter;

            EmployeesService.getEmployees(params).then(function(res) {
                vm.employees = res.data || [];
                vm.totalCount = res.totalCount || 0;
                vm.totalPages = res.totalPages || 1;
            }).finally(function() {
                vm.loadingEmployees = false;
            });
        }

        function loadDepartments() {
            EmployeesService.getDepartments().then(function(res) {
                vm.departments = (res && res.data) || [];
            });
        }

        function goPage(p) {
            if (p < 1 || p > vm.totalPages) return;
            vm.page = p;
            loadEmployees();
        }

        // === Employee form ===
        function resetForm() {
            vm.createAccount = false;
            vm.form = {
                fullName: '',
                department: '',
                position: '',
                hireDate: new Date(),
                gender: '',
                nationalID: '',
                compensationModel: 'FixedSalary',
                baseSalary: 0,
                bankAccount: '',
                isActive: true,
                notes: '',
                userID: null,
                email: '',
                password: '',
                role: '',
                userEmail: '',
                userRole: ''
            };
        }

        function openAddModal() {
            resetForm();
            vm.editMode = false;
            vm.showFormModal = true;
            loadLinkableUsers();
        }

        function openEditModal(emp) {
            vm.editMode = true;
            vm.selectedEmployeeId = emp.employeeID;
            vm.createAccount = false;
            loadLinkableUsers();
            EmployeesService.getEmployee(emp.employeeID).then(function(res) {
                var e = res.data;
                vm.form = {
                    fullName: e.fullName || '',
                    department: e.department || '',
                    position: e.position || '',
                    hireDate: e.hireDate ? new Date(e.hireDate) : new Date(),
                    gender: e.gender || '',
                    nationalID: e.nationalID || '',
                    compensationModel: e.compensationModel || 'FixedSalary',
                    baseSalary: e.baseSalary || 0,
                    bankAccount: e.bankAccount || '',
                    isActive: e.isActive,
                    notes: e.notes || '',
                    userID: e.userID || null,
                    email: '',
                    password: '',
                    role: '',
                    userEmail: e.userEmail || '',
                    userRole: e.userRole || ''
                };
                vm.showFormModal = true;
            });
        }

        function closeFormModal() {
            vm.showFormModal = false;
        }

        function loadLinkableUsers() {
            EmployeesService.getLinkableUsers().then(function(res) {
                vm.linkableUsers = (res && res.data) || [];
            });
        }

        function toggleCreateAccount() {
            if (vm.createAccount) {
                vm.form.userID = null;
            }
        }

        function submitForm() {
            if (!vm.form.fullName) {
                toastr.warning('اسم الموظف مطلوب');
                return;
            }

            var payload = {
                fullName: vm.form.fullName,
                department: vm.form.department,
                position: vm.form.position,
                hireDate: vm.form.hireDate ? $filter('date')(vm.form.hireDate, 'yyyy-MM-dd') : null,
                gender: vm.form.gender,
                nationalID: vm.form.nationalID,
                compensationModel: vm.form.compensationModel,
                baseSalary: vm.form.baseSalary || 0,
                bankAccount: vm.form.bankAccount,
                isActive: vm.form.isActive !== false,
                notes: vm.form.notes,
                userID: vm.form.userID || null,
                email: vm.form.email || '',
                password: vm.form.password || '',
                role: vm.form.role || ''
            };

            if (vm.form.compensationModel === 'Commission' && payload.baseSalary !== 0) {
                toastr.warning('نموذج العمولات لا يقبل راتباً أساسياً — اجعله صفراً');
                return;
            }
            if (vm.form.compensationModel === 'Mixed' && !(payload.baseSalary > 0)) {
                toastr.warning('النموذج المختلط يتطلب راتباً أساسياً أكبر من صفر');
                return;
            }

            var op = vm.editMode
                ? EmployeesService.updateEmployee(vm.selectedEmployeeId, payload)
                : EmployeesService.createEmployee(payload);

            op.then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showFormModal = false;
                    loadEmployees();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        function toggleActive(emp) {
            if (!confirm('هل تريد ' + (emp.isActive ? 'تعطيل' : 'تفعيل') + ' الموظف ' + emp.fullName + '؟')) return;
            EmployeesService.toggleActive(emp.employeeID).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    loadEmployees();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        // === Employee detail ===
        function openDetail(emp) {
            vm.selectedEmployeeId = emp.employeeID;
            vm.detailTab = 'courses';
            EmployeesService.getEmployee(emp.employeeID).then(function(res) {
                vm.selectedEmployee = res.data;
                vm.courses = vm.selectedEmployee.courses || [];
                vm.showDetailModal = true;
            });
        }

        function closeDetail() {
            vm.showDetailModal = false;
            vm.selectedEmployee = null;
        }

        // === Courses ===
        function openCourseModal() {
            vm.courseForm = { courseName: '', provider: '', courseDate: new Date(), certificateNumber: '', expiryDate: null, notes: '' };
            vm.showCourseModal = true;
        }

        function closeCourseModal() {
            vm.showCourseModal = false;
        }

        function submitCourse() {
            if (!vm.courseForm.courseName) {
                toastr.warning('اسم الدورة مطلوب');
                return;
            }
            var payload = {
                courseName: vm.courseForm.courseName,
                provider: vm.courseForm.provider,
                courseDate: vm.courseForm.courseDate ? $filter('date')(vm.courseForm.courseDate, 'yyyy-MM-dd') : null,
                certificateNumber: vm.courseForm.certificateNumber,
                expiryDate: vm.courseForm.expiryDate ? $filter('date')(vm.courseForm.expiryDate, 'yyyy-MM-dd') : null,
                notes: vm.courseForm.notes
            };
            EmployeesService.addCourse(vm.selectedEmployeeId, payload).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showCourseModal = false;
                    openDetail({ employeeID: vm.selectedEmployeeId });
                } else {
                    toastr.error(res.message);
                }
            });
        }

        function deleteCourse(course) {
            if (!confirm('حذف دورة «' + course.courseName + '»؟')) return;
            EmployeesService.deleteCourse(course.courseID).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    openDetail({ employeeID: vm.selectedEmployeeId });
                } else {
                    toastr.error(res.message);
                }
            });
        }

        // === Leaves ===
        function loadLeaves() {
            vm.loadingLeaves = true;
            var params = { page: vm.leavesPage, pageSize: 20 };
            if (vm.leaveFilter) params.status = vm.leaveFilter;

            EmployeesService.getLeaves(params).then(function(res) {
                vm.leaves = res.data || [];
                vm.leavesTotalPages = res.totalPages || 1;
            }).finally(function() {
                vm.loadingLeaves = false;
            });
        }

        function leavesGoPage(p) {
            if (p < 1 || p > vm.leavesTotalPages) return;
            vm.leavesPage = p;
            loadLeaves();
        }

        function openLeaveModal() {
            vm.leaveForm = { leaveType: 'Annual', startDate: new Date(), endDate: new Date(), reason: '' };
            vm.showLeaveModal = true;
        }

        function closeLeaveModal() {
            vm.showLeaveModal = false;
        }

        function submitLeave() {
            if (!vm.leaveForm.startDate || !vm.leaveForm.endDate) {
                toastr.warning('تاريخا بداية ونهاية الإجازة مطلوبان');
                return;
            }
            if (new Date(vm.leaveForm.endDate) < new Date(vm.leaveForm.startDate)) {
                toastr.warning('تاريخ النهاية قبل تاريخ البداية');
                return;
            }
            var payload = {
                leaveType: vm.leaveForm.leaveType,
                startDate: $filter('date')(vm.leaveForm.startDate, 'yyyy-MM-dd'),
                endDate: $filter('date')(vm.leaveForm.endDate, 'yyyy-MM-dd'),
                reason: vm.leaveForm.reason
            };
            EmployeesService.addLeave(vm.selectedEmployeeId, payload).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showLeaveModal = false;
                    loadLeaves();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        function updateLeaveStatus(leave, status) {
            var action = status === 'Approved' ? 'اعتماد' : 'رفض';
            if (!confirm('هل تريد ' + action + ' إجازة ' + leave.employeeName + '؟')) return;
            EmployeesService.updateLeaveStatus(leave.leaveID, status).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    loadLeaves();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        // === Payroll ===
        function loadPayroll() {
            vm.loadingPayroll = true;
            var params = { page: vm.payrollPage, pageSize: 20 };
            if (vm.payrollYear) params.year = vm.payrollYear;
            if (vm.payrollMonth) params.month = vm.payrollMonth;
            if (vm.payrollStatus) params.status = vm.payrollStatus;

            EmployeesService.getPayroll(params).then(function(res) {
                vm.payroll = res.data || [];
                vm.payrollTotalPages = res.totalPages || 1;
            }).finally(function() {
                vm.loadingPayroll = false;
            });
        }

        function loadPayrollSummary() {
            var params = {};
            if (vm.payrollYear) params.year = vm.payrollYear;
            if (vm.payrollMonth) params.month = vm.payrollMonth;

            EmployeesService.getPayrollSummary(params).then(function(res) {
                vm.payrollSummary = (res && res.data) || null;
            });
        }

        function payrollGoPage(p) {
            if (p < 1 || p > vm.payrollTotalPages) return;
            vm.payrollPage = p;
            loadPayroll();
        }

        function runPayroll() {
            if (!confirm('توليد مسودة رواتب شهر ' + vm.months.filter(function(m) { return m.value === vm.payrollMonth; })[0].label + ' ' + vm.payrollYear + '؟')) return;
            EmployeesService.runPayroll({ year: vm.payrollYear, month: vm.payrollMonth }).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    loadPayroll();
                    loadPayrollSummary();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        function postPayroll(rec) {
            if (!confirm('ترحيل قيد استحقاق راتب ' + rec.employeeName + ' محاسبياً؟')) return;
            EmployeesService.postPayroll(rec.salaryRecordID).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    loadPayroll();
                    loadPayrollSummary();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        function reversePayroll(rec) {
            if (!confirm('عكس راتب ' + rec.employeeName + ' وإنشاء قيد عكسي؟')) return;
            EmployeesService.reversePayroll(rec.salaryRecordID).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    loadPayroll();
                    loadPayrollSummary();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        function deletePayrollDraft(rec) {
            if (!confirm('حذف مسودة راتب ' + rec.employeeName + '؟')) return;
            EmployeesService.deletePayrollDraft(rec.salaryRecordID).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    loadPayroll();
                    loadPayrollSummary();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        function openAdjustModal(rec) {
            vm.adjustForm = { salaryRecordID: rec.salaryRecordID, bonus: 0, deduction: 0 };
            vm.showAdjustModal = true;
        }

        function closeAdjustModal() {
            vm.showAdjustModal = false;
        }

        function submitAdjust() {
            if (vm.adjustForm.bonus < 0 || vm.adjustForm.deduction < 0) {
                toastr.warning('المكافأة والخصم لا يكونان سالبين');
                return;
            }
            EmployeesService.adjustPayroll(vm.adjustForm.salaryRecordID, { bonus: vm.adjustForm.bonus || 0, deduction: vm.adjustForm.deduction || 0 }).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showAdjustModal = false;
                    loadPayroll();
                    loadPayrollSummary();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        // === Self-service ===
        function loadMyEmployee() {
            vm.meLoading = true;
            EmployeesService.getMyEmployee().then(function(res) {
                vm.me = res.data;
            }).catch(function() {
                vm.me = null;
            }).finally(function() {
                vm.meLoading = false;
            });
        }

        function openMyLeaveModal() {
            vm.myLeaveForm = { leaveType: 'Annual', startDate: new Date(), endDate: new Date(), reason: '' };
            vm.showMyLeaveModal = true;
        }

        function closeMyLeaveModal() {
            vm.showMyLeaveModal = false;
        }

        function submitMyLeave() {
            if (!vm.myLeaveForm.startDate || !vm.myLeaveForm.endDate) {
                toastr.warning('تاريخا بداية ونهاية الإجازة مطلوبان');
                return;
            }
            if (new Date(vm.myLeaveForm.endDate) < new Date(vm.myLeaveForm.startDate)) {
                toastr.warning('تاريخ النهاية قبل تاريخ البداية');
                return;
            }
            var payload = {
                leaveType: vm.myLeaveForm.leaveType,
                startDate: $filter('date')(vm.myLeaveForm.startDate, 'yyyy-MM-dd'),
                endDate: $filter('date')(vm.myLeaveForm.endDate, 'yyyy-MM-dd'),
                reason: vm.myLeaveForm.reason
            };
            EmployeesService.requestMyLeave(payload).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.showMyLeaveModal = false;
                    loadMyEmployee();
                } else {
                    toastr.error(res.message);
                }
            });
        }

        // === Labels ===
        function getCompensationLabel(model) {
            var found = vm.compensationModels.filter(function(m) { return m.value === model; })[0];
            return found ? found.label : (model || '-');
        }

        function getLeaveTypeLabel(type) {
            var found = vm.leaveTypes.filter(function(m) { return m.value === type; })[0];
            return found ? found.label : (type || '-');
        }

        function getStatusLabel(status) {
            var labels = {
                'Pending': 'قيد الانتظار',
                'Approved': 'معتمدة',
                'Rejected': 'مرفوضة',
                'Draft': 'مسودة',
                'Posted': 'مرحّلة',
                'Reversed': 'معكوسة'
            };
            return labels[status] || status || '-';
        }

        function getRoleLabel(role) {
            var labels = {
                'Admin': 'مدير النظام',
                'Accountant': 'محاسب',
                'Doctor': 'طبيب',
                'Pharmacist': 'صيدلاني',
                'LabTechnician': 'فني مختبرات',
                'Radiologist': 'أخصائي أشعة',
                'Receptionist': 'موظف استقبال',
                'Cashier': 'كاشير',
                'WarehouseKeeper': 'أمين مخزن'
            };
            return labels[role] || role || '';
        }
    }
})();
