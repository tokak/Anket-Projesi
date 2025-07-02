using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Survey.DTOs.SurveyIstatistikDto;
using SurveyProject.DataAccess.Context;

namespace SurveyProject.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ResultsController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public ResultsController(AppDbContext context)
        {
            _appDbContext = context;
        }

        public IActionResult Index()
        {
            var totalSurveys = _appDbContext.TSurveys.Count();
            var totalAnswers = _appDbContext.UserAnswers.Count();

            var surveyCompletionRates = _appDbContext.TSurveys
         .Select(s => new SurveyCompletionRateDto
         {
             SurveyId = s.Id,
             SurveyTitle = s.Title,
             CompletionRate = _appDbContext.Questions.Count(q => q.SurveyId == s.Id) > 0
                                 ? (double)_appDbContext.UserAnswers.Count(ua => ua.SurveyId == s.Id) /
                                   _appDbContext.Questions.Count(q => q.SurveyId == s.Id)
                                 : 0 // Eğer hiç soru yoksa tamamlanma oranını 0 olarak ayarla
         })
         .ToList();

            var latestAnswerDates = _appDbContext.TSurveys
                .Select(s => new LatestAnswerDateDto
                {
                    SurveyId = s.Id,
                    SurveyTitle = s.Title,
                    LastAnswered = _appDbContext.UserAnswers
                        .Where(ua => ua.SurveyId == s.Id)
                        .Max(ua => (DateTime?)ua.SubmitDate) ?? DateTime.MinValue
                })
                .OrderByDescending(lad => lad.LastAnswered)
                .Take(3)
            .ToList();

            var totalRegisteredUsers = _appDbContext.Users.Count();
            var activeUsersLast30Days = _appDbContext.UserAnswers
                .Where(ua => ua.SubmitDate >= DateTime.Now.AddDays(-30))
                .Select(ua => ua.UserId)
                .Distinct()
            .Count();

            var topUsersByAnswerCount = _appDbContext.UserAnswers
                .Where(ua => ua.UserId != null)
                .GroupBy(ua => new { ua.UserId, ua.AppUser.UserName })
                .Select(g => new TopUserAnswerCountDto
                {
                    UserId = g.Key.UserId.Value,
                    UserName = g.Key.UserName,
                    AnswerCount = g.Count()
                })
                .OrderByDescending(tu => tu.AnswerCount)
                .Take(3)
                .ToList();

            // Yeni eklenen kısım: Anket bazında toplam cevap sayılarını al
            var surveyAnswerCounts = _appDbContext.TSurveys
                .Select(s => new SurveyAnswerCountDto
                {
                    SurveyTitle = s.Title,
                    AnswerCount = _appDbContext.UserAnswers.Count(ua => ua.SurveyId == s.Id)
                })
            .ToList();

            var questions = _appDbContext.Questions.ToList().Select(q => new QuestionDto { Id = q.Id, SurveyId = q.SurveyId, Text = q.Text }).ToList();


            var viewModel = new SurveyStatisticsDto
            {
                TotalSurveys = totalSurveys,
                TotalAnswers = totalAnswers,
                SurveyCompletionRates = surveyCompletionRates,
                LatestAnswerDates = latestAnswerDates,
                TotalRegisteredUsers = totalRegisteredUsers,
                ActiveUsersLast30Days = activeUsersLast30Days,
                TopUsersByAnswerCount = topUsersByAnswerCount,
                SurveyAnswerCounts = surveyAnswerCounts, // Yeni eklenen
                Questions = questions // Eğer yoksa ekleyin
            };

            return View(viewModel);
        }

    }
}
