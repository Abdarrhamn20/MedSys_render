(function() {
    'use strict';

    angular.module('medicalApp').factory('WarehouseService', WarehouseService);

    WarehouseService.$inject = ['$http'];

    function WarehouseService($http) {
        var API = '/api/warehouse';

        return {
            getWarehouses: function() {
                return $http.get(API).then(r);
            },
            createWarehouse: function(data) {
                return $http.post(API, data).then(r);
            },
            updateWarehouse: function(id, data) {
                return $http.put(API + '/' + id, data).then(r);
            },
            deleteWarehouse: function(id) {
                return $http.delete(API + '/' + id).then(r);
            },

            getCategories: function() {
                return $http.get(API + '/categories').then(r);
            },
            getCategoriesFlat: function() {
                return $http.get(API + '/categories/flat').then(r);
            },
            createCategory: function(data) {
                return $http.post(API + '/categories', data).then(r);
            },
            updateCategory: function(id, data) {
                return $http.put(API + '/categories/' + id, data).then(r);
            },

            getItems: function(params) {
                return $http.get(API + '/items', { params: params }).then(r);
            },
            getItem: function(id) {
                return $http.get(API + '/items/' + id).then(r);
            },
            createItem: function(data) {
                return $http.post(API + '/items', data).then(r);
            },
            updateItem: function(id, data) {
                return $http.put(API + '/items/' + id, data).then(r);
            },

            getMovements: function(params) {
                return $http.get(API + '/movements', { params: params }).then(r);
            },
            getMovement: function(id) {
                return $http.get(API + '/movements/' + id).then(r);
            },
            createMovement: function(data) {
                return $http.post(API + '/movements', data).then(r);
            },
            postMovement: function(id) {
                return $http.post(API + '/movements/' + id + '/post', {}).then(r);
            },
            reverseMovement: function(id) {
                return $http.post(API + '/movements/' + id + '/reverse', {}).then(r);
            },

            getStock: function(params) {
                return $http.get(API + '/stock', { params: params }).then(r);
            },
            getLowStock: function() {
                return $http.get(API + '/low-stock').then(r);
            },

            getCounts: function(params) {
                return $http.get(API + '/counts', { params: params }).then(r);
            },
            getCount: function(id) {
                return $http.get(API + '/counts/' + id).then(r);
            },
            createCount: function(data) {
                return $http.post(API + '/counts', data).then(r);
            },
            updateCount: function(id, data) {
                return $http.put(API + '/counts/' + id, data).then(r);
            },
            postCount: function(id) {
                return $http.post(API + '/counts/' + id + '/post', {}).then(r);
            },
            reverseCount: function(id) {
                return $http.post(API + '/counts/' + id + '/reverse', {}).then(r);
            },

            exportStock: function(params) {
                return exportFile(API + '/export/stock', params);
            },
            exportMovements: function(params) {
                return exportFile(API + '/export/movements', params);
            },
            exportItems: function(params) {
                return exportFile(API + '/export/items', params);
            },
            exportCounts: function(params) {
                return exportFile(API + '/export/counts', params);
            }
        };

        function r(response) {
            return response.data;
        }

        function exportFile(url, params) {
            var q = [];
            if (params) {
                for (var key in params) {
                    var value = params[key];
                    if (value !== null && value !== undefined && value !== '') {
                        q.push(encodeURIComponent(key) + '=' + encodeURIComponent(value));
                    }
                }
            }
            var sep = q.length > 0 ? '?' : '';
            return $http.get(url + sep + q.join('&'), { responseType: 'arraybuffer' }).then(function(response) {
                var disposition = response.headers('Content-Disposition') || '';
                var filename = 'export.xlsx';
                var match = disposition.match(/filename="?([^";]+)"?/);
                if (match) filename = match[1];
                var blob = new Blob([response.data], { type: response.headers('Content-Type') || 'application/octet-stream' });
                var link = document.createElement('a');
                link.href = URL.createObjectURL(blob);
                link.download = filename;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                setTimeout(function() { URL.revokeObjectURL(link.href); }, 1000);
                return { success: true };
            });
        }
    }
})();
