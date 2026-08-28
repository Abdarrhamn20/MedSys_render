namespace MedicalSystem.DTOs
{
    public class CreateTelemedicineSessionDTO
    {
        public int AppointmentID { get; set; }
        public string? SessionNotes { get; set; }
    }

    public class TelemedicineSessionDTO
    {
        public int SessionID { get; set; }
        public int AppointmentID { get; set; }
        public string RoomCode { get; set; } = string.Empty;
        public string Status { get; set; } = "Waiting";
        public int CreatedByUserID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string? SessionNotes { get; set; }
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        public string? DoctorSpecialty { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public TimeSpan? AppointmentTime { get; set; }
        public string? AppointmentStatus { get; set; }
    }

    public class EndTelemedicineSessionDTO
    {
        public string? SessionNotes { get; set; }
    }
}
