using SurveyProject.Entities.Entity;

namespace SurveyProject.DataAccess.Abstract
{
    public interface IQuestionsDal:IGenericDal<Question>
    {
        void DeleteUserQuestion(int questionId);
    }
}
