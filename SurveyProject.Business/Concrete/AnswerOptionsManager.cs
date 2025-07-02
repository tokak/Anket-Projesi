using Microsoft.EntityFrameworkCore;
using SurveyProject.Business.Abstract;
using SurveyProject.Business.Services.Interfaces;
using SurveyProject.DataAccess.Abstract;
using SurveyProject.Entities.Entity;
using System.Linq.Expressions;

namespace SurveyProject.Business.Concrete
{
    public class AnswerOptionsManager : IAnswerOptionsService
    {
        private readonly IAnswerOptionsDal  _answerOptionsDal;

        public AnswerOptionsManager(IAnswerOptionsDal  answerOptionsDal)
        {
            _answerOptionsDal = answerOptionsDal;
        }

        public void TSaveUserAnswerAsync(AnswerOption answer)
        {
            _answerOptionsDal.Insert(answer);
        }

        public void TAdd(AnswerOption entity)
        {
            _answerOptionsDal.Insert(entity);
        }

        public void TDelete(AnswerOption entity)
        {
            _answerOptionsDal.Delete(entity);
        }

        public AnswerOption TGetById(int id)
        {
            return _answerOptionsDal.GetById(id);
        }

        public List<AnswerOption> TGetList()
        {
            return _answerOptionsDal.GetListAll();
        }

        public List<AnswerOption> TGetListByFilter(Expression<Func<AnswerOption, bool>> filter)
        {
            return _answerOptionsDal.GetListAll(filter);
        }

        public void TUpdate(AnswerOption entity)
        {
            _answerOptionsDal.Update(entity);
        }

        public void TDeleteUserAnswer(int answerId)
        {
            _answerOptionsDal.DeleteUserAnswer(answerId);
        }
    }
}
