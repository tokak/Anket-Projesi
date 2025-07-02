using System.ComponentModel.DataAnnotations;

namespace Survey.DTOs.LoginDto
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta adresi girin.")]
        public string Email { get; set; }
    }
}
