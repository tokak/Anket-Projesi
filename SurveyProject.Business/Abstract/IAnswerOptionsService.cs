using SurveyProject.Business.Services.Interfaces;
using SurveyProject.Entities.Entity;

namespace SurveyProject.Business.Abstract
{
    public interface IAnswerOptionsService : IGenericService<AnswerOption>
    {
        void TSaveUserAnswerAsync(AnswerOption answer);
    }
}
