namespace SurveyProject.Entities.Entity
{
    public class Question
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public string Text { get; set; }

        // Her soru bir ankete aittir
        public TSurvey Survey { get; set; }

        // Bir sorunun birden fazla cevap seçeneği olabilir
        public ICollection<AnswerOption> AnswerOptions { get; set; }

        // Bir sorunun birden fazla kullanıcı yanıtı olabilir (bu soruya verilen yanıtlar)
        public ICollection<UserAnswer> UserAnswers { get; set; }
    }
}
