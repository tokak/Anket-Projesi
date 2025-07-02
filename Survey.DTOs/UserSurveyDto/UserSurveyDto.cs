namespace Survey.DTOs.UserSurveyDto
{
    public class UserSurveyDto
    {
        public string FullName { get; set; }
        public string SurveyTitle { get; set; }
        public string SurveyLink { get; set; }
        public DateTime? CompletionDate { get; set; } // anket bitirme tarihi
        public int QuestionsAnswered { get; set; }
        public int TotalQuestions { get; set; }
        public bool IsCompleted { get; set; }
        public string State { get; set; } // anket durumu bitirdi yarıda kaldı başlamamış
    }
}
