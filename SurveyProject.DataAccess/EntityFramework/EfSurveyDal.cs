using Microsoft.EntityFrameworkCore;
using SurveyProject.DataAccess.Abstract;
using SurveyProject.DataAccess.Context;
using SurveyProject.DataAccess.Repositories;
using SurveyProject.Entities.Entity;

namespace SurveyProject.DataAccess.EntityFramework
{
    public class EfSurveyDal : GenericRepository<TSurvey>, ISurveyDal
    {
        public EfSurveyDal(AppDbContext context) : base(context)
        {
        }

        public void DeleteUserSurvey(int surveyId)
        {
            var userAnswersToDelete = _context.UserAnswers.Where(x => x.SurveyId == surveyId);
            _context.UserAnswers.RemoveRange(userAnswersToDelete);
            _context.SaveChanges();
        }

        public async Task<TSurvey> GetSurveyWithQuestionsAndOptionsAsync(int surveyId)
        {
            return  await _context.TSurveys 
                              .Include(s => s.Questions) 
                                  .ThenInclude(q => q.AnswerOptions) 
                              .FirstOrDefaultAsync(s => s.Id == surveyId);
        }
    }
}
