using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.DTOs
{
    public class CreateWardDTO
    {
        [Required(ErrorMessage = "اسم القسم بالإنجليزي مطلوب")]
        public string WardName { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم القسم بالعربي مطلوب")]
        public string WardNameAr { get; set; } = string.Empty;

        public string GenderType { get; set; } = "Mixed";
        public int FloorNumber { get; set; } = 1;
    }

    public class CreateRoomDTO
    {
        [Required]
        public int WardID { get; set; }

        [Required(ErrorMessage = "رقم الغرفة مطلوب")]
        public string RoomNumber { get; set; } = string.Empty;

        public string RoomType { get; set; } = "General";
        public decimal DailyRate { get; set; } = 0;
        public int MaxBeds { get; set; } = 2;
    }

    public class CreateBedDTO
    {
        [Required]
        public int RoomID { get; set; }

        [Required(ErrorMessage = "رقم السرير مطلوب")]
        public string BedNumber { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }

    public class CreateAdmissionDTO
    {
        [Required(ErrorMessage = "المريض مطلوب")]
        public int PatientID { get; set; }

        [Required(ErrorMessage = "الطبيب المسؤول مطلوب")]
        public int DoctorID { get; set; }

        [Required(ErrorMessage = "السرير المحدد مطلوب")]
        public int BedID { get; set; }

        [Required(ErrorMessage = "سبب التنويم مطلوب")]
        public string AdmissionReason { get; set; } = string.Empty;
    }

    public class DischargeAdmissionDTO
    {
        [Required(ErrorMessage = "ملخص وتقارير الخروج مطلوبة")]
        public string DischargeSummary { get; set; } = string.Empty;
    }

    public class CreateDailyLogDTO
    {
        public string? Temperature { get; set; }
        public string? BloodPressure { get; set; }
        public string? PulseRate { get; set; }
        public string? OxygenLevel { get; set; }
        public string? DoctorNotes { get; set; }
        public string? NursingNotes { get; set; }
    }

    public class CreateCareOrderDTO
    {
        [Required(ErrorMessage = "المريض المنوم مطلوب")]
        public int AdmissionID { get; set; }

        public int? HealthServiceID { get; set; }

        [Required(ErrorMessage = "نوع الإجراء/الخدمة مطلوب")]
        public string OrderType { get; set; } = "Medication"; // Medication, VitalCheck, NursingProcedure, Diet, IVFluid, HealthService

        [Required(ErrorMessage = "تفاصيل ووصف الخدمة أو الجرعة مطلوب")]
        public string OrderDescription { get; set; } = string.Empty;

        public string Frequency { get; set; } = "Once"; // Once, Every4Hours, Every8Hours, Every12Hours, Daily

        public DateTime ScheduledTime { get; set; } = DateTime.UtcNow;

        public decimal UnitPrice { get; set; } = 0.00m;
    }

    public class ExecuteCareOrderDTO
    {
        public string? Notes { get; set; }
        public string? VitalTemperature { get; set; }
        public string? VitalBloodPressure { get; set; }
        public string? VitalPulse { get; set; }
        public string? VitalOxygen { get; set; }
    }
}
