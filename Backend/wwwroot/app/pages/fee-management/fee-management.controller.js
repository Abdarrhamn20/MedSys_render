(function() {
    'use strict';

    angular.module('medicalApp').controller('FeeManagementController', FeeManagementController);

    FeeManagementController.$inject = ['AuthService', 'UsersService', '$http', 'toastr', '$timeout'];

    function FeeManagementController(AuthService, UsersService, $http, toastr, $timeout) {
        var vm = this;

        // User data
        vm.user = AuthService.getUser() || {};
        vm.role = vm.user.role;
        
        // Doctor View variables
        vm.doctorProfile = null;
        vm.currentFee = 0;
        vm.newFee = 0;
        vm.savingFee = false;

        // Admin View variables
        vm.doctors = [];
        vm.searchQuery = '';
        vm.loading = false;
        vm.page = 1;
        vm.pageSize = 10;
        vm.totalPages = 1;
        vm.selectedDocForEdit = null;
        vm.editDocFee = 0;
        vm.showEditModal = false;
        vm.updatingDocFee = false;
        vm.dayList = [
            { code: 'Sun', nameAr: 'الأحد' },
            { code: 'Mon', nameAr: 'الاثنين' },
            { code: 'Tue', nameAr: 'الثلاثاء' },
            { code: 'Wed', nameAr: 'الأربعاء' },
            { code: 'Thu', nameAr: 'الخميس' },
            { code: 'Fri', nameAr: 'الجمعة' },
            { code: 'Sat', nameAr: 'السبت' }
        ];
        vm.selectedDays = {};
        vm.editWorkStart = null;
        vm.editWorkEnd = null;

        // Admin Stats
        vm.stats = {
            averageFee: 0,
            highestFee: 0,
            lowestFee: 0,
            totalDoctors: 0
        };

        // Actions
        vm.saveDoctorFee = saveDoctorFee;
        vm.loadDoctors = loadDoctors;
        vm.applyFilter = applyFilter;
        vm.pageChange = pageChange;
        vm.openEditModal = openEditModal;
        vm.closeEditModal = closeEditModal;
        vm.updateDoctorFeeAdmin = updateDoctorFeeAdmin;
        vm.setQuickFee = setQuickFee;

        activate();

        function activate() {
            if (vm.role === 'Doctor') {
                loadDoctorOwnProfile();
            } else if (vm.role === 'Admin') {
                loadDoctors();
                loadAllDoctorsForStats();
            }
        }

        // =================== Doctor Specific Logic ===================
        function loadDoctorOwnProfile() {
            vm.loading = true;
            
            // If profileID is missing from local storage, try fetching it from Auth profile API
            if (!vm.user.profileID) {
                $http.get('/api/auth/profile')
                    .then(function(res) {
                        if (res.data.success && res.data.data && res.data.data.profileID) {
                            vm.user.profileID = res.data.data.profileID;
                            // Update stored user to persist it
                            var stored = JSON.parse(localStorage.getItem('medical_user') || '{}');
                            stored.profileID = res.data.data.profileID;
                            localStorage.setItem('medical_user', JSON.stringify(stored));
                            
                            // Load using the fetched ID
                            fetchDoctorData(vm.user.profileID);
                        } else {
                            toastr.error('لم نتمكن من تحديد الهوية الطبية الخاصة بك. يرجى تسجيل الخروج والدخول مجدداً.');
                            vm.loading = false;
                        }
                    })
                    .catch(function() {
                        toastr.error('حدث خطأ أثناء تحديد الهوية الطبية');
                        vm.loading = false;
                    });
            } else {
                fetchDoctorData(vm.user.profileID);
            }
        }

        function fetchDoctorData(profileId) {
            $http.get('/api/doctors/' + profileId)
                .then(function(res) {
                    if (res.data.success) {
                        vm.doctorProfile = res.data.data;
                        vm.currentFee = vm.doctorProfile.consultationFee;
                        vm.newFee = vm.currentFee;
                    } else {
                        toastr.error('فشل في تحميل بيانات الطبيب');
                    }
                })
                .catch(function() {
                    toastr.error('حدث خطأ أثناء تحميل البيانات الطبية');
                })
                .finally(function() {
                    vm.loading = false;
                });
        }

        function saveDoctorFee() {
            if (vm.newFee < 0) {
                toastr.warning('يرجى إدخال قيمة صحيحة لرسوم الكشف (أكبر من أو تساوي صفر)');
                return;
            }

            vm.savingFee = true;
            
            // Build the payload (using the existing DoctorUpdateDTO structure)
            var payload = {
                specialty: vm.doctorProfile.specialty,
                licenseNumber: vm.doctorProfile.licenseNumber,
                emergencyReady: vm.doctorProfile.emergencyReady,
                bio: vm.doctorProfile.bio,
                availableDays: vm.doctorProfile.availableDays,
                workStartTime: vm.doctorProfile.workStartTime,
                workEndTime: vm.doctorProfile.workEndTime,
                consultationDurationMinutes: vm.doctorProfile.consultationDurationMinutes,
                consultationFee: vm.newFee
            };

            $http.put('/api/doctors/' + vm.user.profileID, payload)
                .then(function(res) {
                    if (res.data.success) {
                        toastr.success('تم تحديث رسوم الكشف الطبي بنجاح!');
                        vm.currentFee = vm.newFee;
                        loadDoctorOwnProfile();
                    } else {
                        toastr.error(res.data.message || 'فشل التحديث');
                    }
                })
                .catch(function() {
                    toastr.error('حدث خطأ أثناء حفظ التغييرات');
                })
                .finally(function() {
                    vm.savingFee = false;
                });
        }

        function setQuickFee(val) {
            vm.newFee = val;
        }

        // =================== Admin Specific Logic ===================
        function loadDoctors() {
            vm.loading = true;
            UsersService.getDoctors({
                page: vm.page,
                pageSize: vm.pageSize,
                search: vm.searchQuery
            })
            .then(function(res) {
                vm.doctors = res.data || [];
                vm.totalPages = Math.ceil(res.totalCount / vm.pageSize) || 1;
            })
            .catch(function() {
                toastr.error('فشل في تحميل قائمة الأطباء');
            })
            .finally(function() {
                vm.loading = false;
            });
        }

        function loadAllDoctorsForStats() {
            // Fetch all doctors to compute reliable statistics
            UsersService.getDoctors({ page: 1, pageSize: 1000 })
                .then(function(res) {
                    var allDocs = res.data || [];
                    if (allDocs.length > 0) {
                        var total = 0;
                        var max = 0;
                        var min = allDocs[0].consultationFee;

                        allDocs.forEach(function(doc) {
                            var fee = doc.consultationFee || 0;
                            total += fee;
                            if (fee > max) max = fee;
                            if (fee < min) min = fee;
                        });

                        vm.stats.totalDoctors = allDocs.length;
                        vm.stats.averageFee = total / allDocs.length;
                        vm.stats.highestFee = max;
                        vm.stats.lowestFee = min;
                    } else {
                        vm.stats = { averageFee: 0, highestFee: 0, lowestFee: 0, totalDoctors: 0 };
                    }
                });
        }

        function applyFilter() {
            vm.page = 1;
            loadDoctors();
        }

        function pageChange(dir) {
            vm.page += dir;
            if (vm.page < 1) vm.page = 1;
            if (vm.page > vm.totalPages) vm.page = vm.totalPages;
            loadDoctors();
        }

        function openEditModal(doc) {
            vm.selectedDocForEdit = doc;
            vm.editDocFee = doc.consultationFee;
            vm.showEditModal = true;

            vm.selectedDays = {};
            vm.dayList.forEach(function(d) { vm.selectedDays[d.code] = false; });
            if (doc.availableDays) {
                doc.availableDays.split(',').forEach(function(code) {
                    var c = code.trim();
                    if (vm.selectedDays.hasOwnProperty(c)) vm.selectedDays[c] = true;
                });
            }
            vm.editWorkStart = apiToTime(doc.workStartTime);
            vm.editWorkEnd = apiToTime(doc.workEndTime);
        }

        function closeEditModal() {
            vm.showEditModal = false;
            vm.selectedDocForEdit = null;
        }

        function updateDoctorFeeAdmin() {
            if (vm.editDocFee < 0) {
                toastr.warning('يرجى إدخال قيمة رسوم صحيحة');
                return;
            }

            vm.updatingDocFee = true;
            
            // First fetch the full doctor info to avoid overwriting other properties with defaults
            $http.get('/api/doctors/' + vm.selectedDocForEdit.doctorID)
                .then(function(getRes) {
                    if (getRes.data.success) {
                        var docFull = getRes.data.data;
                        var payload = {
                            specialty: docFull.specialty,
                            licenseNumber: docFull.licenseNumber,
                            emergencyReady: docFull.emergencyReady,
                            bio: docFull.bio,
                            availableDays: buildAvailableDays(),
                            workStartTime: timeToApi(vm.editWorkStart),
                            workEndTime: timeToApi(vm.editWorkEnd),
                            consultationDurationMinutes: docFull.consultationDurationMinutes,
                            consultationFee: vm.editDocFee
                        };

                        return $http.put('/api/doctors/' + vm.selectedDocForEdit.doctorID, payload);
                    } else {
                        throw new Error('فشل تحميل تفاصيل الطبيب');
                    }
                })
                .then(function(putRes) {
                    if (putRes.data.success) {
                        toastr.success('تم تحديث رسوم كشف الطبيب بنجاح بواسطة المدير');
                        closeEditModal();
                        loadDoctors();
                        loadAllDoctorsForStats();
                    } else {
                        toastr.error(putRes.data.message || 'فشل التحديث');
                    }
                })
                .catch(function(err) {
                    var msg = err.data && err.data.message ? err.data.message : 'حدث خطأ أثناء التحديث';
                    toastr.error(msg);
                })
                .finally(function() {
                    vm.updatingDocFee = false;
                });
        }

        function buildAvailableDays() {
            var codes = vm.dayList
                .filter(function(d) { return vm.selectedDays[d.code]; })
                .map(function(d) { return d.code; });
            return codes.join(',');
        }

        function apiToTime(value) {
            if (!value) return null;
            var parts = String(value).split(':');
            if (parts.length < 2) return null;
            var d = new Date();
            d.setHours(parseInt(parts[0], 10), parseInt(parts[1], 10), 0, 0);
            return d;
        }

        function timeToApi(value) {
            if (!value) return null;
            var h = String(value.getHours());
            var m = String(value.getMinutes());
            return (h.length < 2 ? '0' + h : h) + ':' + (m.length < 2 ? '0' + m : m) + ':00';
        }
    }
})();
