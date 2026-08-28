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
    public class PushController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public PushController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: api/push/vapid-public-key
        [HttpGet("vapid-public-key")]
        public IActionResult GetVapidPublicKey()
        {
            var publicKey = _config["PushNotifications:VapidPublicKey"];
            if (string.IsNullOrWhiteSpace(publicKey))
                return Ok(ApiResponse<object>.Ok(new { enabled = false, publicKey = "" }, "إشعارات الدفع غير مفعلة"));

            return Ok(ApiResponse<object>.Ok(new { enabled = true, publicKey }, "مفتاح VAPID"));
        }

        // POST: api/push/subscribe
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribePushDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Endpoint) || string.IsNullOrWhiteSpace(dto.P256DH) || string.IsNullOrWhiteSpace(dto.Auth))
                return BadRequest(ApiResponse.Fail("بيانات الاشتراك غير مكتملة"));

            var userId = JwtHelper.GetUserIdFromClaims(User);

            var existing = await _context.WebPushSubscriptions
                .FirstOrDefaultAsync(s => s.Endpoint == dto.Endpoint);

            if (existing != null)
            {
                existing.UserID = userId;
                existing.P256DH = dto.P256DH;
                existing.Auth = dto.Auth;
                existing.UserAgent = dto.UserAgent;
                existing.IsActive = true;
                existing.LastUsedAt = DateTime.Now;
            }
            else
            {
                _context.WebPushSubscriptions.Add(new WebPushSubscription
                {
                    UserID = userId,
                    Endpoint = dto.Endpoint,
                    P256DH = dto.P256DH,
                    Auth = dto.Auth,
                    UserAgent = dto.UserAgent,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    LastUsedAt = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم تسجيل اشتراك إشعارات الدفع بنجاح"));
        }

        // POST: api/push/unsubscribe
        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe([FromBody] UnsubscribePushDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Endpoint))
                return BadRequest(ApiResponse.Fail("Endpoint غير صالح"));

            var userId = JwtHelper.GetUserIdFromClaims(User);

            var sub = await _context.WebPushSubscriptions
                .FirstOrDefaultAsync(s => s.Endpoint == dto.Endpoint && s.UserID == userId);

            if (sub != null)
            {
                sub.IsActive = false;
                await _context.SaveChangesAsync();
            }

            return Ok(ApiResponse.Ok("تم إلغاء اشتراك إشعارات الدفع"));
        }
    }
}
