namespace MedicalSystem.DTOs
{
    // === Doctor DTOs ===
    public class DoctorUpdateDTO
    {
        public string? Specialty { get; set; }
        public string? LicenseNumber { get; set; }
        public bool EmergencyReady { get; set; }
        public string? Bio { get; set; }
        public string? AvailableDays { get; set; }
        public TimeSpan? WorkStartTime { get; set; }
        public TimeSpan? WorkEndTime { get; set; }
        public int ConsultationDurationMinutes { get; set; }
        public decimal ConsultationFee { get; set; }
    }

    // === Patient DTOs ===
    public class PatientUpdateDTO
    {
        // التركيبة الاسمية الليبية
        public string? FirstName { get; set; }
        public string? FatherName { get; set; }
        public string? GrandfatherName { get; set; }
        public string? FamilyName { get; set; }
        public string? FullName { get; set; }
        public string? BloodType { get; set; }
        public string? ChronicDiseases { get; set; }
        public string? Allergies { get; set; }
        public string? GeneralNotes { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
    }

    public class MergePatientsDTO
    {
        public int SourcePatientID { get; set; }
        public int TargetPatientID { get; set; }
    }
}
