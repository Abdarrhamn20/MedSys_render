(function() {
    'use strict';

    angular.module('medicalApp').controller('ProfileController', ProfileController);

    ProfileController.$inject = ['AuthService', 'UsersService', '$http', 'toastr', '$rootScope', '$window'];

    function ProfileController(AuthService, UsersService, $http, toastr, $rootScope, $window) {
        var vm = this;

        vm.user = AuthService.getUser() || {};
        vm.profile = {};
        vm.editMode = false;
        vm.editData = {};
        vm.saving = false;
        vm.passwordData = {};
        vm.changingPw = false;
        vm.profileStats = {};
        vm.facilityMode = 'General';
        vm.updatingMode = false;

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

        vm.getRoleAr = getRoleAr;
        vm.saveProfile = saveProfile;
        vm.changePassword = changePassword;
        vm.saveFacilityMode = saveFacilityMode;

        activate();

        function activate() {
            loadProfile();
            loadStats();
            if (vm.user.role === 'Admin') {
                loadFacilityMode();
            }
        }

        function loadFacilityMode() {
            $http.get('/api/settings/facility-mode').then(function(res) {
                if (res.data && res.data.data) {
                    vm.facilityMode = res.data.data.facilityMode || 'General';
                }
            });
        }

        function saveFacilityMode(mode) {
            vm.updatingMode = true;
            $http.post('/api/settings/facility-mode', { facilityMode: mode }).then(function(res) {
                if (res.data && res.data.success) {
                    vm.facilityMode = mode;
                    toastr.success('تم تحديث نمط تشغيل المنظومة وتعديل القوائم بنجاح');
                    $rootScope.facilityMode = mode;
                    $rootScope.$broadcast('facilityModeChanged', mode);
                } else {
                    toastr.error(res.data ? res.data.message : 'حدث خطأ أثناء التحديث');
                }
            }).finally(function() {
                vm.updatingMode = false;
            });
        }

        function loadProfile() {
            $http.get('/api/users/' + vm.user.userID).then(function(res) {
                if (res.data.success) {
                    var data = res.data.data;
                    vm.profile = {
                        fullName: data.fullName,
                        email: data.email,
                        phone: data.phone
                    };

                    if (data.doctorProfile) {
                        vm.profile.specialty = data.doctorProfile.specialty;
                        vm.profile.licenseNumber = data.doctorProfile.licenseNumber;
                        vm.profile.emergencyReady = data.doctorProfile.emergencyReady;
                        vm.profile.bio = data.doctorProfile.bio;
                        vm.profile.consultationFee = data.doctorProfile.consultationFee;
                        vm.profile.availableDaysDisplay = formatWorkingDays(data.doctorProfile.availableDays);
                        vm.profile.workHoursDisplay = formatWorkHours(data.doctorProfile.workStartTime, data.doctorProfile.workEndTime);
                        vm.editData = angular.copy(data.doctorProfile);
                        initWorkSchedule(vm.editData);
                    }
                    if (data.patientProfile) {
                        vm.profile.bloodType = data.patientProfile.bloodType;
                        vm.profile.gender = data.patientProfile.gender;
                        vm.profile.chronicDiseases = data.patientProfile.chronicDiseases;
                        vm.profile.allergies = data.patientProfile.allergies;
                        vm.editData = angular.copy(data.patientProfile);
                    }
                }
            });
        }

        function loadStats() {
            UsersService.getDashboardStats().then(function(res) {
                if (res.success) {
                    if (vm.user.role === 'Patient') {
                        vm.profileStats = { appointments: res.data.upcomingAppointments || 0, records: res.data.medicalRecords || 0 };
                    } else if (vm.user.role === 'Doctor') {
                        vm.profileStats = { patients: res.data.totalPatients || 0, completed: res.data.completedThisMonth || 0 };
                    }
                }
            });
        }

        function saveProfile() {
            vm.saving = true;

            // Fallback: إذا كان profileID غير محفوظ في الجلسة يتم جلبه من /api/auth/profile ثم الحفظ
            if (!vm.user.profileID) {
                return AuthService.getProfile().then(function(res) {
                    if (res.success && res.data && res.data.profileID) {
                        vm.user.profileID = res.data.profileID;
                        $window.localStorage.setItem('medical_user', JSON.stringify(vm.user));
                        if ($rootScope.currentUser) {
                            $rootScope.currentUser.profileID = res.data.profileID;
                        }
                        return doSaveProfile();
                    }
                    vm.saving = false;
                    toastr.error('تعذر جلب بيانات الملف الشخصي، يرجى إعادة تسجيل الدخول');
                });
            }

            doSaveProfile();
        }

        function doSaveProfile() {
            var profileId = vm.user.profileID;
            var url, data;

            if (vm.user.role === 'Doctor') {
                url = '/api/doctors/' + profileId;
                data = vm.editData;
                data.availableDays = buildAvailableDays();
                data.workStartTime = timeToApi(vm.editWorkStart);
                data.workEndTime = timeToApi(vm.editWorkEnd);
            } else if (vm.user.role === 'Patient') {
                url = '/api/patients/' + profileId;
                data = vm.editData;
            } else {
                vm.saving = false;
                return;
            }

            $http.put(url, data).then(function(res) {
                if (res.data.success) {
                    toastr.success(res.data.message);
                    vm.editMode = false;
                    loadProfile();
                } else {
                    toastr.error(res.data.message);
                }
            }).catch(function() {
                toastr.error('حدث خطأ');
            }).finally(function() { vm.saving = false; });
        }

        function changePassword() {
            if (!vm.passwordData.currentPassword || !vm.passwordData.newPassword) {
                toastr.warning('أدخل كلمة المرور الحالية والجديدة');
                return;
            }
            vm.changingPw = true;

            AuthService.changePassword(vm.passwordData).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    vm.passwordData = {};
                } else {
                    toastr.error(res.message);
                }
            }).catch(function(err) {
                toastr.error(err.data ? err.data.message : 'حدث خطأ');
            }).finally(function() { vm.changingPw = false; });
        }

        function getRoleAr() {
            return { 'Admin': 'مدير النظام', 'Doctor': 'طبيب', 'Patient': 'مريض', 'Pharmacist': 'صيدلاني', 'LabTechnician': 'فني مختبرات', 'Radiologist': 'أخصائي أشعة', 'Receptionist': 'موظف استقبال', 'Cashier': 'كاشير' }[vm.user.role] || vm.user.role;
        }

        function initWorkSchedule(docData) {
            vm.selectedDays = {};
            vm.dayList.forEach(function(d) { vm.selectedDays[d.code] = false; });
            if (docData.availableDays) {
                docData.availableDays.split(',').forEach(function(code) {
                    var c = code.trim();
                    if (vm.selectedDays.hasOwnProperty(c)) vm.selectedDays[c] = true;
                });
            }
            vm.editWorkStart = apiToTime(docData.workStartTime);
            vm.editWorkEnd = apiToTime(docData.workEndTime);
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

        function formatWorkingDays(daysStr) {
            if (!daysStr) return 'كل الأيام';
            var map = { Sun: 'الأحد', Mon: 'الاثنين', Tue: 'الثلاثاء', Wed: 'الأربعاء', Thu: 'الخميس', Fri: 'الجمعة', Sat: 'السبت' };
            return daysStr.split(',').map(function(d) {
                return map[d.trim()] || d.trim();
            }).join('، ');
        }

        function formatWorkHours(start, end) {
            if (!start && !end) return 'غير محدد (افتراضي 9:00 ص - 5:00 م)';
            if (!start) return 'من ' + formatTime(end);
            if (!end) return 'من ' + formatTime(start);
            return formatTime(start) + ' - ' + formatTime(end);
        }

        function formatTime(timeStr) {
            if (!timeStr) return '—';
            var parts = String(timeStr).split(':');
            var h = parseInt(parts[0], 10);
            var m = parts[1] || '00';
            var suffix = h >= 12 ? 'م' : 'ص';
            var h12 = h % 12 || 12;
            return h12 + ':' + m + ' ' + suffix;
        }
    }
})();
