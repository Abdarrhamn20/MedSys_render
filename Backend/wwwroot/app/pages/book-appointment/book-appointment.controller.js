(function() {
    'use strict';

    angular.module('medicalApp').controller('BookAppointmentController', BookAppointmentController);

    BookAppointmentController.$inject = ['AppointmentService', 'PsychiatricService', 'toastr', '$state', '$stateParams', '$rootScope'];

    function BookAppointmentController(AppointmentService, PsychiatricService, toastr, $state, $stateParams, $rootScope) {
        var vm = this;

        // === Tab Mode: 'medical' | 'psychiatric' ===
        var currentMode = $rootScope.facilityMode || 'General';
        vm.bookingMode = currentMode === 'Psychiatric' ? 'psychiatric' : 'medical';

        // === Medical Booking State ===
        vm.med = {
            steps: ['تقييم الحالة', 'اختيار الطبيب', 'اختيار الموعد', 'تأكيد'],
            currentStep: 1,
            questions: [],
            answers: {},
            evaluating: false,
            triageResult: null,
            recommendedSpecialties: [],
            specialtyFilterActive: false,
            specialties: [],
            selectedSpecialty: '',
            doctors: [],
            selectedDoctor: null,
            selectedDate: null,
            selectedSlot: null,
            slots: [],
            slotsMessage: '',
            notes: '',
            booking: false,
            bookedAppointmentId: null
        };

        // === Psychiatric Booking State ===
        vm.psych = {
            steps: ['تقييم الحالة النفسية', 'اختيار الطبيب النفسي', 'اختيار الموعد', 'تأكيد'],
            currentStep: 1,
            questions: [],
            answers: {},
            evaluating: false,
            triageResult: null,
            recommendedSpecialties: [],
            specialtyFilterActive: false,
            doctors: [],
            selectedDoctor: null,
            selectedDate: null,
            selectedSlot: null,
            slots: [],
            slotsMessage: '',
            notes: '',
            booking: false,
            bookedAppointmentId: null,
            appointmentType: 'Online'
        };
        var today = new Date();
        vm.minDate = today.toISOString().split('T')[0];
        vm.maxDate = null;
        vm.policy = { maxDaysAhead: 30, cancelWindowHours: 6, maxFutureAppointments: 5, slotBufferMinutes: 5 };

        vm.isDoctorWorkingDay = isDoctorWorkingDay;
        vm.calcMaxDate = calcMaxDate;
        vm.formatTime = formatTime;
        vm.formatWorkingDays = formatWorkingDays;
        vm.getDoctorStatus = getDoctorStatus;
        vm.medShowAllDoctors = medShowAllDoctors;
        vm.psychShowAllDoctors = psychShowAllDoctors;

        // Medical functions
        vm.medSetAnswer = medSetAnswer;
        vm.medEvaluate = medEvaluate;
        vm.medLoadDoctors = medLoadDoctors;
        vm.medSelectDoctor = medSelectDoctor;
        vm.medLoadSlots = medLoadSlots;
        vm.medBook = medBook;
        vm.medReset = medReset;

        // Psychiatric functions
        vm.psychSetAnswer = psychSetAnswer;
        vm.psychEvaluate = psychEvaluate;
        vm.psychSelectDoctor = psychSelectDoctor;
        vm.psychLoadSlots = psychLoadSlots;
        vm.psychBook = psychBook;
        vm.psychReset = psychReset;

        // Shared
        vm.switchMode = switchMode;

        activate();

        function activate() {
            // Load general triage questions
            AppointmentService.getTriageQuestions().then(function(res) {
                if (res.success) {
                    var all = res.data || [];
                    vm.med.questions = all.filter(function(q) { return q.category !== 'Psychiatric'; });
                    vm.psych.questions = all.filter(function(q) { return q.category === 'Psychiatric'; });
                }
            });

            // Load specialties
            AppointmentService.getSpecialties().then(function(res) {
                if (res.success) vm.med.specialties = res.data;
            });

            // Load booking policy (max booking days, cancel window, ...)
            AppointmentService.getBookingPolicy().then(function(res) {
                if (res.success) {
                    vm.policy = res.data;
                    vm.maxDate = calcMaxDate();
                }
            });
        }

        function calcMaxDate() {
            var d = new Date();
            d.setDate(d.getDate() + (vm.policy.maxDaysAhead || 30));
            return d.toISOString().split('T')[0];
        }

        // التحقق من أن اليوم من أيام عمل الطبيب (AvailableDays بصيغة "Sun,Mon,...")
        function isDoctorWorkingDay(dateStr, doctor) {
            if (!dateStr || !doctor || !doctor.availableDays) return true;
            var parts = doctor.availableDays.split(',');
            var dayAbbrev = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'][new Date(dateStr).getDay()];
            return parts.some(function(d) { return d.trim().toLowerCase() === dayAbbrev.toLowerCase(); });
        }

        // تحويل "09:00:00" إلى صيغة "9:00 ص"
        function formatTime(timeStr) {
            if (!timeStr) return '—';
            var parts = timeStr.split(':');
            var h = parseInt(parts[0], 10);
            var m = parts[1] || '00';
            var suffix = h >= 12 ? 'م' : 'ص';
            var h12 = h % 12 || 12;
            return h12 + ':' + m + ' ' + suffix;
        }

        // تحويل "Sun,Mon" إلى أسماء الأيام العربية
        function formatWorkingDays(daysStr) {
            if (!daysStr) return 'كل الأيام';
            var map = { Sun: 'الأحد', Mon: 'الاثنين', Tue: 'الثلاثاء', Wed: 'الأربعاء', Thu: 'الخميس', Fri: 'الجمعة', Sat: 'السبت' };
            return daysStr.split(',').map(function(d) {
                return map[d.trim()] || d.trim();
            }).join('، ');
        }

        // يحدد حالة تواجد الطبيب الآن: يعمل اليوم؟ ضمن ساعات العمل الحالية؟
        function getDoctorStatus(doctor) {
            if (!doctor) return { label: '', cls: '', icon: '' };

            var dayAbbrev = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'][new Date().getDay()];
            var worksToday = !doctor.availableDays ||
                doctor.availableDays.split(',').some(function(d) { return d.trim().toLowerCase() === dayAbbrev.toLowerCase(); });
            if (!worksToday) {
                return { label: 'لا يعمل اليوم', cls: 'badge-emergency', icon: 'fa-user-slash' };
            }

            var now = new Date();
            var nowMin = now.getHours() * 60 + now.getMinutes();
            var startMin = timeToMinutes(doctor.workStartTime);
            var endMin = timeToMinutes(doctor.workEndTime);
            if (startMin === null || endMin === null) {
                return { label: 'مقبول حجوزات الآن', cls: 'badge-normal', icon: 'fa-check-circle' };
            }
            if (nowMin >= startMin && nowMin < endMin) {
                return { label: 'متاح الآن', cls: 'badge-normal', icon: 'fa-check-circle' };
            }
            return { label: 'خارج ساعات العمل الآن', cls: 'badge-urgent', icon: 'fa-clock' };
        }

        function timeToMinutes(value) {
            if (!value) return null;
            var parts = String(value).split(':');
            if (parts.length < 2) return null;
            return parseInt(parts[0], 10) * 60 + parseInt(parts[1], 10);
        }

        function switchMode(mode) {
            vm.bookingMode = mode;
            if (mode === 'psychiatric' && vm.psych.questions.length === 0) {
                AppointmentService.getTriageQuestions().then(function(res) {
                    if (res.success) {
                        vm.psych.questions = (res.data || []).filter(function(q) { return q.category === 'Psychiatric'; });
                    }
                });
            }
        }

        // =======================================================
        //  MEDICAL BOOKING
        // =======================================================

        function medSetAnswer(question, value) {
            vm.med.answers[question.questionID] = value;
        }

        function medEvaluate() {
            var s = vm.med;
            s.evaluating = true;
            var answersArray = [];
            s.questions.forEach(function(q) {
                if (s.answers[q.questionID] === true) {
                    answersArray.push({ questionID: q.questionID, answer: true, weight: q.weight });
                }
            });

            AppointmentService.evaluateTriage({ answers: answersArray })
                .then(function(res) {
                    if (res.success) {
                        s.triageResult = res.data;
                        s.recommendedSpecialties = res.data.recommendedSpecialties || [];
                        s.specialtyFilterActive = s.recommendedSpecialties.length > 0;
                        if (s.selectedDoctor) {
                            s.currentStep = 3;
                            medLoadSlots();
                        } else {
                            s.currentStep = 2;
                            medLoadDoctors();
                        }
                    }
                })
                .catch(function() { toastr.error('حدث خطأ في التقييم'); })
                .finally(function() { s.evaluating = false; });
        }

        function medShowAllDoctors() {
            var s = vm.med;
            s.specialtyFilterActive = false;
            s.selectedSpecialty = '';
            medLoadDoctors();
        }

        function medLoadDoctors() {
            var s = vm.med;
            var params = { pageSize: 50 };
            if (s.selectedSpecialty) {
                params.specialty = s.selectedSpecialty;
            }
            AppointmentService.getDoctors(params)
                .then(function(res) {
                    var all = res.data || [];
                    if (s.specialtyFilterActive && !s.selectedSpecialty) {
                        s.doctors = all.filter(function(doc) {
                            return (doc.specialty || '').split(' ').some(function(part) {
                                return s.recommendedSpecialties.some(function(kw) {
                                    return part.indexOf(kw) !== -1;
                                });
                            });
                        });
                    } else {
                        s.doctors = all;
                    }
                });
        }

        function medSelectDoctor(doctor) {
            vm.med.selectedDoctor = doctor;
        }

        function medLoadSlots() {
            var s = vm.med;
            if (!s.selectedDoctor || !s.selectedDate) return;
            s.slotsMessage = '';
            AppointmentService.getAvailableSlots(s.selectedDoctor.doctorID, s.selectedDate)
                .then(function(res) {
                    if (res.success) {
                        s.slots = res.data;
                        s.slotsMessage = res.message;
                        s.selectedSlot = null;
                    }
                });
        }

        function medBook() {
            var s = vm.med;
            if (!s.selectedDoctor || !s.selectedDate || !s.selectedSlot) {
                toastr.warning('يرجى اختيار الطبيب والتاريخ والوقت');
                return;
            }
            var answersArray = [];
            s.questions.forEach(function(q) {
                if (s.answers[q.questionID] === true) {
                    answersArray.push({ questionID: q.questionID, answer: true });
                }
            });
            s.booking = true;
            AppointmentService.createAppointment({
                doctorID: s.selectedDoctor.doctorID,
                appointmentDate: s.selectedDate,
                appointmentTime: s.selectedSlot,
                notes: s.notes,
                answers: answersArray
            }).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    s.bookedAppointmentId = res.data.appointmentId;
                    s.currentStep = 4;
                } else { toastr.error(res.message); }
            }).catch(function(err) {
                toastr.error(err.data ? err.data.message : 'حدث خطأ في الحجز');
            }).finally(function() { s.booking = false; });
        }

        function medReset() {
            var s = vm.med;
            s.currentStep = 1;
            s.answers = {};
            s.triageResult = null;
            s.recommendedSpecialties = [];
            s.specialtyFilterActive = false;
            s.selectedDoctor = null;
            s.selectedDate = null;
            s.selectedSlot = null;
            s.slots = [];
            s.slotsMessage = '';
            s.notes = '';
            s.bookedAppointmentId = null;
        }

        // =======================================================
        //  PSYCHIATRIC BOOKING
        // =======================================================

        function psychSetAnswer(question, value) {
            vm.psych.answers[question.questionID] = value;
        }

        function psychEvaluate() {
            var s = vm.psych;
            s.evaluating = true;
            var answersArray = [];
            s.questions.forEach(function(q) {
                if (s.answers[q.questionID] === true) {
                    answersArray.push({ questionID: q.questionID, answer: true, weight: q.weight });
                }
            });

            AppointmentService.evaluateTriage({ answers: answersArray })
                .then(function(res) {
                    if (res.success) {
                        s.triageResult = res.data;
                        s.recommendedSpecialties = res.data.recommendedSpecialties || [];
                        s.specialtyFilterActive = true;
                        // Load psychiatric doctors only
                        AppointmentService.getDoctors({ specialty: 'الطب النفسي', pageSize: 50 })
                            .then(function(docRes) {
                                s.doctors = docRes.data || [];
                                s.currentStep = 2;
                                if (s.doctors.length === 0) {
                                    toastr.warning('لا يوجد أطباء نفسيون متاحون حالياً');
                                }
                            });
                    }
                })
                .catch(function() { toastr.error('حدث خطأ في التقييم'); })
                .finally(function() { s.evaluating = false; });
        }

        function psychShowAllDoctors() {
            var s = vm.psych;
            s.specialtyFilterActive = false;
            AppointmentService.getDoctors({ specialty: 'الطب النفسي', pageSize: 50 })
                .then(function(docRes) {
                    s.doctors = docRes.data || [];
                });
        }

        function psychSelectDoctor(doctor) {
            vm.psych.selectedDoctor = doctor;
        }

        function psychLoadSlots() {
            var s = vm.psych;
            if (!s.selectedDoctor || !s.selectedDate) return;
            s.slotsMessage = '';
            AppointmentService.getAvailableSlots(s.selectedDoctor.doctorID, s.selectedDate)
                .then(function(res) {
                    if (res.success) {
                        s.slots = res.data;
                        s.slotsMessage = res.message;
                        s.selectedSlot = null;
                    }
                });
        }

        function psychBook() {
            var s = vm.psych;
            if (!s.selectedDoctor || !s.selectedDate || !s.selectedSlot) {
                toastr.warning('يرجى اختيار الطبيب والتاريخ والوقت');
                return;
            }
            var answersArray = [];
            s.questions.forEach(function(q) {
                if (s.answers[q.questionID] === true) {
                    answersArray.push({ questionID: q.questionID, answer: true });
                }
            });
            s.booking = true;
            AppointmentService.createAppointment({
                doctorID: s.selectedDoctor.doctorID,
                appointmentDate: s.selectedDate,
                appointmentTime: s.selectedSlot,
                notes: s.notes,
                appointmentType: s.appointmentType || 'Online',
                answers: answersArray
            }).then(function(res) {
                if (res.success) {
                    toastr.success(res.message);
                    s.bookedAppointmentId = res.data.appointmentId;
                    s.currentStep = 4;
                } else { toastr.error(res.message); }
            }).catch(function(err) {
                toastr.error(err.data ? err.data.message : 'حدث خطأ في الحجز');
            }).finally(function() { s.booking = false; });
        }

        function psychReset() {
            var s = vm.psych;
            s.currentStep = 1;
            s.answers = {};
            s.triageResult = null;
            s.recommendedSpecialties = [];
            s.specialtyFilterActive = false;
            s.selectedDoctor = null;
            s.selectedDate = null;
            s.selectedSlot = null;
            s.slots = [];
            s.slotsMessage = '';
            s.notes = '';
            s.bookedAppointmentId = null;
        }
    }
})();
