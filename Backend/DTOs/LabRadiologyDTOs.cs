using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.DTOs
{
    public class CreateLabOrderDTO
    {
        [Required]
        public int PatientUserID { get; set; }

        public int DoctorID { get; set; }

        // التوافق الخلفي: فحص واحد
        public int LabTestID { get; set; }

        // المتقدم: عدة فحوصات/بانلات
        public List<int> LabTestIDs { get; set; } = new();

        public string? ResultNotes { get; set; }
    }

    public class UpdateLabResultDTO
    {
        [Required]
        public string ResultValue { get; set; } = string.Empty;

        public string? TechnicianNotes { get; set; }
    }

    public class LabTestDTO
    {
        [Required]
        public string TestName { get; set; } = string.Empty;

        [Required]
        public string Code { get; set; } = string.Empty;

        public string? Category { get; set; }

        public decimal Price { get; set; } = 25.00m;

        public string? Unit { get; set; }

        public bool IsPanel { get; set; }

        public int? PanelID { get; set; }

        public int? DeviceID { get; set; }

        public List<LabReferenceRangeDTO> ReferenceRanges { get; set; } = new();
    }

    public class LabReferenceRangeDTO
    {
        public string? Gender { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public decimal NormalMin { get; set; }
        public decimal NormalMax { get; set; }
        public string? RangeNotes { get; set; }
    }

    public class CultureSensitivityDTO
    {
        public string? Organism { get; set; }
        public string? GramStain { get; set; }
        public string? CultureStatus { get; set; }
        public string? QuantitativeResult { get; set; }
    }

    public class SensitivityResultDTO
    {
        [Required]
        public string AntibioticName { get; set; } = string.Empty;

        public string? Interpretation { get; set; }

        public decimal? ZoneDiameter { get; set; }
    }

    public class LabDeviceDTO
    {
        [Required]
        public string DeviceName { get; set; } = string.Empty;

        [Required]
        public string DeviceCode { get; set; } = string.Empty;

        public string? DeviceModel { get; set; }

        public string? ConnectionType { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class DeviceCaptureDTO
    {
        [Required]
        public int LabOrderItemID { get; set; }

        [Required]
        public string Value { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }

    public class AddPanelMemberDTO
    {
        [Required]
        public int MemberTestID { get; set; }
    }

    public class CreateRadiologyOrderDTO
    {
        [Required]
        public int PatientUserID { get; set; }

        public int DoctorID { get; set; }

        public int? TemplateID { get; set; }

        [Required]
        public string Modality { get; set; } = "X-Ray"; // X-Ray, CT, MRI, Ultrasound

        [Required]
        public string BodyPart { get; set; } = "Chest";
    }

    public class UpdateRadiologyReportDTO
    {
        [Required]
        public string ReportText { get; set; } = string.Empty;

        public string? ImagePath { get; set; }
    }
}
