using SurveyProject.DataAccess.Abstract;
using SurveyProject.DataAccess.Context;
using SurveyProject.DataAccess.Repositories;
using SurveyProject.Entities.Entity;

namespace SurveyProject.DataAccess.EntityFramework
{
    public class EfUserAnswerDal : GenericRepository<UserAnswer>, IUserAnswerDal
    {
        public EfUserAnswerDal(AppDbContext context) : base(context)
        {
        }

        public List<UserAnswer> GetUserAnswersForSurveyAsync(int surveyId, string userId)
        {
           return  _context.Set<UserAnswer>()
                                 .Where(ua => ua.SurveyId == surveyId && ua.UserId.ToString() == userId)
                                 .ToList();
        }
    }
}
