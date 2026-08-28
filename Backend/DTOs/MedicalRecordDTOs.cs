namespace MedicalSystem.DTOs
{
    public class CreateMedicalRecordDTO
    {
        public int AppID { get; set; }
        public string? Diagnosis { get; set; }
        public string? DiagnosisAr { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? DoctorNotes { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public string? FollowUpNotes { get; set; }
        public List<PrescriptionDTO>? Prescriptions { get; set; }
        public bool SendToPharmacy { get; set; } = true;
    }

    public class PrescriptionDTO
    {
        public string MedicationName { get; set; } = string.Empty;
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public string? Duration { get; set; }
        public string? Instructions { get; set; }
        public string? DispenseStatus { get; set; }
    }
}
