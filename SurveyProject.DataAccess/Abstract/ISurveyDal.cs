using SurveyProject.Entities.Entity;

namespace SurveyProject.DataAccess.Abstract
{
    public interface ISurveyDal : IGenericDal<TSurvey>
    {
        Task<TSurvey> GetSurveyWithQuestionsAndOptionsAsync(int surveyId);
        void DeleteUserSurvey(int surveyId);
    }
}
