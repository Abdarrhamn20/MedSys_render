(function() {
    'use strict';

    angular.module('medicalApp').controller('TelemedicineController', TelemedicineController);

    TelemedicineController.$inject = ['$scope', '$state', '$stateParams', '$timeout', 'AuthService', 'TelemedicineService', 'toastr'];

    function TelemedicineController($scope, $state, $stateParams, $timeout, AuthService, TelemedicineService, toastr) {
        var vm = this;
        vm.appointmentId = $stateParams.appointmentId;
        vm.role = AuthService.getUserRole();

        vm.status = 'loading'; // loading | waiting | ringing | connected | ended | error
        vm.localStream = null;
        vm.remoteStream = null;
        vm.remoteName = 'الطرف الآخر';
        vm.session = null;
        vm.roomCode = null;
        vm.chatMessages = [];
        vm.chatText = '';
        vm.endNotes = '';
        vm.chatOpen = false;

        var hubConnection = null;
        var peerConnection = null;
        var localVideoEl = null;
        var remoteVideoEl = null;
        var iceCandidatesBuffer = [];

        vm.endCall = endCall;
        vm.sendChat = sendChat;
        vm.toggleMute = toggleMute;
        vm.toggleVideo = toggleVideo;
        vm.toggleChat = toggleChat;
        vm.goBack = goBack;

        init();

        function init() {
            if (!vm.appointmentId) {
                vm.status = 'error';
                vm.errorMessage = 'رقم الموعد غير صالح';
                return;
            }
            localVideoEl = document.getElementById('localVideo');
            remoteVideoEl = document.getElementById('remoteVideo');
            loadSession();
        }

        function loadSession() {
            TelemedicineService.createOrGetSession({ appointmentID: vm.appointmentId })
                .then(function(resp) {
                    if (!resp.success) throw new Error(resp.message || 'تعذر تحميل الجلسة');
                    vm.session = resp.data;
                    vm.roomCode = resp.data.roomCode;
                    return getUserMedia();
                })
                .then(function() { return startSignalR(); })
                .then(function() {
                    vm.status = 'waiting';
                    return startSession();
                })
                .catch(function(err) {
                    console.error('Telemedicine init error:', err);
                    vm.status = 'error';
                    vm.errorMessage = (err && err.message) || 'تعذر بدء جلسة الفيديو';
                    toastr.error(vm.errorMessage);
                });
        }

        function getUserMedia() {
            if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
                return Promise.reject(new Error('متصفحك لا يدعم الفيديو المباشر'));
            }
            return navigator.mediaDevices.getUserMedia({ video: { width: 640, height: 480 }, audio: true })
                .then(function(stream) {
                    vm.localStream = stream;
                    localVideoEl = localVideoEl || document.getElementById('localVideo');
                    if (localVideoEl) {
                        localVideoEl.srcObject = stream;
                        localVideoEl.muted = true;
                    }
                });
        }

        function startSession() {
            if (vm.session && vm.session.status !== 'Active') {
                return TelemedicineService.startSession(vm.session.sessionID)
                    .then(function(resp) {
                        if (resp.success && resp.data && resp.data.sessionID) {
                            vm.session = resp.data;
                        }
                    })
                    .catch(function(err) { console.warn('start session:', err); });
            }
            return Promise.resolve();
        }

        function startSignalR() {
            hubConnection = new signalR.HubConnectionBuilder()
                .withUrl('/hubs/telemedicine', {
                    accessTokenFactory: function() { return AuthService.getToken(); }
                })
                .withAutomaticReconnect()
                .build();

            hubConnection.on('PeerJoined', function(connectionId, userName) {
                vm.remoteName = userName;
                vm.status = 'connected';
                $scope.$applyAsync();
                createOffer(connectionId);
            });

            hubConnection.on('PeerLeft', function(connectionId) {
                closePeerConnection();
                vm.status = 'waiting';
                vm.remoteName = 'الطرف الآخر';
                $scope.$applyAsync();
                toastr.info('غادر الطرف الآخر');
            });

            hubConnection.on('ReceiveOffer', function(offer, fromConnectionId) {
                vm.status = 'connected';
                $scope.$applyAsync();
                createAnswer(offer, fromConnectionId);
            });

            hubConnection.on('ReceiveAnswer', function(answer) {
                if (peerConnection) {
                    peerConnection.setRemoteDescription(new RTCSessionDescription(JSON.parse(answer)));
                }
            });

            hubConnection.on('ReceiveIceCandidate', function(candidateJson, fromConnectionId) {
                var candidate = JSON.parse(candidateJson);
                if (peerConnection && peerConnection.remoteDescription) {
                    peerConnection.addIceCandidate(candidate).catch(function(e) { console.warn('ice', e); });
                } else {
                    iceCandidatesBuffer.push(candidate);
                }
            });

            hubConnection.on('ReceiveChat', function(userName, message) {
                vm.chatMessages.push({ user: userName, text: message, mine: false });
                $scope.$applyAsync();
            });

            return hubConnection.start()
                .then(function() {
                    return hubConnection.invoke('JoinRoom', vm.roomCode);
                });
        }

        function createPeerConnection() {
            if (peerConnection) return peerConnection;
            var config = { iceServers: [{ urls: 'stun:stun.l.google.com:19302' }] };
            peerConnection = new RTCPeerConnection(config);
            peerConnection.onicecandidate = function(event) {
                if (event.candidate) {
                    hubConnection.invoke('SendIceCandidate', vm.roomCode, JSON.stringify(event.candidate), peerConnection.remotePeerId || '');
                }
            };
            peerConnection.ontrack = function(event) {
                vm.remoteStream = event.streams[0];
                remoteVideoEl = remoteVideoEl || document.getElementById('remoteVideo');
                if (remoteVideoEl) remoteVideoEl.srcObject = event.streams[0];
                $scope.$applyAsync();
            };
            vm.localStream.getTracks().forEach(function(track) {
                peerConnection.addTrack(track, vm.localStream);
            });
            peerConnection.remotePeerId = '';
            return peerConnection;
        }

        function createOffer(targetConnectionId) {
            createPeerConnection();
            peerConnection.remotePeerId = targetConnectionId;
            return peerConnection.createOffer()
                .then(function(offer) { return peerConnection.setLocalDescription(offer); })
                .then(function() {
                    hubConnection.invoke('SendOffer', vm.roomCode, JSON.stringify(peerConnection.localDescription), targetConnectionId);
                })
                .catch(function(e) { console.error('offer', e); });
        }

        function createAnswer(offerJson, fromConnectionId) {
            createPeerConnection();
            peerConnection.remotePeerId = fromConnectionId;
            return peerConnection.setRemoteDescription(new RTCSessionDescription(JSON.parse(offerJson)))
                .then(function() { return peerConnection.createAnswer(); })
                .then(function(answer) { return peerConnection.setLocalDescription(answer); })
                .then(function() {
                    hubConnection.invoke('SendAnswer', vm.roomCode, JSON.stringify(peerConnection.localDescription), fromConnectionId);
                })
                .then(function() {
                    iceCandidatesBuffer.forEach(function(c) { peerConnection.addIceCandidate(c).catch(function() {}); });
                    iceCandidatesBuffer = [];
                })
                .catch(function(e) { console.error('answer', e); });
        }

        function closePeerConnection() {
            if (peerConnection) {
                peerConnection.onicecandidate = null;
                peerConnection.ontrack = null;
                peerConnection.close();
                peerConnection = null;
            }
            vm.remoteStream = null;
        }

        function cleanupCall() {
            if (peerConnection) closePeerConnection();
            if (vm.localStream) {
                vm.localStream.getTracks().forEach(function(t) { t.stop(); });
                vm.localStream = null;
            }
            if (hubConnection && hubConnection.state === 'Connected') {
                hubConnection.invoke('LeaveRoom', vm.roomCode).catch(function() {});
                hubConnection.stop().catch(function() {});
            }
        }

        function endCall() {
            vm.status = 'ended';
            cleanupCall();
        }

        function goBack() {
            if (vm.status !== 'ended') {
                endCall();
            }
            // حفظ ملاحظات الطبيب عن المكالمة ثم العودة للجدول
            if (vm.session && vm.session.sessionID) {
                TelemedicineService.endSession(vm.session.sessionID, { notes: vm.endNotes })
                    .then(function(resp) {
                        if (resp.success && resp.data) vm.session = resp.data;
                    })
                    .catch(function() {});
            }
            $state.go('app.appointments');
        }

        function sendChat() {
            var text = (vm.chatText || '').trim();
            if (!text) return;
            hubConnection.invoke('SendChat', vm.roomCode, text).catch(function() {});
            vm.chatMessages.push({ user: 'أنا', text: text, mine: true });
            vm.chatText = '';
        }

        function toggleMute() {
            if (vm.localStream) {
                var audio = vm.localStream.getAudioTracks()[0];
                if (audio) { audio.enabled = !audio.enabled; vm.muted = !audio.enabled; }
            }
        }

        function toggleVideo() {
            if (vm.localStream) {
                var video = vm.localStream.getVideoTracks()[0];
                if (video) { video.enabled = !video.enabled; vm.videoOff = !video.enabled; }
            }
        }

        function toggleChat() { vm.chatOpen = !vm.chatOpen; }

        function goBack() {
            if (vm.status === 'connected' || vm.status === 'waiting' || vm.status === 'ringing') {
                endCall();
            }
            $state.go('app.appointments');
        }

        $scope.$on('$destroy', function() {
            if (hubConnection && hubConnection.state === 'Connected') {
                hubConnection.stop().catch(function() {});
            }
            if (vm.localStream) {
                vm.localStream.getTracks().forEach(function(t) { t.stop(); });
            }
        });
    }
})();
