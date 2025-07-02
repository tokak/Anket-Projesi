namespace Survey.DTOs.TakeSurveyDto
{
    public class ListTakeSurveyDto
    {
        public int SurveyId { get; set; }
        public string SurveyTitle { get; set; }
        public string SurveyDescription { get; set; }
        public int TotalQuestions { get; set; }
        public int CurrentQuestionIndex { get; set; } // 0'dan başlar
        public QuestionDto CurrentQuestion { get; set; }
        public Dictionary<int, int> UserAnswers { get; set; } = new Dictionary<int, int>(); // SoruId -> SeçilenCevapId

        public bool ShowWelcomeMessage { get; set; }
    }

    
    public class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<AnswerOptionDto> AnswerOptions { get; set; } = new List<AnswerOptionDto>();
    }

    
    public class AnswerOptionDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    
    public class Survey
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } // Anketin aktifliğini kontrol etmek için
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }

    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int SurveyId { get; set; }
        public Survey Survey { get; set; }
        public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
    }

    public class AnswerOption
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }

    public class UserAnswerDto
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int QuestionId { get; set; }
        public int SelectedOptionId { get; set; }
        public string? UserId { get; set; }
        public DateTime SubmitDate { get; set; }
    }
}
