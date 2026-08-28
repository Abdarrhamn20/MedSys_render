(function() {
    'use strict';

    angular.module('medicalApp').factory('RealtimeService', RealtimeService);

    RealtimeService.$inject = ['AuthService', '$rootScope'];

    function RealtimeService(AuthService, $rootScope) {
        var connection = null;
        var started = false;

        var service = {
            start: start,
            stop: stop,
            isConnected: function() { return !!(connection && connection.state === 'Connected'); }
        };

        return service;

        function start() {
            if (!window.signalR) return;
            if (started) return;
            started = true;

            connection = new signalR.HubConnectionBuilder()
                .withUrl('/hubs/notifications', {
                    accessTokenFactory: function() { return AuthService.getToken(); }
                })
                .withAutomaticReconnect()
                .build();

            // الحدث العام: إعادة توجيهه لكل منصّت في التطبيق مع حالة الاتصال
            connection.on('notification-received', function(notification) {
                $rootScope.$broadcast('realtime:notification', notification);
            });

            connection.onreconnecting(function() {
                $rootScope.$broadcast('realtime:status', 'reconnecting');
            });

            connection.onreconnected(function() {
                $rootScope.$broadcast('realtime:status', 'connected');
            });

            connection.onclose(function() {
                $rootScope.$broadcast('realtime:status', 'disconnected');
            });

            connection.start().catch(function(err) {
                console.warn('Realtime notifications failed:', err);
            });
        }

        function stop() {
            if (connection) {
                connection.stop().catch(function() {});
                connection = null;
            }
            started = false;
        }
    }
})();
