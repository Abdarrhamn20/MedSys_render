using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalSystem.Data;
using MedicalSystem.DTOs;
using MedicalSystem.Models;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public SettingsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("mobile-pwa")]
        public async Task<IActionResult> GetMobilePWASetting()
        {
            // Master developer switch from appsettings.json
            var configValue = _configuration.GetValue<bool?>("Licensing:EnableMobilePWA");
            
            bool isEnabled;
            if (configValue.HasValue)
            {
                isEnabled = configValue.Value;
            }
            else
            {
                var setting = await _context.SystemSettings
                    .FirstOrDefaultAsync(s => s.SettingKey == "EnableMobilePWA");
                isEnabled = setting != null && setting.SettingValue.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            return Ok(ApiResponse<object>.Ok(new { enabled = isEnabled }, "حالة ترخيص ميزة تطبيق الموبايل PWA"));
        }

        // POST: api/settings/mobile-pwa (Admin only)
        [HttpPost("mobile-pwa")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMobilePWASetting([FromBody] MobilePWASettingDTO dto)
        {
            var setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.SettingKey == "EnableMobilePWA");

            if (setting == null)
            {
                setting = new SystemSetting
                {
                    SettingKey = "EnableMobilePWA",
                    SettingValue = dto.Enabled ? "true" : "false",
                    UpdatedAt = DateTime.Now
                };
                _context.SystemSettings.Add(setting);
            }
            else
            {
                setting.SettingValue = dto.Enabled ? "true" : "false";
                setting.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            var msg = dto.Enabled
                ? "تم تفعيل تطبيق الموبايل للعيادة"
                : "تم تعطيل تطبيق الموبايل للعيادة (يعمل ويب فقط)";

            return Ok(ApiResponse<object>.Ok(new { enabled = dto.Enabled }, msg));
        }

        // Developer-only secret activation endpoint (لا يمكن لمدير العيادة استدعاءه بدون المفتاح السري للمبرمج)
        [HttpPost("developer/license")]
        public async Task<IActionResult> UpdateDeveloperLicense([FromBody] DeveloperLicenseDTO dto, [FromHeader(Name = "X-Developer-Key")] string? developerKey)
        {
            // يُقرأ المفتاح من الإعدادات فقط (يُفضّل ضبطه عبر متغير بيئة LICENSING__DEVELOPERSECRETKEY) — لا يُصرّح بقيمة افتراضية في الكود
            var configSecretKey = _configuration.GetValue<string>("Licensing:DeveloperSecretKey");
            if (string.IsNullOrEmpty(configSecretKey))
                return StatusCode(500, ApiResponse.Fail("مفتاح ترخيص المبرمج غير مُعرّف في الإعدادات."));

            if (string.IsNullOrEmpty(developerKey) || developerKey != configSecretKey)
            {
                if (dto.SecretKey != configSecretKey)
                {
                    return Unauthorized(ApiResponse.Fail("غير مصرح! مفتاح ترخيص المبرمج السري غير صحيح."));
                }
            }

            var setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.SettingKey == "EnableMobilePWA");

            if (setting == null)
            {
                setting = new SystemSetting
                {
                    SettingKey = "EnableMobilePWA",
                    SettingValue = dto.EnableMobilePWA ? "true" : "false",
                    UpdatedAt = DateTime.Now
                };
                _context.SystemSettings.Add(setting);
            }
            else
            {
                setting.SettingValue = dto.EnableMobilePWA ? "true" : "false";
                setting.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            var msg = dto.EnableMobilePWA 
                ? "تم تفعيل ترخيص تطبيق الموبايل بنجاح للعيادة" 
                : "تم تعطيل ترخيص تطبيق الموبايل للعيادة (يعمل ويب فقط)";

            return Ok(ApiResponse<object>.Ok(new { enabled = dto.EnableMobilePWA }, msg));
        }

        // GET: api/settings/facility-mode
        [HttpGet("facility-mode")]
        public async Task<IActionResult> GetFacilityMode()
        {
            var setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.SettingKey == "FacilityMode");

            var mode = setting != null ? setting.SettingValue : "General";
            return Ok(ApiResponse<object>.Ok(new { facilityMode = mode }, "نمط تشغيل المنظومة الحالية"));
        }

        // POST: api/settings/facility-mode (Admin only)
        [HttpPost("facility-mode")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFacilityMode([FromBody] FacilityModeDTO dto)
        {
            var allowedModes = new[] { "General", "Psychiatric", "Hybrid" };
            var mode = string.IsNullOrEmpty(dto.FacilityMode) ? "General" : dto.FacilityMode;

            if (!allowedModes.Contains(mode))
                return BadRequest(ApiResponse.Fail("نمط التشغيل غير صالح. القيم المسموحة: General, Psychiatric, Hybrid."));

            var setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.SettingKey == "FacilityMode");

            if (setting == null)
            {
                setting = new SystemSetting
                {
                    SettingKey = "FacilityMode",
                    SettingValue = mode,
                    UpdatedAt = DateTime.Now
                };
                _context.SystemSettings.Add(setting);
            }
            else
            {
                setting.SettingValue = mode;
                setting.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok(new { facilityMode = mode }, "تم تحديث نمط تشغيل المنظومة بنجاح"));
        }
    }

    public class DeveloperLicenseDTO
    {
        public bool EnableMobilePWA { get; set; }
        public string? SecretKey { get; set; }
    }

    public class MobilePWASettingDTO
    {
        public bool Enabled { get; set; }
    }
}
