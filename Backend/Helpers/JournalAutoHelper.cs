using Microsoft.EntityFrameworkCore;
using MedicalSystem.Data;
using MedicalSystem.Models;

namespace MedicalSystem.Helpers
{
    public static class JournalAutoHelper
    {
        private const string FiscalClosedThroughKey = "FiscalClosedThrough";

        /// <summary>
        /// إنشاء قيد محاسبي مرحّل تلقائياً من سطر مدين/دائن، مع ترقيم تسلسلي.
        /// يُرجع null إذا كان القيد غير متوازن أو المبالغ صفرية.
        /// </summary>
        public static async Task<JournalEntry?> CreatePostedEntryAsync(
            ApplicationDbContext context,
            int createdByUserId,
            DateTime entryDate,
            string description,
            string sourceModule,
            int? sourceReferenceID,
            IReadOnlyList<(int AccountID, decimal Debit, decimal Credit, string? Notes)> lines)
        {
            var totalDebit = lines.Sum(l => l.Debit);
            var totalCredit = lines.Sum(l => l.Credit);

            if (totalDebit <= 0 || totalDebit != totalCredit)
                return null;

            var year = entryDate.Year;
            var count = await context.JournalEntries.CountAsync(e => e.EntryDate.Year == year);

            var entry = new JournalEntry
            {
                EntryNumber = $"JE-{year}-{(count + 1):0000}",
                EntryDate = entryDate,
                Description = description,
                SourceModule = sourceModule,
                SourceReferenceID = sourceReferenceID,
                Status = "Posted",
                CreatedByUserID = createdByUserId,
                CreatedAt = DateTime.Now,
                PostedAt = DateTime.Now,
                PostedByUserID = createdByUserId
            };

            foreach (var line in lines)
            {
                entry.Lines.Add(new JournalEntryLine
                {
                    AccountID = line.AccountID,
                    Debit = line.Debit,
                    Credit = line.Credit,
                    Notes = line.Notes
                });
            }

            context.JournalEntries.Add(entry);
            return entry;
        }

        /// <summary>تاريخ الإقفال المالي الحالي (بعد هذا التاريخ لا يُقبل ترحيل) — null يعني غير مقفل.</summary>
        public static async Task<DateTime?> GetFiscalClosedThroughAsync(ApplicationDbContext context)
        {
            var raw = await context.SystemSettings
                .Where(s => s.SettingKey == FiscalClosedThroughKey)
                .Select(s => s.SettingValue)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(raw))
                return null;

            return DateTime.TryParse(raw, out var date) ? date.Date : (DateTime?)null;
        }

        /// <summary>التحقق من أن التاريخ ضمن فترة عمل غير مقفلة. يُرجع رسالة الخطأ أو null عند القبول.</summary>
        public static async Task<string?> ValidateFiscalDateAsync(ApplicationDbContext context, DateTime date)
        {
            var closedThrough = await GetFiscalClosedThroughAsync(context);
            if (closedThrough.HasValue && date.Date <= closedThrough.Value)
                return $"لا يمكن الترحيل قبل/في تاريخ الإقفال المالي ({closedThrough.Value:yyyy-MM-dd})";
            return null;
        }

        /// <summary>حساب الإيراد المطابق لنوع الفاتورة، مع إرجاع null عند عدم العثور عليه.</summary>
        public static async Task<int?> GetRevenueAccountIdAsync(ApplicationDbContext context, string invoiceType)
        {
            var code = invoiceType switch
            {
                "Pharmacy" => "4020",
                "Inpatient" => "4050",
                _ => "4010"
            };
            return await context.ChartAccounts
                .Where(a => a.AccountCode == code)
                .Select(a => (int?)a.AccountID)
                .FirstOrDefaultAsync();
        }

        /// <summary>حساب الصندوق الرئيسي (1010).</summary>
        public static async Task<int?> GetCashAccountIdAsync(ApplicationDbContext context)
        {
            return await context.ChartAccounts
                .Where(a => a.AccountCode == "1010")
                .Select(a => (int?)a.AccountID)
                .FirstOrDefaultAsync();
        }

