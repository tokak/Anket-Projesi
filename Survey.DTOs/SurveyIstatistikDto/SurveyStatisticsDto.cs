using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.DTOs.SurveyIstatistikDto
{
    // Genel istatistikleri tutan DTO (Data Transfer Object)
    public class SurveyStatisticsDto
    {
        // Diğer özellikleriniz burda olacak...
        public int TotalSurveys { get; set; }
        public int TotalAnswers { get; set; }
        public List<SurveyCompletionRateDto> SurveyCompletionRates { get; set; }
        public List<LatestAnswerDateDto> LatestAnswerDates { get; set; }
        public int TotalRegisteredUsers { get; set; }
        public int ActiveUsersLast30Days { get; set; }
        public List<TopUserAnswerCountDto> TopUsersByAnswerCount { get; set; }

        // Yeni eklenen özellik:
        public List<SurveyAnswerCountDto> SurveyAnswerCounts { get; set; }
        public List<QuestionDto> Questions { get; set; } // Eğer bu özellik yoksa ekleyin (Ortalama hesaplama için)
    }

    public class SurveyCompletionRateDto
    {
        public int SurveyId { get; set; }
        public string SurveyTitle { get; set; }
        public double CompletionRate { get; set; }
    }

    public class LatestAnswerDateDto
    {
        public int SurveyId { get; set; }
        public string SurveyTitle { get; set; }
        public DateTime LastAnswered { get; set; }
    }

    public class TopUserAnswerCountDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public int AnswerCount { get; set; }
    }

    // Yeni DTO sınıfı:
    public class SurveyAnswerCountDto
    {
        public string SurveyTitle { get; set; }
        public int AnswerCount { get; set; }
    }

    // Eğer yoksa ekleyin (Ortalama hesaplama için)
    public class QuestionDto
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public string Text { get; set; }
    }

}
