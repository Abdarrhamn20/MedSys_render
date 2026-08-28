using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalSystem.Data;
using MedicalSystem.DTOs;
using MedicalSystem.Models;
using MedicalSystem.Helpers;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/accounting")]
    [Authorize(Roles = "Admin,Accountant,Cashier")]
    public class AccountingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AccountingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        //  شجرة الحسابات
        // ============================================================

        // GET: api/accounting/chart
        [HttpGet("chart")]
        public async Task<IActionResult> GetChart()
        {
            var accounts = await _context.ChartAccounts
                .OrderBy(a => a.AccountCode)
                .ToListAsync();

            var tree = accounts
                .Where(a => a.ParentAccountID == null)
                .OrderBy(a => a.AccountCode)
                .Select(a => BuildAccountNode(a, accounts))
                .ToList();

            return Ok(ApiResponse<object>.Ok(tree));
        }

        // GET: api/accounting/chart/flat
        [HttpGet("chart/flat")]
        public async Task<IActionResult> GetChartFlat()
        {
            var accounts = await _context.ChartAccounts
                .OrderBy(a => a.AccountCode)
                .Select(a => new
                {
                    a.AccountID,
                    a.AccountCode,
                    a.AccountName,
                    a.AccountNameAr,
                    a.AccountType,
                    a.ParentAccountID,
                    a.OpeningBalance,
                    a.IsActive
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(accounts));
        }

        // POST: api/accounting/chart
        [HttpPost("chart")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> CreateAccount([FromBody] ChartAccountDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.AccountCode) || string.IsNullOrWhiteSpace(dto.AccountNameAr))
                return BadRequest(ApiResponse.Fail("رقم الحساب واسمه بالعربية مطلوبان"));

            var codeExists = await _context.ChartAccounts.AnyAsync(a => a.AccountCode == dto.AccountCode);
            if (codeExists)
                return BadRequest(ApiResponse.Fail("رقم الحساب مستخدم مسبقاً"));

            if (dto.ParentAccountID.HasValue)
            {
                var parent = await _context.ChartAccounts.FindAsync(dto.ParentAccountID.Value);
                if (parent == null)
                    return BadRequest(ApiResponse.Fail("الحساب الأب غير موجود"));
                if (parent.AccountType != dto.AccountType)
                    return BadRequest(ApiResponse.Fail($"نوع الحساب الأب ({parent.AccountType}) لا يتطابق مع نوع الحساب الجديد ({dto.AccountType})"));
            }

            var account = new ChartAccount
            {
                AccountCode = dto.AccountCode.Trim(),
                AccountName = string.IsNullOrWhiteSpace(dto.AccountName) ? dto.AccountNameAr.Trim() : dto.AccountName.Trim(),
                AccountNameAr = dto.AccountNameAr.Trim(),
                AccountType = dto.AccountType,
                ParentAccountID = dto.ParentAccountID,
                OpeningBalance = dto.OpeningBalance,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now
            };

            _context.ChartAccounts.Add(account);

            var userId = JwtHelper.GetUserIdFromClaims(User);
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "AccountCreated",
                EntityType = "ChartAccount",
                EntityID = account.AccountID,
                UserID = userId,
                Details = $"إنشاء حساب جديد {account.AccountNameAr} برقم {account.AccountCode}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { account.AccountID }, "تم إنشاء الحساب بنجاح"));
        }

        // PUT: api/accounting/chart/5
        [HttpPut("chart/{id}")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] ChartAccountDTO dto)
        {
            var account = await _context.ChartAccounts.FindAsync(id);
            if (account == null)
                return NotFound(ApiResponse.Fail("الحساب غير موجود"));

            if (string.IsNullOrWhiteSpace(dto.AccountCode) || string.IsNullOrWhiteSpace(dto.AccountNameAr))
                return BadRequest(ApiResponse.Fail("رقم الحساب واسمه بالعربية مطلوبان"));

            var codeExists = await _context.ChartAccounts.AnyAsync(a => a.AccountCode == dto.AccountCode && a.AccountID != id);
            if (codeExists)
                return BadRequest(ApiResponse.Fail("رقم الحساب مستخدم مسبقاً"));

            if (dto.ParentAccountID.HasValue)
            {
                if (dto.ParentAccountID.Value == id)
                    return BadRequest(ApiResponse.Fail("لا يمكن أن يكون الحساب أباً لنفسه"));

                var parent = await _context.ChartAccounts.FindAsync(dto.ParentAccountID.Value);
                if (parent == null)
                    return BadRequest(ApiResponse.Fail("الحساب الأب غير موجود"));
                if (parent.AccountType != dto.AccountType)
                    return BadRequest(ApiResponse.Fail("نوع الحساب الأب لا يتطابق مع نوع الحساب الجديد"));
            }

            account.AccountCode = dto.AccountCode.Trim();
            account.AccountName = string.IsNullOrWhiteSpace(dto.AccountName) ? dto.AccountNameAr.Trim() : dto.AccountName.Trim();
            account.AccountNameAr = dto.AccountNameAr.Trim();
            account.AccountType = dto.AccountType;
            account.ParentAccountID = dto.ParentAccountID;
            account.OpeningBalance = dto.OpeningBalance;
            account.IsActive = dto.IsActive;

            var userId = JwtHelper.GetUserIdFromClaims(User);
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "AccountUpdated",
                EntityType = "ChartAccount",
                EntityID = account.AccountID,
                UserID = userId,
                Details = $"تعديل الحساب {account.AccountNameAr} برقم {account.AccountCode}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم تحديث الحساب بنجاح"));
        }

        // ============================================================
        //  القيود المحاسبية
        // ============================================================

        // GET: api/accounting/journal-entries
        [HttpGet("journal-entries")]
        public async Task<IActionResult> GetJournalEntries(
            [FromQuery] string? status,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.JournalEntries.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(e => e.Status == status);

            if (from.HasValue)
                query = query.Where(e => e.EntryDate >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(e => e.EntryDate < to.Value.Date.AddDays(1));

            var totalCount = await query.CountAsync();

            var entries = await query
                .OrderByDescending(e => e.EntryDate)
                .ThenByDescending(e => e.JournalEntryID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new
                {
                    e.JournalEntryID,
                    e.EntryNumber,
                    e.EntryDate,
                    e.Description,
                    e.SourceModule,
                    e.SourceReferenceID,
                    e.Status,
                    e.CreatedAt,
                    e.PostedAt,
                    LinesCount = e.Lines.Count,
                    TotalDebit = e.Lines.Sum(l => l.Debit),
                    TotalCredit = e.Lines.Sum(l => l.Credit),
                    CreatedByName = e.CreatedByUser != null ? e.CreatedByUser.FullName : null
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<object>
            {
                Data = entries.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/accounting/journal-entries/5
        [HttpGet("journal-entries/{id}")]
        public async Task<IActionResult> GetJournalEntry(int id)
        {
            var entry = await _context.JournalEntries
                .Include(e => e.Lines)
                    .ThenInclude(l => l.Account)
                .Include(e => e.CreatedByUser)
                .FirstOrDefaultAsync(e => e.JournalEntryID == id);

            if (entry == null)
                return NotFound(ApiResponse.Fail("القيد غير موجود"));

            var result = new
            {
                entry.JournalEntryID,
                entry.EntryNumber,
                entry.EntryDate,
                entry.Description,
                entry.SourceModule,
                entry.SourceReferenceID,
                entry.Status,
                entry.CreatedAt,
                entry.PostedAt,
                CreatedByName = entry.CreatedByUser?.FullName,
                TotalDebit = entry.Lines.Sum(l => l.Debit),
                TotalCredit = entry.Lines.Sum(l => l.Credit),
                Lines = entry.Lines
                    .OrderBy(l => l.JournalEntryLineID)
                    .Select(l => new
                    {
                        l.JournalEntryLineID,
                        l.AccountID,
                        AccountCode = l.Account.AccountCode,
                        AccountNameAr = l.Account.AccountNameAr,
                        l.Debit,
                        l.Credit,
                        l.Notes
                    })
                    .ToList()
            };

            return Ok(ApiResponse<object>.Ok(result));
        }

        // POST: api/accounting/journal-entries
        [HttpPost("journal-entries")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> CreateJournalEntry([FromBody] JournalEntryDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Description))
                return BadRequest(ApiResponse.Fail("بيان القيد مطلوب"));

            if (dto.Lines == null || dto.Lines.Count < 2)
                return BadRequest(ApiResponse.Fail("يجب أن يتكون القيد من سطرين على الأقل (مدين ودائن)"));

            var totalDebit = 0m;
            var totalCredit = 0m;

            foreach (var line in dto.Lines)
            {
                if (line.AccountID <= 0)
                    return BadRequest(ApiResponse.Fail("رقم حساب غير صالح في سطر القيد"));

                var account = await _context.ChartAccounts.FindAsync(line.AccountID);
                if (account == null)
                    return BadRequest(ApiResponse.Fail("أحد الحسابات غير موجود"));
                if (!account.IsActive)
                    return BadRequest(ApiResponse.Fail($"الحساب {account.AccountNameAr} غير مفعّل"));

                if (line.Debit < 0 || line.Credit < 0)
                    return BadRequest(ApiResponse.Fail("لا يجوز أن تكون قيمة المدين أو الدائن سالبة"));

                totalDebit += line.Debit;
                totalCredit += line.Credit;
            }

            if (totalDebit <= 0 && totalCredit <= 0)
                return BadRequest(ApiResponse.Fail("يجب إدخال مبلغ واحد على الأقل في القيد"));

            if (totalDebit != totalCredit)
                return BadRequest(ApiResponse.Fail($"القيد غير متوازن: مجموع المدين ({totalDebit:N2}) لا يساوي مجموع الدائن ({totalCredit:N2})"));

            var userId = JwtHelper.GetUserIdFromClaims(User);

            var entryDate = dto.EntryDate == default ? DateTime.Now : dto.EntryDate;
            var fiscalError = await JournalAutoHelper.ValidateFiscalDateAsync(_context, entryDate);
            if (fiscalError != null)
                return BadRequest(ApiResponse.Fail(fiscalError));

            var entry = new JournalEntry
            {
                EntryNumber = await GenerateEntryNumberAsync(),
                EntryDate = entryDate,
                Description = dto.Description.Trim(),
                SourceModule = string.IsNullOrWhiteSpace(dto.SourceModule) ? "Manual" : dto.SourceModule,
                SourceReferenceID = dto.SourceReferenceID,
                Status = "Draft",
                CreatedByUserID = userId,
                CreatedAt = DateTime.Now
            };

            foreach (var line in dto.Lines)
            {
                entry.Lines.Add(new JournalEntryLine
                {
                    AccountID = line.AccountID,
                    Debit = line.Debit,
                    Credit = line.Credit,
                    Notes = line.Notes
                });
            }

            _context.JournalEntries.Add(entry);

            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "JournalEntryCreated",
                EntityType = "JournalEntry",
                EntityID = entry.JournalEntryID,
                UserID = userId,
                Details = $"إنشاء قيد {entry.EntryNumber} بمبلغ {totalDebit:N2} د.ل — {entry.Description}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { entry.JournalEntryID, entry.EntryNumber }, "تم إنشاء القيد بنجاح (بالحالة مسودة)"));
        }

        // POST: api/accounting/journal-entries/5/post
        [HttpPost("journal-entries/{id}/post")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> PostJournalEntry(int id)
        {
            var entry = await _context.JournalEntries
                .Include(e => e.Lines)
                .FirstOrDefaultAsync(e => e.JournalEntryID == id);

            if (entry == null)
                return NotFound(ApiResponse.Fail("القيد غير موجود"));

            if (entry.Status == "Posted")
                return BadRequest(ApiResponse.Fail("القيد مرحّل بالفعل"));

            if (entry.Status == "Reversed")
                return BadRequest(ApiResponse.Fail("لا يمكن ترحيل قيد عكسي"));

            var totalDebit = entry.Lines.Sum(l => l.Debit);
            var totalCredit = entry.Lines.Sum(l => l.Credit);
            if (totalDebit != totalCredit)
                return BadRequest(ApiResponse.Fail("لا يمكن ترحيل قيد غير متوازن"));

            var fiscalError = await JournalAutoHelper.ValidateFiscalDateAsync(_context, entry.EntryDate);
            if (fiscalError != null)
                return BadRequest(ApiResponse.Fail(fiscalError));

            var userId = JwtHelper.GetUserIdFromClaims(User);
            entry.Status = "Posted";
            entry.PostedAt = DateTime.Now;
            entry.PostedByUserID = userId;

            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "JournalEntryPosted",
                EntityType = "JournalEntry",
                EntityID = entry.JournalEntryID,
                UserID = userId,
                Details = $"ترحيل قيد {entry.EntryNumber} بقيمة {totalDebit:N2} د.ل",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم ترحيل القيد {entry.EntryNumber} بنجاح"));
        }

        // POST: api/accounting/journal-entries/5/reverse
        [HttpPost("journal-entries/{id}/reverse")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> ReverseJournalEntry(int id)
        {
            var entry = await _context.JournalEntries
                .Include(e => e.Lines)
                .FirstOrDefaultAsync(e => e.JournalEntryID == id);

            if (entry == null)
                return NotFound(ApiResponse.Fail("القيد غير موجود"));

            if (entry.Status != "Posted")
                return BadRequest(ApiResponse.Fail("يُعكس فقط القيد المرحّل"));

            var userId = JwtHelper.GetUserIdFromClaims(User);

            var reverse = new JournalEntry
            {
                EntryNumber = await GenerateEntryNumberAsync(),
                EntryDate = DateTime.Now,
                Description = "عكس قيد " + entry.EntryNumber + " — " + entry.Description,
                SourceModule = "Manual",
                SourceReferenceID = entry.JournalEntryID,
                Status = "Draft",
                CreatedByUserID = userId,
                CreatedAt = DateTime.Now
            };

            foreach (var line in entry.Lines)
            {
                reverse.Lines.Add(new JournalEntryLine
                {
                    AccountID = line.AccountID,
                    Debit = line.Credit,
                    Credit = line.Debit,
                    Notes = "عكس: " + line.Notes
                });
            }

            _context.JournalEntries.Add(reverse);

            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "JournalEntryReversed",
                EntityType = "JournalEntry",
                EntityID = reverse.JournalEntryID,
                UserID = userId,
                Details = $"إنشاء قيد عكسي لقيد {entry.EntryNumber}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { reverse.JournalEntryID, reverse.EntryNumber }, "تم إنشاء القيد العكسي (بالحالة مسودة، يُرحَّل يدوياً)"));
        }

        // ============================================================
        //  كشف حساب + ميزان المراجعة + ملخص
        // ============================================================

        // GET: api/accounting/ledger/5?from=...&to=...
        [HttpGet("ledger/{accountId}")]
        public async Task<IActionResult> GetLedger(int accountId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var account = await _context.ChartAccounts.FindAsync(accountId);
            if (account == null)
                return NotFound(ApiResponse.Fail("الحساب غير موجود"));

            var fromDate = from ?? new DateTime(2000, 1, 1);
            var toDate = (to ?? DateTime.Today).AddDays(1);

            // رصيد سابق: القيود المرحّلة قبل بداية الفترة
            var openingDebit = await _context.JournalEntryLines
                .Where(l => l.AccountID == accountId && l.JournalEntry.Status == "Posted" && l.JournalEntry.EntryDate < fromDate)
                .SumAsync(l => (decimal?)l.Debit) ?? 0m;

            var openingCredit = await _context.JournalEntryLines
                .Where(l => l.AccountID == accountId && l.JournalEntry.Status == "Posted" && l.JournalEntry.EntryDate < fromDate)
                .SumAsync(l => (decimal?)l.Credit) ?? 0m;

            var openingBalance = ComputeBalance(account.AccountType, account.OpeningBalance, openingDebit, openingCredit);

            var lines = await _context.JournalEntryLines
                .Include(l => l.JournalEntry)
                .Where(l => l.AccountID == accountId &&
                            l.JournalEntry.Status == "Posted" &&
                            l.JournalEntry.EntryDate >= fromDate &&
                            l.JournalEntry.EntryDate < toDate)
                .OrderBy(l => l.JournalEntry.EntryDate)
                .ThenBy(l => l.JournalEntry.JournalEntryID)
                .ThenBy(l => l.JournalEntryLineID)
                .Select(l => new
                {
                    l.JournalEntryID,
                    l.JournalEntry.EntryNumber,
                    l.JournalEntry.EntryDate,
                    l.JournalEntry.Description,
                    l.Debit,
                    l.Credit,
                    l.Notes
                })
                .ToListAsync();

            var running = openingBalance;
            var ledgerLines = new List<object>();
            foreach (var line in lines)
            {
                running = IsDebitNormal(account.AccountType)
                    ? running + line.Debit - line.Credit
                    : running + line.Credit - line.Debit;

                ledgerLines.Add(new
                {
                    line.JournalEntryID,
                    line.EntryNumber,
                    line.EntryDate,
                    line.Description,
                    line.Debit,
                    line.Credit,
                    Balance = running
                });
            }

            var periodDebit = lines.Sum(l => l.Debit);
            var periodCredit = lines.Sum(l => l.Credit);

            return Ok(ApiResponse<object>.Ok(new
            {
                AccountID = account.AccountID,
                AccountCode = account.AccountCode,
                AccountNameAr = account.AccountNameAr,
                AccountType = account.AccountType,
                OpeningBalance = openingBalance,
                PeriodDebit = periodDebit,
                PeriodCredit = periodCredit,
                ClosingBalance = running,
                Lines = ledgerLines
            }));
        }

        // GET: api/accounting/trial-balance?asOf=...
        [HttpGet("trial-balance")]
        public async Task<IActionResult> GetTrialBalance([FromQuery] DateTime? asOf)
        {
            var query = _context.JournalEntryLines
                .Where(l => l.JournalEntry.Status == "Posted");

            if (asOf.HasValue)
            {
                var end = asOf.Value.Date.AddDays(1);
                query = query.Where(l => l.JournalEntry.EntryDate < end);
            }

            var rows = await query
                .GroupBy(l => new { l.AccountID, l.Account.AccountCode, l.Account.AccountNameAr, l.Account.AccountType, l.Account.OpeningBalance })
                .Select(g => new
                {
                    g.Key.AccountID,
                    g.Key.AccountCode,
                    g.Key.AccountNameAr,
                    g.Key.AccountType,
                    g.Key.OpeningBalance,
                    TotalDebit = g.Sum(l => l.Debit),
                    TotalCredit = g.Sum(l => l.Credit)
                })
                .ToListAsync();

            var result = rows
                .OrderBy(r => r.AccountCode)
                .Select(r => new
                {
                    r.AccountID,
                    r.AccountCode,
                    r.AccountNameAr,
                    r.AccountType,
                    r.OpeningBalance,
                    r.TotalDebit,
                    r.TotalCredit,
                    Balance = ComputeBalance(r.AccountType, r.OpeningBalance, r.TotalDebit, r.TotalCredit)
                })
                .ToList();

            var totals = new
            {
                TotalDebit = rows.Sum(r => r.TotalDebit),
                TotalCredit = rows.Sum(r => r.TotalCredit)
            };

            return Ok(ApiResponse<object>.Ok(new { Accounts = result, Totals = totals }));
        }

        // GET: api/accounting/summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var postedLines = _context.JournalEntryLines
                .Where(l => l.JournalEntry.Status == "Posted");

            // الإيرادات: من نوع Revenue (طبيعتها دائنة)
            var revenueCredit = await postedLines
                .Where(l => l.Account.AccountType == "Revenue")
                .SumAsync(l => (decimal?)l.Credit) ?? 0m;
            var revenueDebit = await postedLines
                .Where(l => l.Account.AccountType == "Revenue")
                .SumAsync(l => (decimal?)l.Debit) ?? 0m;

            // المصروفات: من نوع Expense (طبيعتها مدينة)
            var expenseDebit = await postedLines
                .Where(l => l.Account.AccountType == "Expense")
                .SumAsync(l => (decimal?)l.Debit) ?? 0m;
            var expenseCredit = await postedLines
                .Where(l => l.Account.AccountType == "Expense")
                .SumAsync(l => (decimal?)l.Credit) ?? 0m;

            // رصيد الصندوق (1010)
            var cash = await GetCashBalanceAsync();

            var draftCount = await _context.JournalEntries.CountAsync(e => e.Status == "Draft");

            return Ok(ApiResponse<object>.Ok(new
            {
                Revenue = revenueCredit - revenueDebit,
                Expenses = expenseDebit - expenseCredit,
                Net = (revenueCredit - revenueDebit) - (expenseDebit - expenseCredit),
                CashBalance = cash,
                DraftEntriesCount = draftCount
            }));
        }

        // ============================================================
        //  دوال مساعدة
        // ============================================================

        private async Task<string> GenerateEntryNumberAsync()
        {
            var year = DateTime.Now.Year;
            var count = await _context.JournalEntries.CountAsync(e => e.EntryDate.Year == year);
            return $"JE-{year}-{(count + 1):0000}";
        }

        private static bool IsDebitNormal(string accountType)
            => accountType is "Asset" or "Expense";

        private static decimal ComputeBalance(string accountType, decimal opening, decimal totalDebit, decimal totalCredit)
        {
            if (IsDebitNormal(accountType))
                return opening + totalDebit - totalCredit;
            return opening + totalCredit - totalDebit;
        }

        private async Task<decimal> GetCashBalanceAsync()
        {
            var cashAccount = await _context.ChartAccounts.FirstOrDefaultAsync(a => a.AccountCode == "1010");
            if (cashAccount == null)
                return 0m;

            var debit = await _context.JournalEntryLines
                .Where(l => l.AccountID == cashAccount.AccountID && l.JournalEntry.Status == "Posted")
                .SumAsync(l => (decimal?)l.Debit) ?? 0m;

            var credit = await _context.JournalEntryLines
                .Where(l => l.AccountID == cashAccount.AccountID && l.JournalEntry.Status == "Posted")
                .SumAsync(l => (decimal?)l.Credit) ?? 0m;

            return ComputeBalance(cashAccount.AccountType, cashAccount.OpeningBalance, debit, credit);
        }

        private static object BuildAccountNode(ChartAccount account, List<ChartAccount> all)
        {
            var children = all
                .Where(a => a.ParentAccountID == account.AccountID)
                .OrderBy(a => a.AccountCode)
                .Select(a => BuildAccountNode(a, all))
                .ToList();

            return new
            {
                account.AccountID,
                account.AccountCode,
                account.AccountName,
                account.AccountNameAr,
                account.AccountType,
                account.OpeningBalance,
                account.IsActive,
                Children = children
            };
        }
    }
}
