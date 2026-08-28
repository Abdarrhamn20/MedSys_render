(function() {
    'use strict';

    angular.module('medicalApp').factory('toastr', ToastService);

    function ToastService() {
        var container = document.getElementById('toastContainer');

        var service = {
            success: success,
            error: error,
            info: info,
            warning: warning
        };

        return service;

        function success(message, title) { showToast(message, title, 'success', 'fa-check-circle'); }
        function error(message, title) { showToast(message, title, 'error', 'fa-times-circle'); }
        function info(message, title) { showToast(message, title, 'info', 'fa-info-circle'); }
        function warning(message, title) { showToast(message, title, 'warning', 'fa-exclamation-triangle'); }

        function showToast(message, title, type, icon) {
            var toast = document.createElement('div');
            toast.className = 'toast-message ' + type;
            toast.innerHTML = '<i class="fas ' + icon + '"></i><span>' + (title ? '<strong>' + title + ':</strong> ' : '') + message + '</span>';
            container.appendChild(toast);

            setTimeout(function() {
                toast.classList.add('fade-out');
                setTimeout(function() {
                    if (toast.parentNode) toast.parentNode.removeChild(toast);
                }, 300);
            }, 3000);
        }
    }
})();
