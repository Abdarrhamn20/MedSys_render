(function() {
    'use strict';

    angular.module('medicalApp').factory('EmployeesService', EmployeesService);

    EmployeesService.$inject = ['$http'];

    function EmployeesService($http) {
        var API = '/api/employees';

        return {
            // Employees
            getEmployees: function(params) { return $http.get(API, { params: params }).then(r); },
            getEmployee: function(id) { return $http.get(API + '/' + id).then(r); },
            createEmployee: function(data) { return $http.post(API, data).then(r); },
            updateEmployee: function(id, data) { return $http.put(API + '/' + id, data).then(r); },
            toggleActive: function(id) { return $http.put(API + '/' + id + '/toggle-active').then(r); },
            getDepartments: function() { return $http.get(API + '/departments').then(r); },
            getLinkableUsers: function(role) { return $http.get(API + '/linkable-users', { params: role ? { role: role } : {} }).then(r); },

            // Courses
            getCourses: function(id) { return $http.get(API + '/' + id + '/courses').then(r); },
            addCourse: function(id, data) { return $http.post(API + '/' + id + '/courses', data).then(r); },
            deleteCourse: function(courseId) { return $http.delete(API + '/courses/' + courseId).then(r); },

            // Leaves
            getLeaves: function(params) { return $http.get(API + '/leaves', { params: params }).then(r); },
            getEmployeeLeaves: function(id) { return $http.get(API + '/' + id + '/leaves').then(r); },
            addLeave: function(id, data) { return $http.post(API + '/' + id + '/leaves', data).then(r); },
            updateLeaveStatus: function(leaveId, status) { return $http.put(API + '/leaves/' + leaveId + '/status', { status: status }).then(r); },

            // Payroll
            runPayroll: function(data) { return $http.post(API + '/payroll/run', data).then(r); },
            getPayroll: function(params) { return $http.get(API + '/payroll', { params: params }).then(r); },
            getPayrollSummary: function(params) { return $http.get(API + '/payroll/summary', { params: params }).then(r); },
            postPayroll: function(id) { return $http.post(API + '/payroll/' + id + '/post').then(r); },
            reversePayroll: function(id) { return $http.post(API + '/payroll/' + id + '/reverse').then(r); },
            deletePayrollDraft: function(id) { return $http.delete(API + '/payroll/' + id).then(r); },
            adjustPayroll: function(id, data) { return $http.put(API + '/payroll/' + id + '/adjust', data).then(r); },

            // Self-service
            getMyEmployee: function() { return $http.get(API + '/me', { skipErrorToast: true }).then(r); },
            requestMyLeave: function(data) { return $http.post(API + '/me/leaves', data).then(r); }
        };

        function r(response) { return response.data; }
    }
})();
