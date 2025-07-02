using System.ComponentModel.DataAnnotations;

namespace Survey.DTOs.QuestionDto
{
    public class QuestionInputDto
    {
        public int Id { get; set; } // Mevcut sorular için ID, yeni sorular için 0
        [Required(ErrorMessage = "Soru Metni boş geçilemez.")]
        public string QuestionText { get; set; }        
        public List<AnswerOptionInputDto> AnswerOptions { get; set; } = new List<AnswerOptionInputDto>();
    }
}
