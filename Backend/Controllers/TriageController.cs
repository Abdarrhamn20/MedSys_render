using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalSystem.Data;
using MedicalSystem.DTOs;
using MedicalSystem.Helpers;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TriageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TriageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/triage/questions
        [HttpGet("questions")]
        public async Task<IActionResult> GetQuestions()
        {
            var questions = await _context.TriageQuestions
                .Where(q => q.IsActive)
                .OrderBy(q => q.SortOrder)
                .Select(q => new
                {
                    q.QuestionID,
                    q.QuestionText,
                    q.QuestionTextAr,
                    q.Weight,
                    q.Category
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(questions));
        }

        // POST: api/triage/evaluate
        [HttpPost("evaluate")]
        public async Task<IActionResult> Evaluate([FromBody] TriageEvaluateDTO dto)
        {
            // تُحسب النتيجة خادمياً من أوزان الأسئلة الفعلية في قاعدة البيانات،
            // ولا تُقبل أي أوزان قادمة من العميل.
            var activeQuestions = await _context.TriageQuestions
                .Where(q => q.IsActive)
                .ToListAsync();

            var result = TriageEvaluator.Evaluate(activeQuestions, dto.Answers);
            var maxPossibleScore = activeQuestions.Sum(q => q.Weight);

            var response = new
            {
                triageScore = result.Score,
                priorityId = result.PriorityId,
                levelName = result.LevelName,
                levelNameAr = result.LevelNameAr,
                colorCode = result.ColorCode,
                recommendation = result.Recommendation,
                recommendedSpecialties = result.RecommendedSpecialties,
                maxPossibleScore
            };

            return Ok(ApiResponse<object>.Ok(response));
        }
    }
}
