using SurveyProject.Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyProject.DataAccess.Abstract
{
    public interface IAnswerOptionsDal : IGenericDal<AnswerOption>
    {
        void SaveUserAnswerAsync(AnswerOption answer);
        void DeleteUserAnswer(int answerId);
    }
}
