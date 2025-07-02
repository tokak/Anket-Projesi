using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Survey.DTOs.UserSurveyDto;
using Survey.Service.Const;
using SurveyProject.DataAccess.Context;

namespace SurveyProject.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleConsts.Admin)]
    public class UserSurveyController : Controller
    {
        private readonly AppDbContext  _appDbContext;

        public UserSurveyController(AppDbContext context)
        {
            _appDbContext = context;
        }

        public IActionResult Index()
        {
            var intermediateResult = (from ua in _appDbContext.UserAnswers
                                      join survey in _appDbContext.TSurveys on ua.SurveyId equals survey.Id
                                      join appUser in _appDbContext.AppUsers on ua.UserId equals appUser.Id
                                      group ua by new
                                      {
                                          appUser.Id,
                                          FullName = appUser.FirstName + " "+ appUser.LastName,
                                          SurveyTitle = survey.Title,
                                          SurveyLink = survey.Url,
                                          SurveyId = survey.Id
                                      } into groupedAnswers
                                      select new
                                      {
                                          FullName = groupedAnswers.Key.FullName,
                                          SurveyTitle = groupedAnswers.Key.SurveyTitle,
                                          SurveyLink = groupedAnswers.Key.SurveyLink,
                                          SurveyId = groupedAnswers.Key.SurveyId,
                                          QuestionsAnswered = groupedAnswers.Select(a => a.QuestionId).Distinct().Count(),
                                          CompletionDate = groupedAnswers.Max(a => a.SubmitDate),
                                          TotalQuestions = _appDbContext.Questions.Count(q => q.SurveyId == groupedAnswers.Key.SurveyId) // Calculate TotalQuestions here
                                      })
                                      .ToList();

            var userSurveyDtos = intermediateResult
                                    .AsEnumerable()
                                    .Select(result => new UserSurveyDto
                                    {
                                        FullName = result.FullName,
                                        SurveyTitle = result.SurveyTitle,
                                        SurveyLink = result.SurveyLink,
                                        CompletionDate = result.QuestionsAnswered == result.TotalQuestions ? result.CompletionDate : (DateTime?)null,
                                        QuestionsAnswered = result.QuestionsAnswered,
                                        TotalQuestions = result.TotalQuestions,
                                        IsCompleted = result.QuestionsAnswered == result.TotalQuestions,
                                        State = result.QuestionsAnswered == result.TotalQuestions ? "Bitirdi" : (result.QuestionsAnswered > 0 ? "Yarıda Kaldı" : "Başlamamış")
                                    });

            return View(userSurveyDtos.ToList());
        }
    }
}
