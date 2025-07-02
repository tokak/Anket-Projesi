namespace SurveyProject.Entities.Entity
{
    public class AnswerOption
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; }

        // Her cevap seçeneği bir soruya aittir
        public Question Question { get; set; }

        // Bir cevap seçeneği birden fazla kullanıcı yanıtında seçilmiş olabilir
        public ICollection<UserAnswer> UserAnswers { get; set; }
    }
}
