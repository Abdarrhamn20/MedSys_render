using MedicalSystem.DTOs;
using MedicalSystem.Models;

namespace MedicalSystem.Helpers
{
    public class TriageResult
    {
        public int Score { get; set; }
        public int PriorityId { get; set; }
        public string LevelName { get; set; } = string.Empty;
        public string LevelNameAr { get; set; } = string.Empty;
        public string ColorCode { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
        public List<string> RecommendedSpecialties { get; set; } = new();
    }

    public static class TriageEvaluator
    {
        public const int EmergencyThreshold = 50;
        public const int UrgentThreshold = 25;

        // خريطة فئات الأسئلة إلى كلمات مفتاحية للتخصصات الطبية المتعلقة بحالة المريض
        private static readonly Dictionary<string, List<string>> CategorySpecialtyMap = new()
        {
            { "Cardiac", new List<string> { "قلب" } },
            { "Respiratory", new List<string> { "صدر" } },
            { "Neurological", new List<string> { "أعصاب" } },
            { "Psychiatric", new List<string> { "نفسي" } },
            { "General", new List<string> { "عام", "باطني", "أسرة" } }
        };

        // يحسب النتيجة من أوزان الأسئلة الفعلية في قاعدة البيانات فقط،
        // ويتجاهل أي أوزان مرسلة من العميل.
        public static TriageResult Evaluate(IEnumerable<TriageQuestion> activeQuestions, IEnumerable<TriageAnswerDTO>? answers)
        {
            var questionMap = activeQuestions.ToDictionary(q => q.QuestionID);

            int totalScore = 0;
            var answeredCategories = new HashSet<string>();
            if (answers != null)
            {
                foreach (var a in answers.Where(x => x.Answer))
                {
                    if (questionMap.TryGetValue(a.QuestionID, out var question))
                    {
                        totalScore += question.Weight;
                        answeredCategories.Add(question.Category);
                    }
                }
            }

            // التخصصات الموصى بها تُشتق من فئات الأسئلة التي أجاب عليها المريض بنعم
            var recommendedSpecialties = new List<string>();
            foreach (var cat in answeredCategories)
            {
                if (CategorySpecialtyMap.TryGetValue(cat, out var keywords))
                    recommendedSpecialties.AddRange(keywords);
            }
            if (recommendedSpecialties.Count == 0)
                recommendedSpecialties.AddRange(CategorySpecialtyMap["General"]);

            if (totalScore >= EmergencyThreshold)
            {
                recommendedSpecialties.Add("طوارئ");
                return new TriageResult
                {
                    Score = totalScore,
                    PriorityId = 3,
                    LevelName = "Emergency",
                    LevelNameAr = "طوارئ",
                    ColorCode = "#E63946",
                    Recommendation = "حالتك تتطلب رعاية طبية فورية. سيتم تحويلك للمسار السريع للربط بأول طبيب متاح.",
                    RecommendedSpecialties = recommendedSpecialties.Distinct().ToList()
                };
            }

            if (totalScore >= UrgentThreshold)
            {
                return new TriageResult
                {
                    Score = totalScore,
                    PriorityId = 2,
                    LevelName = "Urgent",
                    LevelNameAr = "عاجل",
                    ColorCode = "#FF9F1C",
                    Recommendation = "حالتك تتطلب اهتماماً طبياً قريباً. سيتم تمييز موعدك بأولوية عاجلة.",
                    RecommendedSpecialties = recommendedSpecialties.Distinct().ToList()
                };
            }

            return new TriageResult
            {
                Score = totalScore,
                PriorityId = 1,
                LevelName = "Normal",
                LevelNameAr = "عادي",
                ColorCode = "#2DC653",
                Recommendation = "حالتك مستقرة. يمكنك حجز موعد في الجدول الاعتيادي.",
                RecommendedSpecialties = recommendedSpecialties.Distinct().ToList()
            };
        }
    }
}
