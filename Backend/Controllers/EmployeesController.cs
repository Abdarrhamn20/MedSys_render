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
    [Route("api/employees")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        //  بطاقات الموظفين
        // ============================================================

        // GET: api/employees?search=&department=&page=&pageSize=
        [HttpGet]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> GetEmployees(
            [FromQuery] string? search,
            [FromQuery] string? department,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.EmployeeProfiles.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(e =>
                    e.FullName.Contains(search) ||
                    e.EmployeeNumber.Contains(search) ||
                    (e.Position != null && e.Position.Contains(search)) ||
                    (e.NationalID != null && e.NationalID.Contains(search)));

            if (!string.IsNullOrEmpty(department))
                query = query.Where(e => e.Department == department);

            var totalCount = await query.CountAsync();

            var employees = await query
                .OrderByDescending(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new
                {
                    e.EmployeeID,
                    e.EmployeeNumber,
                    e.FullName,
                    e.Department,
                    e.Position,
                    e.HireDate,
                    e.Gender,
                    e.CompensationModel,
                    e.BaseSalary,
                    e.IsActive,
                    e.UserID,
                    UserRole = e.User != null ? e.User.Role : null,
                    CoursesCount = e.Courses.Count,
                    LeavesCount = e.Leaves.Count,
                    PendingLeaves = e.Leaves.Count(l => l.Status == "Pending")
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<object>
            {
                Data = employees.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/employees/departments
        [HttpGet("departments")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _context.EmployeeProfiles
                .Where(e => e.Department != null && e.Department != "")
                .Select(e => e.Department!)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(departments));
        }

        // GET: api/employees/linkable-users?role= — حسابات دخول غير مرتبطة ببطاقات موظف (للربط من نموذج البطاقة)
        [HttpGet("linkable-users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLinkableUsers([FromQuery] string? role)
        {
            var query = _context.Users
                .Where(u => u.Role != "Patient" && u.Role != "Admin" && u.IsActive)
                .Where(u => !_context.EmployeeProfiles.Any(e => e.UserID == u.UserID));

            if (!string.IsNullOrEmpty(role))
                query = query.Where(u => u.Role == role);

            var users = await query
                .OrderBy(u => u.FullName)
                .Select(u => new
                {
                    u.UserID,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.IsActive
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(users));
        }

        // GET: api/employees/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _context.EmployeeProfiles
                .Include(e => e.User)
                .Include(e => e.Courses)
                .Include(e => e.Leaves)
                .Include(e => e.SalaryRecords)
                .FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee == null)
                return NotFound(ApiResponse.Fail("الموظف غير موجود"));

            var result = new
            {
                employee.EmployeeID,
                employee.EmployeeNumber,
                employee.FullName,
                employee.Department,
                employee.Position,
                employee.HireDate,
                employee.Gender,
                employee.NationalID,
                employee.CompensationModel,
                employee.BaseSalary,
                employee.BankAccount,
                employee.IsActive,
                employee.Notes,
                employee.UserID,
                UserEmail = employee.User?.Email,
                UserPhone = employee.User?.Phone,
                UserRole = employee.User?.Role,
                employee.CreatedAt,
                Courses = employee.Courses
                    .OrderByDescending(c => c.CourseDate)
                    .Select(c => new
                    {
                        c.CourseID,
                        c.CourseName,
                        c.Provider,
                        c.CourseDate,
                        c.CertificateNumber,
                        c.ExpiryDate,
                        c.Notes
                    })
                    .ToList(),
                Leaves = employee.Leaves
                    .OrderByDescending(l => l.StartDate)
                    .Take(20)
                    .Select(l => new
                    {
                        l.LeaveID,
                        l.LeaveType,
                        l.StartDate,
                        l.EndDate,
                        l.Days,
                        l.Reason,
                        l.Status,
                        l.ApprovedAt,
                        ApprovedByName = l.ApprovedByUser != null ? l.ApprovedByUser.FullName : null
                    })
                    .ToList(),
                SalaryRecords = employee.SalaryRecords
                    .OrderByDescending(s => s.PeriodYear)
                    .ThenByDescending(s => s.PeriodMonth)
                    .Take(12)
                    .Select(s => new
                    {
                        s.SalaryRecordID,
                        s.PeriodYear,
                        s.PeriodMonth,
                        s.BaseSalary,
                        s.CommissionAmount,
                        s.Bonus,
                        s.Deduction,
                        s.GrossSalary,
                        s.NetSalary,
                        s.Status,
                        s.PostedAt
                    })
                    .ToList()
            };

            return Ok(ApiResponse<object>.Ok(result));
        }

        // POST: api/employees
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                return BadRequest(ApiResponse.Fail("اسم الموظف مطلوب"));

            var modelError = await ValidateCompensationAsync(dto.CompensationModel, dto.BaseSalary, dto.UserID, dto.Role, null);
            if (modelError != null)
                return BadRequest(ApiResponse.Fail(modelError));

            int? linkedUserID = dto.UserID;

            // إنشاء حساب دخول جديد للموظف (اختياري)
            if (!linkedUserID.HasValue && !string.IsNullOrWhiteSpace(dto.Email))
            {
                if (string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.Role))
                    return BadRequest(ApiResponse.Fail("لإنشاء حساب دخول للموظف يجب تحديد كلمة المرور والدور"));

                var allowedRoles = new[] { "Doctor", "Pharmacist", "LabTechnician", "Radiologist", "Receptionist", "Cashier", "WarehouseKeeper", "Accountant" };
                if (!allowedRoles.Contains(dto.Role!))
                    return BadRequest(ApiResponse.Fail("الدور المحدد غير صالح — الموظفون لا يمكن أن يكونوا مرضى أو مدراء"));

                var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
                if (emailExists)
                    return BadRequest(ApiResponse.Fail("البريد الإلكتروني مسجل مسبقاً"));

                var user = new User
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = dto.Role,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                linkedUserID = user.UserID;

                if (dto.Role == "Doctor")
                {
                    _context.DoctorProfiles.Add(new DoctorProfile
                    {
                        UserID = user.UserID,
                        Specialty = dto.Department ?? "عام",
                        ConsultationFee = dto.CompensationModel == "FixedSalary" ? 0 : 100m
                    });
                }
            }

            if (linkedUserID.HasValue)
            {
                var linked = await _context.Users.FindAsync(linkedUserID.Value);
                if (linked == null)
                    return BadRequest(ApiResponse.Fail("حساب المستخدم المرتبط غير موجود"));
                var alreadyLinked = await _context.EmployeeProfiles.AnyAsync(e => e.UserID == linkedUserID.Value);
                if (alreadyLinked)
                    return BadRequest(ApiResponse.Fail("هذا المستخدم مرتبط بالفعل ببطاقة موظف"));
            }

            var employee = new EmployeeProfile
            {
                UserID = linkedUserID,
                EmployeeNumber = await GenerateEmployeeNumberAsync(),
                FullName = dto.FullName.Trim(),
                Department = dto.Department,
                Position = dto.Position,
                HireDate = dto.HireDate == default ? DateTime.Today : dto.HireDate,
                Gender = dto.Gender,
                NationalID = dto.NationalID,
                CompensationModel = dto.CompensationModel,
                BaseSalary = dto.BaseSalary,
                BankAccount = dto.BankAccount,
                IsActive = dto.IsActive,
                Notes = dto.Notes,
                CreatedAt = DateTime.Now
            };

            _context.EmployeeProfiles.Add(employee);

            var userId = JwtHelper.GetUserIdFromClaims(User);
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "EmployeeCreated",
                EntityType = "Employee",
                EntityID = employee.EmployeeID,
                UserID = userId,
                Details = $"إنشاء بطاقة موظف {employee.FullName} برقم {employee.EmployeeNumber}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { employee.EmployeeID, employee.EmployeeNumber }, "تم إنشاء بطاقة الموظف بنجاح"));
        }

        // PUT: api/employees/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeDTO dto)
        {
            var employee = await _context.EmployeeProfiles.FindAsync(id);
            if (employee == null)
                return NotFound(ApiResponse.Fail("الموظف غير موجود"));

            if (string.IsNullOrWhiteSpace(dto.FullName))
                return BadRequest(ApiResponse.Fail("اسم الموظف مطلوب"));

            var modelError = await ValidateCompensationAsync(dto.CompensationModel, dto.BaseSalary, dto.UserID, dto.Role, employee.EmployeeID);
            if (modelError != null)
                return BadRequest(ApiResponse.Fail(modelError));

            if (dto.UserID.HasValue && dto.UserID.Value != employee.UserID)
            {
                var linked = await _context.Users.FindAsync(dto.UserID.Value);
                if (linked == null)
                    return BadRequest(ApiResponse.Fail("حساب المستخدم المرتبط غير موجود"));
                var alreadyLinked = await _context.EmployeeProfiles.AnyAsync(e => e.UserID == dto.UserID.Value && e.EmployeeID != id);
                if (alreadyLinked)
                    return BadRequest(ApiResponse.Fail("هذا المستخدم مرتبط بالفعل ببطاقة موظف أخرى"));
            }

            employee.UserID = dto.UserID;
            employee.FullName = dto.FullName.Trim();
            employee.Department = dto.Department;
            employee.Position = dto.Position;
            employee.HireDate = dto.HireDate == default ? DateTime.Today : dto.HireDate;
            employee.Gender = dto.Gender;
            employee.NationalID = dto.NationalID;
            employee.CompensationModel = dto.CompensationModel;
            employee.BaseSalary = dto.BaseSalary;
            employee.BankAccount = dto.BankAccount;
            employee.IsActive = dto.IsActive;
            employee.Notes = dto.Notes;

            var userId = JwtHelper.GetUserIdFromClaims(User);
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "EmployeeUpdated",
                EntityType = "Employee",
                EntityID = employee.EmployeeID,
                UserID = userId,
                Details = $"تعديل بطاقة موظف {employee.FullName}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم تحديث بطاقة الموظف بنجاح"));
        }

        // PUT: api/employees/5/toggle-active
        [HttpPut("{id}/toggle-active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var employee = await _context.EmployeeProfiles.FindAsync(id);
            if (employee == null)
                return NotFound(ApiResponse.Fail("الموظف غير موجود"));

            employee.IsActive = !employee.IsActive;

            var userId = JwtHelper.GetUserIdFromClaims(User);
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = employee.IsActive ? "EmployeeActivated" : "EmployeeDeactivated",
                EntityType = "Employee",
                EntityID = employee.EmployeeID,
                UserID = userId,
                Details = $"{(employee.IsActive ? "تفعيل" : "تعطيل")} موظف {employee.FullName}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(employee.IsActive ? "تم تفعيل الموظف" : "تم تعطيل الموظف"));
        }

        // ============================================================
        //  الدورات التدريبية
        // ============================================================

        // GET: api/employees/5/courses
        [HttpGet("{id}/courses")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> GetCourses(int id)
        {
            if (!await _context.EmployeeProfiles.AnyAsync(e => e.EmployeeID == id))
                return NotFound(ApiResponse.Fail("الموظف غير موجود"));

            var courses = await _context.EmployeeCourses
                .Where(c => c.EmployeeID == id)
                .OrderByDescending(c => c.CourseDate)
                .Select(c => new
                {
                    c.CourseID,
                    c.CourseName,
                    c.Provider,
                    c.CourseDate,
                    c.CertificateNumber,
                    c.ExpiryDate,
                    c.Notes
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(courses));
        }

        // POST: api/employees/5/courses
        [HttpPost("{id}/courses")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCourse(int id, [FromBody] EmployeeCourseDTO dto)
        {
            var employee = await _context.EmployeeProfiles.FindAsync(id);
            if (employee == null)
                return NotFound(ApiResponse.Fail("الموظف غير موجود"));

            if (string.IsNullOrWhiteSpace(dto.CourseName))
                return BadRequest(ApiResponse.Fail("اسم الدورة مطلوب"));

            var course = new EmployeeCourse
            {
                EmployeeID = id,
                CourseName = dto.CourseName.Trim(),
                Provider = dto.Provider,
                CourseDate = dto.CourseDate == default ? DateTime.Today : dto.CourseDate,
                CertificateNumber = dto.CertificateNumber,
                ExpiryDate = dto.ExpiryDate,
                Notes = dto.Notes,
                CreatedAt = DateTime.Now
            };

            _context.EmployeeCourses.Add(course);

            var userId = JwtHelper.GetUserIdFromClaims(User);
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "EmployeeCourseAdded",
                EntityType = "EmployeeCourse",
                EntityID = course.CourseID,
                UserID = userId,
                Details = $"إضافة دورة «{course.CourseName}» لموظف {employee.FullName}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { course.CourseID }, "تمت إضافة الدورة بنجاح"));
        }

        // DELETE: api/employees/courses/5
        [HttpDelete("courses/{courseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            var course = await _context.EmployeeCourses.FindAsync(courseId);
            if (course == null)
                return NotFound(ApiResponse.Fail("الدورة غير موجودة"));

            _context.EmployeeCourses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم حذف الدورة"));
        }

        // ============================================================
        //  الإجازات
        // ============================================================

        // GET: api/employees/leaves?status=&page=&pageSize=
        [HttpGet("leaves")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> GetLeaves(
            [FromQuery] string? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.EmployeeLeaves.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(l => l.Status == status);

            var totalCount = await query.CountAsync();

            var leaves = await query
                .OrderByDescending(l => l.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new
                {
                    l.LeaveID,
                    l.EmployeeID,
                    EmployeeName = l.Employee.FullName,
                    EmployeeNumber = l.Employee.EmployeeNumber,
                    Department = l.Employee.Department,
                    l.LeaveType,
                    l.StartDate,
                    l.EndDate,
                    l.Days,
                    l.Reason,
                    l.Status,
                    l.CreatedAt,
                    l.ApprovedAt,
                    ApprovedByName = l.ApprovedByUser != null ? l.ApprovedByUser.FullName : null
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<object>
            {
                Data = leaves.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/employees/5/leaves
        [HttpGet("{id}/leaves")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> GetEmployeeLeaves(int id)
        {
            if (!await _context.EmployeeProfiles.AnyAsync(e => e.EmployeeID == id))
                return NotFound(ApiResponse.Fail("الموظف غير موجود"));

            var leaves = await _context.EmployeeLeaves
                .Where(l => l.EmployeeID == id)
                .OrderByDescending(l => l.StartDate)
                .Select(l => new
                {
                    l.LeaveID,
                    l.LeaveType,
                    l.StartDate,
                    l.EndDate,
                    l.Days,
                    l.Reason,
                    l.Status,
                    l.CreatedAt,
                    l.ApprovedAt,
                    ApprovedByName = l.ApprovedByUser != null ? l.ApprovedByUser.FullName : null
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(leaves));
        }

        // POST: api/employees/5/leaves
        [HttpPost("{id}/leaves")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddLeave(int id, [FromBody] EmployeeLeaveDTO dto)
        {
            var employee = await _context.EmployeeProfiles.FindAsync(id);
            if (employee == null)
                return NotFound(ApiResponse.Fail("الموظف غير موجود"));

            if (dto.StartDate == default || dto.EndDate == default)
                return BadRequest(ApiResponse.Fail("تاريخا بداية ونهاية الإجازة مطلوبان"));

            if (dto.EndDate < dto.StartDate)
                return BadRequest(ApiResponse.Fail("تاريخ نهاية الإجازة قبل تاريخ بدايتها"));

            var validTypes = new[] { "Annual", "Sick", "Unpaid", "Other" };
            if (!validTypes.Contains(dto.LeaveType))
                return BadRequest(ApiResponse.Fail("نوع الإجازة غير صالح"));

            var leave = new EmployeeLeave
            {
                EmployeeID = id,
                LeaveType = dto.LeaveType,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Days = (dto.EndDate - dto.StartDate).Days + 1,
                Reason = dto.Reason,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.EmployeeLeaves.Add(leave);

            var userId = JwtHelper.GetUserIdFromClaims(User);
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "LeaveRequested",
                EntityType = "EmployeeLeave",
                EntityID = leave.LeaveID,
                UserID = userId,
                Details = $"طلب إجازة ({dto.LeaveType}) لموظف {employee.FullName} لمدة {leave.Days} يوم",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { leave.LeaveID }, "تم تسجيل طلب الإجازة بنجاح"));
        }

        // PUT: api/employees/leaves/5/status
        [HttpPut("leaves/{leaveId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateLeaveStatus(int leaveId, [FromBody] LeaveStatusDTO dto)
        {
            var leave = await _context.EmployeeLeaves
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(l => l.LeaveID == leaveId);

            if (leave == null)
                return NotFound(ApiResponse.Fail("الإجازة غير موجودة"));

            if (dto.Status != "Approved" && dto.Status != "Rejected")
                return BadRequest(ApiResponse.Fail("الحالة يجب أن تكون Approved أو Rejected"));

            if (leave.Status != "Pending")
                return BadRequest(ApiResponse.Fail("تمت معالجة هذا الطلب مسبقاً"));

            leave.Status = dto.Status;
            leave.ApprovedByUserID = JwtHelper.GetUserIdFromClaims(User);
            leave.ApprovedAt = DateTime.Now;

            var userId = JwtHelper.GetUserIdFromClaims(User);
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = dto.Status == "Approved" ? "LeaveApproved" : "LeaveRejected",
                EntityType = "EmployeeLeave",
                EntityID = leave.LeaveID,
                UserID = userId,
                Details = $"{(dto.Status == "Approved" ? "اعتماد" : "رفض")} إجازة موظف {leave.Employee.FullName}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(dto.Status == "Approved" ? "تم اعتماد الإجازة" : "تم رفض الإجازة"));
        }

        // ============================================================
        //  الرواتب (التسوية الشهرية)
        // ============================================================

        // POST: api/employees/payroll/run — توليد مسودة رواتب لشهر محدد
        [HttpPost("payroll/run")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RunPayroll([FromBody] PayrollRunDTO dto)
        {
            if (dto.Year < 2000 || dto.Year > 2100 || dto.Month < 1 || dto.Month > 12)
                return BadRequest(ApiResponse.Fail("سنة أو شهر غير صالحين"));

            var periodStart = new DateTime(dto.Year, dto.Month, 1);
            var periodEnd = periodStart.AddMonths(1);

            var employees = await _context.EmployeeProfiles
                .Include(e => e.User)
                .Where(e => e.IsActive)
                .ToListAsync();

            if (employees.Count == 0)
                return BadRequest(ApiResponse.Fail("لا توجد بطاقات موظفين مفعّلة"));

            var commissionsByDoctor = await _context.Invoices
                .Where(i => i.Status == "Paid" && i.CreatedAt >= periodStart && i.CreatedAt < periodEnd && i.DoctorID != null)
                .GroupBy(i => i.DoctorID)
                .Select(g => new { DoctorID = g.Key, Total = g.Sum(i => (decimal?)i.DoctorShare) ?? 0m })
                .ToListAsync();

            var commissionLookup = commissionsByDoctor.ToDictionary(x => x.DoctorID!.Value, x => x.Total);

            var created = 0;
            var skipped = 0;
            var totalNet = 0m;

            var existingKeys = (await _context.SalaryRecords
                .Where(s => s.PeriodYear == dto.Year && s.PeriodMonth == dto.Month)
                .Select(s => s.EmployeeID)
                .ToListAsync()).ToHashSet();

            var userId = JwtHelper.GetUserIdFromClaims(User);

            foreach (var emp in employees)
            {
                if (existingKeys.Contains(emp.EmployeeID))
                {
                    skipped++;
                    continue;
                }

                var baseSalary = emp.CompensationModel == "Commission" ? 0m : emp.BaseSalary;
                var commission = 0m;
                if (emp.CompensationModel is "Commission" or "Mixed" && emp.UserID.HasValue && commissionLookup.TryGetValue(emp.UserID.Value, out var comm))
                    commission = comm;

                var gross = baseSalary + commission;
                var record = new SalaryRecord
                {
                    EmployeeID = emp.EmployeeID,
                    PeriodYear = dto.Year,
                    PeriodMonth = dto.Month,
                    BaseSalary = baseSalary,
                    CommissionAmount = commission,
                    Bonus = 0m,
                    Deduction = 0m,
                    GrossSalary = gross,
                    NetSalary = gross,
                    Status = "Draft",
                    CreatedByUserID = userId,
                    CreatedAt = DateTime.Now
                };

                _context.SalaryRecords.Add(record);
                created++;
                totalNet += gross;
            }

            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "PayrollRun",
                EntityType = "SalaryRecord",
                EntityID = 0,
                UserID = userId,
                Details = $"توليد مسودة رواتب شهر {dto.Month}/{dto.Year} — {created} موظف",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(
                new { Created = created, Skipped = skipped, TotalNet = totalNet },
                $"تم توليد {created} سجل راتب، وتخطّي {skipped} موجودة مسبقاً"));
        }

        // GET: api/employees/payroll?year=&month=&page=&pageSize=
        [HttpGet("payroll")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> GetPayroll(
            [FromQuery] int? year,
            [FromQuery] int? month,
            [FromQuery] string? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.SalaryRecords.AsQueryable();

            if (year.HasValue)
                query = query.Where(s => s.PeriodYear == year.Value);
            if (month.HasValue)
                query = query.Where(s => s.PeriodMonth == month.Value);
            if (!string.IsNullOrEmpty(status))
                query = query.Where(s => s.Status == status);

            var totalCount = await query.CountAsync();

            var records = await query
                .OrderByDescending(s => s.PeriodYear)
                .ThenByDescending(s => s.PeriodMonth)
                .ThenByDescending(s => s.SalaryRecordID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new
                {
                    s.SalaryRecordID,
                    s.EmployeeID,
                    EmployeeName = s.Employee.FullName,
                    EmployeeNumber = s.Employee.EmployeeNumber,
                    Department = s.Employee.Department,
                    CompensationModel = s.Employee.CompensationModel,
                    s.PeriodYear,
                    s.PeriodMonth,
                    s.BaseSalary,
                    s.CommissionAmount,
                    s.Bonus,
                    s.Deduction,
                    s.GrossSalary,
                    s.NetSalary,
                    s.Status,
                    s.PostedAt,
                    s.JournalEntryID,
                    s.CreatedAt,
                    CreatedByName = s.CreatedByUser != null ? s.CreatedByUser.FullName : null
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<object>
            {
                Data = records.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/employees/payroll/summary?year=&month=
        [HttpGet("payroll/summary")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> GetPayrollSummary([FromQuery] int? year, [FromQuery] int? month)
        {
            var query = _context.SalaryRecords.AsQueryable();

            if (year.HasValue)
                query = query.Where(s => s.PeriodYear == year.Value);
            if (month.HasValue)
                query = query.Where(s => s.PeriodMonth == month.Value);

            var list = await query.ToListAsync();

            var summary = new
            {
                TotalRecords = list.Count,
                TotalBaseSalary = list.Sum(s => s.BaseSalary),
                TotalCommissions = list.Sum(s => s.CommissionAmount),
                TotalBonus = list.Sum(s => s.Bonus),
                TotalDeductions = list.Sum(s => s.Deduction),
                TotalGross = list.Sum(s => s.GrossSalary),
                TotalNet = list.Sum(s => s.NetSalary),
                DraftCount = list.Count(s => s.Status == "Draft"),
                PostedCount = list.Count(s => s.Status == "Posted"),
                ReversedCount = list.Count(s => s.Status == "Reversed")
            };

            return Ok(ApiResponse<object>.Ok(summary));
        }

        // POST: api/employees/payroll/5/post — ترحيل استحقاق الرواتب قيداً محاسبياً
        [HttpPost("payroll/{id}/post")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> PostPayroll(int id)
        {
            var record = await _context.SalaryRecords
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.SalaryRecordID == id);

            if (record == null)
                return NotFound(ApiResponse.Fail("سجل الراتب غير موجود"));

            if (record.Status == "Posted")
                return BadRequest(ApiResponse.Fail("سجل الراتب مرحّل بالفعل"));

            if (record.Status == "Reversed")
                return BadRequest(ApiResponse.Fail("لا يمكن ترحيل سجل راتب معكوس"));

            if (record.NetSalary <= 0)
                return BadRequest(ApiResponse.Fail("صافي الراتب صفر — لا يمكن ترحيله"));

            var periodEnd = new DateTime(record.PeriodYear, record.PeriodMonth, 1).AddMonths(1).AddDays(-1);
            var fiscalError = await JournalAutoHelper.ValidateFiscalDateAsync(_context, periodEnd);
            if (fiscalError != null)
                return BadRequest(ApiResponse.Fail(fiscalError));

            var salaryExpense = await _context.ChartAccounts.FirstOrDefaultAsync(a => a.AccountCode == "5010");
            var accruedSalaries = await _context.ChartAccounts.FirstOrDefaultAsync(a => a.AccountCode == "2020");
            if (salaryExpense == null || accruedSalaries == null)
                return BadRequest(ApiResponse.Fail("حسابات الرواتب (5010/2020) غير مهيأة في شجرة الحسابات"));

            var userId = JwtHelper.GetUserIdFromClaims(User);

            var entry = await JournalAutoHelper.CreatePostedEntryAsync(
                _context,
                userId,
                periodEnd,
                $"استحقاق راتب {record.Employee.FullName} — شهر {record.PeriodMonth}/{record.PeriodYear}",
                "Salary",
                record.SalaryRecordID,
                new List<(int, decimal, decimal, string?)>
                {
                    (salaryExpense.AccountID, record.NetSalary, 0m, record.Employee.FullName),
                    (accruedSalaries.AccountID, 0m, record.NetSalary, record.Employee.FullName)
                });

            if (entry == null)
                return BadRequest(ApiResponse.Fail("فشل إنشاء القيد المحاسبي للراتب"));

            await _context.SaveChangesAsync();

            record.JournalEntryID = entry.JournalEntryID;
            record.Status = "Posted";
            record.PostedAt = DateTime.Now;

            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "PayrollPosted",
                EntityType = "SalaryRecord",
                EntityID = record.SalaryRecordID,
                UserID = userId,
                Details = $"ترحيل راتب {record.Employee.FullName} ({record.NetSalary:N2} د.ل) — شهر {record.PeriodMonth}/{record.PeriodYear}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { record.JournalEntryID }, $"تم ترحيل راتب {record.Employee.FullName} وقيده المحاسبي بنجاح"));
        }

        // POST: api/employees/payroll/5/reverse — عكس قيد استحقاق الراتب
        [HttpPost("payroll/{id}/reverse")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> ReversePayroll(int id)
        {
            var record = await _context.SalaryRecords
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.SalaryRecordID == id);

            if (record == null)
                return NotFound(ApiResponse.Fail("سجل الراتب غير موجود"));

            if (record.Status != "Posted")
                return BadRequest(ApiResponse.Fail("يُعكس فقط سجل الراتب المرحّل"));

            var fiscalError = await JournalAutoHelper.ValidateFiscalDateAsync(_context, DateTime.Now);
            if (fiscalError != null)
                return BadRequest(ApiResponse.Fail(fiscalError));

            var salaryExpense = await _context.ChartAccounts.FirstOrDefaultAsync(a => a.AccountCode == "5010");
            var accruedSalaries = await _context.ChartAccounts.FirstOrDefaultAsync(a => a.AccountCode == "2020");
            if (salaryExpense == null || accruedSalaries == null)
                return BadRequest(ApiResponse.Fail("حسابات الرواتب (5010/2020) غير مهيأة في شجرة الحسابات"));

            var userId = JwtHelper.GetUserIdFromClaims(User);

            var entry = await JournalAutoHelper.CreatePostedEntryAsync(
                _context,
                userId,
                DateTime.Now,
                $"عكس استحقاق راتب {record.Employee.FullName} — شهر {record.PeriodMonth}/{record.PeriodYear}",
                "Salary",
                record.SalaryRecordID,
                new List<(int, decimal, decimal, string?)>
                {
                    (accruedSalaries.AccountID, record.NetSalary, 0m, record.Employee.FullName),
                    (salaryExpense.AccountID, 0m, record.NetSalary, record.Employee.FullName)
                });

            if (entry == null)
                return BadRequest(ApiResponse.Fail("فشل إنشاء القيد العكسي للراتب"));

            await _context.SaveChangesAsync();

            record.Status = "Reversed";
            record.PostedAt = DateTime.Now;

            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "PayrollReversed",
                EntityType = "SalaryRecord",
                EntityID = record.SalaryRecordID,
                UserID = userId,
                Details = $"عكس راتب {record.Employee.FullName} — شهر {record.PeriodMonth}/{record.PeriodYear}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok($"تم عكس راتب {record.Employee.FullName}"));
        }

        // DELETE: api/employees/payroll/5 — حذف مسودة
        [HttpDelete("payroll/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePayrollDraft(int id)
        {
            var record = await _context.SalaryRecords.FindAsync(id);
            if (record == null)
                return NotFound(ApiResponse.Fail("سجل الراتب غير موجود"));

            if (record.Status != "Draft")
                return BadRequest(ApiResponse.Fail("يُحذف فقط سجل الراتب المسودة"));

            _context.SalaryRecords.Remove(record);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم حذف مسودة الراتب"));
        }

        // PUT: api/employees/payroll/5/adjust — إضافة مكافأة أو خصم
        [HttpPut("payroll/{id}/adjust")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> AdjustPayroll(int id, [FromBody] SalaryAdjustDTO dto)
        {
            var record = await _context.SalaryRecords
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.SalaryRecordID == id);

            if (record == null)
                return NotFound(ApiResponse.Fail("سجل الراتب غير موجود"));

            if (record.Status != "Draft")
                return BadRequest(ApiResponse.Fail("يُعدَّل سجل الراتب في حالة المسودة فقط"));

            if (dto.Bonus < 0 || dto.Deduction < 0)
                return BadRequest(ApiResponse.Fail("المكافأة والخصم لا يكونان سالبين"));

            record.Bonus = dto.Bonus;
            record.Deduction = dto.Deduction;
            record.GrossSalary = record.BaseSalary + record.CommissionAmount + dto.Bonus;
            record.NetSalary = record.GrossSalary - dto.Deduction;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(
                new { record.GrossSalary, record.NetSalary },
                $"تم تعديل راتب {record.Employee.FullName} — الصافي {record.NetSalary:N2} د.ل"));
        }

        // ============================================================
        //  الخدمة الذاتية للموظف
        // ============================================================

        // GET: api/employees/me — بطاقتي وإجازاتي ورواتبي (للحساب المرتبط)
        [HttpGet("me")]
        public async Task<IActionResult> GetMyEmployee()
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var employee = await _context.EmployeeProfiles
                .Include(e => e.Leaves)
                .Include(e => e.SalaryRecords)
                .FirstOrDefaultAsync(e => e.UserID == userId);

            if (employee == null)
                return NotFound(ApiResponse.Fail("لا توجد بطاقة موظف مرتبطة بحسابك"));

            var result = new
            {
                employee.EmployeeID,
                employee.EmployeeNumber,
                employee.FullName,
                employee.Department,
                employee.Position,
                employee.HireDate,
                employee.CompensationModel,
                employee.BaseSalary,
                Leaves = employee.Leaves
                    .OrderByDescending(l => l.StartDate)
                    .Select(l => new
                    {
                        l.LeaveID,
                        l.LeaveType,
                        l.StartDate,
                        l.EndDate,
                        l.Days,
                        l.Status,
                        l.Reason
                    })
                    .ToList(),
                SalaryRecords = employee.SalaryRecords
                    .OrderByDescending(s => s.PeriodYear)
                    .ThenByDescending(s => s.PeriodMonth)
                    .Take(12)
                    .Select(s => new
                    {
                        s.SalaryRecordID,
                        s.PeriodYear,
                        s.PeriodMonth,
                        s.BaseSalary,
                        s.CommissionAmount,
                        s.Bonus,
                        s.Deduction,
                        s.GrossSalary,
                        s.NetSalary,
                        s.Status
                    })
                    .ToList()
            };

            return Ok(ApiResponse<object>.Ok(result));
        }

        // POST: api/employees/me/leaves — طلب إجازة من الموظف نفسه
        [HttpPost("me/leaves")]
        public async Task<IActionResult> RequestMyLeave([FromBody] EmployeeLeaveDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var employee = await _context.EmployeeProfiles.FirstOrDefaultAsync(e => e.UserID == userId);
            if (employee == null)
                return NotFound(ApiResponse.Fail("لا توجد بطاقة موظف مرتبطة بحسابك"));

            if (dto.StartDate == default || dto.EndDate == default)
                return BadRequest(ApiResponse.Fail("تاريخا بداية ونهاية الإجازة مطلوبان"));

            if (dto.EndDate < dto.StartDate)
                return BadRequest(ApiResponse.Fail("تاريخ نهاية الإجازة قبل تاريخ بدايتها"));

            var validTypes = new[] { "Annual", "Sick", "Unpaid", "Other" };
            if (!validTypes.Contains(dto.LeaveType))
                return BadRequest(ApiResponse.Fail("نوع الإجازة غير صالح"));

            var leave = new EmployeeLeave
            {
                EmployeeID = employee.EmployeeID,
                LeaveType = dto.LeaveType,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Days = (dto.EndDate - dto.StartDate).Days + 1,
                Reason = dto.Reason,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.EmployeeLeaves.Add(leave);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { leave.LeaveID }, "تم إرسال طلب الإجازة — بانتظار الاعتماد"));
        }

        // ============================================================
        //  دوال مساعدة
        // ============================================================

        private async Task<string> GenerateEmployeeNumberAsync()
        {
            var year = DateTime.Now.Year;
            var count = await _context.EmployeeProfiles.CountAsync(e => e.EmployeeNumber.StartsWith($"EMP-{year}-"));
            return $"EMP-{year}-{(count + 1):0000}";
        }

        private async Task<string?> ValidateCompensationAsync(
            string model,
            decimal baseSalary,
            int? userId,
            string? newRole,
            int? excludeEmployeeID)
        {
            if (model != "FixedSalary" && model != "Commission" && model != "Mixed")
                return "نموذج التعويض غير صالح — القيم المسموحة: FixedSalary, Commission, Mixed";

            if (model == "Commission")
            {
                if (baseSalary != 0)
                    return "نموذج العمولات لا يقبل راتباً أساسياً — اجعل الراتب الأساسي صفراً";

                // العمولات تأتي من محرك عمولات الأطباء فقط
                var isDoctor = false;
                if (userId.HasValue)
                {
                    var linked = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserID == userId.Value);
                    isDoctor = linked?.Role == "Doctor";
                }
                else if (newRole == "Doctor")
                {
                    isDoctor = true;
                }

                if (!isDoctor)
                    return "نموذج العمولات (Commission) يُستخدم فقط للأطباء المرتبطين بمحرك عمولات الأطباء";
            }
            else if (model == "FixedSalary" && baseSalary < 0)
            {
                return "الراتب الأساسي لا يكون سالباً";
            }
            else if (model == "Mixed" && baseSalary <= 0)
            {
                return "النموذج المختلط (راتب + عمولات) يتطلب راتباً أساسياً أكبر من صفر";
            }

            return null;
        }
    }
}
