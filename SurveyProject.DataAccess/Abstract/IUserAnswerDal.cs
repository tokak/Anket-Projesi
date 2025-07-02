using SurveyProject.Entities.Entity;

namespace SurveyProject.DataAccess.Abstract
{
    public interface IUserAnswerDal : IGenericDal<UserAnswer>
    {
        List<UserAnswer> GetUserAnswersForSurveyAsync(int surveyId, string userId);
    }
}
