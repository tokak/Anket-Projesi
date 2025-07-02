using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Survey.DTOs.DashboardDto;
using Survey.Service.Const;
using SurveyProject.DataAccess.Context;
using SurveyProject.Entities.Entity;

namespace SurveyProject.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleConsts.Admin)]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager; // Kullanıcı yönetimi için

        public DashboardController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardSummary = new DashboardSummaryDto();

            // --- Completed Survey Metrics ---

            // Get all unique survey IDs that have at least one user answer
            var surveyIdsWithAnswers = await _context.UserAnswers
                .Select(ua => ua.SurveyId)
                .Distinct()
                .ToListAsync();

            var completedSurveyData = new List<(int SurveyId, int QuestionCount, int AnswerCount)>();

            foreach (var surveyId in surveyIdsWithAnswers)
            {
                var totalQuestionsInSurvey = await _context.Questions
                    .Where(q => q.SurveyId == surveyId)
                    .CountAsync();

                // Group user answers by UserId and SurveyId to check for completion per user per survey
                var userAnswersBySurvey = await _context.UserAnswers
                    .Where(ua => ua.SurveyId == surveyId)
                    .GroupBy(ua => ua.UserId)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        AnswerCount = g.Count(),
                        UniqueQuestionAnsweredCount = g.Select(x => x.QuestionId).Distinct().Count()
                    })
                    .ToListAsync();

                foreach (var userSurveyProgress in userAnswersBySurvey)
                {
                    // A survey is considered completed by a user if they have answered all questions in that survey.
                    if (userSurveyProgress.UniqueQuestionAnsweredCount == totalQuestionsInSurvey && totalQuestionsInSurvey > 0)
                    {
                        // This specific completion instance counts towards TotalCompletedSurveys
                        dashboardSummary.TotalCompletedSurveys++;
                        completedSurveyData.Add((surveyId, totalQuestionsInSurvey, userSurveyProgress.AnswerCount));
                    }
                }
            }

            // Calculate TotalUniqueCompletedSurveys, TotalQuestionsInCompletedSurveys, TotalAnswersInCompletedSurveys
            dashboardSummary.TotalUniqueCompletedSurveys = completedSurveyData.Select(cs => cs.SurveyId).Distinct().Count();
            dashboardSummary.TotalQuestionsInCompletedSurveys = completedSurveyData.Sum(cs => cs.QuestionCount);
            dashboardSummary.TotalAnswersInCompletedSurveys = completedSurveyData.Sum(cs => cs.AnswerCount);


            // --- In-Progress Survey Metrics ---

            // Get all surveys (active ones, assuming you only care about active surveys for in-progress)
            var allActiveSurveys = await _context.TSurveys
                .Where(s => s.IsActive) // Assuming only active surveys can be in-progress
                .Include(s => s.Questions)
                .ToListAsync();

            var inProgressSurveyTracking = new Dictionary<int, HashSet<Guid?>>(); // Key: SurveyId, Value: Set of UserIds who have started this survey

            foreach (var survey in allActiveSurveys)
            {
                var totalQuestionsInSurvey = survey.Questions.Count;

                // Get all user answers for the current survey
                var userAnswersForSurvey = await _context.UserAnswers
                    .Where(ua => ua.SurveyId == survey.Id)
                    .GroupBy(ua => ua.UserId)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        AnswerCount = g.Count(),
                        UniqueQuestionAnsweredCount = g.Select(x => x.QuestionId).Distinct().Count()
                    })
                    .ToListAsync();

                foreach (var userSurveyProgress in userAnswersForSurvey)
                {
                    // An survey is in-progress if a user has answered some questions but not all.
                    // Or if a user has just started (AnswerCount > 0) but not completed.
                    if (userSurveyProgress.UniqueQuestionAnsweredCount > 0 && userSurveyProgress.UniqueQuestionAnsweredCount < totalQuestionsInSurvey)
                    {
                        // This specific instance is in progress
                        dashboardSummary.TotalInProgressSurveys++;
                        if (!inProgressSurveyTracking.ContainsKey(survey.Id))
                        {
                            inProgressSurveyTracking[survey.Id] = new HashSet<Guid?>();
                        }
                        inProgressSurveyTracking[survey.Id].Add(userSurveyProgress.UserId);
                        dashboardSummary.TotalQuestionsInInProgressSurveys += totalQuestionsInSurvey; // Add all questions for the survey being in-progress
                        dashboardSummary.TotalAnswersInInProgressSurveys += userSurveyProgress.AnswerCount;
                    }
                    // Also consider surveys "in-progress" if they have questions but no answers yet (started by someone, but no answers submitted)
                    // This might require a different approach or tracking "survey starts" if not tied to an answer.
                    // For now, we're assuming "in-progress" means at least one answer, but not all.
                }

                // Handle the case where a survey has questions but no user has answered anything yet, but it's conceptually "in progress"
                // This is a bit tricky without a "survey started" event.
                // If you define "in-progress" as "any survey that is active and hasn't been fully completed by anyone yet",
                // then you'd need to consider surveys that have zero answers but are active.
                // For this implementation, "in-progress" implies some user interaction (at least one answer).
                // If you want to count surveys that have *no* answers but are active as "in-progress", you'd add:
                // if (userAnswersForSurvey.Count == 0 && totalQuestionsInSurvey > 0) { /* logic to increment TotalInProgressSurveys etc. */}
            }

            dashboardSummary.TotalUniqueInProgressSurveys = inProgressSurveyTracking.Keys.Count();

            return View(dashboardSummary);
        }
    }
}