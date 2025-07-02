using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyProject.Entities.Entity
{
    //  Kullanıcı cevaplarını tutacak model 
    public class UserAnswer
    {
        public int Id { get; set; }

        public int SurveyId { get; set; }
        public TSurvey Survey { get; set; } // Bu yanıt hangi ankete ait

        public int QuestionId { get; set; }
        public Question Question { get; set; } // Bu yanıt hangi soruya ait

        public int SelectedOptionId { get; set; }
        public AnswerOption SelectedOption { get; set; } // Kullanıcının seçtiği cevap seçeneği

        public Guid? UserId { get; set; } // AppUser ile ilişki için null olabilir (anonim yanıtlar için)
        public AppUser AppUser { get; set; } // Bu yanıtı veren kullanıcı

        public DateTime SubmitDate { get; set; }
    }
}
