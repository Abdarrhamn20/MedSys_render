using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalSystem.Data;
using MedicalSystem.Models;
using MedicalSystem.Helpers;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Only Admin can view Audit Logs
    public class AuditLogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuditLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/auditlogs?page=&pageSize=&actionType=&entityType=&search=&from=&to=
        [HttpGet]
        public async Task<IActionResult> GetAuditLogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? actionType = null,
            [FromQuery] string? entityType = null,
            [FromQuery] string? search = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 200);

            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(actionType))
                query = query.Where(a => a.ActionType.Contains(actionType));

            if (!string.IsNullOrEmpty(entityType))
                query = query.Where(a => a.EntityType.Contains(entityType));

            if (!string.IsNullOrEmpty(search))
                query = query.Where(a =>
                    a.Details.Contains(search) ||
                    (a.User != null && a.User.FullName.Contains(search)));

            if (from.HasValue)
                query = query.Where(a => a.Timestamp >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(a => a.Timestamp < to.Value.Date.AddDays(1));

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var logs = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new
                {
                    a.LogID,
                    a.ActionType,
                    a.EntityType,
                    a.EntityID,
                    a.Details,
                    a.Timestamp,
                    User = a.User != null ? new { a.User.UserID, a.User.FullName, a.User.Role } : null
                })
                .ToListAsync();

            // قائمة أنواع العمليات المتاحة للفلترة (من السجلات الفعلية)
            var actionTypes = await _context.AuditLogs
                .Select(a => a.ActionType)
                .Distinct()
                .OrderBy(t => t)
                .Take(200)
                .ToListAsync();

            var entityTypes = await _context.AuditLogs
                .Select(a => a.EntityType)
                .Distinct()
                .OrderBy(t => t)
                .Take(200)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                data = logs,
                totalCount,
                totalPages,
                currentPage = page,
                actionTypes,
                entityTypes
            });
        }
    }
}
