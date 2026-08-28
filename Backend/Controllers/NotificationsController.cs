using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalSystem.Data;
using MedicalSystem.DTOs;
using MedicalSystem.Helpers;
using MedicalSystem.Models;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/notifications?page=1&pageSize=20
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);

            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var query = _context.UserNotifications
                .Where(n => n.UserID == userId);

            var totalCount = await query.CountAsync();

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationDTO
                {
                    NotificationID = n.NotificationID,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    RelatedEntityType = n.RelatedEntityType,
                    RelatedEntityID = n.RelatedEntityID,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<object>
            {
                Data = notifications.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/notifications/unread-count
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var count = await _context.UserNotifications.CountAsync(n => n.UserID == userId && !n.IsRead);
            return Ok(ApiResponse<object>.Ok(new { count }, "عدد الإشعارات غير المقروءة"));
        }

        // POST: api/notifications/{id}/read
        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var notification = await _context.UserNotifications
                .FirstOrDefaultAsync(n => n.NotificationID == id && n.UserID == userId);

            if (notification == null)
                return NotFound(ApiResponse.Fail("الإشعار غير موجود"));

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم تحديد الإشعار كمقروء"));
        }

        // POST: api/notifications/read-all
        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllRead()
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var unread = await _context.UserNotifications
                .Where(n => n.UserID == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in unread)
                n.IsRead = true;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم تحديد {unread.Count} إشعار كمقروء"));
        }
    }
}
