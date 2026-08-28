using Microsoft.EntityFrameworkCore;
using MedicalSystem.Data;

namespace MedicalSystem.Helpers
{
    public interface ISettingsService
    {
        Task<decimal> GetDecimalAsync(string key, decimal defaultValue);
        Task<int> GetIntAsync(string key, int defaultValue);
        Task<bool> GetBoolAsync(string key, bool defaultValue);
    }

    public class SettingsService : ISettingsService
    {
        private readonly ApplicationDbContext _context;

        public SettingsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetDecimalAsync(string key, decimal defaultValue)
        {
            var setting = await _context.SystemSettings.AsNoTracking().FirstOrDefaultAsync(s => s.SettingKey == key);
            return decimal.TryParse(setting?.SettingValue, out var value) ? value : defaultValue;
        }

        public async Task<int> GetIntAsync(string key, int defaultValue)
        {
            var setting = await _context.SystemSettings.AsNoTracking().FirstOrDefaultAsync(s => s.SettingKey == key);
            return int.TryParse(setting?.SettingValue, out var value) ? value : defaultValue;
        }

        public async Task<bool> GetBoolAsync(string key, bool defaultValue)
        {
            var setting = await _context.SystemSettings.AsNoTracking().FirstOrDefaultAsync(s => s.SettingKey == key);
            if (setting == null) return defaultValue;
            return bool.TryParse(setting.SettingValue, out var value) ? value : defaultValue;
        }
    }
}
