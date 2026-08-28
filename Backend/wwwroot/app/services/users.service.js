(function() {
    'use strict';

    angular.module('medicalApp').factory('UsersService', UsersService);

    UsersService.$inject = ['$http'];

    function UsersService($http) {
        var API = '/api';

        return {
            // Users
            getUsers: function(params) { return $http.get(API + '/users', { params: params }).then(r); },
            getUser: function(id) { return $http.get(API + '/users/' + id).then(r); },
            createUser: function(data) { return $http.post(API + '/users', data).then(r); },
            updateUser: function(id, data) { return $http.put(API + '/users/' + id, data).then(r); },
            toggleActive: function(id) { return $http.put(API + '/users/' + id + '/toggle-active').then(r); },
            deleteUser: function(id) { return $http.delete(API + '/users/' + id).then(r); },
            getUserStats: function() { return $http.get(API + '/users/stats').then(r); },

            // Doctors
            getDoctors: function(params) { return $http.get(API + '/doctors', { params: params }).then(r); },
            getDoctor: function(id) { return $http.get(API + '/doctors/' + id).then(r); },
            updateDoctor: function(id, data) { return $http.put(API + '/doctors/' + id, data).then(r); },
            getSpecialties: function() { return $http.get(API + '/doctors/specialties').then(r); },

            // Patients
            getPatients: function(params) { return $http.get(API + '/patients', { params: params }).then(r); },
            getPatient: function(id) { return $http.get(API + '/patients/' + id).then(r); },
            updatePatient: function(id, data) { return $http.put(API + '/patients/' + id, data).then(r); },
            getNextFileNumber: function() { return $http.get(API + '/patients/next-file-number').then(r); },
            mergePatients: function(data) { return $http.post(API + '/patients/merge', data).then(r); },

            // Dashboard
            getDashboardStats: function() { return $http.get(API + '/dashboard/stats').then(r); },
            getRecentAppointments: function() { return $http.get(API + '/dashboard/recent-appointments').then(r); }
        };

        function r(response) { return response.data; }
    }
})();
