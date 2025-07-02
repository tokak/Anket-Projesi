using SurveyProject.Business.Services.Interfaces;
using SurveyProject.Entities.Entity;

namespace SurveyProject.Business.Abstract
{
    public interface ISurveyService:IGenericService<TSurvey>
    {
        Task<TSurvey> TGetSurveyWithQuestionsAndOptionsAsync(int surveyId);
        void TDeleteUserSurvey(int surveyId);
    }
}
