using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.DTOs.SurveyDto
{
    public class UpdateSurveyDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Lütfen anket başlığını giriniz.")]
        [Display(Name = "Anket Başlığı")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Lütfen anket için bir URL giriniz.")]
        [Display(Name = "Anket Linki (URL)")]
        public string Url { get; set; }

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "Aktif mi?")]
        public bool IsActive { get; set; }
    }
}
