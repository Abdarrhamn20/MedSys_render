(function() {
    'use strict';

    angular.module('medicalApp').factory('NotificationService', NotificationService);

    NotificationService.$inject = ['$http'];

    function NotificationService($http) {
        var API = '/api/notifications';

        return {
            getAll: function(page, pageSize) {
                return $http.get(API, { params: { page: page || 1, pageSize: pageSize || 10 } }).then(r);
            },
            getUnreadCount: function() {
                return $http.get(API + '/unread-count').then(r);
            },
            markRead: function(id) {
                return $http.post(API + '/' + id + '/read').then(r);
            },
            markAllRead: function() {
                return $http.post(API + '/read-all').then(r);
            }
        };

        function r(response) { return response.data; }
    }
})();
