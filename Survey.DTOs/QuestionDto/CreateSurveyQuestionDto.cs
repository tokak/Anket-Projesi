using System.ComponentModel.DataAnnotations;

namespace Survey.DTOs.QuestionDto
{
    public class CreateSurveyQuestionDto
    {
        public int SurveyId { get; set; }
        public string? SurveyTitle { get; set; }
        public List<QuestionInputDto> Questions { get; set; }

    }
}
