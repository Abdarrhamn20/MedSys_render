namespace MedicalSystem.DTOs
{
    // === Triage DTOs ===
    public class TriageEvaluateDTO
    {
        public List<TriageAnswerDTO> Answers { get; set; } = new();
    }

    public class TriageAnswerDTO
    {
        public int QuestionID { get; set; }
        public bool Answer { get; set; }
        public int Weight { get; set; }
    }

    // === Appointment DTOs ===
    public class CreateAppointmentDTO
    {
        public int DoctorID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string? Notes { get; set; }
        // نوع الكشف: Online (استشارة فيديو عن بعد) أو WalkIn (حضور للعيادة). الافتراضي Online لأنها منظومة عيادة افتراضية.
        public string AppointmentType { get; set; } = "Online";
        // تُحسب النتيجة والأولوية خادمياً من هذه الإجابات ولا تُقبل من العميل.
        public List<TriageAnswerDTO> Answers { get; set; } = new();
    }

    public class UpdateAppointmentStatusDTO
    {
        public string Status { get; set; } = string.Empty;
        public string? CancellationReason { get; set; }
    }
}
