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
    [Route("api/[controller]")]
    [Authorize]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MedicalRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/medicalrecords?patientId=&page=1
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? patientId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            var query = _context.MedicalRecords
                .Include(m => m.Appointment)
                    .ThenInclude(a => a.Patient).ThenInclude(p => p.User)
                .Include(m => m.Appointment)
                    .ThenInclude(a => a.Doctor).ThenInclude(d => d.User)
                .AsQueryable();

            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                query = query.Where(m => m.Appointment.DoctorID == doctorId);
            }
            else if (role == "Patient")
            {
                var pid = await _context.PatientProfiles.Where(p => p.UserID == userId).Select(p => p.PatientID).FirstOrDefaultAsync();
                query = query.Where(m => m.Appointment.PatientID == pid);
            }
            else if (role != "Admin")
            {
                return Forbid();
            }

            if (patientId.HasValue)
                query = query.Where(m => m.Appointment.PatientID == patientId.Value);

            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var totalCount = await query.CountAsync();

            var records = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.RecordID,
                    m.AppID,
                    m.Diagnosis,
                    m.DiagnosisAr,
                    m.TreatmentPlan,
                    m.DoctorNotes,
                    m.CreatedAt,
                    PatientName = m.Appointment.Patient.User.FullName,
                    DoctorName = m.Appointment.Doctor.User.FullName,
                    DoctorSpecialty = m.Appointment.Doctor.Specialty,
                    m.Appointment.AppointmentDate,
                    PrescriptionCount = m.Prescriptions.Count()
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

        // GET: api/medicalrecords/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _context.MedicalRecords
                .Where(m => m.RecordID == id)
                .Select(m => new
                {
                    m.RecordID,
                    m.AppID,
                    m.Diagnosis,
                    m.DiagnosisAr,
                    m.TreatmentPlan,
                    m.DoctorNotes,
                    m.FollowUpDate,
                    m.FollowUpNotes,
                    m.CreatedAt,
                    PatientName = m.Appointment.Patient.User.FullName,
                    PatientUserID = m.Appointment.Patient.User.UserID,
                    PatientID = m.Appointment.Patient.PatientID,
                    PatientPhone = m.Appointment.Patient.User.Phone,
                    PatientBloodType = m.Appointment.Patient.BloodType,
                    PatientAllergies = m.Appointment.Patient.Allergies,
                    PatientChronicDiseases = m.Appointment.Patient.ChronicDiseases,
                    DoctorID = m.Appointment.Doctor.DoctorID,
                    DoctorName = m.Appointment.Doctor.User.FullName,
                    DoctorSpecialty = m.Appointment.Doctor.Specialty,
                    m.Appointment.AppointmentDate,
                    m.Appointment.TriageScore,
                    PriorityLevel = m.Appointment.Priority.LevelNameAr,
                    PriorityColor = m.Appointment.Priority.ColorCode,
                    Prescriptions = m.Prescriptions.Select(p => new
                    {
                        p.PrescriptionID,
                        p.MedicationName,
                        p.Dosage,
                        p.Frequency,
                        p.Duration,
                        p.Instructions,
                        p.CreatedAt,
                        p.DispenseStatus
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (record == null)
                return NotFound(ApiResponse.Fail("السجل الطبي غير موجود"));

            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);

            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (record.DoctorID != doctorId)
                    return Forbid();
            }
            else if (role == "Patient")
            {
                if (record.PatientUserID != userId)
                    return Forbid();
            }
            else if (role != "Admin")
            {
                return Forbid();
            }

            return Ok(ApiResponse<object>.Ok(record));
        }

        // POST: api/medicalrecords
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Create([FromBody] CreateMedicalRecordDTO dto)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();

            // Verify appointment belongs to this doctor
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.AppID == dto.AppID && a.DoctorID == doctorId);
            if (appointment == null)
                return BadRequest(ApiResponse.Fail("الموعد غير موجود أو لا ينتمي لك"));

            // Check if record already exists
            var exists = await _context.MedicalRecords.AnyAsync(m => m.AppID == dto.AppID);
            if (exists)
                return BadRequest(ApiResponse.Fail("يوجد سجل طبي لهذا الموعد مسبقاً"));

            var record = new MedicalRecord
            {
                AppID = dto.AppID,
                Diagnosis = !string.IsNullOrWhiteSpace(dto.Diagnosis) ? dto.Diagnosis : (dto.DiagnosisAr ?? "غير محدد"),
                DiagnosisAr = dto.DiagnosisAr,
                TreatmentPlan = dto.TreatmentPlan,
                DoctorNotes = dto.DoctorNotes,
                FollowUpDate = dto.FollowUpDate,
                FollowUpNotes = dto.FollowUpNotes,
                CreatedAt = DateTime.Now
            };

            _context.MedicalRecords.Add(record);

            // Update appointment status to Completed
            appointment.Status = "Completed";

            await _context.SaveChangesAsync();

            // Add prescriptions if provided
            if (dto.Prescriptions != null && dto.Prescriptions.Any())
            {
                foreach (var p in dto.Prescriptions)
                {
                    _context.Prescriptions.Add(new Prescription
                    {
                        RecordID = record.RecordID,
                        MedicationName = p.MedicationName,
                        Dosage = p.Dosage ?? string.Empty,
                        Frequency = p.Frequency,
                        Duration = p.Duration,
                        Instructions = p.Instructions,
                        DispenseStatus = dto.SendToPharmacy ? "Pending" : "Draft",
                        CreatedAt = DateTime.Now
                    });
                }
                await _context.SaveChangesAsync();
            }

            return Ok(ApiResponse<object>.Ok(new { recordId = record.RecordID }, "تم إنشاء السجل الطبي بنجاح"));
        }

        // PUT: api/medicalrecords/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateMedicalRecordDTO dto)
        {
            var record = await _context.MedicalRecords
                .Include(m => m.Appointment)
                .FirstOrDefaultAsync(m => m.RecordID == id);
            if (record == null)
                return NotFound(ApiResponse.Fail("السجل الطبي غير موجود"));

            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);
            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (record.Appointment.DoctorID != doctorId)
                    return Forbid();
            }

            record.Diagnosis = dto.Diagnosis ?? record.Diagnosis;
            record.DiagnosisAr = dto.DiagnosisAr ?? record.DiagnosisAr;
            record.TreatmentPlan = dto.TreatmentPlan ?? record.TreatmentPlan;
            record.DoctorNotes = dto.DoctorNotes ?? record.DoctorNotes;
            record.FollowUpDate = dto.FollowUpDate ?? record.FollowUpDate;
            record.FollowUpNotes = dto.FollowUpNotes ?? record.FollowUpNotes;

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok("تم تحديث السجل الطبي بنجاح"));
        }

        // POST: api/medicalrecords/5/prescriptions
        [HttpPost("{id}/prescriptions")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AddPrescription(int id, [FromBody] PrescriptionDTO dto)
        {
            var record = await _context.MedicalRecords
                .Include(m => m.Appointment)
                .FirstOrDefaultAsync(m => m.RecordID == id);
            if (record == null)
                return NotFound(ApiResponse.Fail("السجل الطبي غير موجود"));

            var userId = JwtHelper.GetUserIdFromClaims(User);
            var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
            if (record.Appointment.DoctorID != doctorId)
                return Forbid();

            // حالة الوصفة تُقبل فقط كمسودة أو معلّقة (تمنع حقن حالة مزورة مثل Dispensed)
            var requestedStatus = dto.DispenseStatus ?? "Pending";
            if (requestedStatus != "Pending" && requestedStatus != "Draft")
                return BadRequest(ApiResponse.Fail("حالة الوصفة يجب أن تكون Pending أو Draft فقط"));

            var prescription = new Prescription
            {
                RecordID = id,
                MedicationName = dto.MedicationName,
                Dosage = dto.Dosage ?? string.Empty,
                Frequency = dto.Frequency,
                Duration = dto.Duration,
                Instructions = dto.Instructions,
                DispenseStatus = requestedStatus,
                CreatedAt = DateTime.Now
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { prescriptionId = prescription.PrescriptionID }, "تم إضافة الوصفة بنجاح"));
        }

        // POST: api/medicalrecords/5/send-prescriptions
        [HttpPost("{id}/send-prescriptions")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> SendPrescriptionsToPharmacy(int id)
        {
            var userId = JwtHelper.GetUserIdFromClaims(User);
            var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();

            var record = await _context.MedicalRecords
                .Include(m => m.Appointment)
                .Include(m => m.Prescriptions)
                .FirstOrDefaultAsync(m => m.RecordID == id && m.Appointment.DoctorID == doctorId);

            if (record == null)
                return NotFound(ApiResponse.Fail("السجل الطبي غير موجود أو لا ينتمي لك"));

            var draftPrescriptions = record.Prescriptions.Where(p => p.DispenseStatus == "Draft").ToList();
            if (!draftPrescriptions.Any())
                return BadRequest(ApiResponse.Fail("لا توجد وصفات طبية مسودة لإرسالها"));

            foreach (var p in draftPrescriptions)
            {
                p.DispenseStatus = "Pending";
            }

            // Audit Log
            _context.AuditLogs.Add(new AuditLog
            {
                ActionType = "PrescriptionsSentToPharmacy",
                EntityType = "MedicalRecord",
                EntityID = record.RecordID,
                UserID = userId,
                Details = $"تم إرسال {draftPrescriptions.Count} وصفة طبية للصيدلية من السجل الطبي #{record.RecordID}",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok("تم إرسال الوصفات الطبية إلى الصيدلية بنجاح"));
        }

        // DELETE: api/medicalrecords/prescriptions/5
        [HttpDelete("prescriptions/{prescriptionId}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> DeletePrescription(int prescriptionId)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.MedicalRecord)
                    .ThenInclude(m => m.Appointment)
                .FirstOrDefaultAsync(p => p.PrescriptionID == prescriptionId);
            if (prescription == null)
                return NotFound(ApiResponse.Fail("الوصفة غير موجودة"));

            var userId = JwtHelper.GetUserIdFromClaims(User);
            var role = JwtHelper.GetUserRoleFromClaims(User);
            if (role == "Doctor")
            {
                var doctorId = await _context.DoctorProfiles.Where(d => d.UserID == userId).Select(d => d.DoctorID).FirstOrDefaultAsync();
                if (prescription.MedicalRecord.Appointment.DoctorID != doctorId)
                    return Forbid();
            }

            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("تم حذف الوصفة بنجاح"));
        }
    }
}
