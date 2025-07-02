namespace Survey.DTOs.TakeSurveyDto
{
    public class SubmitAnswerRequestDto
    {
        public int SurveyId { get; set; }
        public int QuestionId { get; set; }
        public int SelectedOptionId { get; set; }
        public int CurrentQuestionIndex { get; set; }
    }
}
