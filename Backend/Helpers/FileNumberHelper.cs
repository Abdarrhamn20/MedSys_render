using MedicalSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalSystem.Helpers
{
    public static class FileNumberHelper
    {
        /// <summary>
        /// توليد رقم ملف تسلسلي للنمط PT-YYYY-NNNN (مثل PT-2026-0001)
        /// </summary>
        public static async Task<string> GenerateNextAsync(ApplicationDbContext context)
        {
            var year = DateTime.Now.Year;
            var prefix = $"PT-{year}-";

            var count = await context.PatientProfiles
                .Where(p => p.FileNumber != null && p.FileNumber.StartsWith(prefix))
                .CountAsync();

            // ضمان التفرد عند الاستدعاء المتوازي: استمر بالعد حتى رقم غير موجود
            string number;
            var seq = count;
            do
            {
                seq++;
                number = $"{prefix}{seq:0000}";
            }
            while (await context.PatientProfiles.AnyAsync(p => p.FileNumber == number));

            return number;
        }
    }
}
