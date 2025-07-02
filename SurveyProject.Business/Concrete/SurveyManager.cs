using SurveyProject.Business.Abstract;
using SurveyProject.DataAccess.Abstract;
using SurveyProject.Entities.Entity;
using System.Linq.Expressions;

namespace SurveyProject.Business.Concrete
{
    public class SurveyManager : ISurveyService
    {
        private readonly ISurveyDal _surveyDal;

        public SurveyManager(ISurveyDal surveyDal)
        {
            _surveyDal = surveyDal;
        }

        public async Task<TSurvey> TGetSurveyWithQuestionsAndOptionsAsync(int surveyId)
        {
            return await _surveyDal.GetSurveyWithQuestionsAndOptionsAsync(surveyId);
        }

        public void TAdd(TSurvey entity)
        {
            _surveyDal.Insert(entity);
        }

        public void TDelete(TSurvey entity)
        {
            _surveyDal.Delete(entity);
        }

        public TSurvey TGetById(int id)
        {
            return _surveyDal.GetById(id);
        }

        public List<TSurvey> TGetList()
        {
            return _surveyDal.GetListAll();
        }

        public List<TSurvey> TGetListByFilter(Expression<Func<TSurvey, bool>> filter)
        {
            return _surveyDal.GetListAll(filter);
        }

        public void TUpdate(TSurvey entity)
        {
            _surveyDal.Update(entity);
        }

        public void TDeleteUserSurvey(int surveyId)
        {
            _surveyDal.DeleteUserSurvey(surveyId);
        }
    }
}