        /// <summary>حساب البنوك (1020).</summary>
        public static async Task<int?> GetBankAccountIdAsync(ApplicationDbContext context)
        {
            return await context.ChartAccounts
                .Where(a => a.AccountCode == "1020")
                .Select(a => (int?)a.AccountID)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// توليد رقم سند تسلسلي موحد (RC/PY/TR-السنة-تسلسل) لكل أنواع الإنشاء.
        /// </summary>
        public static async Task<string> GenerateVoucherNumberAsync(ApplicationDbContext context, string type)
        {
            var prefix = type switch
            {
                "Receipt" => "RC",
                "Payment" => "PY",
                _ => "TR"
            };

            var year = DateTime.Now.Year;
            var count = await context.Vouchers.CountAsync(v => v.VoucherType == type && v.VoucherDate.Year == year);
            return $"{prefix}-{year}-{(count + 1):0000}";
        }

        /// <summary>
        /// إنشاء سند قبض مرحّل مرتبط بفاتورة/حجز/مريض (تحصيل تلقائي) دون قيد محاسبي،
        /// لأن القيد يُنشأ عبر CreateInvoiceCollectionEntryAsync.
        /// </summary>
        public static async Task CreateInvoiceCollectionVoucherAsync(
            ApplicationDbContext context,
            int userId,
            Invoice invoice)
        {
            // خزينة التحصيل: البنك إن كانت البطاقة، وإلا الصندوق الرئيسي
            var treasury = invoice.PaymentMethod == "Card"
                ? await context.Treasuries.FirstOrDefaultAsync(t => t.Account.AccountCode == "1020")
                : await context.Treasuries.FirstOrDefaultAsync(t => t.Account.AccountCode == "1010");

            if (treasury == null)
                return;

            var revenueAccountId = await GetRevenueAccountIdAsync(context, invoice.InvoiceType);
            if (!revenueAccountId.HasValue)
                return;

            var voucher = new Voucher
            {
                VoucherNumber = await GenerateVoucherNumberAsync(context, "Receipt"),
                VoucherType = "Receipt",
                VoucherDate = invoice.PaidAt ?? DateTime.Now,
                TreasuryID = treasury.TreasuryID,
                AccountID = revenueAccountId.Value,
                PatientUserID = invoice.PatientUserID,
                InvoiceID = invoice.InvoiceID,
                AppointmentID = invoice.AppointmentID,
                Amount = invoice.TotalAmount,
                Description = $"تحصيل فاتورة #{invoice.InvoiceID} ({invoice.InvoiceType})",
                Status = "Posted",
                CreatedByUserID = userId,
                CreatedAt = DateTime.Now,
                PostedByUserID = userId,
                PostedAt = invoice.PaidAt ?? DateTime.Now
            };

            context.Vouchers.Add(voucher);
        }

        /// <summary>
        /// إنشاء قيد تلقائي لتحصيل فاتورة: مدين صندوق (نقداً) أو بنك (بطاقة) / دائن حساب الإيراد المطابق.
        /// </summary>
        public static async Task CreateInvoiceCollectionEntryAsync(
            ApplicationDbContext context,
            int userId,
            Invoice invoice)
        {
            int? cashOrBankAccountId = invoice.PaymentMethod == "Card"
                ? await GetBankAccountIdAsync(context)
                : await GetCashAccountIdAsync(context);
            var revenueAccountId = await GetRevenueAccountIdAsync(context, invoice.InvoiceType);

            if (!cashOrBankAccountId.HasValue || !revenueAccountId.HasValue)
                return;

            var entry = await CreatePostedEntryAsync(
                context,
                userId,
                invoice.PaidAt ?? DateTime.Now,
                $"تحصيل فاتورة #{invoice.InvoiceID} ({invoice.InvoiceType})",
                "Invoice",
                invoice.InvoiceID,
                new List<(int, decimal, decimal, string?)>
                {
                    (cashOrBankAccountId.Value, invoice.TotalAmount, 0m, null),
                    (revenueAccountId.Value, 0m, invoice.TotalAmount, null)
                });

            if (entry != null)
            {
                context.AuditLogs.Add(new AuditLog
                {
                    ActionType = "InvoiceJournalAuto",
                    EntityType = "JournalEntry",
                    EntityID = entry.JournalEntryID,
                    UserID = userId,
                    Details = $"قيد تلقائي لتحصيل فاتورة #{invoice.InvoiceID} بقيمة {invoice.TotalAmount:N2} د.ل",
                    Timestamp = DateTime.Now
                });
            }
        }
    }
}
