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
    [Route("api/treasury")]
    [Authorize(Roles = "Admin,Accountant,Cashier")]
    public class TreasuryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TreasuryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        //  الخزائن
        // ============================================================

        // GET: api/treasury
        [HttpGet]
        public async Task<IActionResult> GetTreasuries()
        {
            var treasuries = await _context.Treasuries
                .OrderBy(t => t.TreasuryCode)
                .Select(t => new
                {
                    t.TreasuryID,
                    t.TreasuryName,
                    t.TreasuryNameAr,
                    t.TreasuryCode,
                    t.AccountID,
                    AccountCode = t.Account.AccountCode,
                    t.IsActive,
                    t.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(treasuries));
        }

        // POST: api/treasury
        [HttpPost]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> CreateTreasury([FromBody] TreasuryDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TreasuryNameAr) || string.IsNullOrWhiteSpace(dto.TreasuryCode))
                return BadRequest(ApiResponse.Fail("اسم الخزينة بالعربية والكود مطلوبان"));

            var codeExists = await _context.Treasuries.AnyAsync(t => t.TreasuryCode == dto.TreasuryCode);
            if (codeExists)
                return BadRequest(ApiResponse.Fail("كود الخزينة مستخدم مسبقاً"));

            var account = await _context.ChartAccounts.FindAsync(dto.AccountID);
            if (account == null)
                return BadRequest(ApiResponse.Fail("الحساب المحاسبي المرفق غير موجود"));

            var treasury = new Treasury
            {
                TreasuryName = string.IsNullOrWhiteSpace(dto.TreasuryName) ? dto.TreasuryNameAr.Trim() : dto.TreasuryName.Trim(),
                TreasuryNameAr = dto.TreasuryNameAr.Trim(),
                TreasuryCode = dto.TreasuryCode.Trim(),
                AccountID = dto.AccountID,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now
            };

            _context.Treasuries.Add(treasury);

            var userId = JwtHelper.GetUserIdFromClaims(User);
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "TreasuryCreated",
                EntityType = "Treasury",
                EntityID = treasury.TreasuryID,
                UserID = userId,
                Details = $"إنشاء خزينة {treasury.TreasuryNameAr} بكود {treasury.TreasuryCode}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { treasury.TreasuryID }, "تم إنشاء الخزينة بنجاح"));
        }

        // PUT: api/treasury/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> UpdateTreasury(int id, [FromBody] TreasuryDTO dto)
        {
            var treasury = await _context.Treasuries.FindAsync(id);
            if (treasury == null)
                return NotFound(ApiResponse.Fail("الخزينة غير موجودة"));

            var codeExists = await _context.Treasuries.AnyAsync(t => t.TreasuryCode == dto.TreasuryCode && t.TreasuryID != id);
            if (codeExists)
                return BadRequest(ApiResponse.Fail("كود الخزينة مستخدم مسبقاً"));

            var account = await _context.ChartAccounts.FindAsync(dto.AccountID);
            if (account == null)
                return BadRequest(ApiResponse.Fail("الحساب المحاسبي المرفق غير موجود"));

            treasury.TreasuryName = string.IsNullOrWhiteSpace(dto.TreasuryName) ? dto.TreasuryNameAr.Trim() : dto.TreasuryName.Trim();
            treasury.TreasuryNameAr = dto.TreasuryNameAr.Trim();
            treasury.TreasuryCode = dto.TreasuryCode.Trim();
            treasury.AccountID = dto.AccountID;
            treasury.IsActive = dto.IsActive;

            var userId = JwtHelper.GetUserIdFromClaims(User);
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "TreasuryUpdated",
                EntityType = "Treasury",
                EntityID = treasury.TreasuryID,
                UserID = userId,
                Details = $"تعديل خزينة {treasury.TreasuryNameAr}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم تحديث الخزينة بنجاح"));
        }

        // DELETE: api/treasury/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> DeleteTreasury(int id)
        {
            var treasury = await _context.Treasuries.FindAsync(id);
            if (treasury == null)
                return NotFound(ApiResponse.Fail("الخزينة غير موجودة"));

            var hasVouchers = await _context.Vouchers.AnyAsync(v => v.TreasuryID == id || v.ToTreasuryID == id);
            if (hasVouchers)
                return BadRequest(ApiResponse.Fail("لا يمكن حذف خزينة مرتبطة بسندات. عطّلها بدلاً من الحذف."));

            _context.Treasuries.Remove(treasury);

            var userId = JwtHelper.GetUserIdFromClaims(User);
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "TreasuryDeleted",
                EntityType = "Treasury",
                EntityID = id,
                UserID = userId,
                Details = $"حذف خزينة {treasury.TreasuryNameAr}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم حذف الخزينة بنجاح"));
        }

        // ============================================================
        //  السندات (قبض / صرف / تحويل)
        // ============================================================

        // GET: api/treasury/vouchers?type=...&status=...&from=...&to=...
        [HttpGet("vouchers")]
        public async Task<IActionResult> GetVouchers(
            [FromQuery] string? type,
            [FromQuery] string? status,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Vouchers.AsQueryable();

            // الكاشير يرى سندات خزينته المخصصة فقط
            var role = JwtHelper.GetUserRoleFromClaims(User);
            if (role == "Cashier")
            {
                var cashier = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserID == JwtHelper.GetUserIdFromClaims(User));
                if (cashier == null || !cashier.AssignedTreasuryID.HasValue)
                    return StatusCode(403, ApiResponse.Fail("لا توجد خزينة مخصصة لحساب الكاشير"));
                var myTreasury = cashier.AssignedTreasuryID.Value;
                query = query.Where(v => v.TreasuryID == myTreasury);
            }

            if (!string.IsNullOrEmpty(type))
                query = query.Where(v => v.VoucherType == type);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(v => v.Status == status);

            if (from.HasValue)
                query = query.Where(v => v.VoucherDate >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(v => v.VoucherDate < to.Value.Date.AddDays(1));

            var totalCount = await query.CountAsync();

            var vouchers = await query
                .OrderByDescending(v => v.VoucherDate)
                .ThenByDescending(v => v.VoucherID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new
                {
                    v.VoucherID,
                    v.VoucherNumber,
                    v.VoucherType,
                    v.VoucherDate,
                    v.TreasuryID,
                    TreasuryNameAr = v.Treasury.TreasuryNameAr,
                    ToTreasuryNameAr = v.ToTreasury != null ? v.ToTreasury.TreasuryNameAr : null,
                    v.AccountID,
                    AccountCode = v.Account != null ? v.Account.AccountCode : null,
                    v.PatientUserID,
                    PatientName = v.PatientUser != null ? v.PatientUser.FullName : null,
                    v.InvoiceID,
                    v.AppointmentID,
                    v.Amount,
                    v.Description,
                    v.Status,
                    v.CreatedAt,
                    v.PostedAt,
                    CreatedByName = v.CreatedByUser != null ? v.CreatedByUser.FullName : null
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<object>
            {
                Data = vouchers.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/treasury/vouchers/5
        [HttpGet("vouchers/{id}")]
        public async Task<IActionResult> GetVoucher(int id)
        {
            var voucher = await _context.Vouchers
                .Include(v => v.Treasury)
                .Include(v => v.ToTreasury)
                .Include(v => v.Account)
                .Include(v => v.PatientUser)
                .Include(v => v.CreatedByUser)
                .Include(v => v.PostedByUser)
                .FirstOrDefaultAsync(v => v.VoucherID == id);

            if (voucher == null)
                return NotFound(ApiResponse.Fail("السند غير موجود"));

            // الكاشير يرى تفاصيل سندات خزينته المخصصة فقط
            var role = JwtHelper.GetUserRoleFromClaims(User);
            if (role == "Cashier")
            {
                var cashier = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserID == JwtHelper.GetUserIdFromClaims(User));
                if (cashier == null || !cashier.AssignedTreasuryID.HasValue)
                    return StatusCode(403, ApiResponse.Fail("لا توجد خزينة مخصصة لحساب الكاشير"));
                if (voucher.TreasuryID != cashier.AssignedTreasuryID.Value)
                    return StatusCode(403, ApiResponse.Fail("يُسمح للكاشير بالاطلاع على سندات خزينته المخصصة فقط"));
            }

            var result = new
            {
                voucher.VoucherID,
                voucher.VoucherNumber,
                voucher.VoucherType,
                voucher.VoucherDate,
                voucher.TreasuryID,
                TreasuryNameAr = voucher.Treasury.TreasuryNameAr,
                voucher.ToTreasuryID,
                ToTreasuryNameAr = voucher.ToTreasury?.TreasuryNameAr,
                voucher.AccountID,
                AccountCode = voucher.Account?.AccountCode,
                AccountNameAr = voucher.Account?.AccountNameAr,
                voucher.PatientUserID,
                PatientName = voucher.PatientUser?.FullName,
                voucher.InvoiceID,
                voucher.AppointmentID,
                voucher.Amount,
                voucher.Description,
                voucher.Status,
                voucher.CreatedAt,
                voucher.PostedAt,
                CreatedByName = voucher.CreatedByUser?.FullName,
                PostedByName = voucher.PostedByUser?.FullName
            };

            return Ok(ApiResponse<object>.Ok(result));
        }

        // POST: api/treasury/vouchers
        [HttpPost("vouchers")]
        public async Task<IActionResult> CreateVoucher([FromBody] VoucherDTO dto)
        {
            if (dto.Amount <= 0)
                return BadRequest(ApiResponse.Fail("يجب أن يكون مبلغ السند أكبر من صفر"));

            var validTypes = new[] { "Receipt", "Payment", "Transfer" };
            if (!validTypes.Contains(dto.VoucherType))
                return BadRequest(ApiResponse.Fail("نوع السند غير صالح (Receipt/Payment/Transfer)"));

            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            // الكاشير يعمل على خزينته المخصصة فقط وبسندات قبض/صرف (لا تحويل)
            if (role == "Cashier")
            {
                if (dto.VoucherType == "Transfer")
                    return StatusCode(403, ApiResponse.Fail("لا يُسمح للكاشير بإنشاء سندات تحويل"));
                var cashier = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserID == userId);
                if (cashier == null || !cashier.AssignedTreasuryID.HasValue)
                    return StatusCode(403, ApiResponse.Fail("لا توجد خزينة مخصصة لحساب الكاشير"));
                if (dto.TreasuryID != cashier.AssignedTreasuryID.Value)
                    return StatusCode(403, ApiResponse.Fail("يُسمح للكاشير بالعمل على خزينته المخصصة فقط"));
            }

            var treasury = await _context.Treasuries.FindAsync(dto.TreasuryID);
            if (treasury == null || !treasury.IsActive)
                return BadRequest(ApiResponse.Fail("الخزينة غير موجودة أو غير مفعّلة"));

            if (dto.VoucherType == "Transfer")
            {
                if (!dto.ToTreasuryID.HasValue || dto.ToTreasuryID.Value == dto.TreasuryID)
                    return BadRequest(ApiResponse.Fail("حدد خزينة تحويل مختلفة عن خزينة المصدر"));
                var toTreasury = await _context.Treasuries.FindAsync(dto.ToTreasuryID.Value);
                if (toTreasury == null || !toTreasury.IsActive)
                    return BadRequest(ApiResponse.Fail("خزينة التحويل إليها غير موجودة أو غير مفعّلة"));
            }
            else
            {
                if (!dto.AccountID.HasValue)
                    return BadRequest(ApiResponse.Fail("حدد الحساب المقابل للسند"));
                var account = await _context.ChartAccounts.FindAsync(dto.AccountID.Value);
                if (account == null || !account.IsActive)
                    return BadRequest(ApiResponse.Fail("الحساب المقابل غير موجود أو غير مفعّل"));
            }

            var voucher = new Voucher
            {
                VoucherNumber = await JournalAutoHelper.GenerateVoucherNumberAsync(_context, dto.VoucherType),
                VoucherType = dto.VoucherType,
                VoucherDate = dto.VoucherDate == default ? DateTime.Now : dto.VoucherDate,
                TreasuryID = dto.TreasuryID,
                ToTreasuryID = dto.VoucherType == "Transfer" ? dto.ToTreasuryID : null,
                AccountID = dto.VoucherType == "Transfer" ? null : dto.AccountID,
                PatientUserID = dto.PatientUserID,
                InvoiceID = dto.InvoiceID,
                AppointmentID = dto.AppointmentID,
                Amount = dto.Amount,
                Description = dto.Description?.Trim() ?? string.Empty,
                Status = "Draft",
                CreatedByUserID = userId,
                CreatedAt = DateTime.Now
            };

            _context.Vouchers.Add(voucher);

            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "VoucherCreated",
                EntityType = "Voucher",
                EntityID = voucher.VoucherID,
                UserID = userId,
                Details = $"إنشاء سند {voucher.VoucherNumber} ({voucher.VoucherType}) بمبلغ {voucher.Amount:N2} د.ل",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { voucher.VoucherID, voucher.VoucherNumber }, "تم إنشاء السند بنجاح (بالحالة مسودة)"));
        }

        // POST: api/treasury/vouchers/5/post
        [HttpPost("vouchers/{id}/post")]
        [Authorize(Roles = "Admin,Accountant,Cashier")]
        public async Task<IActionResult> PostVoucher(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
                return NotFound(ApiResponse.Fail("السند غير موجود"));

            if (voucher.Status == "Posted")
                return BadRequest(ApiResponse.Fail("السند مرحّل بالفعل"));

            if (voucher.Status == "Reversed")
                return BadRequest(ApiResponse.Fail("لا يمكن ترحيل سند عكسي"));

            var fiscalError = await JournalAutoHelper.ValidateFiscalDateAsync(_context, voucher.VoucherDate);
            if (fiscalError != null)
                return BadRequest(ApiResponse.Fail(fiscalError));

            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            // الكاشير يرحّل سندات خزينته المخصصة فقط
            if (role == "Cashier")
            {
                var cashier = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserID == userId);
                if (cashier == null || !cashier.AssignedTreasuryID.HasValue)
                    return StatusCode(403, ApiResponse.Fail("لا توجد خزينة مخصصة لحساب الكاشير"));
                if (voucher.TreasuryID != cashier.AssignedTreasuryID.Value)
                    return StatusCode(403, ApiResponse.Fail("يُسمح للكاشير بالترحيل على خزينته المخصصة فقط"));
            }

            var treasury = await _context.Treasuries.FindAsync(voucher.TreasuryID);
            if (treasury == null)
                return BadRequest(ApiResponse.Fail("خزينة السند غير موجودة"));

            List<(int AccountID, decimal Debit, decimal Credit, string? Notes)> lines;

            if (voucher.VoucherType == "Transfer")
            {
                var toTreasury = await _context.Treasuries.FindAsync(voucher.ToTreasuryID ?? 0);
                if (toTreasury == null)
                    return BadRequest(ApiResponse.Fail("خزينة التحويل إليها غير موجودة"));

                lines = new List<(int, decimal, decimal, string?)>
                {
                    (toTreasury.AccountID, voucher.Amount, 0m, null),
                    (treasury.AccountID, 0m, voucher.Amount, null)
                };
            }
            else
            {
                var account = await _context.ChartAccounts.FindAsync(voucher.AccountID ?? 0);
                if (account == null)
                    return BadRequest(ApiResponse.Fail("الحساب المقابل للسند غير موجود"));

                // قبض: مدين الخزينة / دائن الحساب المقابل — صرف: مدين الحساب المقابل / دائن الخزينة
                if (voucher.VoucherType == "Receipt")
                {
                    lines = new List<(int, decimal, decimal, string?)>
                    {
                        (treasury.AccountID, voucher.Amount, 0m, null),
                        (account.AccountID, 0m, voucher.Amount, null)
                    };
                }
                else
                {
                    lines = new List<(int, decimal, decimal, string?)>
                    {
                        (account.AccountID, voucher.Amount, 0m, null),
                        (treasury.AccountID, 0m, voucher.Amount, null)
                    };
                }
            }

            var entry = await JournalAutoHelper.CreatePostedEntryAsync(
                _context,
                userId,
                voucher.VoucherDate,
                $"سند {VoucherTypeNameAr(voucher.VoucherType)} {voucher.VoucherNumber} — {voucher.Description}",
                "Treasury",
                voucher.VoucherID,
                lines);

            if (entry == null)
                return BadRequest(ApiResponse.Fail("تعذر إنشاء القيد المحاسبي للسند (تحقق من التوازن والمبالغ)"));

            voucher.Status = "Posted";
            voucher.PostedAt = DateTime.Now;
            voucher.PostedByUserID = userId;

            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "VoucherPosted",
                EntityType = "Voucher",
                EntityID = voucher.VoucherID,
                UserID = userId,
                Details = $"ترحيل سند {voucher.VoucherNumber} بقيمة {voucher.Amount:N2} د.ل — قيد {entry.EntryNumber}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { voucher.VoucherID, entry.EntryNumber }, "تم ترحيل السند وتوليد القيد المحاسبي بنجاح"));
        }

        // POST: api/treasury/vouchers/5/reverse
        [HttpPost("vouchers/{id}/reverse")]
        [Authorize(Roles = "Admin,Accountant,Cashier")]
        public async Task<IActionResult> ReverseVoucher(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
                return NotFound(ApiResponse.Fail("السند غير موجود"));

            if (voucher.Status != "Posted")
                return BadRequest(ApiResponse.Fail("يُعكس فقط السند المرحّل"));

            var fiscalError = await JournalAutoHelper.ValidateFiscalDateAsync(_context, DateTime.Now);
            if (fiscalError != null)
                return BadRequest(ApiResponse.Fail(fiscalError));

            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            // الكاشير يعكس سندات خزينته المخصصة فقط
            if (role == "Cashier")
            {
                var cashier = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserID == userId);
                if (cashier == null || !cashier.AssignedTreasuryID.HasValue)
                    return StatusCode(403, ApiResponse.Fail("لا توجد خزينة مخصصة لحساب الكاشير"));
                if (voucher.TreasuryID != cashier.AssignedTreasuryID.Value)
                    return StatusCode(403, ApiResponse.Fail("يُسمح للكاشير بالعكس على خزينته المخصصة فقط"));
            }

            var treasury = await _context.Treasuries.FindAsync(voucher.TreasuryID);
            if (treasury == null)
                return BadRequest(ApiResponse.Fail("خزينة السند غير موجودة"));

            // قيد عكسي: عكس اتجاه المدين/الدائن
            List<(int AccountID, decimal Debit, decimal Credit, string? Notes)> lines;

            if (voucher.VoucherType == "Transfer")
            {
                var toTreasury = await _context.Treasuries.FindAsync(voucher.ToTreasuryID ?? 0);
                if (toTreasury == null)
                    return BadRequest(ApiResponse.Fail("خزينة التحويل إليها غير موجودة"));

                lines = new List<(int, decimal, decimal, string?)>
                {
                    (treasury.AccountID, voucher.Amount, 0m, null),
                    (toTreasury.AccountID, 0m, voucher.Amount, null)
                };
            }
            else
            {
                var account = await _context.ChartAccounts.FindAsync(voucher.AccountID ?? 0);
                if (account == null)
                    return BadRequest(ApiResponse.Fail("الحساب المقابل للسند غير موجود"));

                if (voucher.VoucherType == "Receipt")
                {
                    lines = new List<(int, decimal, decimal, string?)>
                    {
                        (account.AccountID, voucher.Amount, 0m, null),
                        (treasury.AccountID, 0m, voucher.Amount, null)
                    };
                }
                else
                {
                    lines = new List<(int, decimal, decimal, string?)>
                    {
                        (treasury.AccountID, voucher.Amount, 0m, null),
                        (account.AccountID, 0m, voucher.Amount, null)
                    };
                }
            }

            var entry = await JournalAutoHelper.CreatePostedEntryAsync(
                _context,
                userId,
                DateTime.Now,
                $"عكس سند {voucher.VoucherNumber} — {voucher.Description}",
                "Treasury",
                voucher.VoucherID,
                lines);

            if (entry == null)
                return BadRequest(ApiResponse.Fail("تعذر إنشاء القيد العكسي للسند"));

            voucher.Status = "Reversed";

            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "VoucherReversed",
                EntityType = "Voucher",
                EntityID = voucher.VoucherID,
                UserID = userId,
                Details = $"عكس سند {voucher.VoucherNumber} بقيمة {voucher.Amount:N2} د.ل — قيد {entry.EntryNumber}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { voucher.VoucherID, entry.EntryNumber }, "تم عكس السند وتوليد القيد العكسي بنجاح"));
        }

        // ============================================================
        //  يومية الخزينة
        // ============================================================

        // GET: api/treasury/daily-journal?treasuryId=...&from=...&to=...
        [HttpGet("daily-journal")]
        public async Task<IActionResult> GetDailyJournal(
            [FromQuery] int? treasuryId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var fromDate = from ?? new DateTime(2000, 1, 1);
            var toDate = (to ?? DateTime.Today).AddDays(1);

            var query = _context.Vouchers
                .Where(v => v.Status == "Posted" &&
                            v.VoucherDate >= fromDate &&
                            v.VoucherDate < toDate);

            // الكاشير يرى يومية خزينته المخصصة فقط
            var role = JwtHelper.GetUserRoleFromClaims(User);
            if (role == "Cashier")
            {
                var cashier = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserID == JwtHelper.GetUserIdFromClaims(User));
                if (cashier == null || !cashier.AssignedTreasuryID.HasValue)
                    return StatusCode(403, ApiResponse.Fail("لا توجد خزينة مخصصة لحساب الكاشير"));
                var myTreasury = cashier.AssignedTreasuryID.Value;
                query = query.Where(v => v.TreasuryID == myTreasury);
            }

            if (treasuryId.HasValue)
                query = query.Where(v => v.TreasuryID == treasuryId.Value || v.ToTreasuryID == treasuryId.Value);

            var vouchers = await query
                .OrderBy(v => v.VoucherDate)
                .ThenBy(v => v.VoucherID)
                .Select(v => new
                {
                    v.VoucherID,
                    v.VoucherNumber,
                    v.VoucherType,
                    v.VoucherDate,
                    v.TreasuryID,
                    TreasuryNameAr = v.Treasury.TreasuryNameAr,
                    ToTreasuryNameAr = v.ToTreasury != null ? v.ToTreasury.TreasuryNameAr : null,
                    AccountCode = v.Account != null ? v.Account.AccountCode : null,
                    AccountNameAr = v.Account != null ? v.Account.AccountNameAr : null,
                    PatientName = v.PatientUser != null ? v.PatientUser.FullName : null,
                    v.Amount,
                    v.Description
                })
                .ToListAsync();

            var totalReceipts = vouchers.Where(v => v.VoucherType == "Receipt").Sum(v => v.Amount);
            var totalPayments = vouchers.Where(v => v.VoucherType == "Payment").Sum(v => v.Amount);
            var totalTransfers = vouchers.Where(v => v.VoucherType == "Transfer").Sum(v => v.Amount);

            return Ok(ApiResponse<object>.Ok(new
            {
                Vouchers = vouchers,
                Totals = new { TotalReceipts = totalReceipts, TotalPayments = totalPayments, TotalTransfers = totalTransfers }
            }));
        }

        // ============================================================
        //  المذنيه اليومية (رصيد المرضى المستحق)
        // ============================================================

        // GET: api/treasury/receivables
        [HttpGet("receivables")]
        public async Task<IActionResult> GetReceivables([FromQuery] DateTime? asOf)
        {
            var query = _context.Invoices
                .Where(i => i.Status == "Unpaid");

            if (asOf.HasValue)
                query = query.Where(i => i.CreatedAt < asOf.Value.Date.AddDays(1));

            var rows = await query
                .GroupBy(i => new { i.PatientUserID, PatientName = i.PatientUser.FullName, i.PatientUser.Phone })
                .Select(g => new
                {
                    g.Key.PatientUserID,
                    g.Key.PatientName,
                    g.Key.Phone,
                    TotalDue = g.Sum(i => i.TotalAmount),
                    InvoiceCount = g.Count()
                })
                .OrderByDescending(r => r.TotalDue)
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(rows));
        }

        // ============================================================
        //  الإقفال المالي
        // ============================================================

        // GET: api/treasury/closure
        [HttpGet("closure")]
        public async Task<IActionResult> GetClosure()
        {
            var closedThrough = await JournalAutoHelper.GetFiscalClosedThroughAsync(_context);
            return Ok(ApiResponse<object>.Ok(new { ClosedThrough = closedThrough?.ToString("yyyy-MM-dd") }));
        }

        // POST: api/treasury/closure
        [HttpPost("closure")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> SetClosure([FromBody] FiscalClosureDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ClosedThrough))
                return BadRequest(ApiResponse.Fail("حدد تاريخ الإقفال أو أرسل null لفتح النظام"));

            var date = DateTime.Parse(dto.ClosedThrough);
            var userId = JwtHelper.GetUserIdFromClaims(User);

            // التحقق من أن كل السندات المرحّلة ضمن الفترة المغلقة فعلاً مرحّلة ولا توجد سندات مسودة قبل التاريخ
            var pendingDrafts = await _context.Vouchers
                .AnyAsync(v => v.Status == "Draft" && v.VoucherDate <= date.Date);

            if (pendingDrafts)
                return BadRequest(ApiResponse.Fail("لا يمكن الإقفال: توجد سندات مسودة بتاريخ الإقفال أو قبلها. رحّلها أو عكسها أولاً."));

            var setting = await _context.SystemSettings.FindAsync("FiscalClosedThrough");
            if (setting == null)
            {
                _context.SystemSettings.Add(new SystemSetting
                {
                    SettingKey = "FiscalClosedThrough",
                    SettingValue = date.ToString("yyyy-MM-dd"),
                    UpdatedAt = DateTime.UtcNow
                });
            }
            else
            {
                setting.SettingValue = date.ToString("yyyy-MM-dd");
                setting.UpdatedAt = DateTime.UtcNow;
            }

            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "FiscalClosureSet",
                EntityType = "SystemSetting",
                EntityID = 0,
                UserID = userId,
                Details = $"الإقفال المالي حتى {date:yyyy-MM-dd}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم الإقفال المالي حتى {date:yyyy-MM-dd}"));
        }

        // POST: api/treasury/closure/open
        [HttpPost("closure/open")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> OpenClosure()
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);

            var setting = await _context.SystemSettings.FindAsync("FiscalClosedThrough");
            if (setting != null)
                _context.SystemSettings.Remove(setting);

            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "FiscalClosureOpened",
                EntityType = "SystemSetting",
                EntityID = 0,
                UserID = userId,
                Details = "فتح الإقفال المالي (إلغاء القفل)",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم فتح الإقفال المالي"));
        }

        // ============================================================
        //  دوال مساعدة
        // ============================================================

        private static string VoucherTypeNameAr(string type)
            => type switch
            {
                "Receipt" => "قبض",
                "Payment" => "صرف",
                _ => "تحويل"
            };
    }
}
