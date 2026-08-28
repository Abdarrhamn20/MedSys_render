(function() {
    'use strict';

    angular.module('medicalApp').factory('AppointmentService', AppointmentService);

    AppointmentService.$inject = ['$http'];

    function AppointmentService($http) {
        var API = '/api';

        return {
            // Appointments
            getAppointments: function(params) { return $http.get(API + '/appointments', { params: params }).then(r); },
            getAppointment: function(id) { return $http.get(API + '/appointments/' + id).then(r); },
            createAppointment: function(data) { return $http.post(API + '/appointments', data).then(r); },
            updateStatus: function(id, data) { return $http.put(API + '/appointments/' + id + '/status', data).then(r); },
            cancelAppointment: function(id) { return $http.delete(API + '/appointments/' + id).then(r); },
            cancelWithReason: function(id, reason) {
                return $http.put(API + '/appointments/' + id + '/status', { status: 'Cancelled', cancellationReason: reason }).then(r);
            },
            getAvailableSlots: function(doctorId, date) { return $http.get(API + '/appointments/available-slots', { params: { doctorId: doctorId, date: date } }).then(r); },
            getBookingPolicy: function() { return $http.get(API + '/appointments/policy').then(r); },

            // Triage
            getTriageQuestions: function() { return $http.get(API + '/triage/questions').then(r); },
            evaluateTriage: function(data) { return $http.post(API + '/triage/evaluate', data).then(r); },

            // Doctors (for booking)
            getDoctors: function(params) { return $http.get(API + '/doctors', { params: params }).then(r); },
            getDoctor: function(id) { return $http.get(API + '/doctors/' + id).then(r); },
            getSpecialties: function() { return $http.get(API + '/doctors/specialties').then(r); }
        };

        function r(response) { return response.data; }
    }
})();
