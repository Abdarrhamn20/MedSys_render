(function() {
    'use strict';

    angular.module('medicalApp').controller('AuditLogsController', AuditLogsController);

    AuditLogsController.$inject = ['$http', 'toastr'];

    function AuditLogsController($http, toastr) {
        var vm = this;

        vm.logs = [];
        vm.actionTypes = [];
        vm.entityTypes = [];
        vm.filters = { actionType: '', entityType: '', search: '', from: null, to: null };
        vm.page = 1;
        vm.pageSize = 50;
        vm.totalCount = 0;
        vm.totalPages = 1;
        vm.loading = false;

        vm.loadLogs = loadLogs;
        vm.goPage = goPage;
        vm.resetFilters = resetFilters;

        activate();

        function activate() {
            loadLogs();
        }

        function loadLogs() {
            vm.loading = true;
            var params = { page: vm.page, pageSize: vm.pageSize };
            if (vm.filters.actionType) params.actionType = vm.filters.actionType;
            if (vm.filters.entityType) params.entityType = vm.filters.entityType;
            if (vm.filters.search) params.search = vm.filters.search;
            if (vm.filters.from) params.from = vm.filters.from;
            if (vm.filters.to) params.to = vm.filters.to;

            $http.get('/api/auditlogs', { params: params }).then(function(res) {
                var d = res.data;
                vm.logs = d.data || [];
                vm.totalCount = d.totalCount || 0;
                vm.totalPages = d.totalPages || 1;
                if (d.actionTypes && d.actionTypes.length) vm.actionTypes = d.actionTypes;
                if (d.entityTypes && d.entityTypes.length) vm.entityTypes = d.entityTypes;
            }).catch(function(err) {
                toastr.error(err.data && err.data.message ? err.data.message : 'تعذر تحميل سجل التدقيق');
            }).finally(function() {
                vm.loading = false;
            });
        }

        function goPage(p) {
            if (p < 1 || p > vm.totalPages) return;
            vm.page = p;
            loadLogs();
        }

        function resetFilters() {
            vm.filters = { actionType: '', entityType: '', search: '', from: null, to: null };
            vm.page = 1;
            loadLogs();
        }
    }
})();
