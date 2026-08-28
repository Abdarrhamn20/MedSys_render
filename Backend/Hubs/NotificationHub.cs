using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MedicalSystem.Hubs
{
    // ناقل فوري للإشعارات داخل النظام — يبث حدثاً لكل متصفحات المستخدم المسجلين
    // (الخادم → العميل فقط؛ يمرر الرسائل عبر Clients.User(userId))
    [Authorize]
    public class NotificationHub : Hub
    {
    }
}
