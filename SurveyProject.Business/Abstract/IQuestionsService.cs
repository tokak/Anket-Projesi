using SurveyProject.Business.Services.Interfaces;
using SurveyProject.Entities.Entity;

namespace SurveyProject.Business.Abstract
{
    public interface IQuestionsService:IGenericService<Question>
    {
        void TDeleteUserQuestion(int questionId);
    }
}
