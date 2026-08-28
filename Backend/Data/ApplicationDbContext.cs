using Microsoft.EntityFrameworkCore;
using MedicalSystem.Models;

namespace MedicalSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<DoctorProfile> DoctorProfiles { get; set; }
        public DbSet<PatientProfile> PatientProfiles { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<TriageQuestion> TriageQuestions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<DispenseRecord> DispenseRecords { get; set; }
        public DbSet<MedicationRequest> MedicationRequests { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<PsychiatricRecord> PsychiatricRecords { get; set; }
        public DbSet<CustomAssessmentTemplate> CustomAssessmentTemplates { get; set; }
        public DbSet<PatientAssessment> PatientAssessments { get; set; }
        public DbSet<SoapNote> SoapNotes { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Bed> Beds { get; set; }
        public DbSet<Admission> Admissions { get; set; }
        public DbSet<InpatientDailyLog> InpatientDailyLogs { get; set; }
        public DbSet<InpatientCareOrder> InpatientCareOrders { get; set; }
        public DbSet<InpatientCareExecution> InpatientCareExecutions { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<DoctorCommission> DoctorCommissions { get; set; }
        public DbSet<LabTest> LabTests { get; set; }
        public DbSet<LabReferenceRange> LabReferenceRanges { get; set; }
        public DbSet<LabOrder> LabOrders { get; set; }
        public DbSet<LabOrderItem> LabOrderItems { get; set; }
        public DbSet<CultureSensitivity> CultureSensitivities { get; set; }
        public DbSet<SensitivityResult> SensitivityResults { get; set; }
        public DbSet<LabDevice> LabDevices { get; set; }
        public DbSet<RadiologyTemplate> RadiologyTemplates { get; set; }
        public DbSet<RadiologyOrder> RadiologyOrders { get; set; }
        public DbSet<TelemedicineSession> TelemedicineSessions { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<WebPushSubscription> WebPushSubscriptions { get; set; }
        public DbSet<ChartAccount> ChartAccounts { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; }
        public DbSet<Treasury> Treasuries { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<InventoryCategory> InventoryCategories { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }

        public DbSet<StockMovementItem> StockMovementItems { get; set; }

        public DbSet<StockCount> StockCounts { get; set; }

        public DbSet<StockCountItem> StockCountItems { get; set; }
        public DbSet<EmployeeProfile> EmployeeProfiles { get; set; }
        public DbSet<EmployeeCourse> EmployeeCourses { get; set; }
        public DbSet<EmployeeLeave> EmployeeLeaves { get; set; }
        public DbSet<SalaryRecord> SalaryRecords { get; set; }
        public DbSet<HealthService> HealthServices { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // PG-timezone-mapping: PostgreSQL ÙŠØ®Ø²Ù‘Ù† ÙƒÙ„ DateTime Ø¨Ù„Ø§ Ù…Ù†Ø·Ù‚Ø© Ø²Ù…Ù†ÙŠØ© (Ù…ÙƒØ§ÙØ¦ datetime2)
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        var columnType = property.GetColumnType();
                        if (columnType == null || columnType.IndexOf("time zone", StringComparison.OrdinalIgnoreCase) >= 0)
                            property.SetColumnType("timestamp without time zone");
                    }
                }
            }

            // === User Configuration ===
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Role).HasDefaultValue("Patient");
                entity.Property(u => u.IsActive).HasDefaultValue(true);
                // الكاشير يرتبط بخزينته المخصصة (لا حذف تسلسلي حتى لا تفقد الخزينة)
                entity.HasOne(u => u.AssignedTreasury)
                      .WithMany()
                      .HasForeignKey(u => u.AssignedTreasuryID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // === User -> DoctorProfile (One-to-One) ===
            modelBuilder.Entity<DoctorProfile>(entity =>
            {
                entity.HasOne(d => d.User)
                      .WithOne(u => u.DoctorProfile)
                      .HasForeignKey<DoctorProfile>(d => d.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // === الموظفون (الموارد البشرية والرواتب) ===
            modelBuilder.Entity<EmployeeProfile>(entity =>
            {
                entity.HasIndex(e => e.EmployeeNumber).IsUnique();
                // قد يكون الموظف مرتبطاً بحساب دخول أو لا (موظف خارج النظام) — بدون حذف تسلسلي
                entity.HasIndex(e => e.UserID).IsUnique();
                entity.HasOne(e => e.User)
                      .WithOne()
                      .HasForeignKey<EmployeeProfile>(e => e.UserID)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.Property(e => e.CompensationModel).HasDefaultValue("FixedSalary");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.BaseSalary).HasDefaultValue(0m);
            });

            modelBuilder.Entity<EmployeeLeave>(entity =>
            {
                entity.HasOne(l => l.ApprovedByUser)
                      .WithMany()
                      .HasForeignKey(l => l.ApprovedByUserID)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(l => l.Status).HasDefaultValue("Pending");
            });

            modelBuilder.Entity<SalaryRecord>(entity =>
            {
                entity.HasIndex(s => new { s.EmployeeID, s.PeriodYear, s.PeriodMonth }).IsUnique();
                entity.HasOne(s => s.JournalEntry)
                      .WithMany()
                      .HasForeignKey(s => s.JournalEntryID)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(s => s.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(s => s.CreatedByUserID)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(s => s.Status).HasDefaultValue("Draft");
            });

            // === Appointment Configuration ===
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasOne(a => a.Patient)
                      .WithMany(p => p.Appointments)
                      .HasForeignKey(a => a.PatientID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Doctor)
                      .WithMany(d => d.Appointments)
                      .HasForeignKey(a => a.DoctorID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Priority)
                      .WithMany(p => p.Appointments)
                      .HasForeignKey(a => a.PriorityID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(a => a.Status).HasDefaultValue("Pending");
                entity.Property(a => a.RowVersion).IsRowVersion();
            });

            // === Appointment -> MedicalRecord (One-to-One) ===
            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.HasOne(m => m.Appointment)
                      .WithOne(a => a.MedicalRecord)
                      .HasForeignKey<MedicalRecord>(m => m.AppID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // === MedicalRecord -> Prescriptions (One-to-Many) ===
            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.HasOne(p => p.MedicalRecord)
                      .WithMany(m => m.Prescriptions)
                      .HasForeignKey(p => p.RecordID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // === MedicalRecord -> PsychiatricRecord (One-to-One Extension) ===
            modelBuilder.Entity<PsychiatricRecord>(entity =>
            {
                entity.HasOne(p => p.MedicalRecord)
                      .WithOne(m => m.PsychiatricRecord)
                      .HasForeignKey<PsychiatricRecord>(p => p.RecordID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // === MedicalRecord -> SoapNote (One-to-One Extension) ===
            modelBuilder.Entity<SoapNote>(entity =>
            {
                entity.HasOne(s => s.MedicalRecord)
                      .WithOne(m => m.SoapNote)
                      .HasForeignKey<SoapNote>(s => s.RecordID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(s => s.Subjective).HasMaxLength(int.MaxValue);
                entity.Property(s => s.Objective).HasMaxLength(int.MaxValue);
                entity.Property(s => s.Assessment).HasMaxLength(int.MaxValue);
                entity.Property(s => s.Plan).HasMaxLength(int.MaxValue);
            });

            // === PatientProfile -> User (RiskLevelUpdatedBy) ===
            modelBuilder.Entity<PatientProfile>(entity =>
            {
                entity.HasOne(p => p.User)
                      .WithOne(u => u.PatientProfile)
                      .HasForeignKey<PatientProfile>(p => p.UserID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(p => p.RiskLevel).HasMaxLength(20).HasDefaultValue("Stable");
                entity.Property(p => p.RiskLevelNotes).HasMaxLength(500);
                entity.HasIndex(p => p.FileNumber).IsUnique();
            });

            // === CustomAssessmentTemplate Configuration ===
            modelBuilder.Entity<CustomAssessmentTemplate>(entity =>
            {
                entity.HasOne(c => c.Doctor)
                      .WithMany()
                      .HasForeignKey(c => c.DoctorID)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // === PatientAssessment Configuration ===
            modelBuilder.Entity<PatientAssessment>(entity =>
            {
                entity.HasOne(pa => pa.PatientUser)
                      .WithMany()
                      .HasForeignKey(pa => pa.PatientUserID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pa => pa.CustomAssessmentTemplate)
                      .WithMany(c => c.PatientAssessments)
                      .HasForeignKey(pa => pa.TemplateID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // === Attachment Configuration ===
            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.HasOne(a => a.MedicalRecord)
                      .WithMany(m => m.Attachments)
                      .HasForeignKey(a => a.RecordID)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(a => a.Patient)
                      .WithMany(p => p.Attachments)
                      .HasForeignKey(a => a.PatientID)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // === Seed Data: Priorities ===
            modelBuilder.Entity<Priority>().HasData(
                new Priority { PriorityID = 1, LevelName = "Normal", LevelNameAr = "عادي", Weight = 1, ColorCode = "#2DC653", Icon = "fa-check-circle" },
                new Priority { PriorityID = 2, LevelName = "Urgent", LevelNameAr = "عاجل", Weight = 2, ColorCode = "#FF9F1C", Icon = "fa-exclamation-triangle" },
                new Priority { PriorityID = 3, LevelName = "Emergency", LevelNameAr = "طوارئ", Weight = 3, ColorCode = "#E63946", Icon = "fa-ambulance" }
            );

            // === Seed Data: Admin User (Password: Admin@123) ===
            // ملاحظة: يُستخدم hash ثابت محسوب مسبقاً لكلمة المرور Admin@123 بدلاً من HashPassword الديناميكي
            // (الذي يولّد ملحاً عشوائياً في كل ترحيل ويسبب تغييرات متكررة في snapshot).
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserID = 1,
                    FullName = "مدير النظام",
                    Email = "admin@medical.com",
                    Password = "$2a$11$HL3jH5enP.qhQRongvmAbO3shF9L2Hh25aK4U17IXSD/T9h3OpHMO",
                    Role = "Admin",
                    Phone = "0500000000",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1)
                }
            );

            // === Seed Data: Triage Questions ===
            modelBuilder.Entity<TriageQuestion>().HasData(
                new TriageQuestion { QuestionID = 1, QuestionText = "Do you have chest pain?", QuestionTextAr = "هل تعاني من ألم في الصدر؟", Weight = 25, Category = "Cardiac", IsActive = true, SortOrder = 1 },
                new TriageQuestion { QuestionID = 2, QuestionText = "Do you have difficulty breathing?", QuestionTextAr = "هل تعاني من صعوبة في التنفس؟", Weight = 25, Category = "Respiratory", IsActive = true, SortOrder = 2 },
                new TriageQuestion { QuestionID = 3, QuestionText = "Do you have severe bleeding?", QuestionTextAr = "هل تعاني من نزيف حاد؟", Weight = 20, Category = "General", IsActive = true, SortOrder = 3 },
                new TriageQuestion { QuestionID = 4, QuestionText = "Do you have a high fever (above 39°C)?", QuestionTextAr = "هل لديك حرارة مرتفعة (فوق 39 درجة)؟", Weight = 15, Category = "General", IsActive = true, SortOrder = 4 },
                new TriageQuestion { QuestionID = 5, QuestionText = "Do you feel dizziness or loss of consciousness?", QuestionTextAr = "هل تشعر بدوخة أو فقدان للوعي؟", Weight = 20, Category = "Neurological", IsActive = true, SortOrder = 5 },
                new TriageQuestion { QuestionID = 6, QuestionText = "Do you have severe abdominal pain?", QuestionTextAr = "هل تعاني من ألم شديد في البطن؟", Weight = 15, Category = "General", IsActive = true, SortOrder = 6 },
                new TriageQuestion { QuestionID = 7, QuestionText = "Do you have a persistent headache?", QuestionTextAr = "هل تعاني من صداع مستمر؟", Weight = 10, Category = "Neurological", IsActive = true, SortOrder = 7 },
                new TriageQuestion { QuestionID = 8, QuestionText = "Have you had a recent injury or accident?", QuestionTextAr = "هل تعرضت لإصابة أو حادث مؤخراً؟", Weight = 15, Category = "General", IsActive = true, SortOrder = 8 },
                new TriageQuestion { QuestionID = 9, QuestionText = "Do you have nausea or vomiting?", QuestionTextAr = "هل تعاني من غثيان أو قيء؟", Weight = 8, Category = "General", IsActive = true, SortOrder = 9 },
                new TriageQuestion { QuestionID = 10, QuestionText = "Do you have any chronic diseases?", QuestionTextAr = "هل لديك أمراض مزمنة؟", Weight = 5, Category = "General", IsActive = true, SortOrder = 10 }
            );

            // === Seed Data: Psychiatric Triage Questions (أسئلة نفسية) ===
            modelBuilder.Entity<TriageQuestion>().HasData(
                new TriageQuestion { QuestionID = 11, QuestionText = "Do you feel depressed or hopeless?", QuestionTextAr = "هل تشعر باكتئاب أو يأس أو فقدان أمل؟", Weight = 20, Category = "Psychiatric", IsActive = true, SortOrder = 11 },
                new TriageQuestion { QuestionID = 12, QuestionText = "Do you feel anxious or nervous most of the time?", QuestionTextAr = "هل تشعر بقلق أو توتر معظم الوقت؟", Weight = 15, Category = "Psychiatric", IsActive = true, SortOrder = 12 },
                new TriageQuestion { QuestionID = 13, QuestionText = "Do you have thoughts of harming yourself or others?", QuestionTextAr = "هل لديك أفكار بإيذاء نفسك أو الآخرين؟", Weight = 30, Category = "Psychiatric", IsActive = true, SortOrder = 13 },
                new TriageQuestion { QuestionID = 14, QuestionText = "Do you see or hear things that others do not?", QuestionTextAr = "هل ترى أو تسمع أشياء لا يراها أو يسمعها الآخرون؟", Weight = 25, Category = "Psychiatric", IsActive = true, SortOrder = 14 },
                new TriageQuestion { QuestionID = 15, QuestionText = "Do you have trouble sleeping or changes in appetite?", QuestionTextAr = "هل تعاني من اضطرابات في النوم أو الشهية؟", Weight = 10, Category = "Psychiatric", IsActive = true, SortOrder = 15 }
            );

            // === Seed Data: Standard Psychiatric Assessment Templates (PHQ-9 + GAD-7) ===
            modelBuilder.Entity<CustomAssessmentTemplate>().HasData(
                // PHQ-9 — Patient Health Questionnaire (9 أسئلة، تقييم 0-3 × 9 = 0-27)
                new CustomAssessmentTemplate
                {
                    TemplateID = 1,
                    DoctorID = null,
                    Title = "مقياس الصحة العامة للاكتئاب (PHQ-9)",
                    Description = "استبيان عالمي معياري لقياس شدة أعراض الاكتئاب خلال آخر أسبوعين. يتألف من 9 أسئلة ويستغرق 3 دقائق.",
                    SchemaJson = PHQ9_SCHEMA_JSON,
                    TemplateType = "PHQ9",
                    IsStandard = true,
                    MaxScore = 27,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                // GAD-7 — Generalized Anxiety Disorder (7 أسئلة، تقييم 0-3 × 7 = 0-21)
                new CustomAssessmentTemplate
                {
                    TemplateID = 2,
                    DoctorID = null,
                    Title = "مقياس القلق المعمم (GAD-7)",
                    Description = "استبيان عالمي معياري لقياس شدة القلق والتوتر خلال آخر أسبوعين. يتألف من 7 أسئلة ويستغرق دقيقتين.",
                    SchemaJson = GAD7_SCHEMA_JSON,
                    TemplateType = "GAD7",
                    IsStandard = true,
                    MaxScore = 21,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1)
                }
            );

            // === DispenseRecord Configuration ===
            modelBuilder.Entity<DispenseRecord>(entity =>
            {
                entity.Property(d => d.TotalPrice).HasColumnType("decimal(18,2)");
            });

            // === Medication Configuration (decimal precision) ===
            modelBuilder.Entity<Medication>(entity =>
            {
                entity.Property(m => m.PurchasePrice).HasColumnType("decimal(18,2)");
                entity.Property(m => m.SellingPrice).HasColumnType("decimal(18,2)");
            });

            // === MedicationRequest Configuration ===
            modelBuilder.Entity<MedicationRequest>(entity =>
            {
                entity.HasOne(mr => mr.DoctorUser)
                      .WithMany()
                      .HasForeignKey(mr => mr.DoctorUserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // === Invoice Configuration ===
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasOne(i => i.PatientUser)
                      .WithMany()
                      .HasForeignKey(i => i.PatientUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(i => i.Appointment)
                      .WithMany()
                      .HasForeignKey(i => i.AppointmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(i => i.DispenseRecord)
                      .WithMany()
                      .HasForeignKey(i => i.DispenseRecordID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // === Inpatient Module Configurations ===
            modelBuilder.Entity<Room>(entity =>
            {
                entity.Property(r => r.DailyRate).HasColumnType("decimal(18,2)");
                entity.HasOne(r => r.Ward)
                      .WithMany(w => w.Rooms)
                      .HasForeignKey(r => r.WardID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Bed>(entity =>
            {
                entity.HasOne(b => b.Room)
                      .WithMany(r => r.Beds)
                      .HasForeignKey(b => b.RoomID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Admission>(entity =>
            {
                entity.HasOne(a => a.Patient)
                      .WithMany()
                      .HasForeignKey(a => a.PatientID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Doctor)
                      .WithMany()
                      .HasForeignKey(a => a.DoctorID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Bed)
                      .WithMany(b => b.Admissions)
                      .HasForeignKey(a => a.BedID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(a => a.RowVersion).IsRowVersion();
            });

            modelBuilder.Entity<InpatientDailyLog>(entity =>
            {
                entity.HasOne(l => l.Admission)
                      .WithMany(a => a.DailyLogs)
                      .HasForeignKey(l => l.AdmissionID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.LoggedByUser)
                      .WithMany()
                      .HasForeignKey(l => l.LoggedByUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InpatientCareOrder>(entity =>
            {
                entity.Property(o => o.UnitPrice).HasColumnType("decimal(18,2)");
                entity.HasOne(o => o.Admission)
                      .WithMany()
                      .HasForeignKey(o => o.AdmissionID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(o => o.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(o => o.CreatedByUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InpatientCareExecution>(entity =>
            {
                entity.HasOne(e => e.Order)
                      .WithMany(o => o.Executions)
                      .HasForeignKey(e => e.OrderID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ExecutedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.ExecutedByUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // === Seed Data: Initial Wards ===
            modelBuilder.Entity<Ward>().HasData(
                new Ward { WardID = 1, WardName = "Surgical Ward", WardNameAr = "جناح الجراحة العامة", GenderType = "Mixed", FloorNumber = 2, IsActive = true },
                new Ward { WardID = 2, WardName = "Internal Medicine Ward", WardNameAr = "جناح الباطنية والمرضى الداخليين", GenderType = "Mixed", FloorNumber = 2, IsActive = true },
                new Ward { WardID = 3, WardName = "Intensive Care Unit (ICU)", WardNameAr = "قسم العناية المركزة (ICU)", GenderType = "Mixed", FloorNumber = 3, IsActive = true }
            );

            // === Seed Data: Initial Rooms ===
            modelBuilder.Entity<Room>().HasData(
                new Room { RoomID = 1, WardID = 1, RoomNumber = "101-VIP", RoomType = "VIP", DailyRate = 500, MaxBeds = 1, IsActive = true },
                new Room { RoomID = 2, WardID = 1, RoomNumber = "102-A", RoomType = "General", DailyRate = 200, MaxBeds = 2, IsActive = true },
                new Room { RoomID = 3, WardID = 2, RoomNumber = "201-A", RoomType = "Private", DailyRate = 350, MaxBeds = 1, IsActive = true },
                new Room { RoomID = 4, WardID = 3, RoomNumber = "ICU-01", RoomType = "ICU", DailyRate = 1000, MaxBeds = 1, IsActive = true }
            );

            // === Seed Data: Initial Beds ===
            modelBuilder.Entity<Bed>().HasData(
                new Bed { BedID = 1, RoomID = 1, BedNumber = "B101-1", Status = "Vacant", Notes = "سرير عناية فاخر" },
                new Bed { BedID = 2, RoomID = 2, BedNumber = "B102-1", Status = "Vacant", Notes = "سرير عادي جانبي" },
                new Bed { BedID = 3, RoomID = 2, BedNumber = "B102-2", Status = "Vacant", Notes = "سرير عادي نافذة" },
                new Bed { BedID = 4, RoomID = 3, BedNumber = "B201-1", Status = "Vacant", Notes = "سرير خاص مفرد" },
                new Bed { BedID = 5, RoomID = 4, BedNumber = "BICU-1", Status = "Vacant", Notes = "سرير عناية مركزة مجهز بمراقبة حيوية" }
            );

            // === Seed Data: System Settings (Dynamic PWA License Flag + Booking Policy) ===
            modelBuilder.Entity<SystemSetting>().HasData(
                new SystemSetting { SettingKey = "EnableMobilePWA", SettingValue = "true", UpdatedAt = DateTime.UtcNow.ToLocalTime() },
                new SystemSetting { SettingKey = "DefaultCommissionRatio", SettingValue = "50", UpdatedAt = DateTime.UtcNow.ToLocalTime() },
                new SystemSetting { SettingKey = "CancelWindowHours", SettingValue = "6", UpdatedAt = DateTime.UtcNow.ToLocalTime() },
                new SystemSetting { SettingKey = "MaxFutureAppointmentsPerPatient", SettingValue = "5", UpdatedAt = DateTime.UtcNow.ToLocalTime() },
                new SystemSetting { SettingKey = "MaxBookingDaysAhead", SettingValue = "30", UpdatedAt = DateTime.UtcNow.ToLocalTime() },
                new SystemSetting { SettingKey = "SlotBufferMinutes", SettingValue = "5", UpdatedAt = DateTime.UtcNow.ToLocalTime() }
            );

            // === LabOrders Foreign Keys (Prevent Multiple Cascade Paths in SQL Server) ===
            modelBuilder.Entity<LabOrder>()
                .HasOne(l => l.PatientUser)
                .WithMany()
                .HasForeignKey(l => l.PatientUserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LabOrder>()
                .HasOne(l => l.Doctor)
                .WithMany()
                .HasForeignKey(l => l.DoctorID)
                .OnDelete(DeleteBehavior.Restrict);

            // === LabOrderItem Configuration (Advanced Lab) ===
            modelBuilder.Entity<LabOrderItem>(entity =>
            {
                entity.HasOne(i => i.LabOrder)
                      .WithMany(o => o.Items)
                      .HasForeignKey(i => i.LabOrderID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(i => i.LabTest)
                      .WithMany()
                      .HasForeignKey(i => i.LabTestID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(i => new { i.LabOrderID, i.LabTestID }).IsUnique();
            });

            // === CultureSensitivity Configuration (Culture & Sensitivity) ===
            modelBuilder.Entity<CultureSensitivity>(entity =>
            {
                entity.HasOne(c => c.LabOrderItem)
                      .WithMany()
                      .HasForeignKey(c => c.LabOrderItemID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(c => c.LabOrderItemID).IsUnique();
            });

            modelBuilder.Entity<SensitivityResult>(entity =>
            {
                entity.HasOne(s => s.CultureSensitivity)
                      .WithMany(c => c.SensitivityResults)
                      .HasForeignKey(s => s.CultureSensitivityID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(s => s.ZoneDiameter).HasPrecision(18, 2);
            });

            // === LabDevice Configuration ===
            modelBuilder.Entity<LabDevice>(entity =>
            {
                entity.HasIndex(d => d.DeviceCode).IsUnique();
            });

            // === LabTest -> Panel (self-reference) ===
            modelBuilder.Entity<LabTest>(entity =>
            {
                entity.HasOne(t => t.ParentPanel)
                      .WithMany(t => t.PanelChildren)
                      .HasForeignKey(t => t.PanelID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // === RadiologyOrders Foreign Keys (Prevent Multiple Cascade Paths in SQL Server) ===
            modelBuilder.Entity<RadiologyOrder>()
                .HasOne(r => r.PatientUser)
                .WithMany()
                .HasForeignKey(r => r.PatientUserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RadiologyOrder>()
                .HasOne(r => r.Doctor)
                .WithMany()
                .HasForeignKey(r => r.DoctorID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RadiologyOrder>()
                .HasOne(r => r.Radiologist)
                .WithMany()
                .HasForeignKey(r => r.RadiologistID)
                .OnDelete(DeleteBehavior.Restrict);

            // === TelemedicineSession Configuration ===
            modelBuilder.Entity<TelemedicineSession>(entity =>
            {
                entity.HasOne(s => s.Appointment)
                      .WithMany()
                      .HasForeignKey(s => s.AppointmentID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(s => s.RoomCode).IsUnique();
            });

            // === UserNotification Configuration ===
            modelBuilder.Entity<UserNotification>(entity =>
            {
                entity.HasOne(n => n.User)
                      .WithMany()
                      .HasForeignKey(n => n.UserID)
                      .OnDelete(DeleteBehavior.Cascade);

                // فهرس لاستعلام سريع عن إشعارات مستخدم وغير المقروءة منها
                entity.HasIndex(n => new { n.UserID, n.IsRead });
            });

            // === WebPushSubscription Configuration ===
            modelBuilder.Entity<WebPushSubscription>(entity =>
            {
                entity.HasOne(s => s.User)
                      .WithMany()
                      .HasForeignKey(s => s.UserID)
                      .OnDelete(DeleteBehavior.Cascade);

                // كل اشتراك (جهاز) فريد برابط الـ endpoint
                entity.HasIndex(s => s.Endpoint).IsUnique();
            });

            // === Accounting Module (شجرة الحسابات والقيود) ===
            modelBuilder.Entity<ChartAccount>(entity =>
            {
                entity.HasIndex(a => a.AccountCode).IsUnique();
                entity.Property(a => a.AccountCode).HasMaxLength(20);
                entity.Property(a => a.AccountName).HasMaxLength(100);
                entity.Property(a => a.AccountNameAr).HasMaxLength(100);
                entity.Property(a => a.AccountType).HasMaxLength(20).HasDefaultValue("Asset");
                entity.Property(a => a.IsActive).HasDefaultValue(true);

                entity.HasOne(a => a.ParentAccount)
                      .WithMany(a => a.Children)
                      .HasForeignKey(a => a.ParentAccountID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<JournalEntry>(entity =>
            {
                entity.HasIndex(e => e.EntryNumber).IsUnique();
                entity.Property(e => e.EntryNumber).HasMaxLength(30);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Draft");

                entity.HasOne(e => e.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedByUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.PostedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.PostedByUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<JournalEntryLine>(entity =>
            {
                entity.Property(l => l.Debit).HasColumnType("decimal(18,2)");
                entity.Property(l => l.Credit).HasColumnType("decimal(18,2)");

                entity.HasOne(l => l.JournalEntry)
                      .WithMany(e => e.Lines)
                      .HasForeignKey(l => l.JournalEntryID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.Account)
                      .WithMany()
                      .HasForeignKey(l => l.AccountID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // === Treasury & Vouchers (الخزائن والسندات) ===
            modelBuilder.Entity<Treasury>(entity =>
            {
                entity.HasIndex(t => t.TreasuryCode).IsUnique();
                entity.Property(t => t.TreasuryCode).HasMaxLength(20);
                entity.Property(t => t.TreasuryName).HasMaxLength(50);
                entity.Property(t => t.TreasuryNameAr).HasMaxLength(50);
                entity.Property(t => t.IsActive).HasDefaultValue(true);

                entity.HasOne(t => t.Account)
                      .WithMany()
                      .HasForeignKey(t => t.AccountID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.HasIndex(v => v.VoucherNumber).IsUnique();
                entity.Property(v => v.VoucherNumber).HasMaxLength(30);
                entity.Property(v => v.VoucherType).HasMaxLength(20);
                entity.Property(v => v.Description).HasMaxLength(200);
                entity.Property(v => v.Amount).HasColumnType("decimal(18,2)");
                entity.Property(v => v.Status).HasMaxLength(20).HasDefaultValue("Draft");

                entity.HasOne(v => v.Treasury)
                      .WithMany()
                      .HasForeignKey(v => v.TreasuryID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(v => v.ToTreasury)
                      .WithMany()
                      .HasForeignKey(v => v.ToTreasuryID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(v => v.Account)
                      .WithMany()
                      .HasForeignKey(v => v.AccountID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(v => v.PatientUser)
                      .WithMany()
                      .HasForeignKey(v => v.PatientUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(v => v.Invoice)
                      .WithMany()
                      .HasForeignKey(v => v.InvoiceID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(v => v.Appointment)
                      .WithMany()
                      .HasForeignKey(v => v.AppointmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(v => v.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(v => v.CreatedByUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(v => v.PostedByUser)
                      .WithMany()
                      .HasForeignKey(v => v.PostedByUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // === Seed Data: خزينة افتراضية (الصندوق الرئيسي) مرتبطة بالحساب 1010 ===
            modelBuilder.Entity<Treasury>().HasData(
                new Treasury { TreasuryID = 1, TreasuryName = "Main Cash", TreasuryNameAr = "الصندوق الرئيسي", TreasuryCode = "CASH-01", AccountID = 2, IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Treasury { TreasuryID = 2, TreasuryName = "Main Bank", TreasuryNameAr = "الحساب البنكي الرئيسي", TreasuryCode = "BANK-01", AccountID = 3, IsActive = true, CreatedAt = new DateTime(2026, 1, 1) }
            );

            // === Inventory Module (المخزن المتكامل) ===
            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.HasIndex(w => w.WarehouseCode).IsUnique();
                entity.Property(w => w.WarehouseCode).HasMaxLength(20);
                entity.Property(w => w.WarehouseName).HasMaxLength(50);
                entity.Property(w => w.WarehouseNameAr).HasMaxLength(50);
                entity.Property(w => w.Location).HasMaxLength(200);
                entity.Property(w => w.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<InventoryCategory>(entity =>
            {
                entity.Property(c => c.CategoryName).HasMaxLength(100);
                entity.Property(c => c.CategoryNameAr).HasMaxLength(100);
                entity.Property(c => c.IsActive).HasDefaultValue(true);

                entity.HasOne(c => c.ParentCategory)
                      .WithMany(c => c.Children)
                      .HasForeignKey(c => c.ParentCategoryID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InventoryItem>(entity =>
            {
                entity.HasIndex(i => i.ItemCode).IsUnique();
                entity.Property(i => i.ItemCode).HasMaxLength(50);
                entity.Property(i => i.ItemName).HasMaxLength(200);
                entity.Property(i => i.ItemNameAr).HasMaxLength(200);
                entity.Property(i => i.Unit).HasMaxLength(50);
                entity.Property(i => i.PurchasePrice).HasColumnType("decimal(18,2)");
                entity.Property(i => i.SellingPrice).HasColumnType("decimal(18,2)");
                entity.Property(i => i.Manufacturer).HasMaxLength(200);
                entity.Property(i => i.IsActive).HasDefaultValue(true);

                entity.HasOne(i => i.Category)
                      .WithMany()
                      .HasForeignKey(i => i.CategoryID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(i => i.Medication)
                      .WithMany()
                      .HasForeignKey(i => i.MedicationID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StockMovement>(entity =>
            {
                entity.HasIndex(m => m.MovementNumber).IsUnique();
                entity.Property(m => m.MovementNumber).HasMaxLength(30);
                entity.Property(m => m.MovementType).HasMaxLength(20);
                entity.Property(m => m.ReferenceType).HasMaxLength(100);
                entity.Property(m => m.Notes).HasMaxLength(300);
                entity.Property(m => m.Status).HasMaxLength(20).HasDefaultValue("Draft");

                entity.HasOne(m => m.Warehouse)
                      .WithMany()
                      .HasForeignKey(m => m.WarehouseID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.ToWarehouse)
                      .WithMany()
                      .HasForeignKey(m => m.ToWarehouseID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(m => m.CreatedByUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.PostedByUser)
                      .WithMany()
                      .HasForeignKey(m => m.PostedByUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StockMovementItem>(entity =>
            {
                entity.Property(i => i.Quantity).HasColumnType("decimal(18,2)");
                entity.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(i => i.Notes).HasMaxLength(200);

                entity.HasOne(i => i.Movement)
                      .WithMany(m => m.Items)
                      .HasForeignKey(i => i.MovementID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(i => i.Item)
                      .WithMany()
                      .HasForeignKey(i => i.ItemID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StockCount>(entity =>
            {
                entity.HasIndex(c => c.StockCountNumber).IsUnique();
                entity.Property(c => c.StockCountNumber).HasMaxLength(30);
                entity.Property(c => c.Notes).HasMaxLength(300);
                entity.Property(c => c.Status).HasMaxLength(20).HasDefaultValue("Draft");

                entity.HasOne(c => c.Warehouse)
                      .WithMany()
                      .HasForeignKey(c => c.WarehouseID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(c => c.CreatedByUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.PostedByUser)
                      .WithMany()
                      .HasForeignKey(c => c.PostedByUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.ReversedByUser)
                      .WithMany()
                      .HasForeignKey(c => c.ReversedByUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StockCountItem>(entity =>
            {
                entity.HasIndex(i => new { i.StockCountID, i.ItemID }).IsUnique();
                entity.Property(i => i.SystemQuantity).HasColumnType("decimal(18,2)");
                entity.Property(i => i.CountedQuantity).HasColumnType("decimal(18,2)");
                entity.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(i => i.Notes).HasMaxLength(200);

                entity.HasOne(i => i.StockCount)
                      .WithMany(c => c.Items)
                      .HasForeignKey(i => i.StockCountID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(i => i.Item)
                      .WithMany()
                      .HasForeignKey(i => i.ItemID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // === Seed Data: مخزن افتراضي (المخزن الرئيسي) ===
            modelBuilder.Entity<Warehouse>().HasData(
                new Warehouse { WarehouseID = 1, WarehouseName = "Main Warehouse", WarehouseNameAr = "المخزن الرئيسي", WarehouseCode = "WARE-01", Location = "الطابق الأرضي", IsActive = true, CreatedAt = new DateTime(2026, 1, 1) }
            );

            // === Seed Data: شجرة حسابات افتراضية (مخطط قياسي للعيادات) ===
            // الأصول (1xxx) — الخصوم (2xxx) — حقوق الملكية (3xxx) — الإيرادات (4xxx) — المصروفات (5xxx)
            modelBuilder.Entity<ChartAccount>().HasData(
                // أصول
                new ChartAccount { AccountID = 1,  AccountCode = "1000", AccountName = "Assets", AccountNameAr = "الأصول", AccountType = "Asset", ParentAccountID = null, IsActive = true },
                new ChartAccount { AccountID = 2,  AccountCode = "1010", AccountName = "Cash on Hand", AccountNameAr = "الصندوق (النقدية)", AccountType = "Asset", ParentAccountID = 1, IsActive = true },
                new ChartAccount { AccountID = 3,  AccountCode = "1020", AccountName = "Bank Accounts", AccountNameAr = "البنوك", AccountType = "Asset", ParentAccountID = 1, IsActive = true },
                new ChartAccount { AccountID = 4,  AccountCode = "1030", AccountName = "Accounts Receivable (Patients)", AccountNameAr = "حسابات قبض (مرضى)", AccountType = "Asset", ParentAccountID = 1, IsActive = true },
                new ChartAccount { AccountID = 5,  AccountCode = "1100", AccountName = "Inventory", AccountNameAr = "المخزون (أدوية ومواد)", AccountType = "Asset", ParentAccountID = 1, IsActive = true },
                // خصوم
                new ChartAccount { AccountID = 6,  AccountCode = "2000", AccountName = "Liabilities", AccountNameAr = "الخصوم", AccountType = "Liability", ParentAccountID = null, IsActive = true },
                new ChartAccount { AccountID = 7,  AccountCode = "2010", AccountName = "Accounts Payable (Suppliers)", AccountNameAr = "حسابات دائنة (موردون)", AccountType = "Liability", ParentAccountID = 6, IsActive = true },
                new ChartAccount { AccountID = 8,  AccountCode = "2020", AccountName = "Accrued Salaries", AccountNameAr = "رواتب ومستحقات مستحقة", AccountType = "Liability", ParentAccountID = 6, IsActive = true },
                new ChartAccount { AccountID = 9,  AccountCode = "2030", AccountName = "Accrued Doctor Commissions", AccountNameAr = "عمولات أطباء مستحقة", AccountType = "Liability", ParentAccountID = 6, IsActive = true },
                // حقوق ملكية
                new ChartAccount { AccountID = 10, AccountCode = "3000", AccountName = "Equity", AccountNameAr = "حقوق الملكية", AccountType = "Equity", ParentAccountID = null, IsActive = true },
                new ChartAccount { AccountID = 11, AccountCode = "3010", AccountName = "Owner's Capital", AccountNameAr = "رأس المال", AccountType = "Equity", ParentAccountID = 10, IsActive = true },
                new ChartAccount { AccountID = 12, AccountCode = "3020", AccountName = "Retained Earnings", AccountNameAr = "أرباح أو خسائر مرحّلة", AccountType = "Equity", ParentAccountID = 10, IsActive = true },
                // إيرادات
                new ChartAccount { AccountID = 13, AccountCode = "4000", AccountName = "Revenues", AccountNameAr = "الإيرادات", AccountType = "Revenue", ParentAccountID = null, IsActive = true },
                new ChartAccount { AccountID = 14, AccountCode = "4010", AccountName = "Consultation Revenue", AccountNameAr = "إيرادات الكشوفات والعيادة", AccountType = "Revenue", ParentAccountID = 13, IsActive = true },
                new ChartAccount { AccountID = 15, AccountCode = "4020", AccountName = "Pharmacy Revenue", AccountNameAr = "إيرادات الصيدلية", AccountType = "Revenue", ParentAccountID = 13, IsActive = true },
                new ChartAccount { AccountID = 16, AccountCode = "4030", AccountName = "Laboratory Revenue", AccountNameAr = "إيرادات المختبر", AccountType = "Revenue", ParentAccountID = 13, IsActive = true },
                new ChartAccount { AccountID = 17, AccountCode = "4040", AccountName = "Radiology Revenue", AccountNameAr = "إيرادات الأشعة", AccountType = "Revenue", ParentAccountID = 13, IsActive = true },
                new ChartAccount { AccountID = 18, AccountCode = "4050", AccountName = "Inpatient Revenue", AccountNameAr = "إيرادات الإيواء والتنويم", AccountType = "Revenue", ParentAccountID = 13, IsActive = true },
                // مصروفات
                new ChartAccount { AccountID = 19, AccountCode = "5000", AccountName = "Expenses", AccountNameAr = "المصروفات", AccountType = "Expense", ParentAccountID = null, IsActive = true },
                new ChartAccount { AccountID = 20, AccountCode = "5010", AccountName = "Salaries Expense", AccountNameAr = "مصروف رواتب الموظفين", AccountType = "Expense", ParentAccountID = 19, IsActive = true },
                new ChartAccount { AccountID = 21, AccountCode = "5020", AccountName = "Doctor Commissions Expense", AccountNameAr = "مصروف عمولات الأطباء", AccountType = "Expense", ParentAccountID = 19, IsActive = true },
                new ChartAccount { AccountID = 22, AccountCode = "5030", AccountName = "Rent Expense", AccountNameAr = "مصروف الإيجار", AccountType = "Expense", ParentAccountID = 19, IsActive = true },
                new ChartAccount { AccountID = 23, AccountCode = "5040", AccountName = "Utilities Expense", AccountNameAr = "مصروف الكهرباء والماء", AccountType = "Expense", ParentAccountID = 19, IsActive = true },
                new ChartAccount { AccountID = 24, AccountCode = "5050", AccountName = "Maintenance Expense", AccountNameAr = "مصروف الصيانة والتجهيزات", AccountType = "Expense", ParentAccountID = 19, IsActive = true },
                new ChartAccount { AccountID = 25, AccountCode = "5060", AccountName = "General Expense", AccountNameAr = "مصروفات عامة ومتنوعة", AccountType = "Expense", ParentAccountID = 19, IsActive = true }
            );
        }

        // ==================================================
        //  Standard Assessment Schemas (PHQ-9 & GAD-7)
        //  Structure: { questions: [{ id, text, type, options, weight }] }
        //  type="scoring" يعني أن الخيارات لها أوزان رقمية
        // ==================================================

        private const string PHQ9_SCHEMA_JSON = @"{
  ""questions"": [
    { ""id"": 1, ""text"": ""نشاط أو اهتمام أقل بالأشياء عادةً ما تستمتع بها"", ""type"": ""scoring"", ""options"": [""لا إطلاقاً = 0"", ""عدة أيام = 1"", ""أكثر من نصف الأيام = 2"", ""تقريباً كل يوم = 3""], ""weights"": [0, 1, 2, 3] },
    { ""id"": 2, ""text"": ""شعور بالاكتئاب أو اليأس أو قلة الأمل"", ""type"": ""scoring"", ""options"": [""لا إطلاقاً = 0"", ""عدة أيام = 1"", ""أكثر من نصف الأيام = 2"", ""تقريباً كل يوم = 3""], ""weights"": [0, 1, 2, 3] },
    { ""id"": 3, ""text"": ""صعوبة في النوم أو البقاء نائماً أو النوم المفرط"", ""type"": ""scoring"", ""options"": [""لا إطلاقاً = 0"", ""عدة أيام = 1"", ""أكثر من نصف الأيام = 2"", ""تقريباً كل يوم = 3""], ""weights"": [0, 1, 2, 3] },
    { ""id"": 4, ""text"": ""الشعور بالإرهايد أو ضعف الطاقة"", ""type"": ""scoring"", ""options"": [""لا إطلاقاً = 0"", ""عدة أيام = 1"", ""أكثر من نصف الأيام = 2"", ""تقريباً كل يوم = 3""], ""weights"": [0, 1, 2, 3] },
    { ""id"": 5, ""text"": ""قلة الشهية أو الإفراط في الأكل"", ""type"": ""scoring"", ""options"": [""لا إطلاقاً = 0"", ""عدة أيام = 1"", ""أكثر من نصف الأيام = 2"", ""تقريباً كل يوم = 3""], ""weights"": [0, 1, 2, 3] },
    { ""id"": 6, ""text"": ""تقدير سلبى لذاتك (أشعر أنني فاشل أو لقد خيّبت ظروف عائلتي)"", ""type"": ""scoring"", ""options"": [""لا إطلاقاً = 0"", ""عدة أيام = 1"", ""أكثر من نصف الأيام = 2"", ""تقريباً كل يوم = 3""], ""weights"": [0, 1, 2, 3] },
    { ""id"": 7, ""text"": ""صعوبة في التركيز على الأنشطة مثل القراءة أو التلفاز"", ""type"": ""scoring"", ""options"": [""لا إطلاقاً = 0"", ""عدة أيام = 1"", ""أكثر من نصف الأيام = 2"", ""تقريباً كل يوم = 3""], ""weights"": [0, 1, 2, 3] },
    { ""id"": 8, ""text"": ""تتحرك أو تتحدث ببطء لدرجة ملاحظة الآخرين، أو العكس، تتحرك بضجر أكثر من المعتاد"", ""type"": ""scoring"", ""options"": [""لا إطلاقاً = 0"", ""عدة أيام = 1"", ""أكثر من نصف الأيام = 2"", ""تقريباً كل يوم = 3""], ""weights"": [0, 1, 2, 3] },
    { ""id"": 9, ""text"": ""أفكار بأنك قد تتأذى أو أنك قد تؤذى نفسك بطريقة ما"", ""type"": ""scoring"", ""options"": [""لا إطلاقاً = 0"", ""عدة أيام = 1"", ""أكثر من نصف الأيام = 2"", ""تقريباً كل يوم = 3""], ""weights"": [0, 1, 2, 3] }
  ],
  ""scoring"": {
    ""min"": 0,
    ""max"": 27,
    ""ranges"": [
      { ""min"": 0,  ""max"": 4,  ""label"": ""الحد الأدنى من أعراض الاكتئاب"",        ""color"": ""#2DC653"", ""recommendation"": ""لا يتطلب تدخلاً علاجياً، مراقبة دورية."" },
      { ""min"": 5,  ""max"": 9,  ""label"": ""أعراض اكتئاب خفيفة"",                 ""color"": ""#FF9F1C"", ""recommendation"": ""يُوصى بالمتابعة مع طبيب مختص للدعم النفسي."" },
      { ""min"": 10, ""max"": 14, ""label"": ""أعراض اكتئاب متوسطة"",                ""color"": ""#FF6B35"", ""recommendation"": ""توصية بتقييم سريري وعلاج دوائي محتمل."" },
      { ""min"": 15, ""max"": 19, ""label"": ""أعراض اكتئاب متوسطة الشدة"",          ""color"": ""#E63946"", ""recommendation"": ""توصية بعلاج دوائي فوري + علاج سلوكي معرفي."" },
      { ""min"": 20, ""max"": 27, ""label"": ""أعراض اكتئاب شديدة"",                 ""color"": ""#9B2D30"", ""recommendation"": ""توصية عاجلة بتدخل طبي نفسي مكثف وتقييم خطر السلوك الانتحاري."" }
    ]
  }
}";

        private const string GAD7_SCHEMA_JSON = @"{
  ""questions"": [
    { ""id"": 1, ""text"": ""الشعور بالتوتر أو القلق أو العصبية"", ""type"": ""scoring"", ""options"": [""لا إطلاقاً = 0"", ""عدة أيام = 1"", ""أكثر من نصف الأيام = 2"", ""تقريباً كل يوم = 3""], ""weights"": [0, 1, 2, 3] },
    { ""id"": 2, ""text"": ""عدم القدرة على إيقاف القلق أو التحكم به"", ""type"": ""scoring"", ""options"": [""لا إطلاقاً = 0"", ""عدة أيام = 1"", ""أكثر من نصف الأيام = 2"", ""تقريباً كل يوم = 3""], ""weights"": [0, 1, 2, 3] },
    { ""id"": 3, ""text"": ""القلق المفرط على أشياء مختلفة"", ""type"": ""scoring"", ""options"": [""لا إطلاقاً = 0"", ""عدة أيام = 1"", ""أكثر من نصف الأيام = 2"", ""تقريباً كل يوم = 3""], ""weights"": [0, 1, 2, 3] },
    { ""id"": 4, ""text"": ""صعوبة في الاسترخاء"", ""type"": ""scoring"", ""options"": [""لا إطلاقاً = 0"", ""عدة أيام = 1"", ""أكثر من نصف الأيام = 2"", ""تقريباً كل يوم = 3""], ""weights"": [0, 1, 2, 3] },
    { ""id"": 5, ""text"": ""الشعور بالضجر لدرجة يصعب الجلوس في مكان"", ""type"": ""scoring"", ""options"": [""لا إطلاقاً = 0"", ""عدة أيام = 1"", ""أكثر من نصف الأيام = 2"", ""تقريباً كل يوم = 3""], ""weights"": [0, 1, 2, 3] },
    { ""id"": 6, ""text"": ""الشعور بالانزعاج أو توقع حدوث شيء سيء بسهولة"", ""type"": ""scoring"", ""options"": [""لا إطلاقاً = 0"", ""عدة أيام = 1"", ""أكثر من نصف الأيام = 2"", ""تقريباً كل يوم = 3""], ""weights"": [0, 1, 2, 3] },
    { ""id"": 7, ""text"": ""الشعور بالخوف أو الرعب بدون سبب واضح"", ""type"": ""scoring"", ""options"": [""لا إطلاقاً = 0"", ""عدة أيام = 1"", ""أكثر من نصف الأيام = 2"", ""تقريباً كل يوم = 3""], ""weights"": [0, 1, 2, 3] }
  ],
  ""scoring"": {
    ""min"": 0,
    ""max"": 21,
    ""ranges"": [
      { ""min"": 0,  ""max"": 4,  ""label"": ""الحد الأدنى من أعراض القلق"",        ""color"": ""#2DC653"", ""recommendation"": ""لا يتطلب تدخلاً علاجياً، مراقبة دورية."" },
      { ""min"": 5,  ""max"": 9,  ""label"": ""أعراض قلق خفيفة"",                 ""color"": ""#FF9F1C"", ""recommendation"": ""يُوصى بالمتابعة مع طبيب مختص للدعم النفسي."" },
      { ""min"": 10, ""max"": 14, ""label"": ""أعراض قلق متوسطة"",                ""color"": ""#FF6B35"", ""recommendation"": ""توصية بتقييم سريري وعلاج دوائي محتمل."" },
      { ""min"": 15, ""max"": 21, ""label"": ""أعراض قلق شديدة"",                 ""color"": ""#E63946"", ""recommendation"": ""توصية عاجلة بتدخل طبي نفسي مكثف وتقييم خطر الحالة."" }
    ]
  }
}";
    }
}
