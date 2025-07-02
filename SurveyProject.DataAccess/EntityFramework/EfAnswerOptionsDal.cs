using Microsoft.EntityFrameworkCore;
using SurveyProject.DataAccess.Abstract;
using SurveyProject.DataAccess.Context;
using SurveyProject.DataAccess.Repositories;
using SurveyProject.Entities.Entity;

namespace SurveyProject.DataAccess.EntityFramework
{
    public class EfAnswerOptionsDal : GenericRepository<AnswerOption>, IAnswerOptionsDal

    {
        public EfAnswerOptionsDal(AppDbContext context) : base(context)
        {
        }

        public void SaveUserAnswerAsync(AnswerOption answer)
        {
            _context.Set<AnswerOption>().Add(answer);
            _context.SaveChanges();
        }

        public void DeleteUserAnswer(int answerId)
        {
            var answer = _context.UserAnswers.FirstOrDefault(x => x.SelectedOptionId == answerId);
            if (answer!= null)
            {
                _context.UserAnswers.Remove(answer);
                _context.SaveChanges();
            }         
        }

     
    }
}
