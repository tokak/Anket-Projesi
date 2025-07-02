using SurveyProject.Business.Abstract;
using SurveyProject.DataAccess.Abstract;
using SurveyProject.Entities.Entity;
using System.Linq.Expressions;

namespace SurveyProject.Business.Concrete
{
    public class QuestionsManager : IQuestionsService
    {

        private readonly IQuestionsDal _questionsDal;

        public QuestionsManager(IQuestionsDal  questionsDal)
        {
            _questionsDal = questionsDal;
        }
        public void TAdd(Question entity)
        {
            _questionsDal.Insert(entity);
        }

        public void TDelete(Question entity)
        {
            _questionsDal.Delete(entity);
        }

        public void TDeleteUserQuestion(int questionId)
        {
            _questionsDal.DeleteUserQuestion(questionId);
        }

        public Question TGetById(int id)
        {
            return _questionsDal.GetById(id);
        }

        public List<Question> TGetList()
        {
            return _questionsDal.GetListAll();
        }

        public List<Question> TGetListByFilter(Expression<Func<Question, bool>> filter)
        {
            return _questionsDal.GetListAll(filter);
        }

        public void TUpdate(Question entity)
        {
            _questionsDal.Update(entity);
        }
    }
}
