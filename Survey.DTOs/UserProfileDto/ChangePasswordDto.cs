using System.ComponentModel.DataAnnotations;

namespace Survey.DTOs.UserProfileDto
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Mevcut şifrenizi girmeniz zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Şifre")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Yeni şifrenizi girmeniz zorunludur.")]
        [StringLength(100, ErrorMessage = "{0} en az {2} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre Tekrar")]
        [Compare("NewPassword", ErrorMessage = "Yeni şifre ve onay şifresi eşleşmiyor.")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OldPassword == NewPassword)
            {
                yield return new ValidationResult(
                    "Yeni şifre, mevcut şifrenizden farklı olmalıdır.",
                    new[] { nameof(NewPassword) }
                );
            }
        }
    }    
}
