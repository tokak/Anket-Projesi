using SurveyProject.DataAccess.Abstract;
using SurveyProject.DataAccess.Context;
using SurveyProject.DataAccess.Repositories;
using SurveyProject.Entities.Entity;

namespace SurveyProject.DataAccess.EntityFramework
{
    public class EfQuestionDal : GenericRepository<Question>, IQuestionsDal
    {
        public EfQuestionDal(AppDbContext context) : base(context)
        {
        }

        public void DeleteUserQuestion(int questionId)
        {
           var question = _context.UserAnswers.FirstOrDefault(x=>x.QuestionId == questionId);
            _context.UserAnswers.Remove(question);
            _context.SaveChanges();
        }
    }
}
