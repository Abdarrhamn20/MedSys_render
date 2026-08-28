using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MedicalSystem.Hubs
{
    // ناقل إشارات WebRTC — يمرر Offer/Answer/ICE بين المشاركين في نفس الغرفة
    [Authorize]
    public class TelemedicineHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "مستخدم";
            await Clients.Caller.SendAsync("Connected", userName);
            await base.OnConnectedAsync();
        }

        public async Task JoinRoom(string roomCode)
        {
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "مستخدم";
            var connectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(connectionId, roomCode);
            await Clients.OthersInGroup(roomCode).SendAsync("PeerJoined", connectionId, userName);
        }

        public async Task LeaveRoom(string roomCode)
        {
            await Clients.OthersInGroup(roomCode).SendAsync("PeerLeft", Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);
        }

        public async Task SendOffer(string roomCode, string offer, string targetConnectionId)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveOffer", offer, Context.ConnectionId);
        }

        public async Task SendAnswer(string roomCode, string answer, string targetConnectionId)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveAnswer", answer, Context.ConnectionId);
        }

        public async Task SendIceCandidate(string roomCode, string candidate, string targetConnectionId)
        {
            if (!string.IsNullOrEmpty(targetConnectionId))
                await Clients.Client(targetConnectionId).SendAsync("ReceiveIceCandidate", candidate, Context.ConnectionId);
            else
                await Clients.OthersInGroup(roomCode).SendAsync("ReceiveIceCandidate", candidate, Context.ConnectionId);
        }

        public async Task SendChat(string roomCode, string message)
        {
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "مستخدم";
            await Clients.OthersInGroup(roomCode).SendAsync("ReceiveChat", userName, message);
        }
    }
}
