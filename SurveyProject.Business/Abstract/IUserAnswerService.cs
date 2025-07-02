using SurveyProject.Business.Services.Interfaces;
using SurveyProject.Entities.Entity;

namespace SurveyProject.Business.Abstract
{
    public interface IUserAnswerService:IGenericService<UserAnswer>
    {
        // Belirli bir anket ve kullanıcıya ait tüm cevapları getiren metod
        List<UserAnswer> TGetUserAnswersForSurveyAsync(int surveyId, string userId);
    }
}
