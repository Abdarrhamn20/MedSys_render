using MedicalSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalSystem.Services
{
    // خدمة خلفية: ترسل إشعاراً للمريض عندما يحين موعد الجلسة المقررة
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NotificationBackgroundService> _logger;

        public NotificationBackgroundService(IServiceScopeFactory scopeFactory, ILogger<NotificationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationBackgroundService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await NotifyAppointmentTimeReachedAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in NotificationBackgroundService");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private async Task NotifyAppointmentTimeReachedAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<IAppNotificationService>();

            var now = DateTime.Now;
            var today = DateTime.Today;

            // المواعيد المؤكدة لليوم (لم تبدأ بعد) — تُذكّر المريض عند حلول الوقت
            var dueAppointments = await context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Where(a => a.AppointmentDate == today
                    && a.Status == "Confirmed")
                .ToListAsync(ct);

            foreach (var appointment in dueAppointments)
            {
                if (appointment.Patient?.UserID == null)
                    continue;

                var start = today.Add(appointment.AppointmentTime);
                // أرسل التنبيه عند حلول الوقت وخلال 15 دقيقة بعده (لتغطية تأخر الدخول)
                if (start > now || start < now.AddMinutes(-15))
                    continue;

                var alreadyNotified = await context.UserNotifications.AnyAsync(n =>
                    n.UserID == appointment.Patient.UserID
                    && n.Type == "AppointmentTimeReached"
                    && n.RelatedEntityType == "Appointment"
                    && n.RelatedEntityID == appointment.AppID, ct);

                if (alreadyNotified)
                    continue;

                var doctorName = appointment.Doctor?.User?.FullName ?? "الطبيب";
                await notificationService.SendInAppAndPushAsync(
                    context,
                    appointment.Patient.UserID,
                    "حان موعد جلستك ⏰",
                    $"حان موعد جلستك مع د. {doctorName} في {DateTime.Today.Add(appointment.AppointmentTime):hh:mm tt}. يرجى الاستعداد، سيبدأ الطبيب مكالمة الفيديو قريباً.",
                    "AppointmentTimeReached",
                    "Appointment",
                    appointment.AppID);
            }
        }
    }
}
