using System.ComponentModel.DataAnnotations;

namespace Survey.DTOs.QuestionDto
{
    public class AnswerOptionInputDto
    {
        public int Id { get; set; } // Mevcut cevaplar için ID, yeni cevaplar için 0
        [Required(ErrorMessage = "Cevap Seçeneği boş geçilemez.")]
        public string OptionText { get; set; }
    }
}
