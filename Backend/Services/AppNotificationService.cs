using System.Net;
using System.Text.Json;
using MedicalSystem.Data;
using MedicalSystem.Hubs;
using MedicalSystem.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebPush;

namespace MedicalSystem.Services
{
    // خدمة الإشعارات: تحفظ الإشعار داخل النظام ثم ترسل نسخة Push لكل أجهزة المستخدم
    public interface IAppNotificationService
    {
        Task SendInAppAndPushAsync(ApplicationDbContext context, int userId, string title, string? message,
            string type, string? relatedEntityType = null, int? relatedEntityID = null);
    }

    public class AppNotificationService : IAppNotificationService
    {
        private readonly WebPushClient _webPushClient;
        private readonly IConfiguration _config;
        private readonly ILogger<AppNotificationService> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public AppNotificationService(WebPushClient webPushClient, IConfiguration config,
            ILogger<AppNotificationService> logger, IHubContext<NotificationHub> hubContext)
        {
            _webPushClient = webPushClient;
            _config = config;
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task SendInAppAndPushAsync(ApplicationDbContext context, int userId, string title, string? message,
            string type, string? relatedEntityType = null, int? relatedEntityID = null)
        {
            var notification = new UserNotification
            {
                UserID = userId,
                Title = title,
                Message = message,
                Type = type,
                RelatedEntityType = relatedEntityType,
                RelatedEntityID = relatedEntityID,
                CreatedAt = DateTime.Now
            };

            context.UserNotifications.Add(notification);
            await context.SaveChangesAsync();

            // بث فوري لجميع متصفحات المستخدم المسجلة (SignalR) — يشغّل الرنين ونافذة المكالمة فوراً
            try
            {
                await _hubContext.Clients.User(userId.ToString()).SendAsync("notification-received", new
                {
                    notificationID = notification.NotificationID,
                    title = notification.Title,
                    message = notification.Message,
                    type = notification.Type,
                    relatedEntityType = notification.RelatedEntityType,
                    relatedEntityID = notification.RelatedEntityID,
                    createdAt = notification.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SignalR broadcast failed for user {userId}", userId);
            }

            await SendPushToUserAsync(context, notification);
        }

        private async Task SendPushToUserAsync(ApplicationDbContext context, UserNotification notification)
        {
            var publicKey = _config["PushNotifications:VapidPublicKey"];
            var privateKey = _config["PushNotifications:VapidPrivateKey"];
            var subject = _config["PushNotifications:VapidSubject"] ?? "mailto:admin@ivs-medical.local";

            // إشعارات الدفع غير مفعلة (لا توجد مفاتيح) — يبقى الإشعار داخل النظام فقط
            if (string.IsNullOrWhiteSpace(publicKey) || string.IsNullOrWhiteSpace(privateKey))
                return;

            var subscriptions = await context.WebPushSubscriptions
                .Where(s => s.UserID == notification.UserID && s.IsActive)
                .ToListAsync();

            if (subscriptions.Count == 0)
                return;

            var payload = JsonSerializer.Serialize(new
            {
                title = notification.Title,
                body = notification.Message,
                icon = "/assets/icons/icon-192.png",
                badge = "/assets/icons/icon-192.png",
                type = notification.Type,
                relatedEntityType = notification.RelatedEntityType,
                relatedEntityID = notification.RelatedEntityID,
                timestamp = notification.CreatedAt
            });

            var vapidDetails = new VapidDetails(subject, publicKey, privateKey);

            foreach (var sub in subscriptions)
            {
                try
                {
                    var pushSubscription = new PushSubscription(sub.Endpoint, sub.P256DH, sub.Auth);
                    await _webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
                    sub.LastUsedAt = DateTime.Now;
                }
                catch (WebPushException ex)
                {
                    // الاشتراك صار غير صالح (حُذف المتصفح/السماح) — ألغِ تفعيله
                    if (ex.StatusCode == HttpStatusCode.NotFound || ex.StatusCode == HttpStatusCode.Gone)
                    {
                        _logger.LogWarning("WebPush subscription removed (HTTP {code}): {endpoint}", ex.StatusCode, sub.Endpoint);
                        sub.IsActive = false;
                    }
                    else
                    {
                        _logger.LogWarning(ex, "WebPush failed ({code}) for {endpoint}", ex.StatusCode, sub.Endpoint);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Unexpected WebPush failure for {endpoint}", sub.Endpoint);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
