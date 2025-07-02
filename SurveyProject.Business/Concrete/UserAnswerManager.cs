using SurveyProject.Business.Abstract;
using SurveyProject.DataAccess.Abstract;
using SurveyProject.Entities.Entity;
using System.Linq.Expressions;

namespace SurveyProject.Business.Concrete
{
    public class UserAnswerManager : IUserAnswerService
    {
        private readonly IUserAnswerDal _userAnswerDal;

        public UserAnswerManager(IUserAnswerDal userAnswerDal)
        {
            _userAnswerDal = userAnswerDal;
        }

        public void TAdd(UserAnswer entity)
        {
            _userAnswerDal.Insert(entity);
        }

        public void TDelete(UserAnswer entity)
        {
            _userAnswerDal.Delete(entity);
        }

        public UserAnswer TGetById(int id)
        {
            return _userAnswerDal.GetById(id);
        }

        public List<UserAnswer> TGetList()
        {
            return _userAnswerDal.GetListAll();
        }

        public List<UserAnswer> TGetListByFilter(Expression<Func<UserAnswer, bool>> filter)
        {
            return _userAnswerDal.GetListAll(filter);
        }

        public List<UserAnswer> TGetUserAnswersForSurveyAsync(int surveyId, string userId)
        {
            return _userAnswerDal.GetUserAnswersForSurveyAsync(surveyId, userId);
        }

        public void TUpdate(UserAnswer entity)
        {
            _userAnswerDal.Update(entity);
        }
    }
}
