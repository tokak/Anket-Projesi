using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.DTOs.QuestionDto
{
    //Ankete bağlı soruları listeleme
    public class ListSurveyQuestionsDto
    {
        public int SurveyId { get; set; }
        public string SurveyTitle { get; set; } 
        public string? SurveyDesription { get; set; }
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
    public class AnswerOptionDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<AnswerOptionDto> AnswerOptions { get; set; } = new List<AnswerOptionDto>();
    }

    
   
}
